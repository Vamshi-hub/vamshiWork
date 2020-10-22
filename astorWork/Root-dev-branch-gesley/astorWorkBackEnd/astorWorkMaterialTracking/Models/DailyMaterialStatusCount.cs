using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class DailyMaterialStatusCount
    {
        public int StartDeliveryCount { get; set; }
        public int DeliveredCount { get; set; }
        public int InstalledCount { get; set; }
        public int ProducedCount { get; set; }
    }
}
