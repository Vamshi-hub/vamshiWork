using astorWorkMaterialTracking.Common;
using astorWorkMaterialTracking.Models;
using astorWorkDAO;
using astorWorkShared.GlobalExceptions;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Autodesk.Forge;
using Autodesk.Forge.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static astorWorkMaterialTracking.Models.AutodeskForge;

namespace astorWorkMaterialTracking.Controllers
{
    [Produces("application/json")]
    public class BIMController : CommonBIMController
    {
        private readonly string module = "BIM updates";

        private static readonly HttpClient forgeClient = new HttpClient()
        {
            BaseAddress = new Uri(@"https://developer.api.autodesk.com/"),
            Timeout = TimeSpan.FromSeconds(15)
        };

        public BIMController(astorWorkDbContext context, IAstorWorkBlobStorage blobStorage) : base(context)
        {
            _blobStorage = blobStorage;
        }

        [Route("stages")]
        public async Task<List<MaterialStage>> GetStages()
        {
            List<MaterialStageMaster> stages = await _context.MaterialStageMaster.ToListAsync();

            if (!stages.Any())
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module));

            return await GetMaterialStages(stages);
        }

        [Route("projects/{project_id}/bim-updates")]
        public async Task<List<BIMUpdate>> GetBIMUpdates([FromRoute] int project_id, [FromQuery] int? lastSyncId)
        {
            int[] unsyncedIntMaterialIDs = new int[] { };
            DateTimeOffset lastSyncTime = DateTimeOffset.MinValue;

            if (lastSyncId.HasValue && lastSyncId.Value > 0)
            {
                BIMSyncAudit bimSync = await _context.BIMSyncAudit.FindAsync(lastSyncId.Value);
                if (bimSync != null)
                {
                    string[] unsyncedStrMaterialIDs = bimSync.UnsyncedMaterialIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    unsyncedIntMaterialIDs = Array.ConvertAll(unsyncedStrMaterialIDs, int.Parse);
                    lastSyncTime = bimSync.SyncTime;
                }
            }

            List<MaterialStageAudit> materialStageAudits = await GetMaterialStageAudits(project_id, lastSyncTime, unsyncedIntMaterialIDs);
            //var materials = _context.MaterialMaster.Include(mm => mm.StageAudits).Where(mm => mm.Project.ID == projectId && mm.StageAudits.Any(sa => sa.CreatedDate >= lastSync));

            if (!materialStageAudits.Any())
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "Project ID", project_id.ToString()));

            return materialStageAudits.Select(msa => CreateBIMUpdate(msa.Material, msa.Stage.Name, msa.CreatedDate)).ToList();
        }

        [Route("projects/{project_id}/bim-sync")]
        [HttpPost]
        public async Task<int> UpdateBIMSync([FromRoute] int project_id, [FromBody] BIMSyncResult syncResult)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

                UserMaster user = await _context.GetUserFromHttpContext(HttpContext);

                if (user == null)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("User"));

                ProjectMaster project = _context.ProjectMaster.Find(project_id);

                if (project == null)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("Project", "ID", project_id.ToString()));

                BIMSyncAudit syncAudit = CreateBIMSyncAudit(project, user, syncResult);
                _context.BIMSyncAudit.Add(syncAudit);
                _context.SaveChanges();

                List<UserMaster> receipients = await GetReceipients(project);
                if (receipients.Count > 0)
                {
                    NotificationAudit notification = CreateNotificationAudit(syncAudit);
                    List<UserNotificationAssociation> userNotificationAssociation = GetUserNotificationAssociations(receipients, notification);

                    _context.AddRange(userNotificationAssociation);
                    _context.SaveChanges();
                }

                //return new {bimSyncId = syncAudit.ID};
                return syncAudit.ID;
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
        }

        [Route("/projects/{project_id}/bim-sync/{bim_sync_id}/video")]
        [HttpPost]
        public async Task<BIMSyncAudit> UploadBIMVideo([FromRoute] int project_id, [FromRoute] int bim_sync_id)
        {
            string containerName = AppConfiguration.GetVideoContainer();

            try
            {
                BIMSyncAudit bimSync = await _context.BIMSyncAudit.FindAsync(bim_sync_id);
                if (bimSync == null)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg("BIM Sync", "ID", bim_sync_id.ToString()));

                string fileName = Guid.NewGuid() + ".mp4";
                bool success = await _blobStorage.UploadFile(containerName, fileName, Request.Body);
                if (!success)
                    throw new GenericException(1, "Unable to upload video");

                bimSync.BIMVideoUrl = fileName;
                await _context.SaveChangesAsync();

                return bimSync;
            }
            catch (Exception exc)
            {
                throw new GenericException(2, exc.Message);
            }
        }

        [Route("/projects/{project_id}/bim-sync")]
        [HttpGet]
        public async Task<List<BIMSyncResult>> GetListBIMSync([FromRoute] int project_id)
        {
            string containerName = AppConfiguration.GetVideoContainer();
            List<BIMSyncAudit> bimSyncs = await GetBIMSyncs(project_id);

            if (bimSyncs == null || bimSyncs.Count() == 0)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module,null,null));

            return bimSyncs.Select(bs => CreateBIMSyncResult(bs, bs.SyncedBy, containerName)).ToList();
        }

        [Route("/projects/{project_id}/bim-sync/{bim_sync_id}")]
        [HttpGet]
        public BIMSyncResult GetBIMSyncDetails([FromRoute] int project_id, [FromRoute] int bim_sync_id)
        {
            string containerName = AppConfiguration.GetVideoContainer();
            BIMSyncAudit bimSync = _context.BIMSyncAudit.Include(bs => bs.SyncedBy).FirstOrDefault(bs => bs.ID == bim_sync_id);

            if (bimSync == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, ErrorMessages.DBRecordNotFoundMsg(module, "Project and BIM Sync IDs", project_id.ToString() + " and " + bim_sync_id));

            return CreateBIMSyncResult(bimSync, bimSync.SyncedBy, containerName, true);
        }

        [Route("/get-elements")]
        [HttpGet]
        public async Task<List<KeyValuePair<int, string>>> GetObject([FromQuery] string accessToken, [FromQuery] string urn, [FromQuery] string guid, [FromQuery] string fileName)
        {
            try
            {
                Configuration conf = new Configuration { AccessToken = accessToken };
                DerivativesApi apiInstance = new DerivativesApi(conf);
                dynamic result = await apiInstance.GetModelviewMetadataAsync(urn, guid);
                JToken metaDataJson = JToken.Parse(JsonConvert.SerializeObject(result));
                JToken modelData = metaDataJson["data"]["objects"]["0"]["objects"] as JToken;

                List<KeyValuePair<int, string>> dbIDs = DepthFirstIterate(modelData);

                BIMForgeModel forgeModel = await _context.BIMForgeModel
                                                         .Where(fm => fm.ObjectKey == fileName)
                                                         .FirstOrDefaultAsync();

                if (forgeModel != null && forgeModel.Elements == null)
                {
                    List<int> listIDs = dbIDs.Select(ld => ld.Key).Distinct().ToList();
                    //List<MaterialMaster> materialMasters = _context.MaterialMaster.Where(mm => listIDs.Contains(mm.ID)).ToList();
                    forgeModel.Elements = new List<BIMForgeElement>();
                    forgeModel.Elements.AddRange(GetBIMForgeElement(listIDs));
                    await _context.SaveChangesAsync();
                }
                return dbIDs;
            }
            catch (Exception exc)
            {
                Console.Write(exc.StackTrace);
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
            }
        }

        [Route("/get-objects")]
        [HttpGet]
        public async Task<List<BucketItem>> GetObject([FromQuery] string accessToken)
        {
            try
            {
                Configuration conf = new Configuration
                {
                    AccessToken = accessToken
                };

                BucketsApi bucketApi = new BucketsApi(conf);
                ObjectsApi objectApi = new ObjectsApi(conf);
                BucketsApi bucketsObject = await bucketApi.GetBucketsAsync();
                JToken bucketsJson = JToken.Parse(JsonConvert.SerializeObject(bucketsObject));
                List<BucketItem> listBuckets = new List<BucketItem>();

                foreach (JToken item in bucketsJson["items"].Children())
                {
                    BucketItem bucket = (item as JProperty).Value.ToObject<BucketItem>();
                    listBuckets.Add(bucket);

                    bucket.ObjectItems = new List<BIMForgeModel>();

                    dynamic objectsObject = await objectApi.GetObjectsAsync(bucket.bucketKey);
                    JObject objectsJSON = JToken.Parse(JsonConvert.SerializeObject(objectsObject));

                    foreach (var forgeObject in objectsJSON["items"].Children())
                    {
                        var obj = (forgeObject as JProperty).Value.ToObject<BIMForgeModel>();
                        bucket.ObjectItems.Add(obj);
                        if (!await _context.BIMForgeModel.AnyAsync(fm => fm.ObjectKey == obj.ObjectKey))
                            await _context.BIMForgeModel.AddAsync(obj);
                    }
                }

                await _context.SaveChangesAsync();
                return listBuckets;
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
                //return BadRequest(exc.Message);
            }
        }

        [Route("/get-default-bim-element")]
        [HttpGet]
        public async Task<ForgeElement> GetDefaultForgeElement([FromQuery] int materialMasterId)
        {
            MaterialMaster materialMaster = await _context.MaterialMaster.FindAsync(materialMasterId);
            if (materialMaster != null)
            {
                BIMForgeElement element = materialMaster.Elements.FirstOrDefault();
                if (element != null)
                    await GetForgeElement(element);
            }

            throw new GenericException(ErrorMessages.DbRecordNotFound, "BIM element not found for this material");
        }

        protected async Task<ForgeElement> GetForgeElement(BIMForgeElement element)
        {
            BIMForgeModel model = await _context.BIMForgeModel.FindAsync(element.ForgeModelID);
            return new ForgeElement
            {
                ModelURN = model.ObjectID,
                ElementID = element.DbID
            };
        }

        [Route("bim-viewer/current-progress")]
        [HttpGet]
        public List<dynamic> GetCurrentProgressForModel([FromQuery] string model_urn)
        {
            if (!string.IsNullOrEmpty(model_urn))
            {
                BIMForgeModel modelProgress = GetBIMForgeModel(model_urn);

                if (modelProgress != null)
                {
                    List<dynamic> listProgress = new List<dynamic>();

                    foreach (BIMForgeElement element in modelProgress.Elements)
                    {
                        MaterialStageAudit latestStageAudit = element.MaterialMaster.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault();

                        if (latestStageAudit != null)
                        {
                            listProgress.Add(new
                            {
                                ElementId = element.DbID,
                                Passed = !element.MaterialMaster.QCCases.Any(c => c.Defects.Any(d => d.Status < Enums.QCStatus.QC_passed_by_Maincon)),
                                StageColour = latestStageAudit.Stage.Colour,
                                StageName = latestStageAudit.Stage.Name
                            });
                        }
                    }

                    return listProgress;
                }
            }

            throw new GenericException(ErrorMessages.DbRecordNotFound, "Progress not found for this model");
        }

        [Route("/forge-auth")]
        [HttpGet]
        public async Task<dynamic> GetForgeAuthentication()
        {
            Scope[] scope = new Scope[] {
                Scope.DataRead,
                Scope.DataWrite,
                Scope.BucketCreate,
                Scope.BucketRead,
                Scope.ViewablesRead
            };

            dynamic api = new TwoLeggedApi();
            try
            {
                dynamic bearer = await api.AuthenticateAsyncWithHttpInfo(
                    AppConfiguration.GetForgeClientId(),
                    AppConfiguration.GetForgeClientSecret(),
                    oAuthConstants.CLIENT_CREDENTIALS,
                    scope);

                return bearer.Data;
            }
            catch (Exception exc)
            {
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
            }
        }
    }
}
