using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class BIMSyncResult
    {
        public int ID { get; set; }
        public string ModelID { get; set; }
        public int[] SyncedMaterialIDs { get; set; }

        public string PersonName { get; set; }
        public int CountSyncedMaterials { get; set; }
        public int CountUnsyncedMaterials { get; set; }
        public string VideoURL { get; set; }

        public List<MaterialMaster> SyncedMaterials { get; set; }
        public List<MaterialMaster> UnsyncedMaterials { get; set; }
        public int[] UnsyncedMaterialIDs { get; set; }
        public DateTimeOffset SyncTime { get; set; }
    }
}
