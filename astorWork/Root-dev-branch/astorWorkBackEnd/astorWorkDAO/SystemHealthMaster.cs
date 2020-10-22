using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class SystemHealthMaster
    {
        public int ID { get; set; }

        /* Type:
         * 0 - Notification Scheduler
         * 1 - Email Service
         * 2 - Reporting Scheduler
         */
        public int Type { get; set; }

        /* Status:
         * 0 - Healthy
         * 1 - Failed 
         * 2 - Restarting
         */
        public int Status { get; set; }
        public string Message { get; set; }

        /*
         * Reference: Notification Audit's ID, Report Audit's ID, etc.
         */
        public string Reference { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
    }
}
