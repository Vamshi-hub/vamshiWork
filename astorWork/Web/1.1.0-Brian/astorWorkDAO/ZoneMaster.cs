//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace astorWorkDAO
{
    using System;
    using System.Collections.Generic;
    
    public partial class ZoneMaster
    {
        public int ZoneID { get; set; }
        public string ZoneDescription { get; set; }
        public string ZoneCoordinates { get; set; }
        public string ZoneName { get; set; }
        public string ZoneColor { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public System.DateTime ModifiedDate { get; set; }
        public Nullable<int> YardID { get; set; }
        public byte[] YardLayout { get; set; }
    
        public virtual YardMaster YardMaster { get; set; }
    }
}
