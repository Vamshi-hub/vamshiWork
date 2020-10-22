using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class MaterialStage
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Colour { get; set; }
        public string MaterialTypes { get; set; }
        public int NextStageID { get; set; }
        public int MilestoneID { get; set; }
        public int Order { get; set; }
        public bool IsEditable { get; set; }
        public bool CanIgnoreQC { get; set; }
        public int MaterialCount { get; set; }
    }

}
