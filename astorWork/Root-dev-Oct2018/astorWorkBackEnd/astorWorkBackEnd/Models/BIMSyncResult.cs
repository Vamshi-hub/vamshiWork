using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class BIMSyncResult
    {
        public string ModelId { get; set; }
        public int[] SyncedMaterialIds { get; set; }
        public int[] UnsyncedMaterialIds { get; set; }
        public DateTime SyncTime { get; set; }

    }
}
