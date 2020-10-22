using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkUserManageUnitTest.Common
{
    public class MasterClass
    {
        protected TestContext _sut;

        public MasterClass()
        {
            _sut = new TestContext();
        }

        protected async Task<JToken> ValidateReponse(HttpResponseMessage response)
        {
            JToken result = null;
            Assert.True(response.IsSuccessStatusCode);

            var responseStr = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseStr));

            var json = JToken.Parse(responseStr);
            Assert.NotNull(json);

            result = json.Value<JToken>("data");

            return result;
        }
    }
}
