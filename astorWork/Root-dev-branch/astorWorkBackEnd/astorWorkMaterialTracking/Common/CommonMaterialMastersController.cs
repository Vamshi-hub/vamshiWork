using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkShared.Utilities;

namespace astorWorkMaterialTracking.Common
{
    public class CommonMaterialMastersController : CommonController
    {
        public CommonMaterialMastersController(astorWorkDbContext context) : base(context)
        {
        }

        protected TrackingHistory CreateStageTrackingHistory(MaterialStageAudit materialStageAudit) {
            return new TrackingHistory()
            {
                ID = materialStageAudit.ID,
                StageName = materialStageAudit.Stage.Name,
                StageStatus = 1,
                Location = materialStageAudit.Location.Name,
                CreatedBy = materialStageAudit.CreatedBy.UserName,
                CreatedDate = materialStageAudit.CreatedDate,
                Remarks = materialStageAudit.Remarks,
                IsQCStage = false,
            };
        }

        protected IEnumerable<TrackingHistory> CreateStageTrackingHistories(MaterialMaster materialMaster) {
            return (from materialStageAudit in materialMaster.StageAudits
                    select CreateStageTrackingHistory(materialStageAudit))
                                                      .OrderBy(sa => sa.CreatedDate);
        }

        protected TrackingHistory CreateCaseTrackingHistory(MaterialMaster materialMaster, IEnumerable<MaterialQCCase> materialQCCases) {
            TrackingHistory trackingHistory = new TrackingHistory();
            trackingHistory.ID = materialMaster.ID;
            trackingHistory.StageName = string.Empty;
            trackingHistory.StageStatus = materialQCCases.Where(d => d.Defects.Any(q => q.Status < Enums.QCStatus.QC_passed_by_Maincon)).Count() > 0 ? 2 : 1;
            trackingHistory.Location = string.Empty;

            trackingHistory.CreatedDate = materialQCCases.FirstOrDefault()?.CreatedDate;
            trackingHistory.CreatedBy = string.Empty;
            trackingHistory.Remarks = string.Empty;
            trackingHistory.IsQCStage = true;
            trackingHistory.OpenQCCaseIds = string.Join(",", materialQCCases.Select(q => q.ID).ToList());
            trackingHistory.CountQCCase = materialQCCases.Count();
            trackingHistory.CountQCDefects = materialQCCases.Select(q => q.Defects).Count();
            trackingHistory.CountClosedDefect = materialQCCases.Select(q => q.Defects.Any(d => d.Status == Enums.QCStatus.QC_passed_by_Maincon)).Count();
            trackingHistory.CountOpenDefect = materialQCCases.Select(q => q.Defects.Any(d => d.Status < Enums.QCStatus.QC_passed_by_Maincon)).Count();

            return trackingHistory;
        }

        protected List<TrackingHistory> AddCaseTrackingHistories(MaterialMaster materialMaster, List<TrackingHistory> trackingHistories) {
            IEnumerable<MaterialQCCase> qcCases = materialMaster.QCCases.OrderBy(qc => qc.CreatedDate);

            int trackingHistoriesStagesCount = trackingHistories.Where(t => !t.IsQCStage).Count();
            for (int i = 0; i < trackingHistoriesStagesCount; i++)
            {
                List<MaterialQCCase> materialQCCases = new List<MaterialQCCase>();
                if (i < trackingHistoriesStagesCount - 1)
                    materialQCCases = qcCases.Where(qc => qc.CreatedDate >= trackingHistories.ElementAt(i).CreatedDate
                                                       && qc.CreatedDate < trackingHistories.ElementAt(i + 1).CreatedDate).OrderBy(qc => qc.CreatedDate).ToList();
                else
                    materialQCCases = qcCases.Where(qc => qc.CreatedDate >= trackingHistories.ElementAt(i).CreatedDate).OrderBy(qc => qc.CreatedDate).ToList();

                if (materialQCCases.Count > 0)
                    trackingHistories.Add(CreateCaseTrackingHistory(materialMaster, materialQCCases));
            }

            return trackingHistories.OrderBy(t => t.CreatedDate).ToList();
        }

