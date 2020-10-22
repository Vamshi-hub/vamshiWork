using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class TrackerAssociation
    {
        public List<Tracker> Trackers { get; set; }
        public MaterialMobile Material { get; set; }
    }

    public class Tracker
    {
        public int ID { get; set; }
        public string Tag { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
    }
}
