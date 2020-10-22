using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string Country { get; set; }
        public int TimeZoneOffset { get; set; }
        public int ProjectManagerID { get; set; }
        public string ProjectManagerName { get; set; }
        public List<string> MaterialTypes { get; set; }
        public List<string> Blocks { get; set; }
        public List<string> MRFs { get; set; }
        public string TimeZone { get; set; }
    }
}
