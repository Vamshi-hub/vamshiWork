using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class Notification
    {
        public int ID { get; set; }

        /* Code:
         * 0 - Create MRF
         * 1 - BIM Sync
         * 2 - Delay in delivery
         * 3 - QC failed
         * 4 - QC rectified
         * 5 - Close MRF
         * 6 - Reset password
         * 7 - Create User
         */
        public int Code { get; set; }

        public string NotificationName { get; set; }

        public string Description { get; set; }

        public string TriggerTime { get; set; }

        public bool Enabled { get; set; }
        public bool UpdateRequired { get; set; }

        public int SiteId { get; set; }
        public int ProjectId { get; set; }
    }
}
