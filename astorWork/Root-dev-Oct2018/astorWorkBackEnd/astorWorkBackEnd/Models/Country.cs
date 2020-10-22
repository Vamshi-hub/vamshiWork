using Newtonsoft.Json;
using System.Collections.Generic;

namespace astorWorkBackEnd.Models
{
    public class Country
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Offset { get; set; }

        public static List<Country> GetCountryList()
        {
            var jsonString = System.IO.File.ReadAllText("countries.json");
            var json = JsonConvert.DeserializeObject<List<Country>>(jsonString);

            return json;
        }
    }

}
