using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class MRF
    {
        public string MrfNo { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public DateTimeOffset PlannedCastingDate { get; set; }
        public double Progress { get; set; }
    }
}
