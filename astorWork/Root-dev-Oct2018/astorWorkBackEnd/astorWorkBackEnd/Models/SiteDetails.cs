using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class SiteDetails
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public int VendorID { get; set; }
        public string Vendorname { get; set; }
        public int TimeZone { get; set; }
    }
}
