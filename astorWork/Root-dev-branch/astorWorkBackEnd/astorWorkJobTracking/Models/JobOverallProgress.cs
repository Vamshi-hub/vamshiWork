using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class JobOverallProgress
    {
        public DateTimeOffset? Date { get; set; }
        public double Progress { get; set; }
    }
}
