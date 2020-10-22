using Newtonsoft.Json;
using System.Collections.Generic;

namespace astorWorkMaterialTracking.Models
{
    public class Country
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Offset { get; set; }
        public double OffsetInMinutes { get; set; }

        public static List<Country> GetCountries()
        {
            var jsonString = System.IO.File.ReadAllText("countries.json");
            var json = JsonConvert.DeserializeObject<List<Country>>(jsonString);

            return json;
        }
    }

}
