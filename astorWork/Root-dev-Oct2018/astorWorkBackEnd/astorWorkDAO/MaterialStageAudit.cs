using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

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

        [Required]
        public bool StagePassed { get; set; }

        public int LocationId { get; set; }
        public LocationMaster Location { get; set; }
        
        public string Remarks { get; set; }

        public int CreatedByID { get; set; }
        public UserMaster CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        // public List<MaterialQCPhotos> QCPhotos { get; set; }
        public List<MaterialQCCase> QCCases { get; set; }
    }
}
