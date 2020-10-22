using MaterialTrackApp.Class;
using MaterialTrackApp.DB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MaterialTrackApp.Utility
{
    public sealed class ApiClient
    {
        public static readonly ApiResult GENERIC_RESULT = new ApiResult()
        {
            Status = 1,
            Message = "Unkown error",
            Data = null
        };

        private static volatile ApiClient instance;
        private static object syncRoot = new Object();

        private HttpClient _client;
        private string _endpoint;

        private string _pb_access_token;

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

        public void InitClient(string endpoint)
        {
            _endpoint = endpoint;
            if (_client == null)
            {
                _client = new HttpClient();
                _client.MaxResponseContentBufferSize = 256000;
                _client.Timeout = TimeSpan.FromMilliseconds(8000);
            }
            _client.BaseAddress = new Uri(_endpoint);
        }

        #region Material Tracking operations

        public async Task<ApiResult> MTGetMaterialMasterByStatus(string status)
        {
            var result = GENERIC_RESULT;
            try
            {
                string url = string.Format("Material/Master?Status={0}", status);

                var response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    JArray json = JArray.Parse(content);

                    result = new ApiResult()
                    {
                        Status = 0,
                        Message = string.Empty,
                        Data = json
                    };
                }
                else
                    result.Message = response.ReasonPhrase;
            }
            catch (Exception exc)
            {
                result.Message = exc.Message;
            }

            return result;
        }

        public async Task<ApiResult> MTUpdateMaterialMasterByVendor(List<VendorUpdateData> vendorData)
        {
            var result = GENERIC_RESULT;
            try
            {
                string url = "Material/Master/UpdateByVendor";
                string body = JsonConvert.SerializeObject(vendorData);
                var response = await _client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    string message = string.Format("{0} records are updated", content);

                    result = new ApiResult()
                    {
                        Status = 0,
                        Message = message,
                        Data = null
                    };
                }
                else
                    result.Message = response.ReasonPhrase;
            }
            catch (Exception exc)
            {
                result.Message = exc.Message;
            }

            return result;
        }
        #endregion

        #region PowerBI related operations
        public async Task<ApiResult> PBGetAllGroups(string accessToken)
        {
            var result = GENERIC_RESULT;

            if (string.IsNullOrEmpty(accessToken))
            {
                result.Message = "Unauthorized";
            }
            else
            {
                try
                {
                    string url = string.Format("https://api.powerbi.com/v1.0/myorg/groups");
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await _client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(content);
                        JArray listGroupInfo = (JArray)json.GetValue("value");

                        result = new ApiResult()
                        {
                            Status = 0,
                            Message = string.Empty,
                            Data = listGroupInfo.Select(gi => new GroupInfo
                            {
                                ID = (string)gi["id"],
                                Name = (string)gi["name"],
                                IsReadOnly = (bool)gi["isReadOnly"]
                            }).ToList()
                        };
                    }
                    else
                        result.Message = response.ReasonPhrase;
                }
                catch (Exception exc)
                {
                    result.Message = exc.Message;
                }
            }

            return result;
        }

        public async Task<ApiResult> PBGetAllDashboardByGroup(GroupInfo gi, string accessToken)
        {
            var result = GENERIC_RESULT;

            if (string.IsNullOrEmpty(accessToken))
            {
                result.Message = "Unauthorized";
            }
            else
            {
                try
                {
                    string url = string.Format("https://api.powerbi.com/v1.0/myorg/groups/{0}/dashboards", gi.ID);
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await _client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(content);
                        JArray listDashboardInfo = (JArray)json.GetValue("value");

                        result = new ApiResult()
                        {
                            Status = 0,
                            Message = string.Empty,
                            Data = listDashboardInfo.Select(di => new DashboardInfo
                            {
                                ID = (string)di["id"],
                                DisplayName = (string)di["displayName"],
                                EmbedUrl = (string)di["embedUrl"],
                                Group = gi
                            }).ToList()
                        };
                    }
                    else
                        result.Message = response.ReasonPhrase;
                }
                catch (Exception exc)
                {
                    result.Message = exc.Message;
                }
            }

            return result;
        }

        public async Task<ApiResult> PBGetAllTilesByDashboard(DashboardInfo di, string accessToken)
        {
            var result = GENERIC_RESULT;

            if (string.IsNullOrEmpty(accessToken))
            {
                result.Message = "Unauthorized";
            }
            else
            {
                try
                {
                    string url = string.Format("https://api.powerbi.com/v1.0/myorg/groups/{0}/dashboards/{1}/tiles", di.Group.ID, di.ID);
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await _client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(content);
                        JArray listTileInfo = (JArray)json.GetValue("value");

                        result = new ApiResult()
                        {
                            Status = 0,
                            Message = string.Empty,
                            Data = listTileInfo.Select(ti => new TileInfo
                            {
                                ID = (string)ti["id"],
                                Title = (string)ti["title"],
                                SubTitle = (string)ti["subTitle"],
                                EmbedUrl = (string)ti["embedUrl"],
                                Dashboard = di
                            }).ToList()
                        };
                    }
                    else
                        result.Message = response.ReasonPhrase;
                }
                catch (Exception exc)
                {
                    result.Message = exc.Message;
                }
            }

            return result;
        }

        public async Task<ApiResult> PBGetDashboardEmbedConfig(DashboardInfo di, string accessToken)
        {
            var result = GENERIC_RESULT;

            if (string.IsNullOrEmpty(accessToken))
            {
                result.Message = "Unauthorized";
            }
            else
            {
                try
                {
                    var keyValues = new List<KeyValuePair<string, string>>();
                    keyValues.Add(new KeyValuePair<string, string>("accessLevel", "View"));

                    string url = string.Format("https://api.powerbi.com/v1.0/myorg/groups/{0}/dashboards/{1}/GenerateToken", di.Group.ID, di.ID);
                    var request = new HttpRequestMessage()
                    {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Post,
                        Content = new FormUrlEncodedContent(keyValues)
                    };
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await _client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        JObject json = JObject.Parse(content);

                        var embedConfig = new EmbedConfig()
                        {
                            EmbedToken = (string)json["token"],
                            EmbedUrl = di.EmbedUrl,
                            ID = di.ID
                        };
                        result = new ApiResult()
                        {
                            Status = 0,
                            Message = string.Empty,
                            Data = new List<EmbedConfig>() { embedConfig }
                        };
                    }
                    else
                        result.Message = response.ReasonPhrase;
                }
                catch (Exception exc)
                {
                    result.Message = exc.Message;
                }
            }

            return result;
        }

        public async Task<ApiResult> PBGetTileEmbedConfig(DashboardInfo di, string accessToken)
        {
            var result = GENERIC_RESULT;

            if (string.IsNullOrEmpty(accessToken))
            {
                result.Message = "Unauthorized";
            }
            else
            {
                try
                {
                    var tilesResult = await PBGetAllTilesByDashboard(di, accessToken);

                    if (tilesResult.Status == 0)
                    {
                        var keyValues = new List<KeyValuePair<string, string>>();
                        keyValues.Add(new KeyValuePair<string, string>("accessLevel", "View"));

                        var listEmbedConfig = new List<EmbedTileConfig>();
                        foreach (TileInfo ti in tilesResult.Data)
                        {
                            string url = string.Format("https://api.powerbi.com/v1.0/myorg/groups/{0}/dashboards/{1}/tiles/{2}/GenerateToken", di.Group.ID, di.ID, ti.ID);
                            var request = new HttpRequestMessage()
                            {
                                RequestUri = new Uri(url),
                                Method = HttpMethod.Post,
                                Content = new FormUrlEncodedContent(keyValues)
                            };
                            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                            var response = await _client.SendAsync(request);
                            if (response.IsSuccessStatusCode)
                            {
                                var content = await response.Content.ReadAsStringAsync();
                                JObject json = JObject.Parse(content);

                                var embedConfig = new EmbedTileConfig()
                                {
                                    EmbedToken = (string)json["token"],
                                    EmbedUrl = ti.EmbedUrl,
                                    ID = ti.ID,
                                    DashboardId = di.ID
                                };
                                listEmbedConfig.Add(embedConfig);
                            }
                        }
                        result = new ApiResult()
                        {
                            Status = 0,
                            Message = string.Empty,
                            Data = listEmbedConfig
                        };
                    }
                    else
                        result = tilesResult;
                }
                catch (Exception exc)
                {
                    result.Message = exc.Message;
                }
            }

            return result;
        }

        public async Task<bool> GetAccessToken(string ClientID, string Secret, string Username, string Password, string TenantId)
        {
            bool result = false;
            if (Application.Current.Properties.Keys.Contains("pb_access_token") && ((DateTime)Application.Current.Properties["pb_expire_time"]).Subtract(DateTime.UtcNow).TotalSeconds > 180)
                result = true;
            else
            {
                try
                {
                    List<KeyValuePair<string, string>> vals = new List<KeyValuePair<string, string>>();
                    vals.Add(new KeyValuePair<string, string>("grant_type", "password"));
                    vals.Add(new KeyValuePair<string, string>("scope", "openid"));
                    vals.Add(new KeyValuePair<string, string>("resource", "https://analysis.windows.net/powerbi/api"));
                    vals.Add(new KeyValuePair<string, string>("client_id", ClientID));
                    //vals.Add(new KeyValuePair<string, string>("client_secret", Secret));
                    vals.Add(new KeyValuePair<string, string>("username", Username));
                    vals.Add(new KeyValuePair<string, string>("password", Password));
                    string url = string.Format("https://login.windows.net/{0}/oauth2/token", TenantId);
                    HttpClient hc = new HttpClient();
                    HttpContent content = new FormUrlEncodedContent(vals);
                    HttpResponseMessage response = hc.PostAsync(url, content).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var tokenResult = await response.Content.ReadAsStringAsync();
                        var Token = JsonConvert.DeserializeObject<AccessToken>(tokenResult);
                        Application.Current.Properties["pb_access_token"] = Token.access_token;
                        Application.Current.Properties["pb_refresh_token"] = Token.refresh_token;
                        Application.Current.Properties["pb_expire_time"] = DateTime.UtcNow.AddSeconds(Token.expires_in);

                        result = true;
                    }
                    else
                        Debug.WriteLine(response.Content.ReadAsStringAsync().Result);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    Debug.WriteLine(exc.StackTrace);
                }
            }
            return result;
        }
        #endregion

        public async Task<ApiResult> GetAllBuildings()
        {
            var result = GENERIC_RESULT;

            try
            {
                var response = await _client.GetAsync("buildings");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var items = JsonConvert.DeserializeObject<List<BuildingEntity>>(content);
                    result = new ApiResult()
                    {
                        Status = 0,
                        Message = string.Empty,
                        Data = items
                    };
                }
                else
                    result.Message = response.ReasonPhrase;
            }
            catch (Exception exc)
            {
                result.Message = exc.Message;
            }

            return result;
        }

        public class ApiResult
        {
            // Status:
            // 0 - Success
            // 1 - General error
            // 2 - Network error
            // 3 - DB error
            public int Status { get; set; }
            public string Message { get; set; }
            public IEnumerable<object> Data { get; set; }
        }

        public class VendorUpdateData
        {
            public int MaterialID { get; set; }
            public string BeaconID { get; set; }
            public DateTime CastingDate { get; set; }
            public int LotNo { get; set; }
            public string Status { get; set; }
            public string UserName { get; set; }
            public int UserLocationID { get; set; }
        }
    }
}