        protected List<TrackingHistory> AddSubsequentStages(MaterialMaster materialMaster, List<TrackingHistory> trackingHistories) {
            int currentOrder = trackingHistories.Count > 0 ? materialMaster.StageAudits.Max(s => s.Stage.Order) : 0;

            IEnumerable<MaterialStageMaster> materialStageMasters = _context.MaterialStageMaster.Where(msm => msm.Order > currentOrder).OrderBy(msm => msm.Order);

            foreach (MaterialStageMaster materialStageMaster in materialStageMasters)
            {
                TrackingHistory materialTrackingHistory = new TrackingHistory
                {
                    ID = materialStageMaster.Order,
                    StageName = materialStageMaster.Name,
                    StageStatus = 0
                };
                trackingHistories.Add(materialTrackingHistory);
            }
            return trackingHistories;
        }

        protected List<TrackingHistory> CreateMaterialTrackingHistory(MaterialMaster materialMaster)
        {
            List<TrackingHistory> trackingHistories = CreateStageTrackingHistories(materialMaster).ToList();

            //MaterialQCCase operQCCase = materialMaster.QCCases.Where(q => q.Defects.Any(d => d.IsOpen)).FirstOrDefault();
            trackingHistories = AddCaseTrackingHistories(materialMaster, trackingHistories);

            return AddSubsequentStages(materialMaster, trackingHistories);
        }

        protected MaterialDetail CreateMaterialDetail(int materialID)
        {
            MaterialMaster materialMaster = _context.MaterialMaster
                                                    .Include(m => m.Organisation)
                                                    .Include(m => m.MaterialType)
                                                    .Include(m => m.Trackers)
                                                    .Include(m => m.StageAudits)
                                                        .ThenInclude(sa => sa.Location)
                                                    .Include(m => m.QCCases)
                                                        .ThenInclude(d => d.Defects)
                                                    .Include(m => m.StageAudits)
                                                        .ThenInclude(sa => sa.Stage)
                                                    .Include(m => m.StageAudits)
                                                        .ThenInclude(sa => sa.CreatedBy)
                                                    .Include(m => m.MRF)
                                                    .Include(m => m.DrawingAssociations)
                                                        .ThenInclude(da => da.Drawing)
                                                    .Include(m => m.MaterialType)
                                                    .FirstOrDefault(m => m.ID == materialID);

            MaterialDetail materialDetail = CreateMaterialInfo(materialID, materialMaster);
            materialDetail = CreateMaterialRemarksAndExpectedDeliveryDate(materialDetail, materialMaster);
            materialDetail.TrackingHistory = CreateMaterialTrackingHistory(materialMaster);

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
            materialDetail.MaterialType = materialMaster.MaterialType.Name;
            materialDetail.OrganisationName = materialMaster.Organisation?.Name;
            materialDetail.TrackerType = materialMaster.Trackers?.FirstOrDefault()?.Type;
            materialDetail.TrackerTag = materialMaster.Trackers?.FirstOrDefault()?.Tag;
            materialDetail.TrackerLabel = materialMaster.Trackers?.FirstOrDefault()?.Label;
            materialDetail.CastingDate = materialMaster.MRF.PlannedCastingDate;
            if (materialMaster.MRF != null)
            {
                materialDetail.OrderDate = materialMaster.MRF.OrderDate;
            }

            var drawingAssociation = materialMaster.DrawingAssociations
                                        .OrderByDescending(a => a.Drawing.DrawingIssueDate)
                                        .FirstOrDefault();
            if (drawingAssociation != null)
            {
                materialDetail.Drawing = drawingAssociation.Drawing;
            }

            return materialDetail;
        }

        protected MaterialDetail CreateMaterialRemarksAndExpectedDeliveryDate(MaterialDetail materialDetail, MaterialMaster materialMaster)
        {
            if (materialMaster.MRF != null)
            {
                materialDetail.ExpectedDeliveryDate = materialMaster.MRF.ExpectedDeliveryDate;
            }

            MaterialInfoAudit materialInfoAudit = _context.MaterialInfoAudit.FirstOrDefault(m => m.Material.ID == materialMaster.ID);

            if (materialInfoAudit != null)
            {
                materialDetail.Remarks = materialInfoAudit.Remarks;
                materialDetail.ExpectedDeliveryDate = materialInfoAudit.ExpectedDeliveryDate;
            }

            return materialDetail;
        }

        protected IEnumerable<MaterialStageMaster> AddNewMaterialToAllStages(string materialType)
        {
            IEnumerable<MaterialStageMaster> stages = GetAllStages();

            foreach (MaterialStageMaster stage in stages)
            {
                stage.MaterialTypes += "," + materialType;
            }

            return stages;
        }

