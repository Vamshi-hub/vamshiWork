using System;
using Xamarin.Forms;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace astorTrackP
{
    public class MaterialMaster
    {
        [PrimaryKey]
        public string MaterialNo { get; set; }
        public string MarkingNo { get; set; }
        public string Project { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string DrawingNo { get; set; }
        public string MaterialType { get; set; }
        public string MaterialSize { get; set; }
        public long EstimatedLength { get; set; }
        public long ActualLength { get; set; }
        public long LocationID { get; set; }
        public string Officer { get; set; }
        public string Contractor { get; set; }
        public string RFIDTagID { get; set; }
        public string BeaconID { get; set; }
        public string Status { get; set; }
        public string MRFNo { get; set; }
        public string QAStatus { get; set; }
        public string Location { get; set; }
        public string DeliveredLocation { get; set; }
        public string DeliveryRemarks { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Remarks { get; set; }
        public DateTime? CastingDate { get; set; }
        public int LotNo { get; set; }
    }

    public class MRFModel : ObservableCollection<MaterialMaster>
    {
        public string MRFNo { get; set; }
        public string Location { get; set; }
    }

    public class MaterialDetail
    {
        public string MaterialNo { get; set; }
        public string MarkingNo { get; set; }        
        public string RFIDTagID { get; set; }
        public string BeaconID { get; set; }
        public string Stage { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? LocationID { get; set; }
        public string Location { get; set; }
        public int SeqNo { get; set; }
        public bool? isQC { get; set; }
        public string QCBy { get; set; }
        public string QCStatus { get; set; }
        public DateTime? QCDate { get; set; }
    }

    public class MaterialMasterMarkingNo
    {
        [PrimaryKey]
        public long ID { get; set; }
        public string Prefix { get; set; }
        public string MarkingNo { get; set; }
        public string MaterialType { get; set; }        
        public int Counter { get; set; }
        public int LotNo { get; set; }
        public DateTime? CastingDate { get; set; }
        public string RFIDTagID { get; set; }
        public string BeaconID { get; set; }
        public string Project { get; set; }
    }

    public class MaterialMasterDashboard
    {
        public decimal Progress { get; set; }
        public int Produced { get; set; }
        public int Delivered { get; set; }
        public int Installed { get; set; }        
    }

    public class MaterialMasterMarkingNoModel : ObservableCollection<MaterialMasterMarkingNo>
    {
        public string MaterialType { get; set; }
        public string Prefix { get; set; }
    }
}

