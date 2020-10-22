using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class JobstatusList
    {
        public List<JobScheduleDetails> startedJobs { get; set; }
        public List<JobScheduleDetails> delayedJobs { get; set; }
        public List<JobScheduleDetails> compltedJobs { get; set; }
        public List<JobScheduleDetails> qcfailedJobs { get; set; }
    }
}
