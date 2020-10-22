using System.Collections.Generic;

namespace astorWorkDAO
{
    public class SiteMaster
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public int TimeZoneOffset { get; set; }

        public OrganisationMaster Organisation { get; set; }
        public List<LocationMaster> Locations { get; set; }
        public string Description { get; set; }
    }
}
