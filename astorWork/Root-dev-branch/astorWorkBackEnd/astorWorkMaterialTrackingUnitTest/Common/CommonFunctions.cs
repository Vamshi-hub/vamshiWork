using astorWorkShared.GlobalResponse;
using FluentAssertions;
using Moq;
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

namespace astorWorkBackEndUnitTest.Common
{
    public class CommonFunctions
    {
        private readonly TestContext _sut;
        protected string test;

        public HttpResponseMessage response;
        public string responseString;
        public JArray responseJSONArray;
        public JObject responseJSONObj;
        public int delID;

        public CommonFunctions()
        {
            _sut = new TestContext();
        }

        public async Task GetResponseJSONData(string apiEndPt, bool isArray)
        {
            response = await _sut.Client.GetAsync(apiEndPt);
            responseString = await response.Content.ReadAsStringAsync();

            if (isArray)
                responseJSONArray = JArray.Parse(JObject.Parse(responseString)["data"].ToString());
            else
            {
                string data = JObject.Parse(responseString)["data"].ToString();
                responseJSONObj = JObject.Parse(data);
            }
        }

        public async Task GetResponse(string apiEndPt)
        {
            response = await _sut.Client.GetAsync(apiEndPt);
        }

        public async Task PostJSONDataAndGetResponse(string apiEndPt, string strContent, string checkField = "data")
        {
            StringContent jSONContent = new StringContent(strContent, Encoding.UTF8, "application/json");
            response = await _sut.Client.PostAsync(apiEndPt, jSONContent);
            responseString = await response.Content.ReadAsStringAsync();
            if (checkField != "data")
                responseJSONObj = JObject.Parse(responseString);
            else
            {
                responseJSONObj = JObject.Parse(responseString);
                string responseJSONObjStr = responseJSONObj[checkField].ToString();
                if (responseJSONObjStr != null && responseJSONObjStr.Length > 0)
                    responseJSONObj = JObject.Parse(responseJSONObjStr);
            }
        }

        public async Task<APIResponse> PostMultiPartFormAndGetResponse(string apiEndPt, MultipartFormDataContent formContent)
        {
            try
            {
                response = await _sut.Client.PostAsync(apiEndPt, formContent);
                responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<APIResponse>(responseString);
            }
            catch(Exception exc)
            {
                Console.WriteLine(exc.Message);
                return null;
            }
        }

        public async Task PutJSONData(string apiEndPt, string strContent, bool getResponse=false)
        {
            StringContent jSONContent = new StringContent(strContent, Encoding.UTF8, "application/json");
            response = await _sut.Client.PutAsync(apiEndPt, jSONContent);
            if (getResponse)
            {
                responseString = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseString))
                {
                    responseJSONObj = JObject.Parse(responseString);
                }
            }
            //responseString = await response.Content.ReadAsStringAsync();
            //responseJSONObj = JObject.Parse(responseString);
        }

        public async Task DeleteFromDb(string apiEndPt)
        {
            await _sut.Client.DeleteAsync(apiEndPt);
        }

        public void AssertGetReqConnection(HttpResponseMessage response, Object httpStatusCode = null)
        {
            response.EnsureSuccessStatusCode();
            response.StatusCode.Should().Be(httpStatusCode ?? HttpStatusCode.OK);
        }

        public void AssertFirstValueInList(JArray responseJSONData, string field, string expectedValue)
        {
            //foreach (JObject o in responseJSONData)
            //{
            Assert.Contains(expectedValue, (String)(responseJSONData.First[field]));
            //}
        }

        public void AssertValuesInChildList(JToken token, string field, string expectedValue, bool isMultiple = true, int arrayIndex = -1)
        {
            string result = "";
            if (isMultiple)
                if (arrayIndex != -1)
                    result = (String)token[arrayIndex][field];
                else
                    result = (String)token.Last[field];
            else
                result = (String)token[field];
            Assert.Contains(expectedValue, result);
        }

        public void AssertValuesInChildList(JArray array, string field, string expectedValue)
        {
            string result = "";

            JToken token = array.Last;
            result = token[field].ToString();
            Assert.Contains(expectedValue, result);
        }

        public void Assert1stInListContainsSelectedFieldValue(HttpResponseMessage response, JArray responseJSONData, string field, string expectedValue)
        {
            AssertGetReqConnection(response);
            AssertFirstValueInList(responseJSONData, field, expectedValue);
        }

        public void AssertListDoesNotContainSelectedFieldValue(HttpResponseMessage response, JArray responseJSONData, string field, string expectedValue)
        {
            AssertGetReqConnection(response);
            Assert.DoesNotContain(expectedValue, responseString);
        }

        public void AssertRecordCount(JArray responseJSONData, int expectedCount)
        {
            int count = responseJSONData.Count;
            Assert.Equal(expectedCount, count);
        }

        public void AssertRecordCountInField(JObject responseJSONObj, int expectedCount, string field)
        {
            string result = responseJSONObj[field].ToString();
            int count = result.Split(',').Length;
            Assert.Equal(expectedCount, count);
        }

        public void AssertContainsValueInField(JObject responseJSONObj, string expectedValue, string field = "")
        {
            string result;
            if (field.Length == 0)
                result = responseJSONObj.ToString();
            result = responseJSONObj[field].ToString();
            Assert.Contains(expectedValue, result);
        }

        public async Task CheckMaterialStage(int projectID, int materialID, string stage, int stageStatus, int arrayIndex)
        {
            await GetResponseJSONData("projects/" + projectID + "/materials/" + materialID, false);
            Assert.Contains(materialID.ToString(), (String)responseJSONObj["id"]);
            AssertValuesInChildList(responseJSONObj["trackingHistory"], "stageName", stage, true, arrayIndex);
            AssertValuesInChildList(responseJSONObj["trackingHistory"], "stageStatus", stageStatus.ToString(), true, arrayIndex);
        }

        public int GetDelID()
        {
            JObject responseJSONObjResult = JObject.Parse(responseJSONObj["data"].ToString());
            return int.Parse(responseJSONObjResult["id"].ToString());
        }
    }
}
