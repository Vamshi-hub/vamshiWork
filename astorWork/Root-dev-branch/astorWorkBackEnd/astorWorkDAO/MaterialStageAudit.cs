using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
    public class MaterialStageAudit
    {
        public int ID { get; set; }

        [JsonIgnore]
        public int MaterialMasterID { get; set; }
        public MaterialMaster MaterialMaster { get; set; }

        public int StageID { get; set; }
        [Required]
        public MaterialStageMaster Stage { get; set; }

        public int LocationID { get; set; }
        public LocationMaster Location { get; set; }

        public string Remarks { get; set; }

        public JobStatus QCStatus { get; set; }
        public int CreatedByID { get; set; }
        public UserMaster CreatedBy { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

    }
}
