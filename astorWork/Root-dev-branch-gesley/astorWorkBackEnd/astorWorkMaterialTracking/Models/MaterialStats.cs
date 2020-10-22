using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class MaterialStats
    {
        public int InstalledMaterialsCount { get; set; }
        public int TotalMaterialsCount { get; set; }
        public double InstalledMaterialsProgress { get; set; }
        public int DeliveredMaterialsCount { get; set; }
        public int RequestedMaterialsCount { get; set; }
        public double DeliveredMaterialsProgress { get; set; }
        public int CompletedMRFCount { get; set; }
        public int TotalMRFCount { get; set; }
        public double CompletedMRFProgress { get; set; }
        public int QCFailedCount { get; set; }
        public int QCTotalCount { get; set; }
        public double QCFailedProgress { get; set; }
        public int ReadytoProject { get; set; }
        public double ReadyToDeliveredMaterialsProgress { get; set; }
        public int ReadyToDeliveredMaterialsCount { get; set; }
        public DateTimeOffset? ProjectStartDate { get; set; }
        public DateTimeOffset? ProjectEndDate { get; set; }
        public string ProjectManager { get; set; }
    }
}
