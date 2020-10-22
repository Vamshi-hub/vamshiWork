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
    [Route("api/CommonMaterialMasters")]
    public class CommonMaterialMastersController : CommonController
    {
        public CommonMaterialMastersController(astorWorkDbContext context) : base(context)
        {
        }

        protected List<TrackingHistory> createMaterialTrackingHistory(MaterialMaster mm)
        {
            List<MaterialStageAudit> stageAudits = mm.StageAudits.OrderBy(sa => sa.Stage.Order).ToList();

            var result = new List<TrackingHistory>();

            foreach (var msa in stageAudits)
            {
                int ID = msa.ID;

                var operQCCase = msa.QCCases.Where(q => q.Defects.Any(d => d.IsOpen)).FirstOrDefault();
                TrackingHistory trackingHistory = new TrackingHistory();

                trackingHistory.ID = msa.ID;
                trackingHistory.StageName = msa.Stage.Name;
                trackingHistory.StageStatus = getStageStatus(msa);
                trackingHistory.CreatedBy = msa.CreatedBy.UserName;
                trackingHistory.CreatedDate = msa.CreatedDate;
                trackingHistory.IsQCStage = msa.Stage.IsQCStage;
                if (msa.Location != null)
                    trackingHistory.Location = msa.Location.Name;
                if (!string.IsNullOrEmpty(msa.Remarks))
                    trackingHistory.Remarks = msa.Remarks;
                trackingHistory.OpenQCCaseId = operQCCase == null ? 0 : operQCCase.ID;
                trackingHistory.CountQCCase = operQCCase == null ? 0 : msa.QCCases.Count;
                trackingHistory.CountClosedDefect = operQCCase == null ? 0 : operQCCase.Defects.Where(d => !d.IsOpen).Count();
                trackingHistory.CountOpenDefect = operQCCase == null ? 0 : operQCCase.Defects.Where(d => d.IsOpen).Count();
                result.Add(trackingHistory);
            }

            int currentOrder = stageAudits.Count > 0 ? stageAudits.Max(s => s.Stage.Order) : 0;

            IEnumerable<MaterialStageMaster> materialStageMasters = _context.MaterialStageMaster.Where(msm => msm.Order > currentOrder).OrderBy(msm => msm.Order);

            foreach (var stageMaster in materialStageMasters)
            {
                var materialTrackingHistory = new TrackingHistory
                {
                    ID = stageMaster.Order,
                    StageName = stageMaster.Name,
                    StageStatus = 0
                };
                result.Add(materialTrackingHistory);
            }
            /*

            string maxStageName = _context.MaterialStageMaster.OrderByDescending(s => s.Order).FirstOrDefault().Name;

            order++;

            TrackingHistory materialTrackingHistory = new TrackingHistory();

            while (materialTrackingHistory.StageName != maxStageName)
            {
                var sm = _context.MaterialStageMaster
                         .Where(s => s.Order == order).FirstOrDefault();

                materialTrackingHistory = new TrackingHistory();
                materialTrackingHistory.ID = order;
                materialTrackingHistory.StageName = sm.Name;
                materialTrackingHistory.StageStatus = 0;
                result.Add(materialTrackingHistory);

                order++;
            }
            */
            return result;
        }

        protected int getStageStatus(MaterialStageAudit materialStageAudit)
        {
            if (materialStageAudit.StagePassed)
                return 1;
            else {  // QC Status fail
                foreach (MaterialQCCase qcCase in materialStageAudit.QCCases) {
                    foreach (MaterialQCDefect qcDefect in qcCase.Defects) {
                        if (qcDefect.IsOpen)    // Have open QC defects
                            return 2;
                    }
                }
            }

            return 3;
        }

        protected MaterialDetail CreateMaterialDetail(int materialID)
        {
            MaterialMaster materialMaster = _context.MaterialMaster
                            .Include(m => m.Vendor)
                            .Include(m => m.Tracker)
                            .Include(m => m.StageAudits)
                                .ThenInclude(sa => sa.Location)
                            .Include(m => m.StageAudits)
                                .ThenInclude(sa => sa.QCCases).ThenInclude(d => d.Defects)
                            .Include(m => m.StageAudits)
                                .ThenInclude(sa => sa.Stage)
                            .Include(m => m.StageAudits)
                                .ThenInclude(sa => sa.CreatedBy)
                            .Include(m => m.MRF)
                            .Include(m => m.DrawingAssociations)
                            .ThenInclude(da => da.Drawing)
                            .FirstOrDefault(m => m.ID == materialID);

            MaterialDetail materialDetail = CreateMaterialInfo(materialID, materialMaster);
            materialDetail = CreateMaterialRemarksAndExpectedDeliveryDate(materialDetail, materialMaster);
            materialDetail.TrackingHistory = createMaterialTrackingHistory(materialMaster);

            return materialDetail;
        }

        protected MaterialDetail CreateMaterialInfo(int materialID, MaterialMaster materialMaster)
        {
            MaterialDetail materialDetail = new MaterialDetail();

            materialDetail.ID = materialMaster.ID;
            materialDetail.MarkingNo = materialMaster.MarkingNo;
            materialDetail.Block = materialMaster.Block;
            materialDetail.Level = materialMaster.Level;
            materialDetail.Zone = materialMaster.Zone;
            materialDetail.MaterialType = materialMaster.MaterialType;
            materialDetail.VendorName = materialMaster.Vendor?.Name;
            materialDetail.TrackerType = materialMaster.Tracker?.Type;
            materialDetail.TrackerTag = materialMaster.Tracker?.Tag;
            materialDetail.TrackerLabel = materialMaster.Tracker?.Label;
            materialDetail.CastingDate = materialMaster.CastingDate;
            if (materialMaster.MRF != null)
                materialDetail.OrderDate = materialMaster.MRF.OrderDate;

            var drawingAssociation = materialMaster.DrawingAssociations
                                        .OrderByDescending(a => a.Drawing.DrawingIssueDate)
                                        .FirstOrDefault();
            if (drawingAssociation != null)
                materialDetail.Drawing = drawingAssociation.Drawing;

            return materialDetail;
        }

        protected MaterialDetail CreateMaterialRemarksAndExpectedDeliveryDate(MaterialDetail materialDetail, MaterialMaster materialMaster)
        {
            if (materialMaster.MRF != null)
                materialDetail.ExpectedDeliveryDate = materialMaster.MRF.ExpectedDeliveryDate;

            MaterialInfoAudit materialInfoAudit = _context.MaterialInfoAudit.FirstOrDefault(m => m.Material.ID == materialMaster.ID);

            if (materialInfoAudit != null)
            {
                materialDetail.Remarks = materialInfoAudit.Remarks;
                materialDetail.ExpectedDeliveryDate = materialInfoAudit.ExpectedDeliveryDate;
            }

            return materialDetail;
        }

        protected IEnumerable<MaterialStageMaster> AddNewMaterialToAllStages(string materialType) {
            IEnumerable<MaterialStageMaster> stages = GetAllStages();

            foreach (MaterialStageMaster stage in stages) 
                stage.MaterialTypes += "," + materialType;

            return stages;
        }

        protected bool MaterialTypeExists(string materialType) {
            IEnumerable<MaterialMaster> materialMasters = _context.MaterialMaster.Where(m => m.MaterialType.Contains(materialType));

            if (materialMasters == null)
                return false;

            return (materialMasters.Count() > 0);
        }

        protected IEnumerable<MaterialStageMaster> GetAllStages() {
            return _context.MaterialStageMaster;
        }

        protected bool MaterialMasterExists(int id)
        {
            return _context.MaterialMaster.Any(e => e.ID == id);
        }
    }
}