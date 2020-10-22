using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class JobStats
    {
        public int DelayedJobsCount { get; set; }
        public int StartedJobsCount { get; set; }
        public int CompletedJobsCount { get; set; }
        public int ScheduledJobsCount { get; set; }
        public int ScheduledCount { get; set; }
        public int OngoingCount { get; set; }
        public int CompletedCount { get; set; }
     
      
    }
}