        protected bool MaterialTypeExists(string materialType)
        {
            IEnumerable<MaterialMaster> materialMasters = _context.MaterialMaster.Where(m => m.MaterialType.Name.Contains(materialType));

            if (materialMasters == null)
                return false;

            return (materialMasters.Count() > 0);
        }

        protected IEnumerable<MaterialStageMaster> GetAllStages()
        {
            return _context.MaterialStageMaster;
        }

        protected async Task<List<MaterialMaster>> GetMaterials(int projectID, string block) {
            return await _context.MaterialMaster
                                 .Include(m => m.MRF)
                                 .Include(m => m.Trackers)
                                 .Include(m => m.StageAudits)
                                 .ThenInclude(sa => sa.Stage)
                                 .Include(m => m.Elements)
                                 .ThenInclude(elm => elm.ForgeModel)
                                 .Include(m => m.MaterialInfoAudits)
                                 .Include(m => m.QCCases)
                                 .ThenInclude(d => d.Defects)
                                 .Include(m => m.MaterialType)
                                 .Where(m => m.ProjectID == projectID && (block == "All" ? true : m.Block == block))
                                 .ToListAsync();
        }

        protected async Task<List<MaterialDetail>> CreateMaterialList(List<MaterialMaster> materials) {
            MaterialStageMaster deliveredStage = await GetDeliveredStage();
            List<MaterialDetail> lstmaterials = new List<MaterialDetail>();
            try
            {
                lstmaterials = materials.Select(m => new MaterialDetail
                {
                    ID = m.ID,
                    MarkingNo = m.MarkingNo,
                    Block = m.Block,
                    Level = m.Level,
                    Zone = m.Zone,
                    TrackerTag = m.Trackers?.Where(t => t.Type == Enums.TagType.QR_Code.ToString().Replace('_',' ')).FirstOrDefault()?.Tag,
                    TrackerLabel = m.Trackers?.Where(t => t.Type == Enums.TagType.QR_Code.ToString().Replace('_', ' ')).FirstOrDefault()?.Label,
                    TrackerType = m.Trackers?.Where(t => t.Type == Enums.TagType.QR_Code.ToString().Replace('_', ' ')).FirstOrDefault()?.Type,
                    MaterialType = m.MaterialType.Name,
                    OpenQCCaseID = m.QCCases == null || m.QCCases.Count == 0 || m.QCCases.Where(d => d.Defects.Any(q => q.Status < Enums.QCStatus.QC_passed_by_Maincon)).Count() == 0 ? 0 : m.QCCases.Where(d => d.Defects.Any(q => q.Status < Enums.QCStatus.QC_passed_by_Maincon)).FirstOrDefault().ID,
                    MRFNo = m.MRF == null ? null : m.MRF.MRFNo,
                    ExpectedDeliveryDate = m.MaterialInfoAudits.OrderByDescending(mi => mi.CreatedDate).FirstOrDefault() == null ? (m.MRF == null ? null : m.MRF?.ExpectedDeliveryDate) :
                                           m.MaterialInfoAudits.OrderByDescending(mi => mi.CreatedDate).FirstOrDefault().ExpectedDeliveryDate,
                    StageOrder = m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault() == null ? 0 : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.Order,
                    StageName = m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault() == null ? null : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.Name,
                    StageColour = m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault() == null ? null : m.StageAudits.OrderByDescending(s => s.CreatedDate).FirstOrDefault().Stage.Colour,
                    ForgeModelURN = m.Elements.FirstOrDefault()?.ForgeModel.ObjectID,
                    ForgeElementId = m.Elements.FirstOrDefault()?.DbID,
                    DeliveryStageOrder = deliveredStage.Order
                }).ToList();
            }
            catch(Exception ex)
            {

            }
            return lstmaterials;
        }

        protected List<MaterialMaster> GetVendorMaterials(List<MaterialMaster> materials, OrganisationMaster organisation) {
            return materials.Where(m => m.Organisation == organisation).ToList();
        }

