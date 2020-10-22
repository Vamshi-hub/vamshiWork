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

        public int ProjectId { get; set; }
        [Required]
        public ProjectMaster Project { get; set; }
        [Required]
        public string Block { get; set; }
        [Required]
        public string Level { get; set; }
        [Required]
        public string Zone { get; set; }
        [Required]
        public string MaterialType { get; set; }

        public int VendorId { get; set; }
        public VendorMaster Vendor { get; set; }          
        public MRFMaster MRF { get; set; }

        public int SN { get; set; }
        public DateTimeOffset CastingDate { get; set; }

        public TrackerMaster Tracker { get; set; }
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
    }
}
