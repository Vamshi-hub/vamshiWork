using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class JobStats
    {
        public double RequestedJobsCount { get; set; }
        public int DelayedJobsCount { get; set; }
        public int StartedJobsCount { get; set; }
        public int CompletedJobsCount { get; set; }
        public int QCfailedJobsCount { get; set; }
        public int ScheduledJobsCount { get; set; }
        public double DelayedJobsProgress { get; set; }
        public double StartedJobsProgress { get; set; }
        public double compltedJobsProgress { get; set; }
        public double QcFailedJobsProgress { get; set; }
        public int ScheduledCount { get; set; }
        public int OngoingCount { get; set; }
        public int CompletedCount { get; set; }
        public int TotalQCCount { get; set; }
        public string ProjectManager { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? ProjectStartDate { get; set; }
        public DateTimeOffset? ProjectEndDate { get; set; }
    }
}
