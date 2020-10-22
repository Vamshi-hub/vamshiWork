using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class InventoryAudit
    {
        public int ID { get; set; }
        public string MarkingNo { get; set; }
        public int SN { get; set; }
        public DateTimeOffset? CastingDate { get; set; }

        public int? TrackerID { get; set; }
        public TrackerMaster Tracker { get; set; }

        public int? ProjectID { get; set; }
        public ProjectMaster Project { get; set; }

        public int? OrganisationID { get; set; }
        public OrganisationMaster Organisation { get; set; }

        public int? CreatedByID { get; set; }
        public UserMaster CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
