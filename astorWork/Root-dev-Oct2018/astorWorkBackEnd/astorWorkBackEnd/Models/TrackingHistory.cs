using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
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
        public int OpenQCCaseId { get; set; }
        public int CountQCCase { get; set; }
        public int CountOpenDefect { get; set; }
        public int CountClosedDefect { get; set; }
    }
}
