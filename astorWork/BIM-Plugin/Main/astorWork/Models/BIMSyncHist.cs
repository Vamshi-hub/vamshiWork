//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace astorWork.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BIMSyncHist
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public Nullable<System.DateTime> LastSyncDate { get; set; }
        public int Status { get; set; }
        public string SyncedMaterialIds { get; set; }
        public string MissingMaterialIds { get; set; }
        public string BIMProjectID { get; set; }
        public string VisualizeURL { get; set; }
        public string Remarks { get; set; }
    
        public virtual BIMUserInfo BIMUserInfo { get; set; }
    }
}
