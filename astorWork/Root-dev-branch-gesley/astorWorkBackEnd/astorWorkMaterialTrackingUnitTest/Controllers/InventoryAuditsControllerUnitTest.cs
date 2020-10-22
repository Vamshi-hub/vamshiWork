using astorWorkBackEndUnitTest.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class InventoryAuditsControllerUnitTest : CommonFunctions
    {
        static string projectID = "2";
        static string organisationID = "2";
        string baseAPIEndpoint = "projects/" + projectID + "/vendors/" + organisationID + "/inventory";

        [Fact]
        public async Task TestListInventory_ShouldReturn1()
        {   
            // When
            await GetResponseJSONData(baseAPIEndpoint, true);

            // Then
            AssertRecordCount(responseJSONArray, 1);
            //Assert1stInListContainsSelectedFieldValue(response, responseJSONArray, "markingNo", "K1R10Y");
        }

        [Fact]
        public async Task TestGetInformationForNewInventory_ShouldReturnCount24AndMaxSN1_GivenMaterialTypeSlab()
        {
            // Given
            string materialType = "Slab";

            // When
            await GetResponseJSONData(baseAPIEndpoint + "/pre-create?materialType=" + materialType, false);

            // Then
            AssertRecordCountInField(responseJSONObj, 24, "markingNos");
            AssertContainsValueInField(responseJSONObj, "B1S1C", "markingNos");
            AssertContainsValueInField(responseJSONObj, "1", "maxSN");
        }

        [Fact]
        public async Task TestCreateInventory_ShouldReturnErrorCode405AndMessage_GivenTrackerID7IsUsed()
        {
            // Given
            int trackerID = 7;
            await GetResponseJSONData(baseAPIEndpoint + "/pre-create?materialType=Slab", false);
            int maxSN = int.Parse(responseJSONObj["maxSN"].ToString());

            var content = new
            {
                markingNo = "B1S3C",
                sn = (maxSN + 1),
                castingDate = "2018-04-10",
                trackerId = trackerID
            };
              
            string fieldToCheck = "status";
            
            // When
            await PostJSONDataAndGetResponse(baseAPIEndpoint, JsonConvert.SerializeObject(content), fieldToCheck);

            // Then
            AssertGetReqConnection(response);
            AssertContainsValueInField(responseJSONObj, "405", fieldToCheck);
            //int result = int.Parse(responseJSONObj["statusCode"].ToString());
            //Assert.Equal(1001, result);
        }

        [Fact]
        public async Task TestCreateInventory_ShouldReturnInventoryListCount2_GivenInventoryCreated()
        {
            // Given
            int trackerID = 9;
            await GetResponseJSONData(baseAPIEndpoint + "/pre-create?materialType=Slab", false);
            int maxSN = int.Parse(responseJSONObj["maxSN"].ToString());
            var content = new
            {
                markingNo = "B2S1C",
                sn = (maxSN + 1),
                castingDate = "2018-04-10",
                trackerId = trackerID
            };

            // When
            await PostJSONDataAndGetResponse(baseAPIEndpoint, JsonConvert.SerializeObject(content));

            // Then
            await GetResponseJSONData(baseAPIEndpoint, true);
            AssertRecordCount(responseJSONArray, 2);
            await DeleteFromDb(baseAPIEndpoint + "/" + responseJSONObj["id"].ToString());
            
        }
    }
}
