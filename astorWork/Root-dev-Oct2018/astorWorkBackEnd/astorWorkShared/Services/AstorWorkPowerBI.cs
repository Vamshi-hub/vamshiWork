using astorWorkShared.Utilities;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public class AstorWorkPowerBI : IAstorWorkPowerBI
    {
        public async Task<PowerBIAuthResult> AuthenticatePowerBIAsync(PowerBICredentials credentials)
        {
            List<KeyValuePair<string, string>> vals = new List<KeyValuePair<string, string>>();
            vals.Add(new KeyValuePair<string, string>("grant_type", "password"));
            vals.Add(new KeyValuePair<string, string>("scope", "openid"));
            vals.Add(new KeyValuePair<string, string>("resource", "https://analysis.windows.net/powerbi/api"));
            vals.Add(new KeyValuePair<string, string>("client_id", credentials.ClientId));
            vals.Add(new KeyValuePair<string, string>("client_secret", credentials.ClientSecret));
            vals.Add(new KeyValuePair<string, string>("username", credentials.UserName));
            vals.Add(new KeyValuePair<string, string>("password", credentials.Password));
            string url = string.Format("https://login.windows.net/{0}/oauth2/token", credentials.TenantId);

            PowerBIAuthResult result = null;

            using (var client = new HttpClient())
            {
                var requestContent = new FormUrlEncodedContent(vals);
                var responseMessage = await client.PostAsync(url, requestContent);
                string responseData = "";
                if (responseMessage.IsSuccessStatusCode)
                {
                    Stream data = await responseMessage.Content.ReadAsStreamAsync();
                    using (StreamReader reader = new StreamReader(data, Encoding.UTF8))
                    {
                        responseData = reader.ReadToEnd();
                        result = JsonConvert.DeserializeObject<PowerBIAuthResult>(responseData);
                    }
                }
            }

            return result;
        }

        public async Task<IEnumerable<Report>> GetReportsAsync(string pbToken, string workSpaceGUID)
        {
            IEnumerable<Report> result = null;
            if (!string.IsNullOrEmpty(pbToken) && !string.IsNullOrEmpty(workSpaceGUID))
            {
                try
                {
                    using (var powerBIClient = new PowerBIClient(new TokenCredentials(pbToken)))
                    {
                        var reports = await powerBIClient.Reports.GetReportsInGroupAsync(workSpaceGUID);
                        result = reports.Value;
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine("GetReports failed: " + exc.Message);
                }
            }

            return result;
        }

        public async Task<EmbedToken> GetEmbedTokenAsync(string pbToken, string workSpaceGUID, string reportGUID)
        {
            EmbedToken result = null;
            try
            {
                using (var powerBIClient = new PowerBIClient(new TokenCredentials(pbToken)))
                {
                    var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                    result = await powerBIClient.Reports.GenerateTokenInGroupAsync(workSpaceGUID, reportGUID, generateTokenRequestParameters);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("GetEmbedTokenAsync failed: " + exc.Message);
            }

            return result;
        }
    }
}
