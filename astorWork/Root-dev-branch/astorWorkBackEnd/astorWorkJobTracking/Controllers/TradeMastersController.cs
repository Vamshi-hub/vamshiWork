using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using astorWorkDAO;
using astorWorkJobTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/trades")]
    public class TradeMastersController : Controller
    {
        protected astorWorkDbContext _context;
        protected IAstorWorkImport _importService;

        public TradeMastersController(astorWorkDbContext context, IAstorWorkImport importService) {
            _context = context;
            _importService = importService;
        }

        // GET: api/<controller>
        [HttpGet()]
        public List<TradeMaster> ListTrades()
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            return _context.TradeMaster.ToList();
        }

        // POST api/<controller>
        [HttpPost]
        public async Task CreateJob([FromBody]TradeMaster trade)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            _context.TradeMaster.Add(trade);
            await _context.SaveChangesAsync();
        }

        // POST api/<controller>
        [HttpPost("import-jobs")]
        public async Task<List<JobUploadStatus>> ImportJobs(IFormFile file,  string project_id)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(project_id))
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            IEnumerable<string> jobs = _importService.GetRowsFromFile(file).Distinct();
            List<JobUploadStatus> existingJobs = new List<JobUploadStatus>();
            List<string> addedMaterialTypes = new List<string>();

            for (int i = 1; i < jobs.Count(); i++)
            {
                TradeMaster job = CreateJob(jobs.ElementAt(i));

                if (JobMasterExists(job)) // Check for duplicates in the Db
                {
                    if(!string.IsNullOrEmpty(job.Name))
                    {
                        UpdateTradeMaster(job);
                        await _context.SaveChangesAsync();
                    }
                   
                }                
                else
                {
                    if(!string.IsNullOrEmpty(job.Name))
                    {
                        _context.TradeMaster.Add(job);
                        addedMaterialTypes.Add(job.Name);
                    }
                   
                }
            }

            await _context.SaveChangesAsync();

            if (existingJobs.Count == 0)
                return  new List<JobUploadStatus>();
            else
                return existingJobs;
        }

        private void UpdateTradeMaster(TradeMaster job)
        {
            TradeMaster tradeMaster = _context.TradeMaster
                 .Where(t => t.Name.ToLower() == job.Name.ToLower()).FirstOrDefault();
            tradeMaster.RouteTo = job.RouteTo;
            _context.Entry(tradeMaster).State = EntityState.Modified;
         

        }

        protected TradeMaster CreateJob(string jobInfo)
        {
            TradeMaster job = new TradeMaster();
            string[] jobAttributes = jobInfo.Trim().Split(",");
            job.Name = jobAttributes[0];
            if(jobAttributes[0]!="")
            job.RouteTo = "RTO"; // For Now if any Name they gave any excel it willl set RTO only for ROUTE TO
            //job.Organisation = _context.OrganisationMaster.Where(o => o.Name == jobAttributes[1]).FirstOrDefault();

            return job;
        }

        protected bool JobMasterExists(TradeMaster job)
        {
            return _context.TradeMaster.Any(mt => (mt.Name.ToLower() == job.Name.ToLower()));
        }

        protected List<JobUploadStatus> AddJobUploadStatus(List<JobUploadStatus> existingJobs, TradeMaster job, string message)
        {
            JobUploadStatus jobUploadStatus = new JobUploadStatus();

            jobUploadStatus.Name = job.Name;
            jobUploadStatus.Message = message;

            existingJobs.Add(jobUploadStatus);

            return existingJobs;
        }
    }
}
