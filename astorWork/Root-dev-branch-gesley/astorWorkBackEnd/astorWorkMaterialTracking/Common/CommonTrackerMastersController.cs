using astorWorkDAO;
using astorWorkMaterialTracking.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkMaterialTracking.Common
{
    public class CommonTrackerMastersController : CommonController
    {
        public CommonTrackerMastersController(astorWorkDbContext context) : base(context)
        {
        }

        protected TrackerMaster GenerateQRCode(MaterialMaster material, int currentBatchNo, string trackerType, string label, string qRFormat)
        {
            TrackerMaster qrCode = new TrackerMaster();
            string tag = string.Empty;
            if (string.IsNullOrEmpty(label))
                label = "Q";
            if (!string.IsNullOrEmpty(qRFormat))
            {
                tag = qRFormat;
                int i = 0;
                List<string> columns = new List<string>();
                while ((i = qRFormat.IndexOf('<', i)) != -1)
                {
                    var str = qRFormat.Substring(i);
                    columns.Add(qRFormat.Substring(i+1, str.IndexOf('>')-1));
                    i++;
                }
                var columnsdata = new List<Tuple<string,string>>();
                foreach (var item in columns)
                {
                    Type t = typeof(MaterialMaster);
                    PropertyInfo prop = t.GetProperty(item);
                    if (null != prop)
                    {
                        //columnsdata.Add(new Tuple<string, string>(item, prop.GetValue(this, null).ToString()));
                        tag = tag.Replace($"<{item}>", prop.GetValue(material, null).ToString());
                    }
                }
                //for()
                //tag = tag.Replace()

            }

            qrCode.Label = label;
            qrCode.Tag = string.IsNullOrEmpty(tag) ? Guid.NewGuid().ToString(): tag;
            qrCode.BatchNumber = currentBatchNo;
            qrCode.Type = trackerType;
            qrCode.MaterialID = material.ID;

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
            string result = string.Empty;

            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                result = reader.ReadToEnd().Trim();

            return result.Split('\n').ToList();
        }

        protected async Task<List<MaterialMaster>> GetMaterials(List<int> trackerIDs, int routeToID)
        {
            List<MaterialMaster> materials = await _context.MaterialMaster
                                                           .Include(mm=>mm.DrawingAssociations)
                                                           .ThenInclude(ds=>ds.Drawing)
                                                           .Include(mm => mm.Project)
                                                           .Include(mm => mm.Trackers)
                                                           .Include(mm => mm.MRF)
                                                           .Include(mm => mm.Organisation)
                                                           .Include(mm => mm.QCCases)
                                                           .ThenInclude(qc => qc.Defects)
                                                           .Include(mm => mm.StageAudits)
                                                           .ThenInclude(sa => sa.Location)
                                                           .Include(mm => mm.StageAudits)
                                                           .ThenInclude(sa => sa.Stage)
                                                           .ThenInclude(st=>st.checklists)
                                                           .Include(mm => mm.Elements)
                                                           .ThenInclude(elm => elm.ForgeModel)
                                                           .Include(mm => mm.MaterialType)
                                                           .Where(mm => mm.Trackers.Where(t => trackerIDs.Contains(t.ID)
                                                                                            && t.Material != null).Count() > 0)
                                                           .ToListAsync();

            /*
            List<MaterialMaster> materialMasters = null;
            List<MaterialQCCase> qcCases;
            bool defectRoutedToOrganisation = false;

            
            foreach (MaterialMaster material in materials) {
                qcCases = material.QCCases;
                foreach (MaterialQCCase qcCase in qcCases)
                    defectRoutedToOrganisation = qcCase.Defects.Where(d => d.OrganisationID == routeToID).Count() > 0;
                if (defectRoutedToOrganisation)
                    materialMasters.Add(material);
            }*/

            return materials;
        }

        protected List<MaterialMaster> GetMaterialsforSubCon(List<int> trackerIDs, UserMaster userMaster)
        {
            List<MaterialMaster> materialMaster = null;
            try
            {
                materialMaster = _context.MaterialMaster
                    .Include(mm=>mm.DrawingAssociations)
                    .ThenInclude(ds=>ds.Drawing)
                  .Include(mm => mm.Project)
                  .Include(mm => mm.Trackers)
                  .Include(mm => mm.MRF)
                  .Include(mm => mm.Organisation)
                  .Include(mm => mm.QCCases)
                  .ThenInclude(qc => qc.Defects)
                  .Include(mm => mm.StageAudits)
                  .ThenInclude(sa => sa.Location)
                  .Include(mm => mm.StageAudits)
                  .ThenInclude(sa => sa.Stage)
                  .ThenInclude(st=>st.checklists)
                  .Include(mm => mm.Elements)
                  .ThenInclude(elm => elm.ForgeModel)
                  .Include(mm => mm.MaterialType)
                  .Where(mm => mm.Trackers.Where(t => trackerIDs.Contains(t.ID) && t.Material != null).Count() > 0
                   && mm.QCCases.Select(q => q.Defects.Where(d => d.OrganisationID != null && d.Status == QCStatus.QC_failed_by_Maincon
                   && d.OrganisationID == userMaster.Organisation.ID)).Count() > 0)
                  .ToList();
            }
            catch (Exception)
            {

            }
            return materialMaster;
        }


        protected List<int> GetTrackerIDs(string[] tags)
        {
            return _context.TrackerMaster
                .Where(tm => tags.Contains(tm.Tag))
                .Select(t => t.ID).ToList();
        }

        protected List<string> GetUnusedTrackerTags(string[] tags, List<MaterialMaster> materials)
        {
            return tags.Where(tag => !materials.Any(mm => mm.Trackers.Where(t => t.Material != null).Select(t => t.Tag).Contains(tag))).ToList();
        }

        protected async Task<List<TrackerAssociation>> GetTrackerAssociations(List<string> unusedTrackerTags)
        {
            List<TrackerAssociation> trackerAssociations = new List<TrackerAssociation>();
            try
            {
                //v = _context.TrackerMaster
                //    .Where(tm => unusedTrackerTags.Contains(tm.Tag))
                //    .Select(
                //                tm => new TrackerAssociation
                //                {

                //                    Tracker =  new List<TrackerMaster>().Add(tm)
                //                }
                //           )
                //    .ToList();

                List<Tracker> trackers = await _context.TrackerMaster
                                                       .Where(tm => unusedTrackerTags.Contains(tm.Tag))
                                                       .Select(t => new Tracker{
                                                                                   ID = t.ID,
                                                                                   Tag = t.Tag,
                                                                                   Label = t.Label,
                                                                                   Type = t.Type
                                                                               }
                                                              )
                                                       .ToListAsync();        
                //List<TrackerAssociation> trackerAssociations = new List<TrackerAssociation>();
                foreach (Tracker tracker in trackers)
                {
                    TrackerAssociation trackerAssociation = new TrackerAssociation();
                    trackerAssociation.Trackers = new List<Tracker>();
                    trackerAssociation.Trackers.Add(tracker);
                    trackerAssociations.Add(trackerAssociation);
                }



            }
            catch (Exception)
            {

            }
            return trackerAssociations;
        }

        protected MaterialMobile CreateMaterialMobile(MaterialMaster mm, UserMaster userMaster,string tenant_name)
        {
            return new MaterialMobile
            {
                ID = mm.ID,
                Block = mm.Block,
                Level = mm.Level,
                Zone = mm.Zone,
                MarkingNo = mm.MarkingNo,
                MaterialType = mm.MaterialType.Name,
                OrganisationID = mm.OrganisationID,
                CastingDate = mm.CastingDate,
                ExpectedDeliveryDate = mm.MRF.ExpectedDeliveryDate,
                CurrentStage = mm.StageAudits.OrderBy(sa => sa.CreatedDate).Select(s => new MaterialStage
                {
                    ID = s.Stage.ID,
                    CanIgnoreQC = s.Stage.CanIgnoreQC,
                    Colour = s.Stage.Colour,
                    IsEditable = s.Stage.IsEditable,
                    MilestoneID = s.Stage.MilestoneID,
                    Name = s.Stage.Name,
                }
                                                    ).LastOrDefault(),
                CanIgnoreQC = mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault()?.Stage != null ? mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault().Stage.CanIgnoreQC : true,
                CountQCCase = mm.QCCases.Where(qc => qc.Defects.Any(d => d.Status < QCStatus.QC_passed_by_Maincon)).Count(),
                QCStatusCode = mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault()?.Stage != null ? (int)mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault()?.QCStatus : -1,
                QCStatus = mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault()?.Stage != null ? mm.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault()?.QCStatus.ToString() : string.Empty,
                MRFNo = mm.MRF.MRFNo,
                SN = mm.SN,
                CurrentLocation = mm.StageAudits.OrderBy(sa => sa.CreatedDate).Select(
                                                        l => new MaterialLocation
                                                        {
                                                            Id = l.Location.ID,
                                                            Name = l.Location.Name
                                                        }).LastOrDefault(),
                ForgeElementID = mm.Elements.FirstOrDefault()?.DbID,
                ForgeModelURN = mm.Elements.FirstOrDefault()?.ForgeModel.ObjectID,
                RouteTo = tenant_name.ToLower() == "tenant2" || tenant_name.ToLower() == "alec" ? mm.StageAudits != null && mm.StageAudits.Count() > 1 ? "RTO" : mm.MaterialType.RouteTo : mm.MaterialType.RouteTo,
                IsQCOpen = userMaster.Role.MobileEntryPoint == 1 ? true
                : mm.QCCases
                .Select(q => q.Defects
                .Where(d => d.OrganisationID != null && d.OrganisationID == userMaster.Organisation.ID)).Count() > 0
                ? true : false,
                Length = mm.Length,
                Area = mm.Area,
                DrawingNo = mm.DrawingAssociations?.FirstOrDefault()?.Drawing?.DrawingNo,
                IsChecklist= mm.StageAudits.OrderBy(s => s.CreatedDate).LastOrDefault()?.Stage?.checklists != null && mm.StageAudits.OrderBy(s => s.CreatedDate).LastOrDefault()?.Stage?.checklists?.Count() > 0
            };
        }

        protected TrackerUploadStatus CreateTrackerUploadStatus(TrackerMaster tracker, string message)
        {
            TrackerUploadStatus trackerUploadStatus = new TrackerUploadStatus();

            trackerUploadStatus.Label = tracker.Label;
            trackerUploadStatus.Tag = tracker.Tag;
            trackerUploadStatus.Type = tracker.Type;
            trackerUploadStatus.Message = message;

            return trackerUploadStatus;
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

        protected async Task<TrackerMaster> GenerateAndAssociateQRCodes(MaterialMaster materialMaster, ConfigurationMaster config)
        {

            TrackerMaster qrCode = new TrackerMaster();
            //List<TrackerMaster> qrCodes = new List<TrackerMaster>();
            try
            {
                TrackerMaster trackerMaster = new TrackerMaster();
                string trackerType = "QR Code";
                int currentBatchNumber = GetMaxTrackerBatchNo(trackerType) + 1;
                string label = $"{materialMaster.Block}-{materialMaster.Level}-{materialMaster.Zone}-{materialMaster.MarkingNo}";

                //for (int i = 0; i < qty; i++)
                //{
                string QRFormat = string.Empty;
                if (config != null)
                    QRFormat = config.Setting;
                qrCode = GenerateQRCode(materialMaster, currentBatchNumber, trackerType, label, QRFormat);
                //trackerMaster = CreateTracker(qrCode.Label + "," + qrCode.Tag, trackerType, currentBatchNumber);
                //var qrdbcheck = _context.TrackerMaster.Where(t => t.Tag == qrCode.Tag).ToList();
                //if(qrdbcheck != null && qrdbcheck.Count>0)
                //{
                //    Console.WriteLine($"Dublicate tag: {qrdbcheck.FirstOrDefault().Tag} ");
                //}
                _context.TrackerMaster.Add(qrCode);
                //qrCodes.Add(qrCode);
                //}

                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                throw ex;
            }

           

            return qrCode;
        }
    }
}