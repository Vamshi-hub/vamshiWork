using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace astorWorkDAO
{
    public class LocationMaster
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
/*
0 - Vendor location
1 - Storage location
2 - Installation location
 */
        public int Type { get; set; }

        public int? VendorID { get; set; }
        public VendorMaster Vendor { get; set; }

        public int? SiteID { get; set; }
        public SiteMaster Site { get; set; }
    }
}
