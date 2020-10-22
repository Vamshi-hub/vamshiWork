using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class Site
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int OrganisationID { get; set; }
        public string OrganisationName { get; set; }
        public string Country { get; set; }
        public string Description { get; set; }
        public int TimeZoneOffset { get; set; }
        public int TimeZone { get; set; }
    }
}
