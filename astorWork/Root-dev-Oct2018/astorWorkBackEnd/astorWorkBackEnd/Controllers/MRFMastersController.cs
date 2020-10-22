using astorWorkBackEnd.Common;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkShared.MultiTenancy;
using astorWorkShared.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("projects/{project_id}/mrfs")]
    public class MRFMastersController : CommonMRFController
    {
        private readonly string Module = "MRF";

        public MRFMastersController(astorWorkDbContext context, IAstorWorkEmail emailService, TenantInfo tenantInfo): base(context)
        {
            _emailService = emailService;
            _tenant = tenantInfo;
        }

        // GET: /projects/{project_id}/mrfs?block={block}&marking_no={marking_no}&vendor_id={vendor_id}
        [HttpGet]
        public APIResponse ListMRFs([FromRoute] int project_id, [FromQuery] string block = "", [FromQuery] string marking_no = "")
        {

            if (!ModelState.IsValid)
                return new APIBadRequest();

            var user = _context.GetUserFromHttpContext(HttpContext);

            List<MaterialMaster> materials = null;

            // When marking no. is given, return MRFs for vendor
            if (!string.IsNullOrEmpty(marking_no))
            {
                materials = _context.MaterialMaster.Include(mm => mm.MRF)
                    .Include(mm => mm.Vendor)
                    .Include(mm => mm.StageAudits)
                    .ThenInclude(sa => sa.Stage)
                    .Where(mm => mm.MRF != null && mm.ProjectId == project_id && mm.MarkingNo == marking_no &&
                    mm.StageAudits.Count == 0)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(block))
            {
                materials = _context.MaterialMaster.Include(mm => mm.MRF)
                    .Include(mm => mm.Vendor)
                    .Include(mm => mm.StageAudits)
                    .ThenInclude(sa => sa.Stage)
                    .Where(mm => mm.MRF != null && mm.ProjectId == project_id && mm.Block == block)
                    .ToList();
            }
            else
            {
                materials = _context.MaterialMaster.Include(mm => mm.MRF)
                    .Include(mm => mm.Vendor)
                    .Include(mm => mm.StageAudits)
                    .ThenInclude(sa => sa.Stage)
                    .Where(mm => mm.MRF != null && mm.ProjectId == project_id)
                    .ToList();
            }

            if ((user.RoleID == 7 || user.RoleID == 8) && user.Vendor != null)
                materials = materials.Where(mm => mm.VendorId == user.Vendor.ID).ToList();

            if (materials == null || materials.Count == 0)
                return new DbRecordNotFound(Module, "MRF", string.Empty);

            var mrfs = materials.GroupBy(mm => mm.MRF).Select(g => new MRF
            {
                ID = g.Key.ID,
                MrfNo = g.Key.MRFNo,
                VendorName = g.First().Vendor.Name,
                Block = g.First().Block,
                Level = g.First().Level,
                Zone = g.First().Zone,
                PlannedCastingDate = g.Key.PlannedCastingDate,
                OrderDate = g.Key.OrderDate
            }).Distinct().OrderByDescending(mrf => mrf.PlannedCastingDate).ToList();

            int maxStageId = _context.MaterialStageMaster.OrderBy(msm => msm.Order).Last().ID;
            foreach (var mrf in mrfs)
            {
                mrf.MaterialTypes = materials.Where(mm => mm.MRF.ID == mrf.ID)
                    .Select(mm => mm.MaterialType).Distinct().ToList();
                                
                mrf.Progress = ((double) materials.Where(mm => mm.MRF.ID == mrf.ID && mm.StageAudits.Any(sa => sa.Stage.ID == maxStageId)).Count()) / ((double)materials.Where(mm => mm.MRF.ID == mrf.ID).Count());
            }
            return new APIResponse(0, mrfs);
        }

        // POST: projects/{project_id}/mrfs
        [HttpPost]
        public async Task<APIResponse> CreateMRF([FromRoute] int project_id, [FromBody] MRF mrf)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            if (MRFMasterExists(project_id, mrf))
                return new DbDuplicateRecord(Module, "Block, Level, Zone", mrf.Block + ", " + mrf.Level + ", " + mrf.Zone);

            MRFMaster mrfMaster = await CreateMRFInDb(project_id, mrf);
            string mrfNum = mrfMaster.MRFNo;
            //int noOfMaterials = mrfMaster.Materials.Count();
            List<UserMaster> recipients = _context.UserMaster.Where(u => mrf.OfficerUserIDs.Contains(u.ID) || u.ID == mrf.CreatedByUserID).ToList();
            
            UpdateNotificationAudit(recipients, 0, 0, mrfMaster.ID.ToString());
            //IEnumerable<UserMaster> contactPeople = _context.UserMaster.Where(u => mrf.OfficerUserIDs.Contains(u.ID));

            List<string> attachmentPaths = new List<string>();
            attachmentPaths.Add(CreateMRFDocument(mrfMaster));

            return new APIResponse(0, new { id = mrfMaster.ID, mrfNo = mrfNum, materialCount = mrfMaster.Materials.Count() });
        }

        // GET: projects/{projectID}/mrfs/location?block={block}
        [HttpGet("location")]
        public APIResponse GetLocationsList([FromRoute] int project_id, [FromQuery] string block = "")
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            var materialMaster = _context.MaterialMaster
                                                        .Where(m => m.Project.ID == project_id && m.Block == block)
                                                        .Select(m => new { m.Level, m.Zone })
                                                        .Distinct()
                                                        .OrderBy(m => m.Level.PadLeft(3));

            if (materialMaster == null)
                return new DbRecordNotFound("Location", "Block", block);

            List<Location> locationsList = new List<Location>();
            Location currLevel = null;

            foreach (var m in materialMaster)
            {
                // New Level
                if (currLevel == null)
                    currLevel = CreateNewLocation(currLevel, m.Level, m.Zone);
                else if (currLevel.Level != m.Level)
                {
                    locationsList.Add(currLevel);
                    currLevel = CreateNewLocation(currLevel, m.Level, m.Zone);
                }
                // Zone is within current level
                else
                    currLevel.Zones.Add(m.Zone);
            }

            // Add the Level with the corresponding Zones to the Locations list
            locationsList.Add(currLevel);

            return new APIResponse(0, locationsList);
        }

        // GET: projects/{projectID}/mrfs/material?block={block}
        [HttpGet("material")]
        public APIResponse GetVendorsAndMaterialTypesList([FromRoute] int project_id, [FromQuery] string block, [FromQuery] string level, [FromQuery] string zone)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            IEnumerable<MaterialMaster> materialMaster = _context.MaterialMaster
                                                        .Include(m => m.Vendor)
                                                        .Where(m => m.Project.ID == project_id
                                                         && m.Block == block && m.Level == level
                                                         && m.Zone == zone);

            if (materialMaster == null)
                return new DbRecordNotFound("Material", "Block, Level, Zone", block + ", " + level + ", " + zone);

            return new APIResponse(0, CreateMaterialList(materialMaster));
        }

        // DELETE /projects/{project_id}/mrfs/{material_id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMRFMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                //return BadRequest(new APIBadRequestResponse(ModelState));
            }

            var mRFMaster = await _context.MRFMaster.Include(m => m.Materials).SingleOrDefaultAsync(m => m.ID == id);
            if (mRFMaster == null)
            {
                //return NotFound(new APIResponse(404, Module, "ID", id.ToString()));
            }

            _context.MRFMaster.Remove(mRFMaster);
            await _context.SaveChangesAsync();

            return Ok(mRFMaster);
        }
    }
}