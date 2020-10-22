using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class TrackerAssociation
    {
        public int ID { get; set; }
        public string TrackerLabel { get; set; }
        public MaterialDetail Material { get; set; }
        public InventoryAudit Inventory { get; set; }
    }
}
