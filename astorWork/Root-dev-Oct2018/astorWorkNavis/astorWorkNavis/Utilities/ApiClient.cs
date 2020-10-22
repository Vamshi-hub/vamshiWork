using astorWorkNavis.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace astorWorkNavis.Utilities
{
    public class ApiClient
    {
        #region General field
#if QA
        private string API_BASE_URL = "http://{0}.astorworkqa.com/api/";
        private string DUMMY_TENANT = "";
#elif RELEASE
        private string API_BASE_URL = "http://abc.safe-smart.com/api/";
        private string DUMMY_TENANT = "";
#else
        private string API_BASE_URL = "http://localhost:9000/api/";
        private string DUMMY_TENANT = "localhost";
#endif

        public static readonly ApiResult GENERIC_RESULT = new ApiResult()
        {
            Status = 1,
            Message = "Unkown error",
            Data = null
        };

        public class ApiResult
        {
            // Status:
            // 0 - Success
            // 1 - General error
            // 2 - Network error
            // 3 - DB error
            public int Status { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        }


        private static volatile ApiClient instance;
        private static object syncRoot = new Object();

        private HttpClient _client;
        private string _endpoint;
        private Timer _refreshTimer;

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

        public void InitClient(string tenantName)
        {
            _endpoint = string.Format(API_BASE_URL, tenantName);
            if (_client != null)
            {
                _client.Dispose();
            }
            _client = new HttpClient();
            _client.MaxResponseContentBufferSize = Constants.SETTING_API_BUFFER_SIZE;
            _client.Timeout = TimeSpan.FromSeconds(Constants.SETTING_API_TIMEOUT_SECONDS);
            _client.BaseAddress = new Uri(_endpoint);
            if (!string.IsNullOrEmpty(DUMMY_TENANT))
            {
                _client.DefaultRequestHeaders.Host = DUMMY_TENANT;
            }
        }

        private void SetupAuthorization()
        {
            var accessToken = Properties.Settings.Default.ACCESS_TOKEN;
            if (_client != null && !string.IsNullOrEmpty(accessToken))
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
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
                    if (dataType != null && result.Data != null)
                    {
                        result.Data = JToken.Parse(result.Data.ToString()).ToObject(dataType);
                    }
                }
                else
                    result.Message = response.ReasonPhrase;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Source + " cannot connect server");
                result.Message = "Failed to connect to server, please try again later";
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

                    if (dataType != null)
                        result.Data = JToken.Parse(result.Data.ToString()).ToObject(dataType);
                }
                else
                    result.Message = response.ReasonPhrase;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Source + " cannot connect server");
                result.Message = "Failed to connect to server, please try again later";
            }

            return result;
        }
        public void SetAuthProperties(AuthResult authResult)
        {
            Properties.Settings.Default.ACCESS_TOKEN = authResult.AccessToken;
            Properties.Settings.Default.REFRESH_TOKEN = authResult.RefreshToken;
            Properties.Settings.Default.ACCESS_TOKEN_EXPIRES =
            DateTimeOffset.UtcNow.AddSeconds(authResult.ExpiresIn);

            Properties.Settings.Default.Save();
        }

        public void StartRefreshTimer(AuthResult authResult)
        {
            try
            {
                if (_refreshTimer != null && _refreshTimer.Enabled)
                {
                    _refreshTimer.Stop();
                    _refreshTimer.Dispose();
                }

                _refreshTimer = new Timer((authResult.ExpiresIn - 60) * 1000);
                _refreshTimer.Elapsed += async (sender, e) =>
                {
                    var apiResult = await Instance.AuthRefresh(authResult.RefreshToken);
                    // If cannot refresh, retry in 30 seconds
                    if (apiResult.Status == 0)
                    {
                        var result = apiResult.Data as AuthResult;
                        SetAuthProperties(result);
                        StartRefreshTimer(result);
                    }
                };
                _refreshTimer.AutoReset = false;
                _refreshTimer.Start();
            }
            catch (Exception exc)
            {
                Console.Write(exc);
            }
        }
        #endregion
        #region Authentication operations

        public async Task<ApiResult> AuthLogin(string userName, string password)
        {
            var url = string.Format("authentication/login");
            var body = new { userName, password };
            return await GenericPost(url, JsonConvert.SerializeObject(body), typeof(AuthResult));
        }

        public async Task<ApiResult> AuthRefresh(string refreshToken)
        {
            var url = string.Format("authentication/refresh");
            var body = new { refreshToken };
            return await GenericPost(url, JsonConvert.SerializeObject(body), typeof(AuthResult));
        }

        #endregion
        #region Material Tracking operations

        public async Task<ApiResult> MTGetProjects()
        {
            return await GenericGet("material-tracking/projects", typeof(List<Project>));
        }

        public async Task<ApiResult> MTGetStages()
        {
            return await GenericGet("material-tracking/stages", typeof(List<Stage>));
        }

        #endregion

        #region BIM operations

        public async Task<ApiResult> GetMaterialsForSync(int projectId, int lastSyncId)
        {
            string uri = string.Format("material-tracking/projects/{0}/bim-updates?lastSyncId={1}", projectId, lastSyncId);

            return await GenericGet(uri, typeof(List<MaterialEntity>));
        }

        public async Task<ApiResult> InsertUpdateBimSync(int projectId, object data)
        {
            string uri = string.Format("material-tracking/projects/{0}/bim_sync", projectId);

            return await GenericPost(uri, JsonConvert.SerializeObject(data), typeof(BIMSyncSession));
        }

        public async Task<ApiResult> UploadBIMVideo(int projectId, int bimSyncSessionId, string filePath)
        {
            string uri = string.Format("material-tracking/projects/{0}/bim_sync/{1}/video", projectId, bimSyncSessionId);

            var result = GENERIC_RESULT;

            try
            {
                SetupAuthorization();
                using (var fileStream = File.OpenRead(filePath))
                {
                    var requestBody = new StreamContent(fileStream);
                    var response = await _client.PostAsync(uri, requestBody);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<ApiResult>(content);
                    }
                    else
                        result.Message = response.ReasonPhrase;
                }
            }
            catch (Exception exc)
            {
                Console.Write(exc);
                result.Message = "Failed to connect to server, please try again later";
            }

            return result;

        }

        public async Task<ApiResult> LoginUser(string userName, string password)
        {
            string uri = string.Format("authentication/login");
            var data = new
            {
                userName,
                password
            };

            return await GenericPost(uri, JsonConvert.SerializeObject(data), typeof(AuthResult));
        }

        #endregion
    }
}
