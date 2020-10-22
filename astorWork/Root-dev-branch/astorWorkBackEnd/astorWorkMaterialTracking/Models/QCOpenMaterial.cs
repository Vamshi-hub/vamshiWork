using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class QCOpenMaterial
    {
        public int ID { get; set; }
        public int StageID { get; set; }
        public string MarkingNo { get; set; }
        public string MaterialDescription { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public int CaseID { get; set; }
        public string CaseName { get; set; }
        public string Remarks { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
    }
}
