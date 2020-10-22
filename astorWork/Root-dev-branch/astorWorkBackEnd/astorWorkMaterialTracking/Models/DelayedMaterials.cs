using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class DelayedMaterials
    {
        public List<DelayedMaterial> DelayedProductionMaterials { get; set; }
        public List<DelayedMaterial> DelayedDeliveryMaterials { get; set; }
        public List<DelayedMaterial> DelayedInstallationMaterials { get; set; }
    }
}
