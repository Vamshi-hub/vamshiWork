using astorWorkBackEndUnitTest.Common;
using astorWorkDAO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class MaterialStageMastersControllerUnitTest : CommonFunctions
    {
        private string baseURL = "material-stages/";

        [Fact]
        public async Task TestListMaterialStageMaster_ShouldReturnCount8()
        {
            // When
            await GetResponseJSONData(baseURL, true);

            // Then
            Assert.Equal(6, responseJSONArray.Count);
        }

        [Fact]
        public async Task TestGetMaterialStage_ShouldReturnNameBeforeInstallQC_GivenID8()
        {
            // Given
            int ID = 5;

            // When
            await GetResponseJSONData(baseURL + ID, false);

            // Then
            Assert.Equal("Before Install QC", responseJSONObj["name"]);
        }

        [Fact]
        public async Task TestCreateMaterialStage_ShouldReturnCount1_GivenNewMaterialStageCreated()
        {
            // Given
            string name = "Test Create Stage";

            var content = new
            {
                name = name,
                colour = "#00FF007F",
                isQCStage = 1,
                materialTypes = new List<string> { "Rebar", "Beam", "Façade" },
                nextStageId = 2
            };

            // When
            await PostJSONDataAndGetResponse(baseURL, JsonConvert.SerializeObject(content));
            string ID = responseJSONObj["id"].ToString();
            await GetResponseJSONData(baseURL + ID, false);

            // Then
            Assert.Equal(name, responseJSONObj["name"]);
            await DeleteFromDb(baseURL + ID);
        }

        [Fact]
        public async Task TestEditMaterialStageList_ShouldReturnNewOrder_GivenOrderIsSwapped()
        {
            // Given
            int ID = 24;
            string content = "[" +  
                                 "{" +
                                     "\"id\": 22," +
                                     "\"name\": \"Before Deliver QC\"," +
                                     "\"colour\": \"#ffff99\"," +
                                     "\"isQCStage\": 1," +
                                     "\"isEditable\": 0" +
                                 "}," +
                                 "{" +
                                     "\"id\": " + ID + "," +
                                     "\"name\": \"Delivered\"," +
                                     "\"colour\": \"#66ff99\"," +
                                     "\"isQCStage\": 0," +
                                     "\"isEditable\": 0" +
                                 "}," +
                                 "{" +
                                     "\"id\": 23," +
                                     "\"name\": \"Start Delivery\"," +
                                     "\"colour\": \"#ffff66\"," +
                                     "\"isQCStage\": 0," +
                                     "\"isEditable\": 1" +
                                 "}," +
                                 "{" +
                                     "\"id\": 25," +
                                     "\"name\": \"After Delivered QC\"," +
                                     "\"colour\": \"#00ff00\"," +
                                     "\"isQCStage\": 1," +
                                     "\"isEditable\": 1" +
                                 "}," +
                                 "{" +
                                     "\"id\": 26," +
                                     "\"name\": \"Before Install QC\"," +
                                     "\"colour\": \"#99ccff\"," +
                                     "\"isQCStage\": 1," +
                                     "\"isEditable\": 1" +
                                 "}," +
                                 "{" +
                                     "\"id\": 27," +
                                     "\"name\": \"Installed\"," +
                                     "\"colour\": \"#3399ff\"," +
                                     "\"isQCStage\": 0," +
                                     "\"isEditable\": 0" +
                                 "}" +
                             "]";

            // When
            await PostJSONDataAndGetResponse(baseURL + "stage-list", content);
            await GetResponseJSONData(baseURL + ID, false);

            // Then
            Assert.Equal("2", responseJSONObj["order"]);

            content = "[" +
                        "{" +
                            "\"id\": 22," +
                            "\"name\": \"Before Deliver QC\"," +
                            "\"colour\": \"#ffff99\"," +
                            "\"isQCStage\": 1," +
                            "\"isEditable\": 1" +
                        "}," +
                        "{" +
                            "\"id\": 23," +
                            "\"name\": \"Start Delivery\"," +
                            "\"colour\": \"#ffff66\"," +
                            "\"isQCStage\": 0," +
                            "\"isEditable\": 0" +
                        "}," +
                        "{" +
                            "\"id\": " + ID + "," +
                            "\"name\": \"Delivered\"," +
                            "\"colour\": \"#66ff99\"," +
                            "\"isQCStage\": 0," +
                            "\"isEditable\": 0" +
                        "}," +
                        "{" +
                            "\"id\": 25," +
                            "\"name\": \"After Delivered QC\"," +
                            "\"colour\": \"#00ff00\"," +
                            "\"isQCStage\": 1," +
                            "\"isEditable\": 1" +
                        "}," +
                        "{" +
                            "\"id\": 26," +
                            "\"name\": \"Before Install QC\"," +
                            "\"colour\": \"#99ccff\"," +
                            "\"isQCStage\": 1," +
                            "\"isEditable\": 1" +
                        "}," +
                        "{" +
                            "\"id\": 27," +
                            "\"name\": \"Installed\"," +
                            "\"colour\": \"#3399ff\"," +
                            "\"isQCStage\": 0," +
                            "\"isEditable\": 0" +
                        "}" +
                    "]";
            await PostJSONDataAndGetResponse(baseURL + "stage-list", content);
        }

        [Fact]
        public async Task TestEditMaterialStageList_ShouldReturnStatus406_Given1stStageWrong()
        {
            // Given
            string content = "[" +
                                 "{" +
                                     "\"id\": 10," +
                                     "\"name\": \"Delivered\"," +
                                     "\"colour\": \"#00FF003F\"," +
                                     "\"isQCStage\": 0," +
                                     "\"isEditable\": 0" +
                                 "}," +
                                 "{" +
                                     "\"id\": 12," +
                                     "\"name\": \"Before Deliver QC\"," +
                                     "\"colour\": \"#00FF003F\"," +
                                     "\"isQCStage\": 1," +
                                     "\"isEditable\": 0" +
                                 "}," +
                                 "{" +
                                     "\"id\": 11," +
                                     "\"name\": \"Start Delivery\"," +
                                     "\"colour\": \"#00FF007F\"," +
                                     "\"isQCStage\": 0," +
                                     "\"isEditable\": 1" +
                                 "}," +
                                 "{" +
                                     "\"id\": 9," +
                                     "\"name\": \"After Delivered QC\"," +
                                     "\"colour\": \"#00FF007F\"," +
                                     "\"isQCStage\": 1," +
                                     "\"isEditable\": 1" +
                                 "}," +
                                 "{" +
                                     "\"id\": 8," +
                                     "\"name\": \"Before Install QC\"," +
                                     "\"colour\": \"#D900FF\"," +
                                     "\"isQCStage\": 1," +
                                     "\"isEditable\": 1" +
                                 "}," +

                                 "{" +
                                     "\"id\": 14," +
                                     "\"name\": \"Installed\"," +
                                     "\"colour\": \"#0000FF7F\"," +
                                     "\"isQCStage\": 0," +
                                     "\"isEditable\": 0" +
                                 "}," +
                             "]";

            // When
            await PostJSONDataAndGetResponse(baseURL + "stageList", content, "status");

            // Then
            Assert.Equal("406", responseJSONObj["status"]);
        }

        [Fact]
        public async Task TestEditMaterialStageList_ShouldReturnStatus406_GivenLastStageWrong()
        {
            // Given
            string content = "[" +
                                 "{" +
                                     "\"id\": 12," +
                                     "\"name\": \"Before Deliver QC\"," +
                                     "\"colour\": \"#00FF003F\"," +
                                     "\"isQCStage\": 1," +
                                     "\"isEditable\": 0" +
                                 "}," +
                                 "{" +
                                     "\"id\": 10," +
                                     "\"name\": \"Delivered\"," +
                                     "\"colour\": \"#00FF003F\"," +
                                     "\"isQCStage\": 0," +
                                     "\"isEditable\": 0" +
                                 "}," +
                                 "{" +
                                     "\"id\": 11," +
                                     "\"name\": \"Start Delivery\"," +
                                     "\"colour\": \"#00FF007F\"," +
                                     "\"isQCStage\": 0," +
                                     "\"isEditable\": 1" +
                                 "}," +
                                 "{" +
                                     "\"id\": 9," +
                                     "\"name\": \"After Delivered QC\"," +
                                     "\"colour\": \"#00FF007F\"," +
                                     "\"isQCStage\": 1," +
                                     "\"isEditable\": 1" +
                                 "}," +
                                 "{" +
                                     "\"id\": 14," +
                                     "\"name\": \"Installed\"," +
                                     "\"colour\": \"#0000FF7F\"," +
                                     "\"isQCStage\": 0," +
                                     "\"isEditable\": 0" +
                                 "}," +
                                 "{" +
                                     "\"id\": 8," +
                                     "\"name\": \"Before Install QC\"," +
                                     "\"colour\": \"#D900FF\"," +
                                     "\"isQCStage\": 1," +
                                     "\"isEditable\": 1" +
                                 "}," +
                             "]";

            // When
            await PostJSONDataAndGetResponse(baseURL + "stageList", content, "status");

            // Then
            Assert.Equal("406", responseJSONObj["status"]);
        }

        [Fact]
        public async Task TestEditMaterialStageDetail_ShouldReturnColour00FF007A_GivenVendorColourChangedTo00FF007A()
        {
            // Given
            int ID = 25;
            string colour = "#00FF007A";

            var content = new
            {
                name = "After Delivered QC",
                colour = colour,
                isQCStage = 1,
                materialTypes = new List<string> { "Rebar", "Beam", "Façade" },
                nextStageId = 26
            };

            // When
            await PutJSONData(baseURL + ID, JsonConvert.SerializeObject(content));
            await GetResponseJSONData(baseURL + ID, false);

            // Then
            Assert.Equal(colour, responseJSONObj["colour"]);
            string content2 = "{" +
                          "\"name\": \"After Delivered QC\"," +
                          "\"colour\": \"#00FF007F\"," +
                          "\"isQCStage\": 1" +
                          "\"nextStageId\": 1024" +
                      "}";
            await PutJSONData(baseURL + ID, content2);
        }
    }
}
