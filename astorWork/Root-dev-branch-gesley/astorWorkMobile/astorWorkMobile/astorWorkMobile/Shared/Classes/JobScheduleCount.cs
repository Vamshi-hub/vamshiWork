using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkMobile.Shared.Classes
{
    public class JobScheduleCount
    {
        public int UnassignedJobs { get; set; }
        public int OngoingJobs { get; set; }
        public int DelayedJobs { get; set; }
        public int QCPendingJobs { get; set; }
        public int JobsinQC { get; set; }
        public int QCCompletedJobs { get; set; }
    }
}
