using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using Microsoft.EntityFrameworkCore;

namespace astorWorkMaterialTracking.Common
{
    [Produces("application/json")]
    [Route("api/CommonMaterialStageAudits")]
    public class CommonMaterialStageAuditsController : CommonController
    {
        public CommonMaterialStageAuditsController(astorWorkDbContext context) : base(context)
        {
        }

        protected async Task<MaterialStageAudit> UpdateInventoryToNextStageInDb(MaterialMaster material, InventoryAudit inventoryAudit, MaterialUpdate producedMaterial)
        {
            /*
            bool qcPassed = producedMaterial.QCStatus;

            int SN = material.SN;
            TrackerMaster tracker = _context.TrackerMaster.Find(producedMaterial.TrackerID);
            if (inventoryAudit != null)
            {
                tracker = inventoryAudit.Tracker;
                UpdateInventoryAudit(inventoryAudit);
                SN = inventoryAudit.SN;
            }

            material = UpdateMaterialMaster(material, tracker, SN);

            MaterialStageAudit materialStageAudit = new MaterialStageAudit();
            var allStages = _context.MaterialStageMaster.OrderBy(msm => msm.Order).ToList();
            if (material.StageAudits == null)
            {
                MaterialStageMaster nextStage = allStages.ElementAtOrDefault(0);

                if (nextStage != null)
                    materialStageAudit = await CreateMaterialStageAuditInDb(material, nextStage, producedMaterial);
            }

            if (qcPassed)
            {
                MaterialStageMaster nextStage = allStages.ElementAtOrDefault(1);

                if (nextStage != null)
                    materialStageAudit = await CreateMaterialStageAuditInDb(material, nextStage, producedMaterial);
            }
            await _context.SaveChangesAsync();
            return materialStageAudit;
            */
            return null;
        }

        protected async Task<MaterialStageAudit> CreateMaterialStageAuditInDb(MaterialMaster material, MaterialStageMaster nextStage, MaterialUpdate producedMaterial)
        {
            /*
            var user = _context.GetUserFromHttpContext(HttpContext);

            var materialStageAudit = new MaterialStageAudit
            {
                MaterialMasterID = material.ID,
                StageID = nextStage.ID,
                Remarks = producedMaterial.QCRemarks,
                Location = _context.LocationMaster.Where(l => l.ID == producedMaterial.LocationId).FirstOrDefault(),
                CreatedByID = user.ID,
                CreatedDate = DateTime.Now
            };

            await _context.MaterialStageAudit.AddAsync(materialStageAudit);

            // Disassociate the tag if it is final stage
            if (nextStage.ID == installedStageId)
            {
                material.Tracker = null;
                UpdateMRFCompletion(material.MRF.MRFNo);
            }
            
            return materialStageAudit;
            */
            return null;
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

            /*
            if (materialMaster.StageAudits == null || materialMaster.StageAudits.Count < 1)
                return -1;
            */

            var stageAudits = materialMaster.StageAudits.OrderByDescending(sa => sa.CreatedDate).ToList();
            int stageOrder = stageAudits.FirstOrDefault().Stage.Order;
            var currStage = stageAudits.FirstOrDefault();
            /*
            if (!currStage.Stage.IsQCStage || currStage.StagePassed)
            {
                var nextStage = _context.MaterialStageMaster
                    .Where(sa => sa.Order > stageOrder && sa.MaterialTypes.Contains(materialMaster.MaterialType))
                    .OrderBy(sa => sa.Order).FirstOrDefault();
                if (nextStage != null)
                    stageOrder = nextStage.Order;
                //stageOrder++;
            }
            */

            return stageOrder;
        }

