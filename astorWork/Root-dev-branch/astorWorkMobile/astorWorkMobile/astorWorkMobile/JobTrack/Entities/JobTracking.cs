using astorWorkMobile.MaterialTrack.Entities;
using System;
using System.Collections.Generic;

namespace astorWorkMobile.JobTrack.Entities
{
    /*
     * List of jobs
     */
    public class PPVCJob
    {
        public Material PPVC { get; set; }
        public string JobName { get; set; }
        public DateTime Start { get; set; }
        public string ModuleName
        {
            get
            {
                return $"{PPVC.block}-L{PPVC.level}-{PPVC.zone}-{PPVC.markingNo}";
            }
        }

        /*
         * 0 - Pending
         * 1 - Started
         * 2 - Completed
         * 3 - QC open by current subcon
         * 4 - Previous job QC open
         */ 
        public int JobStatus { get; set; }
    }

    public class PPVCQC
    {
        public PPVCJob Job { get; set; }
        public DateTime TargetRectificationDate { get; set; }
    }

}
