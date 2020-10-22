using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace astorWorkBackEnd.Common
{
    [Produces("application/json")]
    [Route("api/CommonTrackerMasters")]
    public class CommonTrackerMastersController : CommonController
    {
        public CommonTrackerMastersController(astorWorkDbContext context) : base(context)
        {
        }

        public TrackerAssociation CreateTrackerAssociation(TrackerMaster tracker, int? userProjectID = null)
        {
            TrackerAssociation trackerAssociation = new TrackerAssociation();

            trackerAssociation.ID = tracker.ID;
            trackerAssociation.TrackerLabel = tracker.Label;

            MaterialMaster materialMaster = _context.MaterialMaster
                                            .Where(m => m.Tracker == tracker)
                                            .Include(m => m.MRF)
                                            .Include(m => m.StageAudits)
                                            .ThenInclude(sa => sa.Stage)
                                            .Include(m => m.StageAudits)
                                            .ThenInclude(sa => sa.QCCases)
                                            .ThenInclude(qc => qc.Defects)
                                            .Include(m => m.StageAudits)
                                            .ThenInclude(sa => sa.Location)
                                            .Include(m => m.Elements)
                                            .ThenInclude(e => e.ForgeModel)
                                            .Include(m => m.Project)
                                            .FirstOrDefault();

            if (materialMaster != null && materialMaster.StageAudits.Count > 0)
                CreateMaterialAssociation(trackerAssociation, materialMaster, userProjectID);
            else
            {
                trackerAssociation.Inventory = _context.InventoryAudit
                                            .Where(i => i.Tracker == tracker)
                                            .FirstOrDefault();
            }

            return trackerAssociation;
        }

        public TrackerAssociation CreateMaterialAssociation(TrackerAssociation trackerAssociation, MaterialMaster materialMaster, int? userProjectID = null)
        {
            /*
            materialMaster.StageAudits = materialMaster.StageAudits
                .OrderByDescending(sa => sa.CreatedDate).ToList();
                */
            var currentStageAudit = materialMaster.StageAudits
            .OrderByDescending(sa => sa.CreatedDate).FirstOrDefault();

            var isQCOpen = currentStageAudit.QCCases.Any(qc => qc.Defects.Any(d => d.IsOpen));
            int currentStageOrder = currentStageAudit.Stage.Order;
            MaterialStageMaster nextStage = _context.MaterialStageMaster.Where(s => s.Order == currentStageOrder + 1).FirstOrDefault();
            bool nextStageIsInstalledStage = nextStage.ID == installedStageId;
            bool nextStageIsQCStage = nextStage.IsQCStage;

            int materialProjectID = materialMaster.Project.ID;

            MaterialDetail materialDetail = new MaterialDetail
            {
                ID = materialMaster.ID,
                MarkingNo = materialMaster.MarkingNo,
                Block = materialMaster.Block,
                Level = materialMaster.Level,
                Zone = materialMaster.Zone,
                MaterialType = materialMaster.MaterialType,
                StageId = currentStageAudit.ID,
                StageName = currentStageAudit.Stage.Name,
                StageColour = currentStageAudit.Stage.Colour,
                qcStatus = currentStageAudit.StagePassed,
                qcRemarks = currentStageAudit.Remarks,
                MRFNo = materialMaster.MRF.MRFNo,
                CastingDate = materialMaster.CastingDate,
                SN = materialMaster.SN,
                IsOpenQCCase = isQCOpen,
                NextStageIsQCOrInstalledStage = nextStageIsQCStage || nextStageIsInstalledStage,
                SelectedLocation = currentStageAudit.Location,
                ForgeElementId = materialMaster.Elements.FirstOrDefault()?.DbId,
                ForgeModelURN = materialMaster.Elements.FirstOrDefault()?.ForgeModel.ObjectId,
                allowUpdate = userProjectID == null?true: materialProjectID == userProjectID
            };

            trackerAssociation.Material = materialDetail;

            return trackerAssociation;
        }

        protected TrackerMaster GenerateQRCode(int index, int currentBatchNo, string trackerType, string label)
        {
            TrackerMaster qrCode = new TrackerMaster();
            if (string.IsNullOrEmpty(label))
                label = "Q";
            do
            {
                qrCode.Label = label + currentBatchNo + "-" + (index + 1);
                qrCode.Tag = Guid.NewGuid().ToString();
                qrCode.BatchNumber = currentBatchNo;
                qrCode.Type = trackerType;
            } while (TrackerMasterExists(qrCode, trackerType));

            return qrCode;
        }

        protected int GetMaxTrackerBatchNo(string trackerType)
        {
            IEnumerable<TrackerMaster> trackerMaster = _context.TrackerMaster.Where(t => t.Type == trackerType);
            if (trackerMaster.Count() > 0)
                return trackerMaster.Max(t => t.BatchNumber);

            return 0;
        }

        protected TrackerMaster CreateTracker(string trackerInfo, string type, int batchNumber)
        {
            TrackerMaster trackerMaster = new TrackerMaster();
            string[] trackersInfo = trackerInfo.Trim().Split(",");
            trackerMaster.Label = trackersInfo[0];
            trackerMaster.Tag = trackersInfo[1];
            trackerMaster.Type = type;
            trackerMaster.BatchNumber = batchNumber;

            return trackerMaster;
        }

        protected List<string> GetTrackersFromFile(IFormFile file)
        {
            var result = string.Empty;
            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                result = reader.ReadToEnd().Trim();
            }

            return result.Split('\n').ToList();
        }

        protected bool TrackerMasterExists(int id)
        {
            return _context.TrackerMaster.Any(e => e.ID == id);
        }

        protected bool TrackerMasterExists(TrackerMaster tracker, string type)
        {
            int cn = _context.TrackerMaster.Count();
            return _context.TrackerMaster.Any(e => (e.Tag == tracker.Tag || e.Label == tracker.Label) && e.Type == type);
        }
    }
}