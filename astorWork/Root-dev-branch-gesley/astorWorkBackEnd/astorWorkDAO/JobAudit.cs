using System;
using System.Collections.Generic;
using System.Text;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
    public class JobAudit
    {
        public int ID { get; set; }
        public JobSchedule JobSchedule { get; set; }
        public JobStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }
        public UserMaster CreatedBy { get; set; }
    }
}
