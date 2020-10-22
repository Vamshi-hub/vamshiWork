using astorWorkBackEndUnitTest.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class ProjectMastersControllerUnitTest: CommonFunctions
    {
        static string baseAPIEndPt = "projects/";

        [Fact]
        public async Task TestGetProjectInformation_ShouldReturn6MaterialTypes10BlocksAnd3MRFs_GivenProjectID1()
        {
            // Given
            int projectID = 1;

            // When
            await GetResponseJSONData(baseAPIEndPt + projectID.ToString(), false);

            // Then
            AssertRecordCountInField(responseJSONObj, 6, "materialTypes");
            AssertRecordCountInField(responseJSONObj, 10, "blocks");
            AssertRecordCountInField(responseJSONObj, 2, "mrfs");
        }
        [Fact]
        public async Task TestCreateProject()
        {
            // Given
            string strName = "Unit Test" + new Random().Next();

            var strContent = new { ID = 0, Name = strName, Description = "Unit Test", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), ProjectManagerID = 8, ProjectManagerName = "prad"};
            var reqstring = JsonConvert.SerializeObject(strContent);
            // When
            await PostJSONDataAndGetResponse(baseAPIEndPt, reqstring);

            // Then
            AssertGetReqConnection(response);

        }
        [Fact]
        public async Task TestUpdateProject()
        {
            // Given
            string strName = "Unit Test";
            var strContent = new { ID = 8, Name = strName, Description = "Unit Test Update", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1), ProjectManagerID = 7, ProjectManagerName = "admin" };
            var reqstring = JsonConvert.SerializeObject(strContent);
            
            // When
            await PutJSONData(baseAPIEndPt + "8", strContent.ToString());

            // Then
            AssertGetReqConnection(response);
            

        }
    }
}
