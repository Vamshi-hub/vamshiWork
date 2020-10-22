using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class MaterialInProgress
    {
        public string Block { get; set; }
        public string Level { get; set; }
        public int InstalledMaterialCount { get; set; }
        public int TotalMaterialCount { get; set; }
        public double Progress { get; set; }
    }
}
