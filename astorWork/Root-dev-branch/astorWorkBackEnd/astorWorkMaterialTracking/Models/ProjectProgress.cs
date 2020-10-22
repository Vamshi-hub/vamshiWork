using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class ProjectProgress
    {
        public IEnumerable<MaterialOverallProgress> OverallProgress { get; set; }
        public List<MaterialInProgress> InProgress { get; set; }
        public string BimVideoUrl { get; set; }
    }
}
