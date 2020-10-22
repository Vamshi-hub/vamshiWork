using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace astorWorkDAO
{
    public class NotificationAudit
    {
        public int ID { get; set; }

        /* Type: 
         * 0 - Email
         * 1 - SMS
         * 2 - Mobile Push Notification
         * 3 - Web Notification
         */ 
        public int Type { get; set; }

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

        /* Reference:
         * Create MRF - MRF ID
         * BIM Sync - BIM session ID
         * ...
         */
        public string Reference { get; set; }

        public List<UserNotificationAssociation> UserNotificationAssociation { get; set; }

        [NotMapped]
        public IEnumerable<UserMaster> Recipients {
            get
            {
                if (UserNotificationAssociation != null)
                    return UserNotificationAssociation.Select(una => una.Receipient);
                else
                    return null;
            }
        }

        public DateTimeOffset CreatedDate { get; set; }

        /* ProcessedDate:
         * null - not processed or unsuccessful
         */
        public DateTimeOffset? ProcessedDate { get; set; }

        public NotificationTimerMaster NotificationTimer { get; set; }
    }
}
