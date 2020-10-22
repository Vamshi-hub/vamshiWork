using astorWorkBackEndUnitTest.Common;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class BIMControllerUnitTest: CommonFunctions
    {
        [Fact]
        public async Task TestGetStages_ShouldHaveMoreThanOne()
        {
            var endpoint = "stages";

            await GetResponseJSONData(endpoint, true);

            AssertGetReqConnection(response);

            Assert.NotEmpty(responseJSONArray);
        }

        [Fact]
        public async Task TestGetBIMUpdates_ShouldHaveMoreThanOne_GivenNoSyncId()
        {
            var endpoint = string.Format("/projects/{0}/bim_updates", 1);

            await GetResponseJSONData(endpoint, true);

            AssertGetReqConnection(response);

            Assert.NotEmpty(responseJSONArray);
        }

        [Fact]
        public async Task TestGetBIMUpdates_ShouldHaveLess_GivenSyncId()
        {
            var endpoint = string.Format("/projects/{0}/bim_updates", 1);

            await GetResponseJSONData(endpoint, true);

            AssertGetReqConnection(response);

            Assert.NotEmpty(responseJSONArray);

            int countNoSyncId = responseJSONArray.Count;

            endpoint = string.Format("/projects/{0}/bim_updates?lastSyncId={1}", 1, 1);
            await GetResponseJSONData(endpoint, true);

            AssertGetReqConnection(response);

            Assert.NotEmpty(responseJSONArray);

            int countWithSyncId = responseJSONArray.Count;

            countNoSyncId.Should().BeGreaterThan(countWithSyncId);

        }

        [Fact]
        public async Task TestGetBIMSyncSessions_ShouldHaveMoreThanOne()
        {
            var endpoint = string.Format("/projects/{0}/bim_sync", 1);

            await GetResponseJSONData(endpoint, true);

            AssertGetReqConnection(response);

            Assert.NotEmpty(responseJSONArray);
        }

        [Fact]
        public async Task TestGetBIMSyncSessionDetails_ShouldHaveSyncedMaterials()
        {
            var endpoint = string.Format("/projects/{0}/bim_sync", 1);

            await GetResponseJSONData(endpoint, true);

            AssertGetReqConnection(response);

            Assert.NotEmpty(responseJSONArray);

            int syncId = responseJSONArray.First.Value<int>("id");

            endpoint = string.Format("/projects/{0}/bim_sync/{1}", 1, syncId);

            await GetResponseJSONData(endpoint, false);

            AssertGetReqConnection(response);

            Assert.NotEmpty(responseJSONObj.Value<JArray>("syncedMaterials"));
        }


        [Fact]
        public async Task TestGetForgeToken_ShouldHaveAccessToken()
        {
            var endpoint = "/forge-auth";

            await GetResponseJSONData(endpoint, false);

            AssertGetReqConnection(response);

            Assert.NotEmpty(responseJSONObj);
            Assert.NotNull(responseJSONObj["access_token"]);
        }
    }
}
