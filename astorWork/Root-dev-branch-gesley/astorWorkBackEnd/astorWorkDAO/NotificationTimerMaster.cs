using System.ComponentModel.DataAnnotations;

namespace astorWorkDAO
{
    public class NotificationTimerMaster
    {
        public int ID { get; set; }

        /* Code:
         * 0 - Create MRF
         * 1 - BIM Sync
         * 2 - Delay in delivery
         * 3 - QC failed
         * 4 - QC rectified
         * 5 - Close MRF
         * 6 - Reset password
         * 7 - Create User
         */
        [Required]
        public int Code { get; set; }

        /*
         * Time in minutes
         * 8AM - 480
         * 8:30AM - 510
         * 8PM - 1200
         */
        [Required]
        public int TriggerTime { get; set; }

        public bool Enabled { get; set; }
        public bool UpdateRequired { get; set; }

        public SiteMaster Site { get; set; }
        public ProjectMaster Project { get; set; }
    }
}
