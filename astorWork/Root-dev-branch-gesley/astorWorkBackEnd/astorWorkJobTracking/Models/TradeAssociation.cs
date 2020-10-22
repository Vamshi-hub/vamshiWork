using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class TradeAssociation
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<string> MaterialTypes { get; set; }
        public List<string> JobStartedMaterialTypes { get; set; }
        public List<string> ChecklistItems { get; set; }
    }
}
