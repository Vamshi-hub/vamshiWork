using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking
{
    public class Materialstagedetails
    {
       
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public int ProjectID { get; set; }
        public int ID { get; set; }
        public int StageID { get; set; }
        public string StageName { get; set; }
        public string type { get; set; }
        public string MarkingNo { get; set; }
        public string Module { get; set; }
        public string mrfNo { get; set; }
        public int RequestedmaterialCount { get; set; }
        public int TotalmaterialCount { get; set; }
        public int StageOrder { get; set; }
        public string qCStatus { get; set; }
        public DateTimeOffset createdDate { get; set; }
        public DateTimeOffset UtcDate { get; set; }
    }
}
