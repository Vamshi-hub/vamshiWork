using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class Location
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public int siteID { get; set; }
        public string siteName { get; set; }

        public string Level { get; set; }
        public List<string> Zones { get; set; }
    }
}
