using astorWorkUserManageUnitTest.Common;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkUserManageUnitTest
{
    public class AuthControllerUnitTest: CommonFunctions
    {
        string baseAPIEndPt = "api/authentication";

        [Fact]
        public async Task TestLogin_ShouldRetunToken_GivenValidCredentials()
        {
            // Given
            string body = "{\"username\":\"vendor1qc\"," +
                           "\"password\":\"abc12345\"}";

            // When
            await PostJSONDataAndGetResponse("api/authentication/login",body,"accessToken");
            string token = responseJSONObj["accessToken"].ToString();

            // Then
            Assert.True(token.Length > 0);
        }

        [Fact]
        public async Task TestLogin_ShouldRetunToken_GivenWrongCredentials()
        {
            // Given
            string body = "{\"username\":\"vendor1qc\"," +
                           "\"password\":\"abc\"}";

            // When
            await PostJSONDataAndGetResponse("api/authentication/login", body, "statusCode");
           
            // Then
            Assert.Null(responseJSONObj);
        }

        [Fact]
        public async Task TestForgetPassword_ShouldReturnStatus0_GivenValidUsernameAndEmail()
        {
            // Given
            string strContent = "{" +
                                "\"userId\": 23," +
                                "\"userName\": \"adminTest\"," +
                                "\"email\": \"benjamin.chua@astoriasolutions.com\"" +
                                "}";

            // When
            await PutJSONData(baseAPIEndPt + "/forgetPassword", strContent);

            // Then
            AssertValueInField(responseJSONObj, "0", "status");
        }

        [Fact]
        public async Task TestForgetPassword_ShouldReturnStatus1002_GivenValidWithin15min()
        {
            // Given
            string strContent = "{" +
                                "\"userId\": 23," +
                                "\"userName\": \"adminTest\"," +
                                "\"email\": \"benjamin.chua@astoriasolutions.com\"" +
                                "}";

            // When
            await PutJSONData(baseAPIEndPt + "/forgetPassword", strContent);
            await PutJSONData(baseAPIEndPt + "/forgetPassword", strContent);

            // Then
            AssertValueInField(responseJSONObj, "1002", "status");
        }

        [Fact]
        public async Task TestForgetPassword_ShouldReturnStatus0_GivenInvalidEmail()
        {
            // Given
            string strContent = "{" +
                                "\"userId\": 23," +
                                "\"userName\": \"adminTest\"," +
                                "\"email\": \"benjaminchua@astoriasolutions.com\"" +
                                "}";

            // When
            await PutJSONData(baseAPIEndPt + "/forgetPassword", strContent);

            // Then
            AssertValueInField(responseJSONObj, "404", "status");
        }
    }
}
