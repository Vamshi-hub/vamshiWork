using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace astorWorkMaterialTracking.Common
{
    public class CommonBIMController : CommonController
    {
        protected CommonBIMController(astorWorkDbContext context) : base(context)
        {
        }

        protected async Task<List<MaterialStageAudit>> GetMaterialStageAudits(int projectID, DateTimeOffset lastSyncTime, int[] unsyncedMaterialIDs)
        {
            return await _context.MaterialStageAudit
                .Include(sa => sa.Stage)
                .Include(sa => sa.MaterialMaster).ThenInclude(mm => mm.Project)
                .Where(sa => sa.MaterialMaster.Project.ID == projectID &&
                (sa.CreatedDate >= lastSyncTime || unsyncedMaterialIDs.Contains(sa.MaterialMaster.ID))).ToListAsync();
        }

        protected BIMUpdate CreateBIMUpdate(MaterialMaster materialMaster, string stageName, DateTimeOffset createdDate)
        {
            return new BIMUpdate
            {
                MaterialID = materialMaster.ID,
                Block = materialMaster.Block,
                Level = materialMaster.Level,
                Zone = materialMaster.Zone,
                MarkingNo = materialMaster.MarkingNo,
                StageName = stageName,
                UpdateTime = createdDate
            };
        }
        protected BIMSyncAudit CreateBIMSyncAudit(ProjectMaster project, UserMaster user, BIMSyncResult syncResult) {
            return new BIMSyncAudit
            {
                Project = project,
                BIMModelId = syncResult.ModelID,
                SyncedBy = user,
                SyncedMaterialIds = string.Join(',', syncResult.SyncedMaterialIDs),
                UnsyncedMaterialIds = string.Join(',', syncResult.UnsyncedMaterialIDs),
                SyncTime = syncResult.SyncTime
            };
        }

        protected NotificationAudit CreateNotificationAudit(BIMSyncAudit syncAudit) {
            return new NotificationAudit
            {
                Type = 0,
                Code = 1,
                Reference = syncAudit.ID.ToString(),
                CreatedDate = DateTimeOffset.Now
            };
        }

        protected async Task<List<UserMaster>> GetReceipients(ProjectMaster project){
            return await _context.UserMaster.Where(um => um.Project == project && (um.RoleID == 4 || um.RoleID == 6)).ToListAsync();
        }

        protected List<UserNotificationAssociation> GetUserNotificationAssociations(List<UserMaster> receipients, NotificationAudit notification) {
            return receipients.Select(
                                        receipient => new UserNotificationAssociation
                                        {
                                            Receipient = receipient,
                                            Notification = notification
                                        }
                                     ).ToList();
        }

        protected async Task<List<BIMSyncAudit>> GetBIMSyncs(int projectID) {
            return await _context.BIMSyncAudit.Include(bs => bs.SyncedBy).Include(bs => bs.Project).Where(bs => bs.Project.ID == projectID).ToListAsync();
        }

        protected BIMSyncResult CreateBIMSyncResult(BIMSyncAudit bimSync, UserMaster syncedBy, string containerName, bool hasSyncMaterials = false) {
            List<MaterialMaster> syncedMaterials = new List<MaterialMaster>();
            List<MaterialMaster> unsyncedMaterials = new List<MaterialMaster>();

            if (hasSyncMaterials) {
                syncedMaterials = _context.MaterialMaster
                                                .Where(mm => CheckMaterialIdInArray(bimSync.SyncedMaterialIds, mm.ID))
                                                .Select(mm => new MaterialMaster{ ID = mm.ID, Block = mm.Block, Level = mm.Level, Zone = mm.Zone, MarkingNo = mm.MarkingNo })
                                                .ToList();
                unsyncedMaterials = _context.MaterialMaster
                                                  .Where(mm => CheckMaterialIdInArray(bimSync.UnsyncedMaterialIds, mm.ID))
                                                  .Select(mm => new MaterialMaster{ ID = mm.ID, Block = mm.Block, Level = mm.Level, Zone = mm.Zone, MarkingNo = mm.MarkingNo })
                                                  .ToList();
            }

            return new BIMSyncResult
            {
                ID = bimSync.ID,
                ModelID = bimSync.BIMModelId,
                PersonName = syncedBy.PersonName,
                CountSyncedMaterials = StringToArrayLength(bimSync.SyncedMaterialIds),
                CountUnsyncedMaterials = StringToArrayLength(bimSync.UnsyncedMaterialIds),
                VideoURL = string.IsNullOrEmpty(bimSync.BIMVideoUrl) ? null : _blobStorage.GetSignedURL(containerName, bimSync.BIMVideoUrl),
                SyncTime = bimSync.SyncTime,
                SyncedMaterials = syncedMaterials,
                UnsyncedMaterials = unsyncedMaterials
            };
        }

        protected IEnumerable<BIMForgeElement> GetBIMForgeElement(List<int> listIDs)
        {
            List<MaterialMaster> materialMasters = _context.MaterialMaster.Where(mm => listIDs.Contains(mm.ID)).ToList();

            return materialMasters.Select(
                                            mm => new BIMForgeElement
                                            {
                                                DbID = mm.ID,
                                                MaterialMasterID = mm.ID
                                            }
                                         );
        }

        protected BIMForgeModel GetBIMForgeModel(string model_urn)
        {
            return _context.BIMForgeModel
                    .Include(fm => fm.Elements)
                    .ThenInclude(elm => elm.MaterialMaster)
                    .ThenInclude(mm => mm.QCCases)
                    .ThenInclude(c => c.Defects)
                    .Include(fm => fm.Elements)
                    .ThenInclude(elm => elm.MaterialMaster)
                    .ThenInclude(mm => mm.StageAudits)
                    .ThenInclude(sa => sa.Stage)
                    .Where(fm => fm.ObjectID == model_urn)
                    .FirstOrDefault();
        }

        protected List<KeyValuePair<int, string>> DepthFirstIterate(JToken json)
        {
            var dictDbId = new List<KeyValuePair<int, string>>();
            try
            {
                if (json != null && json.HasValues)
                {
                    foreach (var child in json.Children())
                    {
                        dictDbId.AddRange(DepthFirstIterate(child));
                    }
                }
                else if (json.Type == JTokenType.Integer)
                {
                    var parent = json.Parent.Parent;
                    int objectId = parent.Value<int>("objectid");
                    string name = parent.Value<string>("name");
                    if (objectId > 0 && !string.IsNullOrEmpty(name))
                    {
                        dictDbId.Add(new KeyValuePair<int, string>(objectId, name));
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            return dictDbId;
        }

        private bool CheckMaterialIdInArray(string strIn, int materialId)
        {
            bool result = false;
            if (!string.IsNullOrEmpty(strIn))
            {
                var listIds = strIn.Split(',', StringSplitOptions.RemoveEmptyEntries);
                result = listIds.Contains(materialId.ToString());
            }

            return result;
        }

        private int StringToArrayLength(string strIn)
        {
            int count = 0;
            if (!string.IsNullOrEmpty(strIn))
            {
                count = strIn.Split(',', System.StringSplitOptions.RemoveEmptyEntries).Length;
            }

            return count;
        }
    }
}
