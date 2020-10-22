using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class RTOInspectionForm
    {
        public int JobScheduleID { get; set; }
        public string TradeOrActivity { get; set; }
        public string ReferenceNo { get; set; }
        public string Discipline { get; set; }
        public string ModuleName { get; set; }
        public string DrawingNo { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public List<Signatures> Signatures { get; set; }
        public List<string> ChecklistItemName { get; set; }

    }
    public class Signatures
    {
        public string URL { get; set; }
        public string ImageBase64 { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
    }

}
