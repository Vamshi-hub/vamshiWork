using astorWorkBackEnd;
using astorWorkBackEnd.Controllers;
using astorWorkBackEndUnitTest.Common;
using astorWorkDAO;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class MaterialMastersControllerUnitTest : CommonFunctions
    {
        static string baseAPIEndPt = "projects/1/materials";
        static string block = "BLK 7";

        [Fact]
        public async Task TestListMaterials_ShouldReturn408_GivenBlock7()
        {
            // Given
            string block = "BLK 7";

            // When
            await GetResponseJSONData(baseAPIEndPt + "?block=" + block, true);

            // Then
            AssertGetReqConnection(response);
            AssertRecordCount(responseJSONArray, 408);
            Assert1stInListContainsSelectedFieldValue(response, responseJSONArray, "block", block);
        }

        [Fact]
        public async Task TestListMaterials_ShouldReturn0_GivenBlock20()
        {
            // Given
            string block = "BLK 20";

            // When
            await GetResponseJSONData(baseAPIEndPt + "?Block=" + block, true);
            int count = responseJSONArray.Count;

            // Then
            AssertRecordCount(responseJSONArray, 0);
            AssertListDoesNotContainSelectedFieldValue(response, responseJSONArray, "block", block);
        }

        [Fact]
        public async Task TestListMaterialTypes_ShouldReturnCount6()
        {
            // When
            await GetResponseJSONData(baseAPIEndPt + "/types", true);
            int count = responseJSONArray.Count;

            // Then
            AssertRecordCount(responseJSONArray, 6);
        }

        [Fact]
        public async Task TestGetMaterialDetail_GivenMaterialID11IsEdited()
        {
            // Given
            string projectID = "1";
            string materialID = "11";
            string remarks = "Change Remarks";
            string deliveryDate = "2018-04-14";

            // Given
            var content = new
            {
                remarks = remarks,
                expectedDeliveryDate = deliveryDate
            };

            // When
            await PutJSONData(baseAPIEndPt + "/" + materialID, JsonConvert.SerializeObject(content));

            // When
            await GetResponseJSONData("projects/" + projectID + "/materials/" + materialID, false);

            // Then
            AssertGetReqConnection(response);
            Assert.Contains(materialID, (String)responseJSONObj["id"]);
            Assert.Contains(remarks, (String)responseJSONObj["remarks"]);
            DateTime resultDate = DateTime.Parse(deliveryDate);
            DateTime expectedDate = DateTime.Parse(responseJSONObj["expectedDeliveryDate"].ToString());
            Assert.Equal(resultDate, expectedDate);
            AssertValuesInChildList(responseJSONObj["trackingHistory"], "stageName", "Installed");
            AssertValuesInChildList(responseJSONObj["trackingHistory"], "stageStatus", "0");
        }

        [Fact]
        public async Task TestGetMaterialDetail_GivenMaterialID14102IsNotEditedBeforeButAssignedMRF()
        {
            // Given
            string materialID = "4026";

            // When
            await GetResponseJSONData(baseAPIEndPt + "/" + materialID, false);

            // Then
            AssertGetReqConnection(response);
            Assert.Contains(materialID, (String)responseJSONObj["id"]);
            Assert.Contains("04/06/2018", (String)responseJSONObj["orderDate"]);
            //AssertValuesInChildList(responseJSONObj["trackingHistory"], "stageName", "Installed");
            //AssertValuesInChildList(responseJSONObj["trackingHistory"], "stageStatus", "0");
        }

        [Fact]
        public async Task TestGetMaterialDetail_GivenMaterialID26WithNoEditAndNoMRF()
        {
            // Given
            string projectID = "1";
            string materialID = "26";

            // When
            await GetResponseJSONData(baseAPIEndPt + "/" + materialID, false);

            // Then
            AssertGetReqConnection(response);
            Assert.Contains(materialID, (String)responseJSONObj["id"]);
            Assert.Null((string)responseJSONObj["remarks"]);
            Assert.Contains("01/01/0001", ((String)responseJSONObj["expectedDeliveryDate"]));
            responseJSONObj = JObject.Parse(responseJSONObj["drawing"].ToString());
            Assert.Contains("3", ((String)responseJSONObj["revisionNo"]));
            //AssertValuesInChildList(responseJSONObj["trackingHistory"], "stageName", "Installed");
            //AssertValuesInChildList(responseJSONObj["trackingHistory"], "stageStatus", "1");
        }
        [Fact]
        public async Task TestGetMaterialMaster_ImportFromTemplate()
        {

            await GetResponseJSONData("/projects", true);
            int projectId = responseJSONArray.First.Value<int>("id");

            await GetResponseJSONData("/vendors", true);
            int vendorId = responseJSONArray.First.Value<int>("id");

            var emptyFile = new StreamContent(File.OpenRead(@"TestData\import-material-template\empty.csv"));
            var wrongFile = new StreamContent(File.OpenRead(@"TestData\import-material-template\wrong-format.csv"));
            var normalFile = new StreamContent(File.OpenRead(@"TestData\import-material-template\normal.csv"));
            var apiEndpoint = string.Format("/projects/{0}/materials/import-template", projectId);

            using (var formDataContent = new MultipartFormDataContent()) {
                formDataContent.Add(new StringContent("Test Blk", Encoding.UTF8), "Block");
                formDataContent.Add(new StringContent("Test Material", Encoding.UTF8), "MaterialType");
                formDataContent.Add(new StringContent(vendorId.ToString(), Encoding.UTF8), "VendorId");
                formDataContent.Add(emptyFile, "TemplateFile", "empty.csv");
                var result = await PostMultiPartFormAndGetResponse(apiEndpoint, formDataContent);
                Assert.NotEqual(0, result.Status);
            }

            using (var formDataContent = new MultipartFormDataContent())
            {
                formDataContent.Add(new StringContent("Test Blk", Encoding.UTF8), "Block");
                formDataContent.Add(new StringContent("Test Material", Encoding.UTF8), "MaterialType");
                formDataContent.Add(new StringContent(vendorId.ToString(), Encoding.UTF8), "VendorId");
                formDataContent.Add(wrongFile, "TemplateFile", "wrong-format.csv");
                var result = await PostMultiPartFormAndGetResponse(apiEndpoint, formDataContent);
                Assert.NotEqual(0, result.Status);
            }

            using (var formDataContent = new MultipartFormDataContent())
            {
                formDataContent.Add(new StringContent("Test Blk", Encoding.UTF8), "Block");
                formDataContent.Add(new StringContent("Test Material", Encoding.UTF8), "MaterialType");
                formDataContent.Add(new StringContent(vendorId.ToString(), Encoding.UTF8), "VendorId");
                formDataContent.Add(normalFile, "TemplateFile", "normal.csv");
                var result = await PostMultiPartFormAndGetResponse(apiEndpoint, formDataContent);
                Assert.Equal(0, result.Status);

                await PostMultiPartFormAndGetResponse(string.Format("/projects/{0}/materials/delete-template-test", projectId), formDataContent);
            }

        }
    }
}
