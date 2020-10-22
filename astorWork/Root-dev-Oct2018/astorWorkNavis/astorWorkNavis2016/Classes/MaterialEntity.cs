using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkNavis2016.Classes
{
    public class MaterialEntity
    {
        public int MaterialId { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        
        public string MarkingNo { get; set; }
        public string CurrentStatus { get; set; }
        public string StageName { get; set; }

        public DateTimeOffset UpdateTime { get; set; }
    }
}
