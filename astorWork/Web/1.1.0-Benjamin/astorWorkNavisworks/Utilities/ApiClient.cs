using astorWork_Navisworks.Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace astorWork_Navisworks.Utilities
{
    public class ApiClient
    {
        private static ApiClient instance;
        private static HttpClient httpClient;

        private ApiClient() { }

        public static ApiClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ApiClient();
                    // Init HTTP Client
                    httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri(Properties.Settings.Default.API_ENDPOINT);
                    httpClient.Timeout = TimeSpan.FromSeconds(15);
                }
                return instance;
            }
        }

        public async Task<ApiResult> GetMaterialsForSync(string apiKey, string projectId, DateTime lastSyncDT)
        {            
            string uri = string.Format("BIMSync/Materials?api_key={0}&project_id={1}&last_sync_utc_time={2}", apiKey, projectId, lastSyncDT.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));

            return await GenericGet(uri);
        }

        public async Task<ApiResult> UploadBIMVideo(string apiKey, string filePath)
        {
            string uri = string.Format("BIMSync/VideoUpload?api_key={0}", apiKey);

            return await GenericUpload(uri, filePath);

        }

        public async Task<ApiResult> LoginUser(string user_name, string password)
        {
            string uri = string.Format("BIMSync/BIMUser?user_name={0}&password={1}", user_name, password);

            return await GenericGet(uri);
        }

        public async Task<ApiResult> InsertUpdateBimSync(string apiKey, object data)
        {
            string uri = string.Format("BIMSync?api_key={0}", apiKey);

            return await GenericPost(uri, data);
        }

        public async Task<ApiResult> GenericGet(string uri)
        {
            ApiResult result = null;

            try
            {
                var response = await httpClient.GetAsync(uri);


                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    result = new ApiResult
                    {
                        Status = 0,
                        Data = JToken.Parse(responseData)
                    };
                }
                else
                    result = new ApiResult
                    {
                        Status = 1,
                        Message = response.ReasonPhrase
                    };
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

                result = new ApiResult
                {
                    Status = 1,
                    Message = exc.Message
                };
            }

            return result;
        }

        public async Task<ApiResult> GenericUpload(string uri, string filePath)
        {
            ApiResult result = null;

            try
            {
                var content = new StreamContent(new System.IO.FileStream(filePath, System.IO.FileMode.Open));
                var response = await httpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    result = new ApiResult
                    {
                        Status = 0,
                        Data = JToken.Parse(responseData)
                    };
                }
                else
                    result = new ApiResult
                    {
                        Status = 1,
                        Message = response.ReasonPhrase
                    };
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

                result = new ApiResult
                {
                    Status = 1,
                    Message = exc.Message
                };
            }

            return result;

        }

        public async Task<ApiResult> GenericPost(string uri, object data)
        {
            ApiResult result = null;

            try
            {
                var param = JsonConvert.SerializeObject(data);
                var content = new StringContent(param, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    result = new ApiResult
                    {
                        Status = 0,
                        Data = JToken.Parse(responseData)
                    };
                }
                else
                    result = new ApiResult
                    {
                        Status = 1,
                        Message = response.ReasonPhrase
                    };
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

                result = new ApiResult
                {
                    Status = 1,
                    Message = exc.Message
                };
            }

            return result;
        }
    }
}
