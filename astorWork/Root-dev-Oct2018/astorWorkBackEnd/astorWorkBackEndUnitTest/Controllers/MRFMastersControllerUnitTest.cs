using astorWorkBackEndUnitTest.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class MRFMastersControllerUnitTest : CommonFunctions
    {
        static string projectID = "1";
        static int vendorID = 1;
        static string blk = "BLK 2";
        string baseAPIEndPt = "projects/" + projectID + "/mrfs";

        [Fact]
        public async Task TestCreateMRF_ShouldReturnStatus405_GivenExistingMRFWithSameProjectBlockLevelZoneVendorAndMaterialType()
        {
            // Given
            string apiEndPt = baseAPIEndPt;
            var content = new
            {
                block = blk,
                level = 3,
                zone = "B",
                materialTypes = new string[] { "Rebar", "Beam" },
                orderDate = "2018-04-06",
                plannedCastingDate = "2018-04-08",
                expectedDeliveryDate = "2018-04-01",
                vendorId = vendorID,
                officerUserIds = new int[] { 2, 3 }
            };
            string fieldToCheck = "status";

            // When
            await PostJSONDataAndGetResponse(apiEndPt, JsonConvert.SerializeObject(content), fieldToCheck);

            // Then
            AssertGetReqConnection(response);
            Assert.Equal("405", responseJSONObj[fieldToCheck].ToString());
            //AssertRecordCount(responseJSONArray, expectedCount);
            //AssertListContainsSelectedField(response, responseJSONArray, "block", block);
        }

        [Fact]
        public async Task TestCreateMRF_ShouldReturnCount26_GivenNoMRFWithSameProjectBlockLevelZone()
        {
            // Given
            string apiEndPt = baseAPIEndPt;
            var content = new {
                block = blk,
                level = 3,
                zone = "A",
                materialTypes = new string[]{"Rebar", "Beam"},
                orderDate = "2018-04-06",
                plannedCastingDate = "2018-04-08",
                expectedDeliveryDate = "2018-04-01",
                vendorId = 2,
                officerUserIds = new int[] { 2, 3}
            };

             // When
             await PostJSONDataAndGetResponse(apiEndPt, JsonConvert.SerializeObject(content));

            // Then
            int result = int.Parse(responseJSONObj["materialCount"].ToString());
            Assert.Equal(2, result);
            await DeleteFromDb("/projects/1/mrfs/" + responseJSONObj["id"].ToString());
        }

        [Fact]
        public async Task TestListMRFs_ShouldReturnCount1AndProgress_GivenBlock2()
        {
            // Given
            string markingNo = "A2R5B";
            string apiEndPt = baseAPIEndPt + $"?block={blk}&marking_no={markingNo}&vendor_id={vendorID}";

            // When
            await GetResponseJSONData(apiEndPt, true);

            // Then
            AssertRecordCount(responseJSONArray, 1);
            Assert1stInListContainsSelectedFieldValue(response, responseJSONArray, "orderDate", "07/05/2018"); 
            Assert1stInListContainsSelectedFieldValue(response, responseJSONArray, "progress", (0/3).ToString());
        }

        [Fact]
        public async Task TesListMRFs_ShouldReturn1_GivenNoBlock()
        {
            // Given
            string apiEndPt = baseAPIEndPt;

            // When
            await GetResponseJSONData(apiEndPt, true);

            // Then
            AssertGetReqConnection(response);
            AssertRecordCount(responseJSONArray, 2);
        }

        [Fact]
        public async Task TestGetLocationList_ShouldReturn30LevelsAnd2Zones_GivenBlock2()
        {
            // When
            await GetResponseJSONData(baseAPIEndPt + "/location?block=" + blk, true);
            int count = responseJSONArray.Count;

            // Then
            AssertGetReqConnection(response);
            int expectedCount = 30;
            AssertRecordCount(responseJSONArray, expectedCount);
            AssertValuesInChildList(responseJSONArray, "zones", "A");
        }

    }
}
