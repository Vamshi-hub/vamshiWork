using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class JobScheduleCounts
    {
        public int UnassignedJobs { get; set; }
        public int OngoingJobs { get; set; }
        public int DelayedJobs { get; set; }
        public int QCPendingJobs { get; set; }
        public int JobsinQC { get; set; }
        public int QCCompletedJobs { get; set; }
    }
}
