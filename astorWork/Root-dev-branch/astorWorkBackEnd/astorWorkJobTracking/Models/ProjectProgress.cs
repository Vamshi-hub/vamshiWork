using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class ProjectProgress
    {
        public IEnumerable<JobOverallProgress> OverallProgress { get; set; }
    }
}
