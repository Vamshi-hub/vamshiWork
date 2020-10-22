using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace astorWorkDAO
{
    // For those requests that need post-processing and keep track
    public class UserRequestAudit
    {
        public int ID { get; set; }

        [Required]
        public UserMaster User { get; set; }

        /*
         * 0 - Reset password request
         * 1 - Create MRF request
         * 2 - Upload drawing request
         */
        [Required]
        public int RequestType { get; set; }

        [Required]
        public DateTimeOffset RequestTime { get; set; }

        public bool RequestSuccess { get; set; }
    }
}
