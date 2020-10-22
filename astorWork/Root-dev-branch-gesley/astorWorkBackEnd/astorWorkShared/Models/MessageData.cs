using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkShared.Models
{
     public class MessageData
    {
        public string TenantName { get; set; }
        public string Header { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        //public QCPhoto Image { get; set; }
        public string ThumbnailUrl { get; set; }
        public string OriginalAttachmentUrl { get; set; }
        public string ThumbnailImagebase64 { get; set; }
        public string OriginalImagebase64 { get; set; }
        public bool HasImage { get; set; }
        public bool IsSystem { get; set; }
        public int? MaterialID { get; set; }
        public string MarkingNo { get; set; }
        public string ModuleName { get; set; }
        public string JobName { get; set; }
        public int? JobID { get; set; }
        public string ChecklistName { get; set; }
        public int? ChecklistID { get; set; }
        public int? ChecklistItemID { get; set; }
        public string ChecklistItemName { get; set; }

        public List<string> SeenUsers { get; set; }
        public string UserColor { get; set; }
    }
}
