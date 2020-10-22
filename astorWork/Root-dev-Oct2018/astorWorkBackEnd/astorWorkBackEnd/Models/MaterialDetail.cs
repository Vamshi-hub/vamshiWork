using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class MaterialDetail
    {
        public int ID { get; set; }
        public string MarkingNo { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string MaterialType { get; set; }
        public string VendorName { get; set; }
        public string TrackerType { get; set; }
        public string TrackerTag { get; set; }
        public string TrackerLabel { get; set; }
        public string Remarks { get; set; }
        public DateTimeOffset? ExpectedDeliveryDate { get; set; }
        public DateTimeOffset CastingDate { get; set; }
        public DateTimeOffset? OrderDate { get; set; }
        public List<TrackingHistory> TrackingHistory { get; set; }
        public int StageId { get; set; }
        public string StageName { get; set; }
        public string StageColour { get; set; }
        public MaterialDrawingAudit Drawing { get; set; }
        public bool qcStatus { get; set; }
        public string qcRemarks { get; set; }
        public string MRFNo { get; set; }
        public int SN { get; set; }
        public bool IsOpenQCCase { get; set; }
        public bool NextStageIsQCOrInstalledStage { get; set; }
        public LocationMaster SelectedLocation { get; set; }

        public int? ForgeElementId { get; set; }
        public string ForgeModelURN { get; set; }
        public bool allowUpdate { get; set; }
    }
}
