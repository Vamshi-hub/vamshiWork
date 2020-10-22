using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class NotificationDetails
    {
        public int JobScheduleID { get; set; }
        public int MaterialID { get; set; }
        public string  Block  { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string MarkingNo { get; set; }
        public int ProjectID { get; set; }
        public Project Project { get; set; }
        public bool IsSeen { get; set; }
        public int NotificationID { get; set; }
        public string Message { get; set; }
        public string TradeName { get; set; }
        public string AssaignedTo { get; set; }
        public string MaterialType { get; set; }
        public string CreatedBy { get; set; }
        public bool IsJob { get; set; }
        public bool IsMaterial { get; set; }
        public DateTimeOffset ProcessDate { get; set; }
        public string Tag { get; set; }
        public int NotificationCode { get; set; }

    }
}