        protected async Task<MaterialStageAudit> UpdateMaterialStage(MaterialMaster material, MaterialUpdate materialUpdate)
        {
            /*
            bool qcPassed = materialUpdate.QCStatus;

            int nextStageOrder = getNextStageOrder(material.ID);

            if (nextStageOrder < 0)
                nextStageOrder = 1;

            var nextStage = _context.MaterialStageMaster.Where(s => s.Order == nextStageOrder).FirstOrDefault();

            var currStageAudit = material.StageAudits
                .OrderByDescending(sa => sa.CreatedDate)
                .ThenByDescending(sa => sa.Stage.Order)
                .FirstOrDefault();

            var user = _context.GetUserFromHttpContext(HttpContext);

            var newStageAudit = new MaterialStageAudit
            {
                MaterialMasterID = material.ID,
                StageID = currStageAudit.StageID,
                Remarks = materialUpdate.QCRemarks,
                LocationId = materialUpdate.LocationID,
                CreatedByID = user.ID,
                CreatedDate = DateTimeOffset.Now
            };
            // Only advance material to next stage if QC passed
            if (currStageAudit.StagePassed)
            {
                newStageAudit.StageID = nextStage.ID;

                // Disassociate the tag if it is final stage
                if (nextStage.ID == installedStageId)
                {
                    material.Tracker = null;
                    UpdateMRFCompletion(material.MRF.MRFNo);
                }
            }
            else
            {
                //material.StageAudits.Add(currStageAudit);
            }
            //_context.Entry(newStageAudit).State = EntityState.Added;
            material.StageAudits.Add(newStageAudit);

            await _context.SaveChangesAsync();
            return newStageAudit;
            */
            return null;
        }

        protected async Task UpdateMRFCompletion(string MRFNo)
        {
            await SetMaterialStagesID();

            MRFMaster mrf = await _context.MRFMaster.Where(m => m.MRFNo == MRFNo).FirstOrDefaultAsync();

            List<MaterialMaster> materialsInMRF = await _context.MaterialMaster
                                                                .Include(m => m.StageAudits)
                                                                .Include(m => m.MRF)
                                                                .Where(m => m.MRF.MRFNo == MRFNo)
                                                                .ToListAsync();
            double installedMaterialsCount = materialsInMRF.Where(m => m.StageAudits.Any(sa => sa.StageID == installedStageID))
                                                           .Count();

            mrf.MRFCompletion = installedMaterialsCount / materialsInMRF.Count() * 100;
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

        protected bool checkIfStageAuditExist(int materialID, int stageID)
        {
            var material = _context.MaterialMaster.Include(m => m.StageAudits).ThenInclude(sa => sa.Stage)
                .Where(m => m.ID == materialID).FirstOrDefault();

            foreach (MaterialStageAudit s in material.StageAudits)
            {
                var stage = _context.MaterialStageAudit.Include(sa => sa.Stage).Where(sa => sa.Stage.ID == stageID);
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

        protected async Task<List<MaterialMaster>> GetMaterialMaster(int[] materialIDs) {
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
                           .Where(mm => materialIDs.Contains(mm.ID))
                           .ToListAsync();
        }

        protected MaterialStageAudit CreateMaterialStageAudit(int locationID , int currStageID, int userID) {
            return new MaterialStageAudit
            {
                LocationID = locationID,
                StageID = currStageID,
                CreatedByID = userID,
                CreatedDate = DateTimeOffset.Now
            };
        }

        protected async Task<MaterialStageMaster> GetNextStage(MaterialStageAudit currStage, MaterialMaster materialMaster) {
            if (currStage == null)
                return await _context.MaterialStageMaster.Where(sm => sm.MilestoneId == 1).FirstOrDefaultAsync();
            else
                return await _context.MaterialStageMaster
                                          .Where(sm => sm.Order > currStage.Stage.Order && sm.MaterialTypes.Contains(materialMaster.MaterialType.Name))
                                          .OrderBy(sm => sm.Order)
                                          .FirstOrDefaultAsync();
        }

        protected bool MaterialStageAuditExists(int id)
        {
            return _context.MaterialStageAudit.Any(e => e.ID == id);
        }
    }
}