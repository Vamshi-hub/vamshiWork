using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.ViewModels;
using astorWorkMobile.Shared.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile.Shared.Utilities
{
    public sealed class ApiClient
    {
        #region General field
        public static readonly ApiResult GENERIC_RESULT = new ApiResult()
        {
            status = 1,
            message = "Unkown error",
            data = null
        };

        public class ApiResult
        {
            /// <summary>
            /// Status:
            /// 0 - Success,
            /// 1 - General error,
            /// 2 - Network error,
            /// 3 - DB error
            /// </summary>
            public int status { get; set; }
            public string message { get; set; }
            public object data { get; set; }
        }


        private static volatile ApiClient instance;
        private static object syncRoot = new Object();

        private HttpClient _client;
        private string _endpoint;
        private ApiClient() { }
        public void ResetClient()
        {
            _client = null;
        }
        public static ApiClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new ApiClient();
                        }
                    }
                }

                return instance;
            }
        }

        public void InitClient(string endpoint, string dummyTenant, int timeOutSeconds)
        {
            _endpoint = endpoint;
            if (_client == null)
            {
                _client = new HttpClient();
                _client.MaxResponseContentBufferSize = Constants.SETTING_API_BUFFER_SIZE;
                _client.Timeout = TimeSpan.FromSeconds(timeOutSeconds);
                _client.BaseAddress = new Uri(_endpoint);
                if (!string.IsNullOrEmpty(dummyTenant))
                {
                    _client.DefaultRequestHeaders.Host = dummyTenant;
                }
            }
            else
            {
                if (!_client.BaseAddress.AbsoluteUri.Contains(_endpoint))
                {
                    _client = new HttpClient();
                    _client.MaxResponseContentBufferSize = Constants.SETTING_API_BUFFER_SIZE;
                    _client.Timeout = TimeSpan.FromSeconds(timeOutSeconds);
                    _client.BaseAddress = new Uri(_endpoint);
                    if (!string.IsNullOrEmpty(dummyTenant))
                    {
                        _client.DefaultRequestHeaders.Host = dummyTenant;
                    }
                }
            }
        }

        private void SetupAuthorization()
        {
            if (_client != null && Application.Current.Properties.ContainsKey("access_token"))
            {
                var accessToken = Application.Current.Properties["access_token"] as string;
                if (!string.IsNullOrEmpty(accessToken))
                {
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }
        }

        private async Task<ApiResult> GenericGet(string url, Type dataType)
        {
            var result = GENERIC_RESULT;

            try
            {
                SetupAuthorization();

                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<ApiResult>(content);

                    if (result.status == 0 && dataType != null && result.data != null)
                    {
                        result.data = JsonConvert.DeserializeObject(result.data.ToString(), dataType);
                    }

                }
                else
                {
                    result.message = response.ReasonPhrase;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Source + " cannot connect server");
                result.message = "Failed to connect to server, please try again later";
            }

            return result;
        }

        private async Task<ApiResult> GenericPost(string url, string body, Type dataType)
        {
            var result = GENERIC_RESULT;

            try
            {
                SetupAuthorization();

                var requestBody = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(url, requestBody);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<ApiResult>(content);

                    if (result.status == 0 && result.data != null && dataType != null)
                    {
                        result.data = JToken.Parse(result.data.ToString()).ToObject(dataType);
                    }
                }
                else
                {
                    result.message = response.ReasonPhrase;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Source + " cannot connect server");
                result.message = exc.Message;
            }

            return result;
        }

        private async Task<ApiResult> GenericPut(string url, string body, Type dataType)
        {
            var result = GENERIC_RESULT;

            try
            {
                SetupAuthorization();

                var requestBody = new StringContent(body, Encoding.UTF8, "application/json");
                var response = await _client.PutAsync(url, requestBody);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<ApiResult>(content);

                    if (dataType != null)
                    {
                        result.data = JToken.Parse(result.data.ToString()).ToObject(dataType);
                    }
                }
                else
                {
                    result.message = response.ReasonPhrase;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Source + " cannot connect server");
                result.message = "Failed to connect to server, please try again later";
            }

            return result;
        }
        #endregion
        #region Authentication operations

        public async Task<ApiResult> AuthLogin(string userName, string password)
        {
            var url = string.Format("authentication/login");
            var body = new { userName, password };
            return await GenericPost(url, JsonConvert.SerializeObject(body), typeof(AuthResult));
        }

        public async Task<ApiResult> AuthRefresh(string RefreshToken, int UserId)
        {
            var url = string.Format("authentication/refresh");
            var body = new { RefreshToken, UserId };
            return await GenericPost(url, JsonConvert.SerializeObject(body), typeof(AuthResult));
        }

        public async Task<ApiResult> GetTenantSettings()
        {
            var url = string.Format("tenant/settings");
            return await GenericGet(url, typeof(TenantSettings));
        }

        #endregion
        #region Material Tracking operations

        public async Task<ApiResult> MTGetProjects()
        {
            return await GenericGet("material-tracking/projects", typeof(List<Project>));
        }

        public async Task<ApiResult> MTGetProjectDetails(int projectId)
        {
            var url = string.Format("material-tracking/projects/{0}", projectId);
            return await GenericGet(url, typeof(Project));
        }

        public async Task<ApiResult> MTGetOrderedProduced(int projectId, int vendorId)
        {
            var url = string.Format("material-tracking/projects/{0}/materials/ordered-produced?vendor_id={1}", projectId, vendorId);
            return await GenericGet(url, typeof(List<MaterialFrameVM>));
        }
        public async Task<ApiResult> MTGetQcMaterials(int projectId)
        {
            var url = string.Format("material-tracking/projects/{0}/materials/produced-by-vendor", projectId);
            return await GenericGet(url, typeof(List<MaterialFrameVM>));
        }
        public async Task<ApiResult> MTGetMainConProduced()
        {
            var url = string.Format($"material-tracking/projects/0/materials/produced?tenant_name={Application.Current.Properties["tenant_name"].ToString()}");
            return await GenericGet(url, typeof(List<MaterialFrameVM>));
        }

        public async Task<ApiResult> MTGetListTrackerInfo(string[] trackerTags)
        {
            var url = string.Format("material-tracking/trackers/association?{0}&tenant_name={1}",
                string.Join("&", trackerTags.Select(tag => "tags=" + tag)), Application.Current.Properties["tenant_name"].ToString()
                );
            return await GenericGet(url, typeof(List<TrackerAssociation>));
        }
        public async Task<ApiResult> MTGetQCDefectsListTrackerInfo(string[] trackerTags)
        {
            var url = string.Format("material-tracking/trackers/material-by-subcon?tags={0}", trackerTags[0]
                // string.Join("&", trackerTags.Select(tag => "tags=" + tag))
                );
            return await GenericGet(url, typeof(List<TrackerAssociation>));
        }

        public async Task<ApiResult> MTUpdateAssociation(TrackerAssociation trackerAssociation)
        {
            var url = "material-tracking/trackers/association";
            return await GenericPost(url, JsonConvert.SerializeObject(trackerAssociation), null);
        }

        public async Task<ApiResult> MTGetPreInventoryInfo(int projectId, int vendorId, string materialType)
        {
            var url = string.Format("material-tracking/projects/{0}/vendors/{1}/inventory/pre-create?material_type={2}&singular=1",
                projectId, vendorId, materialType);
            return await GenericGet(url, null);
        }

        public async Task<ApiResult> MTGetListMRF(int projectId, string markingNo)
        {
            var url = string.Format("material-tracking/projects/{0}/mrfs?marking_no={1}&singular=1",
                projectId, markingNo);
            return await GenericGet(url, typeof(List<MRF>));
        }

        public async Task<ApiResult> MTCreateInventory(int projectId, int vendorId, int trackerId, string markingNo, int sn, DateTime castingDate)
        {
            var url = string.Format("material-tracking/projects/{0}/vendors/{1}/inventory", projectId, vendorId);
            var body = new { markingNo, sn, castingDate, trackerId };
            return await GenericPost(url, JsonConvert.SerializeObject(body), null);
        }

        public async Task<ApiResult> MTQCAndDeliver(int vendorId, int trackerId, string mrfNo,
            bool qcStatus, string qcRemarks, int locationId)
        {
            var url = string.Format("material-tracking/vendors/{0}/after-inventory", vendorId);
            var body = new { mrfNo, qcStatus, qcRemarks, trackerId, locationId };
            return await GenericPost(url, JsonConvert.SerializeObject(body), null);
        }

        public async Task<ApiResult> MTGetNextStageInfo(int materialId)
        {
            var url = string.Format("material-tracking/materials/{0}/next-stage?singular=1", materialId);
            return await GenericGet(url, typeof(Stage));
        }

        public async Task<ApiResult> MTUpdateMaterialStage(int materialId, int stageId, int locationId, DateTime? castingDate)
        {
            var url = string.Format("material-tracking/materials/{0}/update-stage", materialId);
            var body = new { stageId, locationId, castingDate };
            return await GenericPost(url, JsonConvert.SerializeObject(body), null);
        }
        public async Task<ApiResult> MTUpdateMaterialsStage(int[] materialIds, int stageId, int locationId, DateTime? castingDate)
        {
            var url = string.Format("material-tracking/materials/update-stage-batch");
            var body = new { materialIds, stageId, locationId, castingDate };
            return await GenericPost(url, JsonConvert.SerializeObject(body), null);
        }

        public async Task<ApiResult> MTUpdateMaterialLocation(int materialId, int locationId)
        {
            var url = string.Format("material-tracking/materials/{0}/update-location", materialId);
            var body = new { stageId = 0, locationId };
            return await GenericPost(url, JsonConvert.SerializeObject(body), null);
        }

        public async Task<ApiResult> MTUpdateMaterialQCPhoto(int defectId, string imageContent, string remarks, bool closed)
        {
            var url = string.Format("material-tracking/qc/photo?defect_id={0}", defectId);
            var body = new { imageContent, remarks, closed };
            return await GenericPost(url, JsonConvert.SerializeObject(body), null);
        }

        public async Task<ApiResult> MTGetLocations(int userId)
        {
            var url = string.Format("material-tracking/locations?user_id={0}", userId);
            return await GenericGet(url, typeof(List<Location>));
        }

        public async Task<ApiResult> MTCreateQcDefect(int materialId, int caseId, int organisationId, string remarks)
        {
            string url = string.Format("material-tracking/qc/defect?case_id={0}&material_id={1}&organisation_id={2}", caseId, materialId, organisationId);
            return await GenericPost(url, JsonConvert.SerializeObject(new { remarks }), typeof(int));
        }

        public async Task<ApiResult> MTUpdateQcDefect(int defectId, string remarks, int organisationId, int status, bool closed)
        {
            string url = string.Format("material-tracking/qc/defect?defect_id={0}&organisation_id={1}&status_code={2}", defectId, organisationId, status);
            return await GenericPut(url, JsonConvert.SerializeObject(new { remarks, closed }), typeof(int));
        }

        public async Task<ApiResult> MTGetQCDefectPhots(int defectId)
        {
            var url = string.Format("material-tracking/qc/photo?defect_id={0}", defectId);
            return await GenericGet(url, typeof(List<QCPhoto>));
        }

        public async Task<ApiResult> MTGetQCCases(int materialId)
        {
            var url = string.Format("material-tracking/qc/case?material_id={0}", materialId);
            return await GenericGet(url, typeof(List<QCCase>));
        }

        public async Task<ApiResult> MTGetQCDefects(int caseId)
        {
            var url = string.Format("material-tracking/qc/defect?case_id={0}", caseId);
            return await GenericGet(url, typeof(List<QCDefect>));
        }

        public async Task<ApiResult> MTGetForgeToken()
        {
            var url = string.Format("material-tracking/forge-auth");
            return await GenericGet(url, null);
        }

        public async Task<ApiResult> MTGetForgeModelProgress(string modelURN)
        {
            var url = string.Format("material-tracking/bim-viewer/current-progress?model_urn={0}", modelURN);
            return await GenericGet(url, null);
        }

        public async Task<ApiResult> MTGetOrganisations()
        {
            string endpoint = $"material-tracking/organisations";
            return await GenericGet(endpoint, typeof(List<Organisation>));
        }

        #endregion
        #region Job Tracking operations
        //projects/{projectID}/job-schedule/jobschedule-count

        public async Task<ApiResult> JTGetJobScheduleCount(int projectID)
        {
            var endpoint = $"job-tracking/projects/{projectID}/job-schedule/jobschedule-count";
            return await GenericGet(endpoint, typeof(JobScheduleCount));
        }
        public async Task<ApiResult> JTGetJobSchedule(int projectID, int lastMaterialIndex, int pageSize, string blk, string lvl, string materialType, string subcon, int job_Status)
        {
            var endpoint = $"job-tracking/projects/{projectID}/job-schedule/jobschedulelist?lastMaterialIndex={lastMaterialIndex}&pageSize={pageSize}&block={blk}&level={lvl}&materialType={materialType}&subcon={subcon}&job_Status={job_Status}";
            return await GenericGet(endpoint, typeof(List<JobScheduleDetails>));
        }

        public async Task<ApiResult> JTGetJobScheduleForRTO(int projectId)
        {
            var endpoint = $"job-tracking/projects/{projectId}/job-schedule/jobs-by-RTO";
            return await GenericGet(endpoint, typeof(List<JobScheduleDetails>));
        }

        public async Task<ApiResult> JTAcceptRejectChecklistRTO(int projectId, int checkListId, int jobScheduleId, int status, SignedChecklist signedChecklist)
        {
            var endpoint = $"job-tracking/checklist-audit/accept-reject?checklist_id={checkListId}&job_schedule_id={jobScheduleId}&status_id={status}";
            var body = JsonConvert.SerializeObject(signedChecklist);
            return await GenericPost(endpoint, body, typeof(int));
        }
        public async Task<ApiResult> MTAcceptRejectChecklistRTO(int checkListId, int materialId, int status, SignedChecklist signedChecklist)
        {
            var endpoint = $"job-tracking/structural-checklist/accept-reject?checklist_id={checkListId}&material_id={materialId}&status_id={status}";
            var body = JsonConvert.SerializeObject(signedChecklist);
            return await GenericPost(endpoint, body, typeof(int));
        }

        public async Task<ApiResult> JTAssignJob(int projectId, JobScheduleDetails job)
        {
            var endpoint = $"job-tracking/projects/{projectId}/job-schedule/assign";
            var body = JsonConvert.SerializeObject(job);
            return await GenericPut(endpoint, body, typeof(JobScheduleDetails));
        }

        public async Task<ApiResult> JTGetJobScheduleBySubCon(int projectId, int subcon_id, string tag)
        {
            var endpoint = $"job-tracking/projects/{projectId}/job-schedule/by-subcon?subcon_id={subcon_id}&tag={tag}";
            return await GenericGet(endpoint, typeof(List<JobScheduleDetails>));
        }
        public async Task<ApiResult> JTGetJobScheduleBySubCon(string[] tags)
        {
            //var endpoint = $"job-tracking/projects/0/job-schedule/jobs-by-materialtag";
            var endpoint = string.Format("job-tracking/projects/0/job-schedule/jobs-by-materialtag?{0}",
               string.Join("&", tags.Select(tag => "tags=" + tag))
               );
            //var body = JsonConvert.SerializeObject(tags);
            return await GenericGet(endpoint, typeof(List<JobScheduleDetails>));
        }

        public async Task<ApiResult> JTUpdateJobSchedule(int project_id, int jobschedule_id, int status_id)
        {
            var endpoint = $"job-tracking/projects/{project_id}/job-schedule/update-status/{jobschedule_id}?status_id={status_id}";
            var body = JsonConvert.SerializeObject(status_id);
            return await GenericPut(endpoint, body, typeof(string));
        }
        public async Task<ApiResult> JTGetJobChecklists(int project_id, int jobschedule_id, int materialID)
        {
            var endpoint = $"job-tracking/projects/{project_id}/checklist?job_schedule_id={jobschedule_id}&material_id={materialID}";
            return await GenericGet(endpoint, typeof(List<Checklist>));
        }
        public async Task<ApiResult> JTGetJobChecklistItems(int project_id, int jobschedule_id, int checklist_id, int materialID)
        {
            var endpoint = $"job-tracking/projects/{project_id}/checklist-items?checklist_id={checklist_id}&job_schedule_id={jobschedule_id}&material_id={materialID}";
            return await GenericGet(endpoint, typeof(SignedChecklist));
        }
        public async Task<ApiResult> JTGetRTOList()
        {
            var endpoint = $"user/rtos";
            return await GenericGet(endpoint, typeof(List<User>));
        }
        public async Task<ApiResult> JTPutJobChecklistItems(int project_id, int job_schedule_id, int route_to_id, int checklist_id, SignedChecklist signedChecklist)
        {
            var endpoint = $"job-tracking/checklist-audit/submit/?checklist_id={checklist_id}&job_schedule_id={job_schedule_id}&route_to_id={route_to_id}";
            var body = JsonConvert.SerializeObject(signedChecklist);
            return await GenericPost(endpoint, body, typeof(int));
        }
        public async Task<ApiResult> MTPutJobChecklistItems(int checklist_id, int material_id, int route_to_id, SignedChecklist signedChecklist)
        {
            var endpoint = $"job-tracking/structural-checklist/submit?checklist_id={checklist_id}&material_id={material_id}&route_to_id={route_to_id}";
            var body = JsonConvert.SerializeObject(signedChecklist);
            return await GenericPost(endpoint, body, typeof(int));
        }
        public async Task<ApiResult> MTGetRTOMaterials(int projectId)
        {
            var endpoint = $"material-tracking/projects/{projectId}/materials/rto";
            return await GenericGet(endpoint, typeof(List<MaterialFrameVM>));
        }
        public async Task<ApiResult> MTGetSubconList()
        {
            var endpoint = $"user/subcons";
            return await GenericGet(endpoint, typeof(List<User>));
        }
        #endregion

        #region Notification API
        public async Task<ApiResult> NTGetNotfications(int UserID)
        {
            var endpoint = $"material-tracking/notification-timer/notifaction-details?user_id={UserID}";
            return await GenericGet(endpoint, typeof(List<NotificationDetails>));
        }
        public async Task<ApiResult> NTUpdateSeenBy(int UserID, int NotificationType)
        {
            var endpoint = $"material-tracking/notification-timer/update-seenby/{UserID}?Type={NotificationType}";
            var body = new { UserID, NotificationType };
            return await GenericPost(endpoint, JsonConvert.SerializeObject(body), typeof(List<NotificationDetails>));
        }
        #endregion

        #region Chat API
        public async Task<ApiResult> GetChats()
        {
            string tenantName = string.Empty;
            if (!string.IsNullOrEmpty(Application.Current.Properties["tenant_name"] as string))
                tenantName = Application.Current.Properties["tenant_name"] as string;
            var endpoint = $"job-tracking/chat?tenant_name={tenantName}";
            return await GenericGet(endpoint, typeof(List<ChatMessage>));
        }
        public async Task<ApiResult> PostChat(string userName, ChatMessage message)
        {
            //message.Image = null;
            var endpoint = $"job-tracking/chat/save-chat?user_name={userName}";
            var body = JsonConvert.SerializeObject(message, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
            return await GenericPost(endpoint, body, typeof(int));
        }
        public async Task<ApiResult> PutChatPhoto(string originalImageByte, int chatID)
        {
            var endpoint = $"job-tracking/chat/upload-photo/{chatID}";
            var body = JsonConvert.SerializeObject(originalImageByte);
            return await GenericPut(endpoint, body, typeof(bool));
        }
        public async Task<ApiResult> PutChat(string UserName, List<ChatMessage> messages)
        {
            var endpoint = $"job-tracking/chat/update-seenby?user_name={UserName}";
            var body = JsonConvert.SerializeObject(messages);
            return await GenericPut(endpoint, body, typeof(bool));
        }
        #endregion

        #region Inspection
        public async Task<ApiResult> INGetInspectionForm(int jobID)
        {
            var endpoint = $"job-tracking/inspection/{jobID}";
            return await GenericGet(endpoint, typeof(List<RTOInspection>));
        }

        #endregion
    }
}
