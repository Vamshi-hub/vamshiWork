using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace astorWorkDAO
{
    public class TrackerMaster
    {
        public int ID { get; set; }

        [Required]
        public string Tag { get; set; }

        [Required]
        public string Type { get; set; }

// For QR code batch generation, label should be <Batch Number>-<SN>
        public string Label { get; set; }
        public int BatchNumber { get; set; }
    }
}
