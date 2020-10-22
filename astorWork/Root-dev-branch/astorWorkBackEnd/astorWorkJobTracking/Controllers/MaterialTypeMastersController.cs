using astorWorkDAO;
using astorWorkJobTracking.Models;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/materialtypes")]
    public class MaterialTypeMastersController : Controller
    {
        protected astorWorkDbContext _context;
        protected IAstorWorkImport _importService;

        public MaterialTypeMastersController(astorWorkDbContext context, IAstorWorkImport importService)
        {
            _context = context;
            _importService = importService;
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<List<MaterialTypeMaster>> ListMaterialTypes([FromRoute] int project_id)
        {
            if (!ModelState.IsValid || project_id == 0)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            // Retrieve materials based on Project and Block
            List<MaterialTypeMaster> materialTypes = await GetMaterialTypes(project_id);
            return materialTypes;
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public async Task CreateMaterialType([FromBody] MaterialTypeMaster materialType)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            _context.MaterialTypeMaster.Add(materialType);
            await _context.SaveChangesAsync();
        }

        // POST api/<controller>
        [HttpPost("Import-MaterialTypes")]
        public async Task<List<MaterialTypeUploadStatus>> ImportMaterialTypes(IFormFile file, string project_id)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(project_id))
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            IEnumerable<string> materialTypes = _importService.GetRowsFromFile(file).Distinct();
            List<MaterialTypeUploadStatus> existingMaterialTypes = new List<MaterialTypeUploadStatus>();
            List<string> addedMaterialTypes = new List<string>();

            for (int i = 1; i < materialTypes.Count(); i++)
            {
                MaterialTypeMaster materialType = CreateMaterialType(materialTypes.ElementAt(i));

                if (MaterialTypeMasterExists(materialType)) // Check for duplicates in the Db
                {
                    updateMateralType(materialType);
                    //existingMaterialTypes = AddMaterialTypeUploadStatus(existingMaterialTypes, materialType, "Material Type already exist in the database");
                }
                    
                //else if (addedMaterialTypes.Contains(materialType.Name))    // Check for duplicates in the CSV file
                //    existingMaterialTypes = AddMaterialTypeUploadStatus(existingMaterialTypes, materialType, "Duplicate Material Types found in CSV");
                else
                {
                    if (!string.IsNullOrEmpty(materialType.Name))
                    {
                        _context.MaterialTypeMaster.Add(materialType);
                        addedMaterialTypes.Add(materialType.Name);
                    }

                }
            }

            await _context.SaveChangesAsync();

            if (existingMaterialTypes.Count == 0)
                return new List<MaterialTypeUploadStatus>();
            else
                return existingMaterialTypes;
        }

        protected void updateMateralType(MaterialTypeMaster materialType)
        {
            MaterialTypeMaster materialTypeMaster = _context.MaterialTypeMaster.Where(mt => mt.Name.ToLower() == materialType.Name.ToLower()).FirstOrDefault();
            materialTypeMaster.RouteTo = materialType.RouteTo;
           _context.Entry(materialTypeMaster).State = EntityState.Modified;
           // _context.SaveChanges();
        }

        protected MaterialTypeMaster CreateMaterialType(string materialTypeInfo)
        {
            MaterialTypeMaster materialType = new MaterialTypeMaster();
            string[] materialTypeAttributes = materialTypeInfo.Trim().Split(",");
            materialType.Name = materialTypeAttributes[0];
            materialType.RouteTo = materialTypeAttributes[1];

            return materialType;
        }

        protected bool MaterialTypeMasterExists(MaterialTypeMaster materialType)
        {
            return _context.MaterialTypeMaster.Any(mt => (mt.Name.ToLower() == materialType.Name.ToLower()));
        }

        protected List<MaterialTypeUploadStatus> AddMaterialTypeUploadStatus(List<MaterialTypeUploadStatus> existingMaterialTypes, MaterialTypeMaster materialType, string message)
        {
            MaterialTypeUploadStatus materialTypeUploadStatus = new MaterialTypeUploadStatus();

            materialTypeUploadStatus.Name = materialType.Name;
            materialTypeUploadStatus.Message = message;

            existingMaterialTypes.Add(materialTypeUploadStatus);

            return existingMaterialTypes;
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        protected async Task<List<MaterialTypeMaster>> GetMaterialTypes(int projectID)
        {
            return await _context.MaterialTypeMaster
                                 .ToListAsync();
        }
    }
}
