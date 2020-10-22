using System;
using System.Collections.Generic;
using System.Text;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
    public class ChecklistItemAudit
    {
        public int ID { get; set; }

        public int ChecklistAuditID { get; set; }
        public ChecklistAudit ChecklistAudit { get; set; }

        public int ChecklistItemID { get; set; }
        public ChecklistItemMaster ChecklistItem { get; set; }
        
        public ChecklistItemStatus Status { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public UserMaster CreatedBy { get; set; }

        ///// <summary>
        ///// documentID is a link of cosmos DB document
        ///// to access the chat data based on documentID
        ///// </summary>
        //public string DocumentID { get; set; }
        public List<ChatData> Chats { get; set; }

    }
}
