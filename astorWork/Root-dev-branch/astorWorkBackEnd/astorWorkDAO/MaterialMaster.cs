using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace astorWorkDAO
{
    public class MaterialMaster
    { 
        public int ID { get; set; }

        [Required]
        public string MarkingNo { get; set; }
        
        public int ProjectID { get; set; }
        [Required]
        public ProjectMaster Project { get; set; }
        [Required]
        public string Block { get; set; }
        [Required]
        public string Level { get; set; }
        [Required]
        public string Zone { get; set; }
        [Required]
        public MaterialTypeMaster MaterialType { get; set; }

        public int OrganisationID { get; set; }
        public OrganisationMaster Organisation { get; set; }          
        public MRFMaster MRF { get; set; }

        public int SN { get; set; }
        public DateTimeOffset CastingDate { get; set; }

        public List<TrackerMaster> Trackers { get; set; }
        public List<MaterialStageAudit> StageAudits { get; set; }
        public List<MaterialDrawingAssociation> DrawingAssociations { get; set; }

        public List<MaterialInfoAudit> MaterialInfoAudits { get; set; }
        /*
        public RebarShapeMaster RebarShape { get; set; }
        //Dimensions: "[300, 700, 1200]"
        public string Dimensions { get; set; }
        public int OrderQuantity { get; set; }
        */

        public List<BIMForgeElement> Elements { get; set; }
        public List<MaterialQCCase> QCCases { get; set; }
        public string AssemblyLocation { get; set; }
        public string Dimensions { get; set; }
        public float? Length { get; set; }
        public float? Area { get; set; }

    }
}
