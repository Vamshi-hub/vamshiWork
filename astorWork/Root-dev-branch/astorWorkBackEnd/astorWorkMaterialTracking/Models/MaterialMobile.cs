using astorWorkDAO;
using System;

namespace astorWorkMaterialTracking.Models
{
    public class MaterialMobile
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public string MarkingNo { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string RouteTo { get; set; }
        public string MaterialType { get; set; }
        public int OrganisationID { get; set; }
        public DateTimeOffset CastingDate { get; set; }
        public DateTimeOffset ExpectedDeliveryDate { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public MaterialStage CurrentStage { get; set; }
        public MaterialLocation CurrentLocation { get; set; }
        public int CountQCCase { get; set; }
        public string MRFNo { get; set; }
        public int SN { get; set; }

        public int? ForgeElementID { get; set; }
        public string ForgeModelURN { get; set; }
        public bool CanIgnoreQC { get; set; }
        public string QCStatus { get; set; }
        public int QCStatusCode { get; set; }
        public bool IsQCOpen { get; set; }
        public float? Area { get; set; }
        public float? Length { get; set; }
        public string DrawingNo { get; set; }
        public bool IsChecklist { get; set; }
    }

    public class MaterialLocation
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
