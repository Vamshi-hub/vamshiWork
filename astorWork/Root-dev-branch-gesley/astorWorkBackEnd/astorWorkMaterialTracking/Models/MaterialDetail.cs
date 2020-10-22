using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class MaterialDetail
    {
        public int ID { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string MarkingNo { get; set; }
        public string Jobname { get; set; }
        public string MaterialType { get; set; }
        public string OrganisationName { get; set; }
        public string TrackerType { get; set; }
        public string TrackerTag { get; set; }
        public string TrackerLabel { get; set; }
        public string Remarks { get; set; }
        public DateTimeOffset? ExpectedDeliveryDate { get; set; }
        public DateTimeOffset? CastingDate { get; set; }
        public DateTimeOffset? OrderDate { get; set; }
        public List<TrackingHistory> TrackingHistory { get; set; }
        public int StageID { get; set; }
        public int StageOrder { get; set; }
        public string StageName { get; set; }
        public string StageColour { get; set; }
        //public MaterialDrawingAudit Drawing { get; set; }
        public int OpenQCCaseID { get; set; }
        public int QCStatusCode { get; set; }
        public string QCStatus { get; set; }
        public string QCRemarks { get; set; }
        public string MRFNo { get; set; }
        //public int SN { get; set; }
        //public bool IsOpenQCCase { get; set; }
        //public bool NextStageIsQCOrInstalledStage { get; set; }
        //public LocationMaster SelectedLocation { get; set; }

        public int? ForgeElementId { get; set; }
        public string ForgeModelURN { get; set; }
        //public bool AllowUpdate { get; set; }

        public int DeliveryStageOrder { get; set; }
        public DateTimeOffset ExpectedDelivery { get; set; }
        public string AssemblyLocation { get; set; }
        public string DrawingNo { get; set; }
        public float? Area { get; set; }
        public float? Length { get; set; }
        public String Dimensions { get; set; }
    }
}