        protected async Task<MaterialInfoAudit> UpdateMaterialInfoAudit(MaterialInfoAudit materialInfoAudit, MaterialDetail materialDetail, MaterialMaster materialMaster)
        {
            // check to see if other fields get edited
            materialInfoAudit.Remarks = materialDetail.Remarks;
            materialInfoAudit.ExpectedDeliveryDate = materialDetail.ExpectedDeliveryDate == null ? Convert.ToDateTime(materialDetail.ExpectedDeliveryDate) : DateTimeOffset.Parse(materialDetail.ExpectedDeliveryDate.ToString());
            materialInfoAudit.CreatedBy = await _context.UserMaster.Where(u => u.UserName == "admin").FirstOrDefaultAsync();
            materialInfoAudit.CreatedDate = DateTime.Now;
            materialInfoAudit.Material = materialMaster;

            return materialInfoAudit;
        }

        protected async Task<List<MaterialMaster>> AddMaterials(List<MaterialMaster> materials, string[] columnsInRow, ImportMaterialMasterTemplate importTemplate, int projectID) {
            ProjectMaster project = await _context.ProjectMaster.Where(p => p.ID == projectID).FirstOrDefaultAsync();
            MaterialTypeMaster materialType = new MaterialTypeMaster
            {
                Name = importTemplate.MaterialType,
            };

            foreach (string markingNo in columnsInRow.Skip(2))
            {
                if (markingNo.Length > 0)
                {
                    materials.Add(new MaterialMaster
                    {
                        ProjectID = projectID,
                        OrganisationID = importTemplate.OrganisationID,
                        Block = importTemplate.Block,
                        MaterialType = materialType,
                        Level = columnsInRow[0],
                        Zone = columnsInRow[1],
                        MarkingNo = markingNo
                    });
                }
            }

            return materials;
        }

        protected async Task<List<MaterialMaster>> GetOrderedOrProducedMaterials(int projectID, int organisationID) {
            return await _context.MaterialMaster
                                 .Include(mm => mm.Project)
                                 .Include(mm => mm.Trackers)
                                 .Include(mm => mm.MRF)
                                 .Include(mm => mm.Organisation)
                                 .Include(mm => mm.MaterialType)
                                 .Include(mm => mm.QCCases)
                                 .ThenInclude(qc => qc.Defects)
                                 .Include(mm => mm.StageAudits)
                                 .ThenInclude(sa => sa.Location)
                                 .Include(mm => mm.StageAudits)
                                 .ThenInclude(sa => sa.Stage)
                                 .Include(mm => mm.Elements)
                                 .ThenInclude(elm => elm.ForgeModel)
                                 .Where(mm => mm.OrganisationID == organisationID &&
                                              mm.ProjectID == projectID && 
                                              mm.MRF != null &&
                                              mm.StageAudits.Count <= 1)
                                 .ToListAsync();
        }

        //protected List<TrackerAssociation> GetInventoryList(IEnumerable<MaterialMaster> materials)
        //{
        //    List<TrackerAssociation> lstTrackerAssociation = new List<TrackerAssociation>();
        //    try
        //    {
        //        lstTrackerAssociation = materials.Select(mm => new TrackerAssociation
        //        {
        //            Tracker = mm.Trackers,
        //            Material = new MaterialMobile
        //            {
        //                ID = mm.ID,
        //                Block = mm.Block,
        //                Level = mm.Level,
        //                Zone = mm.Zone,
        //                MarkingNo = mm.MarkingNo,
        //                MaterialType = mm.MaterialType.Name,
        //                OrganisationID = mm.OrganisationID,
        //                CastingDate = mm.CastingDate,
        //                OrderDate = mm.MRF.OrderDate,
        //                CurrentStage = mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault()?.Stage,
        //                CanIgnoreQC = mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault()?.Stage != null ? mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault().Stage.CanIgnoreQC : true,
        //                CountQCCase = mm.QCCases
        //                                                 .Where(qc => qc.Defects.Any(d => d.Status < Enums.QCStatus.QC_passed_by_Maincon)).Count(),
        //                MRFNo = mm.MRF.MRFNo,
        //                SN = mm.SN,
        //                CurrentLocation = mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault()?.Location,
        //                ForgeElementID = mm.Elements.FirstOrDefault()?.DbID,
        //                ForgeModelURN = mm.Elements.FirstOrDefault()?.ForgeModel.ObjectID
        //            }
        //        }
        //                                    ).ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return lstTrackerAssociation;
        //}

        protected bool MaterialMasterExists(int id)
        {
            return _context.MaterialMaster.Any(e => e.ID == id);
        }
    }
}