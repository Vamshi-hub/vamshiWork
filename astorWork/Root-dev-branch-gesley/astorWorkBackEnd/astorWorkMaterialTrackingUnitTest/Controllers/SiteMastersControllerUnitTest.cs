using astorWorkBackEndUnitTest.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;

namespace astorWorkBackEndUnitTest.Controllers
{
    public class SiteMastersControllerUnitTest:CommonFunctions
    {
        private string baseAPIEndPt = "sites/";

        [Fact]
        public async Task TestListSites_ShouldReturnCount2()
        {
            // When
            await GetResponseJSONData(baseAPIEndPt, true);

            // Then
            Assert.Equal(2, responseJSONArray.Count);
        }

        [Fact]
        public async Task TestListCountries_ShouldReturnCountrynameAfghanistan()
        {
            // When
            await GetResponseJSONData(baseAPIEndPt + "countries", true);

            // Then
            Assert1stInListContainsSelectedFieldValue(response, responseJSONArray, "countryName", "Afghanistan");
        }

        [Fact]
        public async Task TestGetSite_ShouldReturnVendorOneProdSite_GivenID1()
        {
            // Given
            int ID = 1;

            // When
            await GetResponseJSONData(baseAPIEndPt + ID, false);

            // Then
            Assert.Equal("VendorOne Prod Site", responseJSONObj["name"]);
        }

        [Fact]
        public async Task TestCreateSite_ShouldReturnCount1_GivenNewSiteCreated()
        {
            // Given
            string siteName = "Test Site";

            var content = new
            {
                name = siteName,
                organisationID = 2,
                country = "AR",
                description = ""
            };

            // When
            await PostJSONDataAndGetResponse(baseAPIEndPt, JsonConvert.SerializeObject(content));
            string ID = responseJSONObj["id"].ToString();
            await GetResponseJSONData(baseAPIEndPt + ID, false);

            // Then
            Assert.Equal(siteName, responseJSONObj["name"]);
            await DeleteFromDb(baseAPIEndPt + ID);
        }

        [Fact]
        public async Task TestEditSite_ShouldReturnVendorID1_GivenVendorIDChangedTo1()
        {
            // Given
            int siteID = 1;
            int organisationID = 1;
            var content = new
            {
                name = "VendorOne Prod Site",
                organisationID = 1,
                country = "AR",
                description = "Test Edit Description"
            };

            // When
            await PutJSONData(baseAPIEndPt + siteID, JsonConvert.SerializeObject(content));
            await GetResponseJSONData(baseAPIEndPt + siteID, false);

            // Then
            Assert.Equal(organisationID, responseJSONObj["vendorId"]);

            content = new
            {
                name = "VendorOne Prod Site",
                organisationID = 2,
                country = "AR",
                description = ""
            };

            await PutJSONData(baseAPIEndPt + siteID, JsonConvert.SerializeObject(content));
        }
    }
}
