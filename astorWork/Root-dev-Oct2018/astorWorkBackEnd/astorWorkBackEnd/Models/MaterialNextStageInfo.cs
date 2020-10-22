using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class MaterialNextStageInfo
    {
        public int MaterialID { get; set; }
        public string NextStageName { get; set; }
        public string NextStageColour { get; set; }
        public bool NextStageQC { get; set; }
    }
}
