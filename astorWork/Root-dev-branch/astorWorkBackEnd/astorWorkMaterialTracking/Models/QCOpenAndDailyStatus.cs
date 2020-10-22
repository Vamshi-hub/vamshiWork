using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class QCOpenAndDailyStatus
    {
        public List<MaterialDetail> DeliveredMaterials { get; set; }
        public List<MaterialDetail> Readytoproject { get; set; }
        public List<MaterialDetail> InstalledMaterials { get; set; }
        public List<QCOpenMaterial> QcOpenMaterials { get; set; }
        public List<MRF> CompletedMRFs { get; set; }
        public DailyMaterialStatusCount DailyMaterialStatusCount { get; set; }
        public List<MaterialDetail> ReadyToDeliveredMaterials { get; set; }
    }
}
