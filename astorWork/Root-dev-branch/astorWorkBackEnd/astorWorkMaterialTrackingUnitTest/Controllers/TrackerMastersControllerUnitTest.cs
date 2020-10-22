using astorWorkBackEndUnitTest.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class TrackerMastersControllerUnitTest: CommonFunctions
    {
        [Fact]
        public async Task TestGetInventoryList_GivenTag065D()
        {
            // Given
            string tag = "065D";

            // When
            await GetResponseJSONData("trackers/association?tags=" + tag, true);

            // Then
            AssertGetReqConnection(response);
            Assert.Contains("1629", (String)responseJSONArray[0]["trackerLabel"]);
            AssertValuesInChildList(responseJSONArray[0]["material"], "id", "16833", false);
            AssertValuesInChildList(responseJSONArray[0]["material"], "stageName", "Start Delivery", false);
        }

        [Fact]
        public async Task TestGenerateQRCodes_ShouldGenerate3UniqueQRCodes_GivenQty3()
        {
            // Given
            int qty = 3;

            // When
            await GetResponseJSONData("trackers/generate-qr-codes?qty=" + qty, true);
            int count = responseJSONArray.Count;

            // Then
            Assert.Equal(qty, count);
        }
    }
}
