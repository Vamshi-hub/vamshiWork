using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace astorWorkDAO
{
    public class UserMaster
    {
        public int ID { get; set; }

        [Required]
        public string UserName { get; set; }
        public string Password { get; set; }
        // Should be a random string
        public string Salt { get; set; }

        [Required]
        public string PersonName { get; set; }
        public string Email { get; set; }

        public int RoleID { get; set; }
        public RoleMaster Role { get; set; }
        public VendorMaster Vendor { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public bool IsActive { get; set; }

        public List<UserMRFAssociation> UserMRFAssociations { get; set; }
        public List<UserSessionAudit> UserSessionAudits { get; set; }
        public List<BIMSyncAudit> UserBIMAudits { get; set; }
        public List<UserNotificationAssociation> UserNotificationAssociation { get; set; }

        [NotMapped]
        public IEnumerable<NotificationAudit> Notifications
        {
            get
            {
                if (UserNotificationAssociation != null)
                    return UserNotificationAssociation.Select(una => una.Notification);
                else
                    return null;
            }
        }

        public int? SiteID { get; set; }
        public SiteMaster Site { get; set; }

        public int? ProjectID { get; set; }
        public ProjectMaster Project { get; set; }

    }
}
