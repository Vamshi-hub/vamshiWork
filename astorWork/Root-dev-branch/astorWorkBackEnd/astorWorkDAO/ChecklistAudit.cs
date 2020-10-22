using System;
using System.Collections.Generic;
using System.Text;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
    public class ChecklistAudit
    {
        public int ID { get; set; }

        public int? JobScheduleID { get; set; }
        public JobSchedule JobSchedule { get; set; }
        public int? MaterialStageAuditID { get; set; }
        public MaterialStageAudit MaterialStageAudit { get; set; }
        public int ChecklistID { get; set; }
        public ChecklistMaster Checklist { get; set; }
        
        public QCStatus Status { get; set; }
        public UserMaster RouteTo { get; set; }

        public string SignatureURL { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public UserMaster CreatedBy { get; set; }

        ///// <summary>
        ///// documentID is a link of cosmos DB document
        ///// to access the chat data based on documentID
        ///// </summary>
        //public string DocumentID { get; set; }

        public List<ChatData> Chats { get; set; }
    }
}
