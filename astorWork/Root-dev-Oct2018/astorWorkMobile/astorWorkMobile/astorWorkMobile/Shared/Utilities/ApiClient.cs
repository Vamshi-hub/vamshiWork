using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http.Headers;
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
            // Status:
            // 0 - Success
            // 1 - General error
            // 2 - Network error
            // 3 - DB error
            public int status { get; set; }
            public string message { get; set; }
            public object data { get; set; }
        }


        private static volatile ApiClient instance;
        private static object syncRoot = new Object();

        private HttpClient _client;
        private string _endpoint;
        private ApiClient() { }

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

        public void InitClient(string endpoint, string dummyTenant)
        {
            _endpoint = endpoint;
            if (_client == null)
            {
                _client = new HttpClient();
                _client.MaxResponseContentBufferSize = Constants.SETTING_API_BUFFER_SIZE;
                _client.Timeout = TimeSpan.FromSeconds(Constants.SETTING_API_TIMEOUT_SECONDS);
            }
            _client.BaseAddress = new Uri(_endpoint);

            if (!string.IsNullOrEmpty(dummyTenant))
            {
                //_client.DefaultRequestHeaders.Add("TenantName", dummyTenant);
                _client.DefaultRequestHeaders.Host = dummyTenant;
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
                    result.message = response.ReasonPhrase;
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
                        result.data = JToken.Parse(result.data.ToString()).ToObject(dataType);
                }
                else
                    result.message = response.ReasonPhrase;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Source + " cannot connect server");
                result.message = "Failed to connect to server, please try again later";
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
                        result.data = JToken.Parse(result.data.ToString()).ToObject(dataType);
                }
                else
                    result.message = response.ReasonPhrase;
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

        public async Task<ApiResult> AuthRefresh(string RefreshToken, string UserId)
        {
            var url = string.Format("authentication/refresh");
            var body = new { RefreshToken, UserId };
            return await GenericPost(url, JsonConvert.SerializeObject(body), typeof(AuthResult));
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

        public async Task<ApiResult> MTGetInventory(int projectId, int vendorId)
        {
            var url = string.Format("material-tracking/projects/{0}/vendors/{1}/inventory", projectId, vendorId);
            return await GenericGet(url, typeof(List<Inventory>));
        }

        public async Task<ApiResult> MTGetListTrackerInfo(string[] trackerTags)
        {
            var url = string.Format("material-tracking/trackers/association?{0}",
                string.Join("&", trackerTags.Select(tag => "tags=" + tag))
                );
            return await GenericGet(url, typeof(List<Tracker>));
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
            return await GenericGet(url, null);
        }

        public async Task<ApiResult> MTUpdateMaterialStage(int materialId, bool qcStatus, 
            string qcRemarks, int locationId)
        {
            var url = string.Format("material-tracking/materials/{0}/update-stage", materialId);
            var body = new { qcStatus, qcRemarks, locationId };
            return await GenericPost(url, JsonConvert.SerializeObject(body), null);
        }

        public async Task<ApiResult> MTUpdateMaterialLocation(int materialId, bool qcStatus,
            string qcRemarks, int locationId)
        {
            var url = string.Format("material-tracking/materials/{0}/update-location", materialId);
            var body = new { qcStatus, qcRemarks, locationId };
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

        public async Task<ApiResult> MTCreateQcDefect(int stageAuditId, int caseId, string remarks)
        {
            var url = string.Format("material-tracking/qc/defect?case_id={0}&stage_audit_id={1}", caseId,stageAuditId);
            var body = new { remarks };
            return await GenericPost(url, JsonConvert.SerializeObject(body), null);
        }

        public async Task<ApiResult> MTUpdateQcDefect(int defectId, string remarks,bool closed)
        {
            var url = string.Format("material-tracking/qc/defect?defect_id={0}", defectId);
            var body = new { remarks,closed };
            return await GenericPut(url, JsonConvert.SerializeObject(body), null);
        }
        public async Task<ApiResult> MTGetQCDefectPhots(int defectId)
        {
            var url = string.Format("material-tracking/qc/photo?defect_id={0}", defectId);
            return await GenericGet(url, typeof(List<QCPhoto>));
        }

        public async Task<ApiResult> MTGetQCCases(int stageAuditId)
        {
            var url = string.Format("material-tracking/qc/case?stage_audit_id={0}", stageAuditId);
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

        #endregion
    }
}
