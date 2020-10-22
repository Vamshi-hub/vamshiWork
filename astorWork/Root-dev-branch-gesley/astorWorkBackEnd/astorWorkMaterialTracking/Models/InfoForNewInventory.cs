using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class InfoForNewInventory
    {
        public int maxSN { get; set; }
        public List<string> markingNos { get; set; }
    }
}
