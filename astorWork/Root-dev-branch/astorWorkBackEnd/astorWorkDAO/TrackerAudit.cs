using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    class TrackerAudit
    {
        public int ID { get; set; }
        public TrackerMaster Tracker { get; set; }
        public bool Processed { get; set; }
    }
}
