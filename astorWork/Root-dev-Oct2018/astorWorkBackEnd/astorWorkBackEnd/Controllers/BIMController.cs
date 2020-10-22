using astorWorkBackEnd.Common;
using astorWorkBackEnd.Models;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Autodesk.Forge;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static astorWorkBackEnd.Models.AutodeskForge;

namespace astorWorkBackEnd.Controllers
{
    [Produces("application/json")]
    [Route("")]
    public class BIMController : CommonController
    {
        private readonly string Module = "BIM";

        private IAstorWorkBlobStorage _blobStorage;
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
        public APIResponse GetStages()
        {
            var stages = _context.MaterialStageMaster;
            if (!stages.Any())
                return new DbRecordNotFound(Module, "Block", "");

            return new APIResponse(0, stages.OrderBy(s => s.Order).Select(s => new
            {
                s.Order,
                s.Name,
                s.Colour,
                s.IsQCStage
            }));
        }

        [Route("projects/{project_id}/bim-updates")]
        public async Task<APIResponse> GetBIMUpdates([FromRoute] int project_id, [FromQuery] int? lastSyncId)
        {
            int[] unsyncedMaterialIds = new int[] { };
            DateTimeOffset lastSyncTime = DateTimeOffset.MinValue;
            if (lastSyncId.HasValue && lastSyncId.Value > 0)
            {
                var bimSync = await _context.BIMSyncAudit.FindAsync(lastSyncId.Value);
                if (bimSync != null)
                {
                    var unsyncedMaterialIdStrs = bimSync.UnsyncedMaterialIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    unsyncedMaterialIds = Array.ConvertAll(unsyncedMaterialIdStrs, int.Parse);
                    lastSyncTime = bimSync.SyncTime;
                }
            }

            var materialStageAudits = _context.MaterialStageAudit
                .Include(sa => sa.Stage)
                .Include(sa => sa.MaterialMaster).ThenInclude(mm => mm.Project)
                .Where(sa => sa.MaterialMaster.Project.ID == project_id &&
                (sa.CreatedDate >= lastSyncTime || unsyncedMaterialIds.Contains(sa.MaterialMaster.ID)));

            /*
            var materials = _context.MaterialMaster.Include(mm => mm.StageAudits).Where(mm => mm.Project.ID == projectId && mm.StageAudits.Any(sa => sa.CreatedDate >= lastSync));

            if (!materials.Any())
                return new DbRecordNotFound(Module, "BIM", "updates");
*/
            if (!materialStageAudits.Any())
                return new DbRecordNotFound(Module, "BIM", "updates");

            return new APIResponse(0, materialStageAudits.Select(sa =>
            new
            {
                materialId = sa.MaterialMaster.ID,
                block = sa.MaterialMaster.Block,
                level = sa.MaterialMaster.Level,
                zone = sa.MaterialMaster.Zone,
                markingNo = sa.MaterialMaster.MarkingNo,
                stageName = sa.Stage.Name,
                updateTime = sa.CreatedDate,
            }));
        }

        [Route("projects/{project_id}/bim-sync")]
        [HttpPost]
        public APIResponse UpdateBIMSync([FromRoute] int project_id, [FromBody] BIMSyncResult syncResult)
        {
            try
            {
                if (!ModelState.IsValid)
                    return new APIBadRequest();

                var user = _context.GetUserFromHttpContext(HttpContext);
                if (user == null)
                    return new DbRecordNotFound(Module, "BIM", "user");

                var project = _context.ProjectMaster.Find(project_id);

                if (project == null)
                    return new DbRecordNotFound(Module, "BIM", "project");

                var syncAudit = new BIMSyncAudit
                {
                    Project = project,
                    BIMModelId = syncResult.ModelId,
                    SyncedBy = user,
                    SyncedMaterialIds = string.Join(',', syncResult.SyncedMaterialIds),
                    UnsyncedMaterialIds = string.Join(',', syncResult.UnsyncedMaterialIds),
                    SyncTime = syncResult.SyncTime
                };

                _context.BIMSyncAudit.Add(syncAudit);

                _context.SaveChanges();

                var recipients = _context.UserMaster.Where(um => um.Project == project && (um.RoleID == 4 || um.RoleID == 6)).ToList();
                if (recipients.Count > 0)
                {
                    var notification = new NotificationAudit
                    {
                        Type = 0,
                        Code = 1,
                        Reference = syncAudit.ID.ToString(),
                        CreatedDate = DateTimeOffset.Now
                    };
                    var userNotificationAssociation = recipients.Select(
                        receipient => new UserNotificationAssociation
                        {
                            Receipient = receipient,
                            Notification = notification
                        });

                    _context.AddRange(userNotificationAssociation);

                    _context.SaveChanges();
                }

                return new APIResponse(0, new
                {
                    bimSyncId = syncAudit.ID
                });
            }
            catch (Exception exc)
            {
                return new APIResponse(1, null, exc.Message);
            }
        }

