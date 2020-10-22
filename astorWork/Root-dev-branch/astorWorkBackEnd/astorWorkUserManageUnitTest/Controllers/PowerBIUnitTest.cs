using astorWorkUserManageUnitTest.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkUserManageUnitTest.Controllers
{
    public class PowerBIUnitTest: MasterClass
    {
        public static readonly string API_BASE_URL = "api/powerbi";

        private string accessToken;

        [Fact]
        public async Task TestAzureADTokenAndExpire()
        {
            accessToken = string.Empty;
            var response = await _sut.Client.GetAsync($"{API_BASE_URL}/authenticate");

            var data = await ValidateReponse(response);
            Assert.NotNull(data);

            try
            {
                accessToken = data.Value<string>("access_token");
                var expiresOn = data.Value<long>("expires_on");

                Assert.False(string.IsNullOrEmpty(accessToken));

                TimeSpan expiresSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1));
                Assert.True(expiresOn > expiresSpan.TotalSeconds);
            }
            catch(Exception exc)
            {
                Assert.Null(exc);
            }
        }

        [Fact]
        public async Task TestListPowerBIReports()
        {
            if(string.IsNullOrEmpty(accessToken))
                await TestAzureADTokenAndExpire();

            List<KeyValuePair<string, string>> vals = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("powerbi_token", accessToken)
            };

            var response = await _sut.Client.GetAsync($"{API_BASE_URL}/reports");

            var data = await ValidateReponse(response);
            Assert.NotNull(data);

            try
            {
                var reports = data as JArray;
                Assert.NotNull(reports);
            }
            catch (Exception exc)
            {
                Assert.Null(exc);
            }
        }
    }
}
