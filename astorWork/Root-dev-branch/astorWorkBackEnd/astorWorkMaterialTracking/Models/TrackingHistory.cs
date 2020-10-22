using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class TrackingHistory
    {
        public int ID { get; set; }
        public string StageName { get; set; }
        public int StageStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string Location { get; set; }
        public string Remarks { get; set; }
        public bool IsQCStage { get; set; }
        public string OpenQCCaseIds { get; set; }
        public int CountQCCase { get; set; }
        public int CountOpenDefect { get; set; }
        public int CountClosedDefect { get; set; }
        public int CountRectifiedDefect { get; set; }
        public int CountQCDefects { get; set; }
        public int TotalStructChecklistCount { get; set; }
        public int TotalStructPassCount { get; set; }
        public int TotalArchiChecklistCount { get; set; }
        public int TotalArchiPassCount { get; set; }
        public string StructQCLastUpdatedBy { get; set; }
        public DateTimeOffset? StructQCLastUpdatedDate { get; set; }
        public string ArchiQCLastUpdatedBy { get; set; }
        public DateTimeOffset? ArchiQCLastUpdatedDate { get; set; }
    }
}