        [Route("/projects/{project_id}/bim-sync/{bim_sync_id}/video")]
        [HttpPost]
        public async Task<APIResponse> UploadBIMVideo([FromRoute] int project_id, [FromRoute] int bim_sync_id)
        {
            var containerName = AppConfiguration.GetVideoContainer();
            try
            {
                var bimSync = await _context.BIMSyncAudit.FindAsync(bim_sync_id);
                if (bimSync == null)
                    return new DbRecordNotFound(Module, "BIM", "sync session");

                var fileName = Guid.NewGuid() + ".mp4";
                var success = await _blobStorage.UploadFile(containerName, fileName, Request.Body);
                if (!success)
                    return new APIResponse(1, null, "Unable to upload video");

                bimSync.BIMVideoUrl = fileName;
                await _context.SaveChangesAsync();

                return new APIResponse(0, bimSync);
            }
            catch (Exception exc)
            {
                return new APIResponse(2, null, exc.Message);
            }
        }

        [Route("/projects/{project_id}/bim-sync")]
        [HttpGet]
        public APIResponse GetListBIMSync([FromRoute] int project_id)
        {
            var containerName = AppConfiguration.GetVideoContainer();
            var bimSyncs = _context.BIMSyncAudit.Include(bs => bs.SyncedBy).Include(bs => bs.Project).Where(bs => bs.Project.ID == project_id);

            if (bimSyncs == null || bimSyncs.Count() == 0)
                return new DbRecordNotFound(Module, "BIM", "sync session");

            return new APIResponse(0, bimSyncs.Select(bs => new
            {
                bs.ID,
                bs.BIMModelId,
                bs.SyncedBy.PersonName,
                CountSyncedMaterials = StringToArrayLength(bs.SyncedMaterialIds),
                CountUnsyncedMaterials = StringToArrayLength(bs.UnsyncedMaterialIds),
                VideoURL = string.IsNullOrEmpty(bs.BIMVideoUrl) ? null : _blobStorage.GetSignedURL(containerName, bs.BIMVideoUrl),
                bs.SyncTime
            }));
        }

        [Route("/projects/{project_id}/bim-sync/{bim_sync_id}")]
        [HttpGet]
        public APIResponse GetBIMSyncDetails([FromRoute] int project_id, [FromRoute] int bim_sync_id)
        {
            var containerName = AppConfiguration.GetVideoContainer();
            var bimSync = _context.BIMSyncAudit.Include(bs => bs.SyncedBy).FirstOrDefault(bs => bs.ID == bim_sync_id);
            if (bimSync == null)
                return new DbRecordNotFound(Module, "BIM", "sync session");

            return new APIResponse(0, new
            {
                bimSync.ID,
                bimSync.BIMModelId,
                bimSync.SyncedBy.PersonName,
                SyncedMaterials = _context.MaterialMaster
                .Where(mm => CheckMaterialIdInArray(bimSync.SyncedMaterialIds, mm.ID))
                .Select(mm => new { mm.ID, mm.Block, mm.Level, mm.Zone, mm.MarkingNo }),
                UnsyncedMaterials = _context.MaterialMaster
                .Where(mm => CheckMaterialIdInArray(bimSync.UnsyncedMaterialIds, mm.ID))
                .Select(mm => new { mm.ID, mm.Block, mm.Level, mm.Zone, mm.MarkingNo }),
                VideoURL = string.IsNullOrEmpty(bimSync.BIMVideoUrl) ? null : _blobStorage.GetSignedURL(containerName, bimSync.BIMVideoUrl),
                bimSync.SyncTime
            });
        }

        [Route("/get-elements")]
        [HttpGet]
        public async Task<IActionResult> GetObject([FromQuery] string accessToken, [FromQuery] string urn, [FromQuery] string guid, [FromQuery] string fileName)
        {
            try
            {
                var conf = new Autodesk.Forge.Client.Configuration
                {
                    AccessToken = accessToken
                };
                var apiInstance = new DerivativesApi(conf);
                var result = await apiInstance.GetModelviewMetadataAsync(urn, guid);
                var metaDataJson = JToken.Parse(JsonConvert.SerializeObject(result));
                var modelData = metaDataJson["data"]["objects"]["0"]["objects"] as JToken;

                var listDbId = DepthFirstIterate(modelData);

                var forgeModel = await _context.BIMForgeModel
                    .Where(fm => fm.ObjectKey == fileName).FirstOrDefaultAsync();
                if (forgeModel != null && forgeModel.Elements == null)
                {
                    var listIds = listDbId.Select(ld => ld.Key).Distinct();
                    var listMM = _context.MaterialMaster.Where(mm => listIds.Contains(mm.ID)).ToList();
                    forgeModel.Elements = new List<BIMForgeElement>();
                    forgeModel.Elements.AddRange(
                        listMM.Select(
                            mm => new BIMForgeElement
                            {
                                DbId = mm.ID,
                                MaterialMasterId = mm.ID
                            }
                    ));
                    await _context.SaveChangesAsync();
                }
                return Ok(listDbId);
            }
            catch (Exception exc)
            {
                Console.Write(exc.StackTrace);
                return BadRequest(exc.Message);
            }
        }

