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
    public class LocationMastersControllerUnitTest : CommonFunctions
    {
        private string baseAPIEndPt = "locations/";

        [Fact]
        public async Task TestListLocations_ShouldReturnCount9()
        {
            // When
            await GetResponseJSONData(baseAPIEndPt, true);

            // Then
            Assert.Equal(9, responseJSONArray.Count);
        }

        [Fact]
        public async Task TestGetLocationDetails_ShouldReturnInstallationArea_GivenID2()
        {
            // Given
            int ID = 2;

            // When
            await GetResponseJSONData(baseAPIEndPt + ID, false);

            // Then
            Assert.Equal("Installation Area", responseJSONObj["name"]);
        }

        [Fact]
        public async Task TestEditLocation_ShouldReturnVendorID2_GivenVendorIDChangedTo2()
        {
            // Given
            int locationID = 4;
            int vendorID = 2;
            var content = new
            {
                name = "VendorTwo's Factory",
                description = "Test Edit Location",
                type = 0,
                vendorId = vendorID
            };

            // When
            await PutJSONData(baseAPIEndPt + locationID, JsonConvert.SerializeObject(content));
            await GetResponseJSONData(baseAPIEndPt + locationID, false);

            // Then
            Assert.Equal(vendorID, responseJSONObj["vendorId"]);

            content = new
            {
                name = "VendorTwo's Factory",
                description = "",
                type = 0,
                vendorId = 1
            };
            
            await PutJSONData(baseAPIEndPt + locationID, JsonConvert.SerializeObject(content));
        }

        [Fact]
        public async Task TestCreateLocation_ShouldReturnCount1_GivenNewLocationCreated()
        {
            // Given
            string locationName = "Test Location";

            var content = new
            {
                name = "Test Location",
                description = "Test Location Description",
                type = 1,
                vendorId = 2
            };

            // When
            await PostJSONDataAndGetResponse(baseAPIEndPt, JsonConvert.SerializeObject(content));
            string ID = responseJSONObj["id"].ToString();
            await GetResponseJSONData(baseAPIEndPt + ID, false);

            // Then
            Assert.Equal(locationName, responseJSONObj["name"]);
            await DeleteFromDb("locations/" + ID);
        }

        [Fact]
        public async Task TestCreateLocation_ShouldReturnStatus405_GivenExistingLocationCreated()
        {
            // Given
            var content = new
            {
                name = "Installation Area",
                description = "Test Post New",
                type = 1,
                vendorId = 1
            };

            // When
            await PostJSONDataAndGetResponse(baseAPIEndPt, JsonConvert.SerializeObject(content), "status");

            // Then
            Assert.Equal("405", responseJSONObj["status"]);
        }
    }
}
