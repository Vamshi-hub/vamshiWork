//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace astorWorkMVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MaterialDetail
    {
        public long MaterialDetailID { get; set; }
        public string MaterialNo { get; set; }
        public string MarkingNo { get; set; }
        public string Stage { get; set; }
        public Nullable<bool> IsQC { get; set; }
        public string QCStatus { get; set; }
        public string QCBy { get; set; }
        public Nullable<System.DateTime> QCDate { get; set; }
        public Nullable<long> LocationID { get; set; }
        public string Location { get; set; }
        public string RFIDTagID { get; set; }
        public string BeaconID { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    }
}
