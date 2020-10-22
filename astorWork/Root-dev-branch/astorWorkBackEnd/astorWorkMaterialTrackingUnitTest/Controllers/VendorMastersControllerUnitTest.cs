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
    public class VendorMastersControllerUnitTest : CommonFunctions
    {
        [Fact]
        public async Task TestGetVendorList_ShouldReturnNonEmpty()
        {
            // When
            await GetResponseJSONData("vendors", true);

            // Then
            Assert.NotEmpty(responseJSONArray);
        }

        [Fact]
        public async Task TestGetVendorDetails_ShouldReturnWithCycleDays()
        {
            await GetResponseJSONData("vendors", true);

            var vendor = responseJSONArray.First;

            await GetResponseJSONData("vendors/" + vendor["id"], false);

            Assert.NotNull(responseJSONObj);
            Assert.NotNull(responseJSONObj["cycleDays"]);
        }

        [Fact]
        public async Task TestEditVendorDetails_ShouldStatus0()
        {
            await GetResponseJSONData("vendors", true);

            var vendor = responseJSONArray.First;

            var name = vendor["name"];
            var cycleDays = vendor["cycleDays"];

            var editData = new { name, cycleDays };

            await PutJSONData("vendors/" + vendor["id"], JsonConvert.SerializeObject(editData), true);

            Assert.NotNull(responseJSONObj["status"]);
            Assert.True(responseJSONObj.Value<int>("status") == 0);
        }

        [Fact]
        public async Task TestCreateVendor_ShouldReturnNonNullID()
        {
            var name = "Test Vendor";
            var cycleDays = 12;

            var createData = new { name, cycleDays };

            await PostJSONDataAndGetResponse("vendors", JsonConvert.SerializeObject(createData));

            Assert.NotNull(responseJSONObj);
            Assert.NotNull(responseJSONObj["id"]);

            await DeleteFromDb("vendors/" + responseJSONObj["id"]);
        }
    }
}
