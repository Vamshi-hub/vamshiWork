using astorWorkBackEndUnitTest.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class DashboardControllerUnitTest: CommonFunctions
    {
        static string projectID = "1";
        static string block = "BLK 2";
        string baseAPIEndPt = "projects/" + projectID + "/dashboard";

        [Fact]
        public async Task TestGetStats_ShouldReturnInstalledMaterialsCount2()
        {
            // When
            await GetResponseJSONData(baseAPIEndPt + "/stats", false);

            // Then
            int installedMaterialsCount = int.Parse(responseJSONObj["installedMaterialsCount"].ToString());
            Assert.Equal(0, installedMaterialsCount);
            //AssertListContainsSelectedField(response, responseJSONArray, "", block);
        }

        [Fact]
        public async Task TestGetQCOpenMaterialsAndDailyStatus_ShouldReturn2()
        {
            // When
            await GetResponseJSONData(baseAPIEndPt + "/qc-open-and-daily-status", false);

            // Then
            AssertRecordCount(JArray.Parse(responseJSONObj["qcOpenMaterialsList"].ToString()), 0);
        }

        [Fact]
        public async Task TestGetOverallProgress_ShouldReturn1AndProgress0GivenProjectID1AndBlock2()
        {
            // When
            await GetResponseJSONData(baseAPIEndPt + "/progress?block=" + block, true);
            int count = responseJSONArray.Count;

            // Then
            AssertGetReqConnection(response);
            AssertRecordCount(responseJSONArray, 0);
            Assert1stInListContainsSelectedFieldValue(response, responseJSONArray, "totalMaterialCount", 0.ToString());
        }

        [Fact]
        public async Task TestGetOverallProgress_GivenProjectID1AndBlock2()
        {
            // When
            await GetResponseJSONData(baseAPIEndPt + "/progress?block=" + block, true);
            int count = responseJSONArray.Count;

            // Then
            AssertGetReqConnection(response);
            int expectedCount = 30;
            AssertRecordCount(responseJSONArray, expectedCount);
            Assert1stInListContainsSelectedFieldValue(response, responseJSONArray, "totalMaterialCount", 78.ToString());
        }
    }
}
