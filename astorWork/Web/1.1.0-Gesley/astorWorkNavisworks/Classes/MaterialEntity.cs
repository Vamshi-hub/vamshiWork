using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWork_Navisworks.Classes
{
    public class MaterialEntity
    {
        public int ID { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        
        public string MarkingNo { get; set; }
        public string CurrentStatus { get; set; }
        public string Status { get; set; }

        public DateTime UpdatedDate { get; set; }
    }
}
