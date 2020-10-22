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
    public sealed class ForgeClient
    {
        public static readonly ApiResult GENERIC_RESULT = new ApiResult()
        {
            Status = 1,
            Message = "Unkown error",
            Data = null
        };

        private static volatile ForgeClient instance;
        private static object syncRoot = new Object();

        private HttpClient _client;
        private string _endpoint;

        private ForgeClient() { }

        public static ForgeClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new ForgeClient();
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
                _client.MaxResponseContentBufferSize = Constants.SETTING_API_BUFFER_SIZE;
                _client.Timeout = TimeSpan.FromSeconds(Constants.SETTING_API_TIMEOUT_SECONDS);
            }
            _client.BaseAddress = new Uri(_endpoint);
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
            public JToken Data { get; set; }
        }
        
    }
}
