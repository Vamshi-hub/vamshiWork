using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class BIMUpdate
    {
        public int MaterialID { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string MarkingNo { get; set; }
        public string StageName { get; set; }
        public DateTimeOffset UpdateTime { get; set; }
    }
}
