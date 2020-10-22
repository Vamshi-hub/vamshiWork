using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class JobQCDetails
    {
        public string CheckListName { get; set; }
        public int TotalQCCount { get; set; }
        public string subConName { get; set; }
        public string QcFailedBy { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public int countPhotos { get; set; }
        public string status { get; set; }
        public DateTimeOffset? PlannedStartDate { get; set; }
        public DateTimeOffset? plannedEndDate { get; set; }
        public int TradeID { get; set; }
        public string TradeName { get; set; }
        public string MarkingNo { get; set; }
        public int ProjectID { get; set; }
        public DateTimeOffset? QCStartDate { get; set; }
        public DateTimeOffset? qcEndDate { get; set; }
        public int ID { get; set; }
        public int ChecklistID { get; set; }
        public int ChecklistAuditID { get; set; }
        public string ImageContent { get; set; }

        public string Remarks { get; set; }
        public string URL { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string type { get; set; }
        public int StageID { get; set; }
        public string StageName { get; set; }
        public string ChecklistStatus { get; set; }
    }
}
