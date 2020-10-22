using astorWorkDAO;
using astorWorkJobTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static astorWorkShared.Utilities.Enums;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/checklist")]
    public class ChecklistMastersController : Controller
    {
        protected astorWorkDbContext _context;

        public ChecklistMastersController(astorWorkDbContext context, IAstorWorkImport importService)
        {
            _context = context;
        }

        /// <summary>
        /// to get the list of checklist for a specfic job or material based on stage 
        /// </summary>
        /// <references>
        /// Mobile: checklist page
        /// </references>
        /// <URL>
        /// Route: projects/1/checklist?job_schedule_id=12
        /// </URL>
        /// <param name="project_id"></param>
        /// <param name="job_schedule_id"></param>
        /// <param name="material_id"></param>
        /// <returns> List of checklists</returns>
        [HttpGet()]
        public async Task<List<Checklist>> ListChecklists([FromRoute] int project_id, [FromQuery]int job_schedule_id, [FromQuery] int material_id)
        {
            try
            {   
                if (job_schedule_id != 0)
                    return await GetChecklistForTrade(job_schedule_id);
                else
                    return await GetChecklistForMaterialStage(material_id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<List<Checklist>> GetChecklistForMaterialStage(int material_id)
        {
            List<Checklist> checklists = new List<Checklist>();
            
            try
            {
                MaterialStageAudit materialStageAudit = await _context.MaterialStageAudit
                    .Include(msa => msa.Stage)
                    .Where(ms => ms.MaterialID == material_id).OrderByDescending(ms => ms.Stage.Order).FirstOrDefaultAsync();
                List<ChecklistAudit> checklistAudits = await _context.ChecklistAudit.Include(chk => chk.RouteTo)
                                                                                    .Include(chk => chk.MaterialStageAudit)
                                                                                    .ThenInclude(ma => ma.Material)
                                                                                    .Where(ca => ca.MaterialStageAudit == materialStageAudit)
                                                                                    .ToListAsync();

                //if (checklistAudits != null && checklistAudits.Count > 0)
                //    materialStageAudit = checklistAudits.OrderByDescending(ca => ca.CreatedDate).FirstOrDefault().MaterialStageAudit;
                //else
                    //materialStageAudit = await _context.MaterialStageAudit.Where(ms => ms.MaterialMasterID == material_id).OrderByDescending(ms => ms.CreatedDate).FirstOrDefaultAsync();

                List<ChecklistMaster> checklistMasters = await _context.ChecklistMaster.Where(cm => cm.MaterialStageID == materialStageAudit.StageID && cm.IsActive == true).ToListAsync();

                foreach (ChecklistMaster checklist in checklistMasters)
                    checklists.Add(await CreateChecklist(checklist, checklistAudits));
            }
           catch(Exception ex)
            {
                throw ex;
            }

            return checklists;
        }

        private async Task<List<Checklist>> GetChecklistForTrade(int job_schedule_id)
        {
            List<Checklist> checklists = new List<Checklist>();
            JobSchedule jobSchedule = null;

            try
            {
                List<ChecklistAudit> checklistAudits = await _context.ChecklistAudit.Include(chk => chk.RouteTo)
                                                                                    .Include(chk => chk.JobSchedule)
                                                                                    .ThenInclude(j => j.Trade)
                                                                                    .Where(ca => ca.JobScheduleID == job_schedule_id)
                                                                                    .ToListAsync();

                if (checklistAudits != null && checklistAudits.Count > 0)
                    jobSchedule = checklistAudits.FirstOrDefault().JobSchedule;
                else
                    jobSchedule = await _context.JobSchedule.Where(js => js.ID == job_schedule_id)
                                                            .FirstOrDefaultAsync();

                List<ChecklistMaster> checklistMasters = await _context.ChecklistMaster.Where(cm => cm.TradeID == jobSchedule.TradeID && cm.IsActive==true && cm.Type==ChecklistType.Architectural_Maincon).ToListAsync();

                if (checklistMasters != null && checklistMasters.Count > 0)
                    foreach (ChecklistMaster checklist in checklistMasters)
                        checklists.Add(await CreateChecklist(checklist, checklistAudits));
                else
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "No checklist found!");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return checklists;
        }

        private async Task<Checklist> CreateChecklist(ChecklistMaster checklist, List<ChecklistAudit> checklistAudits)
        {
            try
            {
                if(checklistAudits != null && checklistAudits.Count > 0)
                    checklistAudits = checklistAudits.Where(ca => ca.Checklist == checklist).OrderByDescending(ca => ca.CreatedDate).ToList();
                ChecklistAudit checklistAudit = checklistAudits == null || checklistAudits.Count == 0 ? null : checklistAudits.FirstOrDefault();

                return new Checklist()
                {
                    ID = checklist.ID,
                    Name = checklist.Name,
                    StatusCode = (checklistAudit == null) ? (int)QCStatus.Pending_QC : (int)checklistAudit.Status,
                    Status = (checklistAudit == null) ? QCStatus.Pending_QC.ToString() : checklistAudit.Status.ToString(),
                    Sequence = checklist.Sequence,
                    RTOID = (checklistAudit == null) || checklistAudits.Where(p => p.RouteTo != null).Count() == 0 ? 0 : checklistAudits.Where(p => p.RouteTo != null).FirstOrDefault().RouteTo.ID,
                    RTOName = (checklistAudit == null) || checklistAudits.Where(p => p.RouteTo != null).Count() == 0 ? "" : checklistAudits.Where(p => p.RouteTo != null).FirstOrDefault().RouteTo.PersonName,
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }


}

