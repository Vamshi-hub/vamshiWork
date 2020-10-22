using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace astorWorkDAO
{
    public class BIMSyncAudit
    {
        public int ID { get; set; }

        [Required]
        public ProjectMaster Project { get; set; }

        [Required]
        public string BIMModelId { get; set; }

        public string BIMVideoUrl { get; set; }
        public string SyncedMaterialIds { get; set; }
        public string UnsyncedMaterialIds { get; set; }

        [Required]
        public UserMaster SyncedBy { get; set; }

        public DateTimeOffset SyncTime { get; set; }

    }
}
