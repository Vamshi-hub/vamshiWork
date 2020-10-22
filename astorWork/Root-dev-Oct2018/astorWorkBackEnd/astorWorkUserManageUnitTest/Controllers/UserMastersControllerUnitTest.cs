using astorWorkUserManage.Models;
using astorWorkUserManageUnitTest.Common;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkUserManageUnitTest.Controllers
{
    public class UserMastersControllerUnitTest: CommonFunctions
    {
        string baseAPIEndPt = "api/user";

        public UserMastersControllerUnitTest() {
            test = "userManage";
        }

        [Fact]
        public async Task TestCreateUser_ShouldReturnStatus0_GivenNewUserProfileCreated()
        {
            // Given
            string strContent = "{" +
                                  "\"userName\": \"adminTest\"," +
                                  "\"personName\": \"Benjamin\"," +
                                  "\"email\": \"benjamin.chua@astoriasolutions.com\"," +
                                  "\"role\": 1," +
                                  "\"vendor\": 1," +
                                "}";
            string fieldToCheck = "status";

            // When
            await PostJSONDataAndGetResponse(baseAPIEndPt, strContent, fieldToCheck);

            // Then
            AssertGetReqConnection(response);
            int result = int.Parse(responseJSONObj[fieldToCheck].ToString());
            await DeleteFromDb(baseAPIEndPt + "/24");
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task TestEditUser_ShouldReturnCount7_GivenUserProfileDeactivated()
        {
            // Given
            string strContent = "{" +
                                  "\"personName\": \"Benjamin\"," +
                                  "\"email\": \"benjamin.chua@astoriasolutions.com\"," +
                                  "\"role\": 1," +
                                  "\"vendor\": 1," +
                                  "\"isActive\": 0," +
                                "}";
            int userID = 23;

            // When
            await PutJSONData(baseAPIEndPt + "/" + userID.ToString(), strContent);

            // Then
            await GetResponseJSONData(baseAPIEndPt, true);
            int result = responseJSONArray.Count;

            // Restore back to original state
            strContent = "{" +
                            "\"personName\": \"Benjamin\"," +
                            "\"email\": \"benjamin.chua@astoriasolutions.com\"," +
                            "\"role\": 1," +
                            "\"vendor\": 1," +
                            "\"isActive\": 1," +
                         "}";

            await PutJSONData(baseAPIEndPt + "/" + userID.ToString(), strContent);

            Assert.Equal(7, result);
        }

        [Fact]
        public async Task TestGetUser_ShouldReturnEmailBenjamin_GivenUserID23()
        {
            // Given
            int UserID = 23;

            // When
            await GetResponseJSONData(baseAPIEndPt + "/" + UserID.ToString(), false);
            
            // Then
            string result = responseJSONObj["email"].ToString();
            Assert.Equal("benjamin.chua@astoriasolutions.com", result);
        }

        [Fact]
        public async Task TestGetUserList_ShouldReturnCount8()
        {
            // When
            await GetResponseJSONData(baseAPIEndPt + "/?isVandor=null", true);

            // Then
            int result = responseJSONArray.Count;
            Assert.Equal(8, result);
        }

        [Fact]
        public async Task TestChangePassword_GivenAdminUser()
        {
            var request = new ChangePasswordRequest
            {
                CurrentPassword = "abc12345",
                NewPassword = "def12345"
            };

            var requestStr = JsonConvert.SerializeObject(request);
            await PutJSONData(baseAPIEndPt + "/1/changePassword", requestStr);

            string fieldToCheck = "status";
            // Then
            AssertGetReqConnection(response);
            int result = int.Parse(responseJSONObj[fieldToCheck].ToString());
            Assert.Equal(0, result);

            await PutJSONData(baseAPIEndPt + "/1/changePassword", requestStr);
            AssertGetReqConnection(response);
            result = int.Parse(responseJSONObj[fieldToCheck].ToString());
            Assert.NotEqual(0, result);

            
            request = new ChangePasswordRequest
            {
                CurrentPassword = "def12345",
                NewPassword = "abc12345"
            };
            requestStr = JsonConvert.SerializeObject(request);
            await PutJSONData(baseAPIEndPt + "/1/changePassword", requestStr);
            AssertGetReqConnection(response);
            result = int.Parse(responseJSONObj[fieldToCheck].ToString());
            Assert.Equal(0, result);            
        }

    }
}
