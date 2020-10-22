using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using Microsoft.EntityFrameworkCore;

namespace astorWorkBackEnd.Common
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
        }

        protected async Task<MaterialStageAudit> CreateMaterialStageAuditInDb(MaterialMaster material, MaterialStageMaster nextStage, MaterialUpdate producedMaterial)
        {
            var user = _context.GetUserFromHttpContext(HttpContext);

            var materialStageAudit = new MaterialStageAudit
            {
                MaterialMasterID = material.ID,
                StageID = nextStage.ID,
                StagePassed = producedMaterial.QCStatus,
                Remarks = producedMaterial.QCRemarks,
                Location = _context.LocationMaster.Where(l => l.ID == producedMaterial.LocationID).FirstOrDefault(),
                CreatedByID = user.ID,
                CreatedDate = DateTime.Now
            };

            await _context.MaterialStageAudit.AddAsync(materialStageAudit);
            /*
            if (material.StageAudits == null)
                material.StageAudits = new List<MaterialStageAudit>();

            material.StageAudits.Add(materialStageAudit);
            */

            // Disassociate the tag if it is final stage
            if (nextStage.ID == installedStageId)
            {
                material.Tracker = null;
                UpdateMRFCompletion(material.MRF.MRFNo);
            }

            //await _context.SaveChangesAsync();
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
            materialNextStageInfo.NextStageQC = nextStage.IsQCStage;
            return materialNextStageInfo;
        }

        protected int getNextStageOrder(int materialID)
        {
            MaterialMaster materialMaster = _context.MaterialMaster
                                            .Where(m => m.ID == materialID)
                                            .Include(m => m.StageAudits)
                                            .ThenInclude(sa => sa.Stage)
                                            .FirstOrDefault();

            if (materialMaster.StageAudits == null || materialMaster.StageAudits.Count < 1)
                return -1;

            var stageAudits = materialMaster.StageAudits.OrderByDescending(sa => sa.CreatedDate).ToList();
            int stageOrder = stageAudits.FirstOrDefault().Stage.Order;
            var currStage = stageAudits.FirstOrDefault();

            if (!currStage.Stage.IsQCStage || currStage.StagePassed)
            {
                var nextStage = _context.MaterialStageMaster
                    .Where(sa => sa.Order > stageOrder && sa.MaterialTypes.Contains(materialMaster.MaterialType))
                    .OrderBy(sa => sa.Order).FirstOrDefault();
                if (nextStage != null)
                    stageOrder = nextStage.Order;
                //stageOrder++;
            }

            return stageOrder;
        }

        protected async Task<MaterialStageAudit> UpdateMaterialStage(MaterialMaster material, MaterialUpdate materialUpdate)
        {
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
                StagePassed = materialUpdate.QCStatus,
                Remarks = materialUpdate.QCRemarks,
                LocationId = materialUpdate.LocationID,
                CreatedByID = user.ID,
                CreatedDate = DateTimeOffset.Now
            };
            // Only advance material to next stage if QC passed
            if (currStageAudit.StagePassed)
            {
                newStageAudit.StageID = nextStage.ID;

                // await _context.MaterialStageAudit.AddAsync(materialStageAudit);
                /*
                if (material.StageAudits == null)
                    material.StageAudits = new List<MaterialStageAudit>();

                material.StageAudits.Add(materialStageAudit);
                */

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
        }

        protected void UpdateMRFCompletion(string MRFNo)
        {
            MRFMaster mrfMaster = _context.MRFMaster.Where(m => m.MRFNo == MRFNo).FirstOrDefault();

            IEnumerable<MaterialMaster> materialsInMRF = _context.MaterialMaster.Include(m => m.StageAudits).Include(m => m.MRF).Where(m => m.MRF.MRFNo == MRFNo);
            double installedMaterialCount = materialsInMRF.Where(m => m.StageAudits.Any(sa => sa.StageID == installedStageId)).Count();

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
            material.Tracker = tracker;
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
        protected bool MaterialStageAuditExists(int id)
        {
            return _context.MaterialStageAudit.Any(e => e.ID == id);
        }
    }
}