using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class ChatData
    {
        public int ID { get; set; }
        public string Message { get; set; }
        public string ThumbnailUrl { get; set; }
        public string OriginalAttachmentUrl { get; set; }
        public UserMaster User { get; set; }
        public int UserID { get; set; }
        public MaterialStageAudit MaterialStageAudit { get; set; }
        public int? MaterialStageAuditID { get; set; }
        public JobSchedule JobSchedule { get; set; }
        public int? JobScheduleID { get; set; }
        public ChecklistMaster Checklist { get; set; }
        public int? ChecklistID { get; set; }
        public ChecklistItemMaster ChecklistItem { get; set; }
        public int? ChecklistItemID { get; set; }
        public bool IsSystem { get; set; }
        public bool HasAttachment { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public List<ChatUserAssociation> SeenBy { get; set; }
    }
}
