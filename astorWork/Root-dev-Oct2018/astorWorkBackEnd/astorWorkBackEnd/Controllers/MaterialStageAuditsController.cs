using astorWorkBackEnd.Common;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("")]
    public class MaterialStageAuditsController : CommonMaterialStageAuditsController
    {
        private IAstorWorkBlobStorage _blobStorage;
        public MaterialStageAuditsController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage) : base(context)
        {
            _blobStorage = blobStorage;
        }

        // POST: /vendors/{vendor_id}/after-inventory
        [HttpPost("vendors/{vendor_id}/after-inventory")]
        public async Task<APIResponse> PostAfterInventoryMaterialStageAudit([FromRoute] int vendor_id, [FromBody] MaterialUpdate producedMaterial)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            InventoryAudit inventoryAudit = _context.InventoryAudit.Include(i => i.Tracker).Where(i => i.Tracker.ID == producedMaterial.TrackerID).FirstOrDefault();
            /*
            if (inventoryAudit == null)
                return new DbRecordNotFound("The Tracker cannot be found in Inventory. This might be a new Tracker or it is already used in the operational stages");
                */
            MaterialMaster material = _context.MaterialMaster.Include(m => m.Tracker)
                .Where(m =>
                (inventoryAudit == null && m.Tracker.ID == producedMaterial.TrackerID) ||
                (inventoryAudit != null && m.MarkingNo == inventoryAudit.MarkingNo && m.MRF.MRFNo == producedMaterial.MRFNo))
                .FirstOrDefault();

            if (material == null)
                return new DbRecordNotFound("Material", "Marking No. and MRF No.", inventoryAudit.MarkingNo + " and " + producedMaterial.MRFNo);

            if (material.Tracker != null)
                return new DbConcurrentUpdate(string.Format("There is already a tracker associated with {0} under {1}", material.MarkingNo, producedMaterial.MRFNo));

            return new APIResponse(0, await UpdateInventoryToNextStageInDb(material, inventoryAudit, producedMaterial));
        }

        // POST: /materials/{material_id}/update-stage
        [HttpPost("materials/{id}/update-stage")]
        public async Task<APIResponse> UpdateMaterialStage([FromRoute] int id, [FromBody] MaterialUpdate materialUpdate)
        {
            // Only gets called when the previous stage QC is passed
            var userClaim = HttpContext.User.Claims.FirstOrDefault(cl => cl.Type.Equals(ClaimTypes.NameIdentifier));
            if (userClaim == null)
                return new DbRecordNotFound("User Claim not found");

            var user = _context.UserMaster.Find(int.Parse(userClaim.Value));

            if (user == null)
                return new DbRecordNotFound("User not found");

            if (!ModelState.IsValid)
                return new APIBadRequest();

            MaterialMaster material = _context.MaterialMaster.Include(m => m.Tracker).Include(m => m.MRF).ThenInclude(m => m.UserMRFAssociations).ThenInclude(una => una.User)
                                        .Include(m => m.StageAudits)
                                        .ThenInclude(sa => sa.Stage)
                                        .Include(m => m.MRF)
                                        .Where(m => m.ID == id)
                                        .FirstOrDefault();

            if (material == null)
                return new DbRecordNotFound("The Tracker cannot be found. This might be a new Tracker or it is still not past the inventory stage.");

            var result = await UpdateMaterialStage(material, materialUpdate);

            MRFMaster mrfMaster = _context.MRFMaster.Where(m => m.ID == material.MRF.ID).FirstOrDefault();
            mrfMaster.UpdatedDate = DateTime.UtcNow;
            mrfMaster.UpdatedBy = user;

            await _context.SaveChangesAsync();

            //var lastStage = _context.MaterialStageMaster.OrderBy(p => p.Order).Last().Name;
            //var MRFMaterilasCurrentStages = material.MRF.Materials.Select(p => p?.StageAudits?.Last().Stage.Name);
            List<MaterialMaster> lstMaterials = _context.MaterialMaster.Include(m => m.MRF).Where(m => m.MRF.ID == material.MRF.ID).ToList();
            int InstalledMaterialCount = _context.MaterialStageAudit.Include(m => m.MaterialMaster).Include(s => s.Stage).Where(msa => lstMaterials.Contains(msa.MaterialMaster) && msa.Stage.ID == installedStageId).Count();

            if (InstalledMaterialCount == lstMaterials.Count)
            {
                List<UserMaster> recipients = material.MRF.UserMRFAssociations.Select(p => p.User).ToList();
                await UpdateNotificationAudit(recipients, 5, 0, material.MRF.ID.ToString());
            }

            return new APIResponse(0, result);
        }
        // POST: /materials/{material_id}/update-stage
        [HttpPost("materials/{id}/update-location")]
        public async Task<APIResponse> UpdateMaterialLocation([FromRoute] int id, [FromBody] MaterialUpdate materialUpdate)
        {
            var userClaim = HttpContext.User.Claims.FirstOrDefault(cl => cl.Type.Equals(ClaimTypes.NameIdentifier));
            if (userClaim == null)
                return new DbRecordNotFound("User Claim not found");

            var user = _context.UserMaster.Find(int.Parse(userClaim.Value));

            if (user == null)
                return new DbRecordNotFound("User not found");

            if (!ModelState.IsValid)
                return new APIBadRequest();

            var material = _context.MaterialMaster.Include(m => m.Tracker).Include(m => m.MRF).ThenInclude(m => m.UserMRFAssociations).ThenInclude(una => una.User)
                                        .Include(m => m.StageAudits)
                                        .ThenInclude(sa => sa.Stage)
                                        .Include(m => m.MRF)
                                        .Where(m => m.ID == id)
                                        .FirstOrDefault();

            var location = await _context.LocationMaster.FindAsync(materialUpdate.LocationID);

            var currStage = material.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault();

            if (material == null)
                return new DbRecordNotFound("The material cannot be found.");
            else if (currStage == null)
                return new DbRecordNotFound("The material does not have any history");
            else if (location == null)
                return new DbRecordNotFound("The location cannot be found.");
            else if (currStage.LocationId == materialUpdate.LocationID)
                return new DbRecordNotFound("The location cannot be the same as the current location.");

            try
            {
                var newStage = new MaterialStageAudit
                {
                    LocationId = materialUpdate.LocationID,
                    MaterialMasterID = material.ID,
                    StageID = currStage.Stage.ID,
                    StagePassed = materialUpdate.QCStatus,
                    Remarks = materialUpdate.QCRemarks,
                    CreatedByID = user.ID,
                    CreatedDate = DateTimeOffset.Now
                };
                await _context.MaterialStageAudit.AddAsync(newStage);
                await _context.SaveChangesAsync();

                return new APIResponse(0, newStage);
            }
            catch (Exception exc)
            {
                return new APIResponse(504, null, ExceptionUtility.GetExceptionDetails(exc));
            }
        }

        // GET: /materials/{material_id}/next-stage
        [HttpGet("materials/{material_id}/next-stage")]
        public APIResponse GetNextMaterialStageAudit([FromRoute] int material_id)
        {
            MaterialNextStageInfo materiaNextStageInfo = CreateMaterialNextStageInfo(material_id);

            if (materiaNextStageInfo == null)
            {
                return new DbRecordNotFound($"Material with Id {material_id} does not have a next stage. The material could still be in inventory or untagged state.");
            }

            return new APIResponse(0, materiaNextStageInfo);
        }

        // PUT: MaterialStageAudits/5
        [HttpPut("MaterialStageAudits/{material_id}")]
        public async Task<IActionResult> PutMaterialStageAudit([FromRoute] int material_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MaterialMaster material = _context.MaterialMaster.Include(m => m.StageAudits).FirstOrDefault();

            if (material_id != material.ID)
            {
                return BadRequest();
            }

            TrackerMaster tracker = material.Tracker;
            InventoryAudit inventoryAudit = _context.InventoryAudit.Where(i => i.MarkingNo == material.MarkingNo && i.Tracker == null).FirstOrDefault();
            inventoryAudit.Tracker = tracker;

            material.StageAudits = null;
            _context.Entry(material).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialStageAuditExists(material_id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/MaterialStageAudits/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialStageAudit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var materialStageAudit = await _context.MaterialStageAudit.SingleOrDefaultAsync(m => m.ID == id);

            if (materialStageAudit == null)
            {
                return NotFound();
            }

            return Ok(materialStageAudit);
        }

        // DELETE: MaterialStageAudits/5?tracker_id={tracker_id}
        [HttpDelete("MaterialStageAudits/{id}")]
        public async Task<IActionResult> DeleteMaterialStageAudit([FromRoute] int id, [FromQuery] int tracker_id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            MaterialStageAudit ma = await _context.MaterialStageAudit.SingleOrDefaultAsync(m => m.ID == id);

            if (ma == null)
            {
                return NotFound();
            }

            MaterialMaster mm = _context.MaterialMaster.Include(m => m.Tracker).Where(m => m.Tracker.ID == tracker_id).FirstOrDefault();
            InventoryAudit ia;
            if (mm != null)
            {
                ia = _context.InventoryAudit.Where(i => i.SN == mm.SN).FirstOrDefault();
                ia.Tracker = mm.Tracker;
                mm.Tracker = null;
            }

            _context.MaterialStageAudit.Remove(ma);
            await _context.SaveChangesAsync();

            return Ok(ma);
        }

    }
}