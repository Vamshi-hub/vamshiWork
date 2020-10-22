using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    [Route("")]
    public class MaterialStageAuditsController : CommonController
    {
        private IAstorWorkBlobStorage _blobStorage;

        public MaterialStageAuditsController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage) : base(context)
        {
            _blobStorage = blobStorage;
        }

        [HttpGet("projects/{project_id}/materialStageAudits/counts")]
        public async Task<List<MaterialStage>> GetMaterialStageCounts([FromRoute] int project_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            return await CreateMaterialStageCounts(project_id);
        }

        protected async Task<List<MaterialStage>> CreateMaterialStageCounts(int projectID)
        {
            List<MaterialStageAudit> materialStages = null;
            List<MaterialStage> materialCounts = new List<MaterialStage>();
            MaterialStage materialStageCounts = new MaterialStage();

            try
            {
                materialStages = await _context.MaterialStageAudit
                                                    .Where(msa => msa.Material.Project.ID == projectID)
                                                    .GroupBy(msa => msa.Material.ID,
                                                                    (id, msa) => new MaterialStageAudit
                                                                                 {
                                                                                    ID = id,
                                                                                    MaterialID = id,
                                                                                    Stage = msa.OrderByDescending(msm => msm.Stage.Order).First().Stage
                                                                                 }
                                                            )
                                                   .ToListAsync();

                var materialStageMasters = await _context.MaterialStageMaster.Select(msm => new { msm.ID, msm.Name, msm.Colour, msm.Order}).Distinct().ToListAsync();
                foreach (var materialStageMaster in materialStageMasters) {
                    List<MaterialStageAudit> materialStageAudits = materialStages.Where(msa => msa.Stage.ID == materialStageMaster.ID).ToList();
                    MaterialStage materialStage = new MaterialStage
                    {
                        ID = materialStageMaster.ID,
                        Name = materialStageMaster.Name,
                        Colour = materialStageMaster.Colour,
                        Order = materialStageMaster.Order,
                        MaterialCount = materialStageAudits.Count()
                    };
                    materialCounts.Add(materialStage);
                }

                return materialCounts.OrderBy(sc => sc.Order).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // POST: /materials/{material_id}/update-stage
        [HttpPost("materials/{id}/update-stage")]
        public async Task<int> UpdateMaterialStage([FromRoute] int id, [FromBody] MaterialUpdate materialUpdate)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            UserMaster user = await _context.GetUserFromHttpContext(HttpContext);

            if (user == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found!");

            MaterialMaster material = await GetMaterialMaster(id);

            if (material == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "The Tracker cannot be found. This might be a new Tracker or it is still not past the inventory stage.");

            MaterialStageMaster finalStage = await GetLastStage();//_context.MaterialStageMaster.Where(sm => sm.MilestoneId == 3).FirstOrDefaultAsync();

            MaterialStageAudit newStageAudit = CreateNewMaterialStage(material, materialUpdate, user.ID);
            material.StageAudits.Add(newStageAudit);

            await DisassociateFinalStageMaterial(material, materialUpdate.StageID, finalStage.ID);

            material = UpdateMRFCompletion(material, finalStage.ID);

            material.MRF.UpdatedDate = DateTime.UtcNow;
            material.MRF.UpdatedBy = user;

            if (materialUpdate.CastingDate.HasValue)
                material.CastingDate = materialUpdate.CastingDate.Value;

            await _context.SaveChangesAsync();

            if (material.MRF.MRFCompletion == 1)
            {
                List<UserMaster> recipients = material.MRF.UserMRFAssociations.Select(p => p.User).ToList();
                await UpdateNotificationAudit(recipients, 5, 0, material.MRF.ID.ToString());
            }
            MaterialStageMaster stageMaster = await Getstages(materialUpdate.StageID);
            List<UserMaster> getSubconList = Getsubconreceipient(material.ID);
            if (stageMaster.MilestoneID == 2 && (getSubconList != null || getSubconList.Count() >0 || getSubconList.Count()!=0))
            {
                await UpdateNotificationAudit(getSubconList, 17, 0, material.MRF.ID.ToString());
            }

            return newStageAudit.ID;
        }
     
    // POST: /materials/update-stage-batch"
    [HttpPost("materials/update-stage-batch")]
        public async Task<int> BatchUpdateMaterialStage([FromBody] MaterialBatchUpdate materialUpdate)
        {
            var stageAudits = new List<MaterialStageAudit>();
            try
            {
                if (!ModelState.IsValid || materialUpdate == null || materialUpdate.MaterialIDs == null)
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);

                if (user == null)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found!");

                List<MaterialMaster> materials = await GetMaterialMaster(materialUpdate.MaterialIDs);

                if (materials.Count() == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "Materials cannot be found!");

                /*
                var finalStage = await _context.MaterialStageMaster.Where(sm => sm.MilestoneId == 3).FirstOrDefaultAsync();
                */

                var now = DateTimeOffset.Now;

                foreach (var material in materials)
                {
                    var newStageAudit = new MaterialStageAudit
                    {
                        MaterialID = material.ID,
                        LocationID = materialUpdate.LocationID,
                        StageID = materialUpdate.StageID,
                        CreatedByID = user.ID,
                        CreatedDate = now
                    };
                    stageAudits.Add(newStageAudit);
                }

                using (var transaction = _context.Database.BeginTransaction())
                {
                    await _context.BulkInsertAsync(stageAudits);

                    if (materialUpdate.CastingDate.HasValue)
                    {
                        foreach (var mm in materials)
                        {
                            mm.CastingDate = materialUpdate.CastingDate.Value;
                            _context.Update(mm);
                        }
                        await _context.SaveChangesAsync();

                        //await _context.BulkUpdateAsync(materials);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return stageAudits.Count;
        }

        // POST: /materials/{material_id}/update-stage
        [HttpPost("materials/{id}/update-location")]
        public async Task<int> UpdateMaterialLocation([FromRoute] int id, [FromBody] MaterialUpdate materialUpdate)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MaterialMaster material = await GetMaterialMaster(id);

            //_context.MaterialMaster.Include(m => m.Tracker)
            //    .Include(m => m.MRF).ThenInclude(m => m.UserMRFAssociations).ThenInclude(una => una.User)
            //                            .Include(m => m.StageAudits)
            //                            .ThenInclude(sa => sa.Stage)
            //                            .Include(m => m.MRF)
            //                            .Where(m => m.ID == id)
            //                            .FirstOrDefault();

            LocationMaster location = await _context.LocationMaster.FindAsync(materialUpdate.LocationID);

            MaterialStageAudit currStage = material.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault();

            if (material == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "Materials cannot be found!");
            else if (currStage == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "The material does not have any history.");
            else if (location == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "The location cannot be found.");
            else if (currStage.LocationID == materialUpdate.LocationID)
                throw new GenericException(ErrorMessages.DbDuplicateRecord, "The location cannot be the same as the current location.");

            try
            {
                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);

                if (user == null)
                    throw new GenericException(ErrorMessages.DbDuplicateRecord, "User not found.");

                MaterialStageAudit newStage = CreateMaterialStageAudit(materialUpdate.LocationID, currStage.StageID, user.ID);
                material.StageAudits.Add(newStage);

                await _context.SaveChangesAsync();

                return newStage.ID;
            }
            catch (Exception ex)
            {
                throw new GenericException(504, ExceptionUtility.GetExceptionDetails(ex));
            }
        }

        // GET: /materials/{material_id}/next-stage
        [HttpGet("materials/{material_id}/next-stage")]
        public async Task<MaterialStage> GetNextMaterialStageAudit([FromRoute] int material_id)
        {
            MaterialMaster materialMaster = await _context.MaterialMaster
                                                       .Include(mm => mm.StageAudits)
                                                       .ThenInclude(sa => sa.Stage)
                                                       .Include(mm => mm.MaterialType)
                                                       .Where(mm => mm.ID == material_id)
                                                       .FirstOrDefaultAsync();


            if (materialMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, $"Material not found.");

            MaterialStageAudit currStage = materialMaster.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault();

            MaterialStage nextStage = await GetNextStage(currStage, materialMaster);

            if (nextStage == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, $"Material does not have a next stage.");

            return nextStage;
        }

        // PUT: MaterialStageAudits/5
        [HttpPut("MaterialStageAudits/{material_id}")]
        public async Task<IActionResult> PutMaterialStageAudit([FromRoute] int material_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MaterialMaster material = await _context.MaterialMaster.Include(m => m.StageAudits).FirstOrDefaultAsync();

            if (material_id != material.ID)
                throw new GenericException(ErrorMessages.DbRecordNotFound, $"Material with Id {material_id} not found.");

            InventoryAudit inventoryAudit = await _context.InventoryAudit.Where(i => i.MarkingNo == material.MarkingNo && i.Tracker == null).FirstOrDefaultAsync();
            inventoryAudit.Tracker = material.Trackers?.FirstOrDefault();

            material.StageAudits = null;
            _context.Entry(material).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MaterialStageAuditExists(material_id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // GET: api/MaterialStageAudits/5
        [HttpGet("MaterialStageAudits/{id}")]
        public async Task<MaterialStageAudit> GetMaterialStageAudit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MaterialStageAudit materialStageAudit = await _context.MaterialStageAudit.SingleOrDefaultAsync(m => m.ID == id);

            if (materialStageAudit == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, $"Material Stage with ID {id} not found.");

            return materialStageAudit;
        }

        // DELETE: MaterialStageAudits/5?tracker_id={tracker_id}
        [HttpDelete("MaterialStageAudits/{id}")]
        public async Task<MaterialStageAudit> DeleteMaterialStageAudit([FromRoute] int id, [FromQuery] int tracker_id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            MaterialStageAudit materialStageAudit = await _context.MaterialStageAudit.SingleOrDefaultAsync(m => m.ID == id);

            if (materialStageAudit == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, $"Material Stage with ID {id} not found.");

            MaterialMaster materialMaster = _context.MaterialMaster.Include(m => m.Trackers).Where(m => m.Trackers.FirstOrDefault().ID == tracker_id).FirstOrDefault();
            InventoryAudit inventoryAudit;
            if (materialMaster != null)
            {
                inventoryAudit = _context.InventoryAudit.Where(i => i.SN == materialMaster.SN).FirstOrDefault();
                inventoryAudit.Tracker = materialMaster.Trackers?.FirstOrDefault();
                materialMaster.Trackers = null;
            }

            _context.MaterialStageAudit.Remove(materialStageAudit);
            await _context.SaveChangesAsync();

            return materialStageAudit;
        }


        protected string CreateIDsListString(List<int> list)
        {
            if (list.Count == 0)
                return null;
            string strList = "";
            foreach (int i in list)
                strList = strList + i.ToString() + ", ";

            return strList.Substring(0, strList.Length - 2);
        }

        protected MaterialNextStageInfo CreateMaterialNextStageInfo(int materialID)
        {
            MaterialStageMaster nextStage = _context.MaterialStageMaster.Where(s => s.Order == getNextStageOrder(materialID)).FirstOrDefault();
            if (nextStage == null)
                return null;

            MaterialNextStageInfo materialNextStageInfo = new MaterialNextStageInfo();

            materialNextStageInfo.MaterialID = materialID;
            materialNextStageInfo.NextStageName = nextStage.Name;
            materialNextStageInfo.NextStageColour = nextStage.Colour;
            return materialNextStageInfo;
        }

        protected int getNextStageOrder(int materialID)
        {
            MaterialMaster materialMaster = _context.MaterialMaster
                                            .Where(m => m.ID == materialID)
                                            .Include(m => m.StageAudits)
                                            .ThenInclude(sa => sa.Stage)
                                            .FirstOrDefault();

            var stageAudits = materialMaster.StageAudits.OrderByDescending(sa => sa.CreatedDate).ToList();
            int stageOrder = stageAudits.FirstOrDefault().Stage.Order;
            var currStage = stageAudits.FirstOrDefault();

            return stageOrder;
        }

        protected void UpdateMRFCompletion(string MRFNo)
        {
            MRFMaster mrfMaster = _context.MRFMaster.Where(m => m.MRFNo == MRFNo).FirstOrDefault();

            var installedStage = _context.MaterialStageMaster.FirstOrDefault(ms => ms.MilestoneID == 3);

            IEnumerable<MaterialMaster> materialsInMRF = _context.MaterialMaster.Include(m => m.StageAudits).Include(m => m.MRF).Where(m => m.MRF.MRFNo == MRFNo);
            double installedMaterialCount = materialsInMRF.Where(m => m.StageAudits.Any(sa => sa.StageID == installedStage?.ID)).Count();

            mrfMaster.MRFCompletion = installedMaterialCount / materialsInMRF.Count() * 100;
            //_context.Entry(mrfMaster).State = EntityState.Modified;
        }

        protected void UpdateInventoryAudit(InventoryAudit inventoryAudit)
        {
            inventoryAudit.Tracker = null;
            _context.Entry(inventoryAudit).State = EntityState.Modified;
        }

        protected MaterialMaster UpdateMaterialMaster(MaterialMaster material, TrackerMaster tracker, int sn)
        {
            material.Trackers.Add(tracker);
            material.SN = sn;
            _context.Entry(material).State = EntityState.Modified;

            return material;
        }

        protected async Task<bool> checkIfStageAuditExist(int materialID, int stageID)
        {
            MaterialMaster material = await _context.MaterialMaster.Include(m => m.StageAudits).ThenInclude(sa => sa.Stage)
                .Where(m => m.ID == materialID).FirstOrDefaultAsync();

            foreach (MaterialStageAudit s in material.StageAudits)
            {
                List<MaterialStageAudit> stage = await _context.MaterialStageAudit.Include(sa => sa.Stage).Where(sa => sa.Stage.ID == stageID).ToListAsync();
                if (stage != null)
                    return true;
            }

            return false;
        }

        protected MaterialStageAudit CreateNewMaterialStage(MaterialMaster material, MaterialUpdate materialUpdate, int userID)
        {
            return new MaterialStageAudit
            {
                StageID = materialUpdate.StageID,
                LocationID = materialUpdate.LocationID,
                CreatedByID = userID,
                CreatedDate = DateTimeOffset.UtcNow
            };
        }

        protected async Task<MaterialMaster> DisassociateFinalStageMaterial(MaterialMaster material, int materialUpdateStageID, int finalStageID)
        {
            if (materialUpdateStageID == finalStageID)
                material.Trackers.Remove(material.Trackers.Where(t => t.Type == "RFID").FirstOrDefault());

            return material;
        }

        protected MaterialMaster UpdateMRFCompletion(MaterialMaster material, int finalStageID)
        {
            double countInstalled = 0;
            foreach (var materialUnderMRF in material.MRF.Materials)
                if (materialUnderMRF.StageAudits.Any(sa => sa.StageID == finalStageID))
                    countInstalled++;

            material.MRF.MRFCompletion = countInstalled / material.MRF.Materials.Count;

            return material;
        }

        protected async Task<MaterialMaster> GetMaterialMaster(int id)
        {
            return await _context.MaterialMaster
                        .Include(m => m.Trackers)
                        .Include(m => m.StageAudits)
                        .ThenInclude(sa => sa.Stage)
                        .Include(m => m.MRF)
                        .ThenInclude(mrf => mrf.UserMRFAssociations)
                        .ThenInclude(una => una.User)
                        .Include(m => m.MRF)
                        .ThenInclude(mrf => mrf.Materials)
                        .ThenInclude(m => m.StageAudits)
                        .ThenInclude(sa => sa.Stage)
                        .Where(m => m.ID == id)
                        .FirstOrDefaultAsync();
        }

        protected async Task<List<MaterialMaster>> GetMaterialMaster(int[] materialIDs)
        {
            return await _context.MaterialMaster
                           .Include(m => m.Trackers)
                           .Include(m => m.StageAudits)
                           .ThenInclude(sa => sa.Stage)
                           .Include(m => m.MaterialType)
                           .Include(m => m.Project)
                           .Include(m => m.MRF)
                           .ThenInclude(mrf => mrf.UserMRFAssociations)
                           .ThenInclude(una => una.User)
                           .Include(m => m.MRF)
                           .ThenInclude(mrf => mrf.Materials)
                           .ThenInclude(m => m.StageAudits)
                           .ThenInclude(sa => sa.Stage)
                           .Where(mm => materialIDs.Contains(mm.ID))
                           .ToListAsync();
        }

        protected MaterialStageAudit CreateMaterialStageAudit(int locationID, int currStageID, int userID)
        {
            return new MaterialStageAudit
            {
                LocationID = locationID,
                StageID = currStageID,
                CreatedByID = userID,
                CreatedDate = DateTimeOffset.Now
            };
        }

        protected async Task<MaterialStage> GetNextStage(MaterialStageAudit currStage, MaterialMaster materialMaster)
        {
            if (currStage == null)
                return await _context.MaterialStageMaster
                                     .Where(sm => sm.MilestoneID == (int)Enums.MileStoneID.Produced)
                                     .Select(sm => new MaterialStage { 
                                                                        ID = sm.ID, 
                                                                        Name = sm.Name,
                                                                        Order = sm.Order,
                                                                        Colour = sm.Colour,
                                                                        MilestoneID = sm.MilestoneID,
                                                                        IsEditable = sm.IsEditable, 
                                                                        MaterialTypes = sm.MaterialTypes
                                                                     }
                                            )
                                     .FirstOrDefaultAsync();
            else
                return await _context.MaterialStageMaster
                                          .Where(sm => sm.Order > currStage.Stage.Order && sm.MaterialTypes.Contains(materialMaster.MaterialType.Name))
                                          .Select(sm => new MaterialStage {
                                                                            ID = sm.ID,
                                                                            Name = sm.Name,
                                                                            Order = sm.Order,
                                                                            Colour = sm.Colour,
                                                                            MilestoneID = sm.MilestoneID,
                                                                            IsEditable = sm.IsEditable,
                                                                            MaterialTypes = sm.MaterialTypes
                                                                          }
                                                 )
                                          .OrderBy(sm => sm.Order)
                                          .FirstOrDefaultAsync();
        }

        protected bool MaterialStageAuditExists(int id)
        {
            return _context.MaterialStageAudit.Any(e => e.ID == id);
        }
    }
}