        private List<KeyValuePair<int, string>> DepthFirstIterate(JToken json)
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

        [Route("/get-objects")]
        [HttpGet]
        public async Task<IActionResult> GetObject([FromQuery] string accessToken)
        {
            try
            {
                var conf = new Autodesk.Forge.Client.Configuration
                {
                    AccessToken = accessToken
                };
                var bucketApi = new BucketsApi(conf);

                var objectApi = new ObjectsApi(conf);
                var bucketsObject = await bucketApi.GetBucketsAsync();
                JToken bucketsJson = JToken.Parse(JsonConvert.SerializeObject(bucketsObject));

                var listBuckets = new List<BucketItem>();
                foreach (var item in bucketsJson["items"].Children())
                {
                    var bucket = (item as JProperty).Value.ToObject<BucketItem>();
                    listBuckets.Add(bucket);

                    bucket.ObjectItems = new List<BIMForgeModel>();

                    var objectsObject = await objectApi.GetObjectsAsync(bucket.bucketKey);
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
                return Ok(listBuckets);
            }
            catch (Exception exc)
            {
                return BadRequest(exc.Message);
            }
        }

        [Route("/get-default-bim-element")]
        [HttpGet]
        public async Task<APIResponse> GetDefaultForgeElement([FromQuery] int materialMasterId)
        {
            var result = new APIResponse
            {
                Status = 404,
                Message = "BIM element not found for this material"
            };
            var materialMaster = await _context.MaterialMaster.FindAsync(materialMasterId);
            if (materialMaster != null)
            {
                var element = materialMaster.Elements.FirstOrDefault();
                if (element != null)
                {
                    var model = await _context.BIMForgeModel.FindAsync(element.ForgeModelId);
                    result = new APIResponse
                    {
                        Status = 0,
                        Data = new
                        {
                            ModelUrn = model.ObjectId,
                            ElementId = element.DbId
                        }
                    };
                }
            }

            return result;
        }

        [Route("bim-viewer/current-progress")]
        [HttpGet]
        public APIResponse GetCurrentProgressForModel([FromQuery] string model_urn)
        {
            var result = new APIResponse
            {
                Status = 404,
                Message = "Progress not found for this model"
            };

            if (!string.IsNullOrEmpty(model_urn))
            {
                var modelProgress = _context.BIMForgeModel
                    .Include(fm => fm.Elements)
                    .ThenInclude(elm => elm.MaterialMaster)
                    .ThenInclude(mm => mm.StageAudits)
                    .ThenInclude(sa => sa.Stage)
                    .Where(fm => fm.ObjectId == model_urn)
                    .FirstOrDefault();
                
                if(modelProgress != null)
                {
                    var listProgress = new List<dynamic>();

                    foreach(var element in modelProgress.Elements)
                    {
                        var latestStageAudit = element.MaterialMaster.StageAudits.OrderBy(sa => sa.CreatedDate).LastOrDefault();

                        if (latestStageAudit != null)
                        {
                            listProgress.Add(new
                            {
                                ElementId = element.DbId,
                                Passed = latestStageAudit.StagePassed,
                                StageColour = latestStageAudit.Stage.Colour,
                                StageName = latestStageAudit.Stage.Name
                            });
                        }
                    }

                    result = new APIResponse
                    {
                        Status = 0,
                        Message = string.Empty,
                        Data = listProgress
                    };
                }
            }

            return result;
        }

        [Route("/forge-auth")]
        [HttpGet]
        public async Task<APIResponse> GetForgeAuthentication()
        {
            var scope = new Scope[] {
                Scope.DataRead,
                Scope.DataWrite,
                Scope.BucketCreate,
                Scope.BucketRead,
                Scope.ViewablesRead
            };

            var api = new TwoLeggedApi();
            try
            {
                var bearer = await api.AuthenticateAsyncWithHttpInfo(
                    AppConfiguration.GetForgeClientId(),
                    AppConfiguration.GetForgeClientSecret(),
                    oAuthConstants.CLIENT_CREDENTIALS, 
                    scope);

                return new APIResponse
                {
                    Status = 0,
                    Data = bearer.Data
                };
            }
            catch (Exception exc)
            {
                return new APIResponse
                {
                    Status = ErrorMessages.BadRequest,
                    Message = exc.Message
                };
            }
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
