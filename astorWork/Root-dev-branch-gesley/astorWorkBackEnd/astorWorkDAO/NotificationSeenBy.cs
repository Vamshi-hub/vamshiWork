using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class NotificationSeenBy
    {
        public int ID { get; set; }
        public int NotificationAuditID { get; set; }
        public NotificationAudit NotificationAudit { get; set; }
        public int UserMasterID { get; set; }
        public UserMaster UserMaster { get; set; }
        public DateTimeOffset SeenDate { get; set; }
    }
}
