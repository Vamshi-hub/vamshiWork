using astorWorkBackEndUnitTest.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class MaterialStageAuditsControllerUnitTest : CommonFunctions
    {
        static int organisationID = 2;
        string afterInventoryAPIEndpoint = $"vendors/{organisationID}/afterInventory";

        [Fact]
        public async Task TestPostAfterInventoryMaterialStageAudit_ShouldReturnStatus404_GivenTrackerIsNotInInventory()
        {
            // Given
            string mrfNo = "MRF-2018-00002";
            int trackerID = 24;
            string strContent = "{" +
                                  "\"mrfNo\": \"" + mrfNo + "\"," +
                                  "\"qcStatus\": 0," +
                                  "\"qcRemarks\": \"QC passed\"," +
                                  "\"trackerId\": " + trackerID + 
                                "}";
            string fieldToCheck = "status";

            // When
            await PostJSONDataAndGetResponse(afterInventoryAPIEndpoint, strContent, fieldToCheck);

            // Then
            AssertGetReqConnection(response);
            int result = int.Parse(responseJSONObj[fieldToCheck].ToString());
            Assert.Equal(404, result);
        }

        [Fact]
        public async Task TestPostAfterInventoryMaterialStageAudit_ShouldReturnStatus0AndBeforeDeliverQC_GivenTrackerIsInInventoryAndQCFailed()
        {
            // Given
            int trackerID = 2430;
            string mrfNo = "MRF-2018-00001";
            string strContent = "{" +
                                  "\"mrfNo\": \""+ mrfNo+"\"," +
                                  "\"qcStatus\": 0," +
                                  "\"qcRemarks\": \"QC failed\"," +
                                  "\"trackerId\": \"" + trackerID + "\"" +
                                "}";
            int materialID = 4025;
            string fieldToCheck = "status";
            

            // When
            await PostJSONDataAndGetResponse(afterInventoryAPIEndpoint, strContent, fieldToCheck);
            delID = GetDelID();

            // Then
            int result = int.Parse(responseJSONObj[fieldToCheck].ToString());
            Assert.Equal(0, result);
            await CheckMaterialStage(7, materialID, "Before Deliver QC", 2, 0);

            await DeleteFromDb($"MaterialStageAudits/{delID}?tracker_id={trackerID}");
        }

        [Fact]
        public async Task TestUpdateMaterialStage_ShouldReturnStatusCode0StageStartDelivery_GivenQCPassed()
        {
            // Given
            int materialID = 14102;
            string strContent = "{" +
                                  "\"qcStatus\": 1," +
                                  "\"qcRemarks\": \"QC passed\"," +
                                "}";
            string fieldToCheck = "status";
            string updateMaterialStageAPIEndpoint = $"materials/{materialID}/updateStage";
            int trackerID = 2430;

            // When
            await PostJSONDataAndGetResponse(updateMaterialStageAPIEndpoint, strContent, fieldToCheck);
            delID = GetDelID();

            // Then
            await CheckMaterialStage(7, materialID, "Start Delivery", 1, 2);

            await DeleteFromDb($"MaterialStageAudits/{delID}?tracker_id={trackerID}");
            delID = delID - 1;
            await DeleteFromDb($"MaterialStageAudits/{delID}?tracker_id={trackerID}");
        }

        [Fact]
        public async Task TestGetNextMaterialStageAudit_ShouldReturnAfterDeliveredQC_GivenMaterialID4011() {
            // Given
            int materialID = 4011;
            string apiEndPt = $"materials/{materialID}/nextStage";
            
            // When
            await GetResponseJSONData(apiEndPt, false);

            // Then
            AssertContainsValueInField(responseJSONObj, "After Delivered QC", "nextStageName");
        }

      
    }
}
