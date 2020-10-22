using astorWorkBackEndUnitTest.Common;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class MaterialQCController : CommonFunctions
    {
        static int defect_id=4;
        static int case_id = 8;
        static int stage_audit_id = 4;
        static int project_id = 1;

        [Fact]
        public async Task TestCreateCase()
        {
            // Given
            string stringRoleName = "Unittest" + new Random().Next();
            string apiEndPt = $"qc/defect?case_id=0&stage_audit_id={stage_audit_id}";

            var strContent = new
            {
                remarks = "Wall broken"+new Random().Next()
            };
            var reqstring = JsonConvert.SerializeObject(strContent);
            // When
            await PostJSONDataAndGetResponse(apiEndPt, reqstring);

            // Then
          
            AssertGetReqConnection(response);
            

        }

        [Fact]

        public async Task TestUpdateDefect()
        {

            // Given
            string stringRoleName = "Unittest" + new Random().Next();
            string apiEndPt = $"qc/defect?defect_id={defect_id}";

            var strContent = new
            {
                remarks = "Wall broken" + new Random().Next(),
                closed = true
            };
            var reqstring = JsonConvert.SerializeObject(strContent);
            // When
            await PutJSONData(apiEndPt, reqstring);

            // Then

            AssertGetReqConnection(response);
        }

        [Fact]
        public async Task TestGetStageQCPhotos()
        {
            // Given
           
            string apiEndPt = $"qc/photo?defect_id={defect_id}";

            // When
            await GetResponseJSONData(apiEndPt, true);

            // Then
            //AssertGetReqConnection(response);
           Assert.NotEmpty(responseJSONArray);
        }
        [Fact]
        public async Task TestGetQCCases()
        {
            // Given

            //string apiEndPt = $"qc/case?stage_audit_id={stage_audit_id}";
            string apiEndPt = $"qc/case?project_id={project_id}";

            // When
            await GetResponseJSONData(apiEndPt,true);

            // Then
            AssertGetReqConnection(response);
           // Assert.NotEmpty(responseJSONArray);
        }
        [Fact]
        public async Task TestGetDefects()
        {
            // Given

            string apiEndPt = $"qc/defect?case_id={8}";

            // When
            await GetResponseJSONData(apiEndPt, true);

            // Then
            //AssertGetReqConnection(response);
            Assert.NotEmpty(responseJSONArray);
        }

        [Fact]
        public async Task TestPostStageQCPhotos()
        {
            var imageBytes = await System.IO.File.ReadAllBytesAsync("TestData/ddd-001.jpg");
            var imageBase64 = Convert.ToBase64String(imageBytes);
            var content = new
            {
                imageContent = imageBase64,
                remarks = "Hole on the wall",
                closed = false
            };
            string apiEndPt = $"qc/photo?defect_id={3}";
            var reqstring = JsonConvert.SerializeObject(content);
            await PostJSONDataAndGetResponse(apiEndPt, reqstring);
            // Then
            AssertGetReqConnection(response);
        }
    }
}
