using astorWork.Helper;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.PowerBI.Api.V2;
using Microsoft.PowerBI.Api.V2.Models;
using Microsoft.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace astorWork.aspx.Material
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            EmbedConfig embedConfig = Task.Run(GetEmbedConfig).Result;

            if (embedConfig.ErrorMessage != null)
            {
                lblError.Text = embedConfig.ErrorMessage;
                divPowerBIWrapper.Disabled = true;
            }
            else
            {
                hdnEmbedToken.Value = embedConfig.EmbedToken.Token;
                hdnEmbedUrl.Value = embedConfig.EmbedUrl;
                hdnReportId.Value = embedConfig.Id;
            }
        }

        private async Task<EmbedConfig> GetEmbedConfig()
        {
            EmbedConfig embedConfig = new EmbedConfig()
            {
                ErrorMessage = "Fail to get report"
            };

            try
            {
                var credential = new UserPasswordCredential(ConfigurationManager.AppSettings["PowerBIUserName"], ConfigurationManager.AppSettings["PowerBIPassword"]);

                string url = string.Format("https://login.windows.net/{0}/oauth2/token", ConfigurationManager.AppSettings["PowerBITenentId"]);

                ClientCredential clientCredential = new ClientCredential(ConfigurationManager.AppSettings["PowerBIClientId"], ConfigurationManager.AppSettings["PowerBIClientSecret"]);
                var authenticationContext = new AuthenticationContext(url);

                var authenticationResult = await authenticationContext.AcquireTokenAsync(ConfigurationManager.AppSettings["PowerBIResourceUrl"], ConfigurationManager.AppSettings["PowerBINativeClientId"], credential);

                Session["PowerBIAccessToken"] = authenticationResult.AccessToken;

                var tokenCredentials = new TokenCredentials(authenticationResult.AccessToken, "Bearer");
                // Create a Power BI Client object. It will be used to call Power BI APIs.
                using (var client = new PowerBIClient(new Uri(ConfigurationManager.AppSettings["PowerBIApiUrl"]), tokenCredentials))
                {
                    // You will need to provide the GroupID where the dashboard resides.
                    ODataResponseListReport reports = await client.Reports.GetReportsInGroupAsync(ConfigurationManager.AppSettings["PowerBIGroupId"]);
                    //ODataResponseListReport reports = await client.Reports.GetReportsAsync();
                    
                    // Get the first report in the group.
                    Report report = reports.Value.Where(r => r.Id == ConfigurationManager.AppSettings["PowerBIMTReportId"]).First();

                    if (report != null)
                    {
                        // Generate Embed Token.
                        var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                        EmbedToken tokenResponse = client.Reports.GenerateTokenInGroup(ConfigurationManager.AppSettings["PowerBIGroupId"], report.Id, generateTokenRequestParameters);

                        // Generate Embed Configuration.
                        embedConfig = new EmbedConfig()
                        {
                            EmbedToken = tokenResponse,
                            EmbedUrl = report.EmbedUrl,
                            Id = report.Id
                        };
                    }
                    else
                    {
                        embedConfig = new EmbedConfig()
                        {
                            ErrorMessage = "No matching report found"
                        };
                    }
                }
            }
            catch (Exception exc)
            {
                embedConfig = new EmbedConfig()
                {
                    ErrorMessage = exc.Message
                };
            }
            /*
            if (Session["PowerBIRefreshToken"] != null)
            {
                using (HttpClient hc = new HttpClient())
                {
                    var values = new Dictionary<string, string>
{
   { "grant_type", "refresh_token" },
   { "resource", ConfigurationManager.AppSettings["PowerBIResourceUrl"] },
   { "client_id", ConfigurationManager.AppSettings["PowerBIClientId"] },
   { "client_secret", ConfigurationManager.AppSettings["PowerBIClientSecret"]},
   { "refresh_token", Session["PowerBIRefreshToken"].ToString()}
};

                    var content = new FormUrlEncodedContent(values);
                    string url = string.Format("https://login.windows.net/{0}/oauth2/token", ConfigurationManager.AppSettings["PowerBITenentId"]);

                    HttpResponseMessage hrm = hc.PostAsync(url, content).Result;
                    string responseData = "";
                    if (hrm.IsSuccessStatusCode)
                    {
                        Stream data = await hrm.Content.ReadAsStreamAsync();
                        using (StreamReader reader = new StreamReader(data, Encoding.UTF8))
                        {
                            responseData = reader.ReadToEnd();
                        }
                    }
                    var token = JsonConvert.DeserializeObject<MicrosoftAccessToken>(responseData);

                    Session["PowerBIAccessToken"] = token.access_token;
                    Session["PowerBIRefreshToken"] = token.refresh_token;
                }
            }
            else
            {
                try
                {
                    using (HttpClient hc = new HttpClient())
                    {
                        List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                        values.Add(new KeyValuePair<string, string>("grant_type", "password"));
                        values.Add(new KeyValuePair<string, string>("scope", "openid"));
                        values.Add(new KeyValuePair<string, string>("resource", "https://analysis.windows.net/powerbi/api"));
                        values.Add(new KeyValuePair<string, string>("client_id", ConfigurationManager.AppSettings["PowerBIClientId"]));
                        values.Add(new KeyValuePair<string, string>("client_secret", ConfigurationManager.AppSettings["PowerBIClientSecret"]));
                        values.Add(new KeyValuePair<string, string>("username", ConfigurationManager.AppSettings["PowerBIUserName"]));
                        values.Add(new KeyValuePair<string, string>("password", ConfigurationManager.AppSettings["PowerBIPassword"]));                        

                        var content = new FormUrlEncodedContent(values);
                        string url = string.Format("https://login.windows.net/{0}/oauth2/token", ConfigurationManager.AppSettings["PowerBITenentId"]);

                        HttpResponseMessage hrm = hc.PostAsync(url, content).Result;
                        string responseData = "";
                        if (hrm.IsSuccessStatusCode)
                        {
                            Stream data = await hrm.Content.ReadAsStreamAsync();
                            using (StreamReader reader = new StreamReader(data, Encoding.UTF8))
                            {
                                responseData = reader.ReadToEnd();
                            }
                        }
                        var token = JsonConvert.DeserializeObject<MicrosoftAccessToken>(responseData);

                        Session["PowerBIAccessToken"] = token.access_token;
                        Session["PowerBIRefreshToken"] = token.refresh_token;
                    }
                }
                catch (Exception exc)
                {
                    embedConfig = new EmbedConfig()
                    {
                        ErrorMessage = exc.Message
                    };
                }
            }

            if (Session["PowerBIAccessToken"] != null)
            {
                try
                {
                    var tokenCredentials = new TokenCredentials(Session["PowerBIAccessToken"].ToString(), "Bearer");
                    // Create a Power BI Client object. It will be used to call Power BI APIs.
                    using (var client = new PowerBIClient(new Uri(ConfigurationManager.AppSettings["PowerBIApiUrl"]), tokenCredentials))
                    {
                        // You will need to provide the GroupID where the dashboard resides.
                        ODataResponseListReport reports = await client.Reports.GetReportsInGroupAsync(ConfigurationManager.AppSettings["PowerBIGroupId"]);
                        //ODataResponseListReport reports = await client.Reports.GetReportsAsync();

                        // Get the first report in the group.
                        Report report = reports.Value.FirstOrDefault();

                        // Generate Embed Token.
                        var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "view");
                        EmbedToken tokenResponse = client.Reports.GenerateTokenInGroup(ConfigurationManager.AppSettings["PowerBIGroupId"], report.Id, generateTokenRequestParameters);

                        // Generate Embed Configuration.
                        embedConfig = new EmbedConfig()
                        {
                            EmbedToken = tokenResponse,
                            EmbedUrl = report.EmbedUrl,
                            Id = report.Id
                        };
                    }
                }
                catch (Exception exc)
                {
                    embedConfig = new EmbedConfig()
                    {
                        ErrorMessage = exc.Message
                    };
                }

            }
            */
            return embedConfig;
        }
    }
}
