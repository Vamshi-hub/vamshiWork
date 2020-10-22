using astorWorkBackEndUnitTest.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class RoleMastersControllerUnitTest: CommonFunctions
    {
        
        [Fact]
        public async Task TestListRoles_ShouldHaveMoreThanOne()
        {
            // When
            await GetResponseJSONData("role", true);

            // Then
            AssertGetReqConnection(response);
            Assert.NotEmpty(responseJSONArray);
        }
        [Fact]
        public async Task TestGetRole_ShouldHaveMoreThanOne()
        {
            // Given
             int roleID = 6;
            // When
            await GetResponseJSONData("role/" + roleID, true);
            AssertGetReqConnection(response);
           
            Assert.NotEmpty(responseJSONArray);
        }
        [Fact]
        public async Task TestGetPages_ShouldHaveMoreThanOne()
        {
            
            // When
            await GetResponseJSONData("role/" + "pages", true);
            AssertGetReqConnection(response);

            Assert.NotEmpty(responseJSONArray);
        }
        [Fact]
        public async Task TestGetDefaultPages_ShouldHaveMoreThanOne()
        {

            // When
            await GetResponseJSONData("role/" + "defaultPages", true);
            AssertGetReqConnection(response);

            Assert.NotEmpty(responseJSONArray);
        }

        [Fact]
        public async Task TestCreateRole()
        {
            // Given
            string stringRoleName = "Unittest" + new Random().Next();
            string apiEndPt = "role";
        
            var strContent = new { RoleID = 0, RoleName = stringRoleName, DefaultPageID = 21, listofpages = new[] { new { ModuleID = 4, ModuleName = "Material-Tracking", pageId = 16, PageName = "Material", accessLevel = "3" } } };
            var reqstring = JsonConvert.SerializeObject(strContent);
            // When
            await PostJSONDataAndGetResponse(apiEndPt, reqstring);

            // Then
            AssertGetReqConnection(response);
         
        }
        [Fact]
        public async Task TestUpdateRole()
        {
            // Given
            string stringRoleName = "administrator";
            string apiEndPt = "role/11";
            var strContent = new { RoleID = 0, RoleName = stringRoleName, DefaultPageID = 21, listofpages = new[] { new { ModuleID = 4, ModuleName = "Material-Tracking", pageId = 17, PageName = "Material", accessLevel = "0" } } };
            var reqstring = JsonConvert.SerializeObject(strContent);
            // When
            await PutJSONData(apiEndPt, strContent.ToString());
            AssertGetReqConnection(response);
            // When
            
        }
    }
}
