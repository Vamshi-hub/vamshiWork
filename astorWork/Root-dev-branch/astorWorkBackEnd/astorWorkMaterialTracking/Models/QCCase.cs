using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class QCCase
    {
        public int ID { get; set; }
        public string CaseName { get; set; }
        public string MarkingNo { get; set; }
        public string StageName { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public bool IsOpen { get; set; }
        public int CountOpenDefects { get; set; }
        public int CountClosedDefects { get; set; }
        public double Progress { get; set; }
        public string Duration { get; set; }
        public int StageAuditId { get; set; }
    }
}
