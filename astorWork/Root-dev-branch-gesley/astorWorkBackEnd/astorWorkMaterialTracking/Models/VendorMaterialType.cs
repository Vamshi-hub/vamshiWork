using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class VendorMaterialType
    {
        public int OrganisationId { get; set; }
        public List<string> MaterialTypes { get; set; }
    }
}
