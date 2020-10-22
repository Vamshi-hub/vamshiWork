using astorWorkBackEndUnitTest.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class NotificationTimerControllerUnitTest : CommonFunctions
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
    }
}
