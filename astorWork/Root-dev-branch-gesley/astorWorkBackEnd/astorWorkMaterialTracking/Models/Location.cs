using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class Location
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public int SiteID { get; set; }
        public string SiteName { get; set; }

        public string Level { get; set; }
        public List<string> Zones { get; set; }
    }
}
