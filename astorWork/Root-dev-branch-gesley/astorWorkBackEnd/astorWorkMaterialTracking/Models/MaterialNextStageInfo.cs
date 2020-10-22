using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class MaterialNextStageInfo
    {
        public int MaterialID { get; set; }
        public string NextStageName { get; set; }
        public string NextStageColour { get; set; }
    }
}
