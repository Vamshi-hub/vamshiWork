using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class MRF
    {
        public int ID { get; set; }
        public string MrfNo { get; set; }
        public string Block { get; set; }
        public List<string> Level { get; set; }
        public List<string> Zone { get; set; }
        public List<string> MaterialTypes { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public DateTimeOffset? PlannedCastingDate { get; set; }
        public DateTimeOffset ExpectedDeliveryDate { get; set; }
        public int OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public List<int> OfficerUserIDs { get; set; }
        public Double Progress { get; set; }
        public int MaterialCount { get; set; }
        public int CreatedByUserID { get; set; }
    }
}
