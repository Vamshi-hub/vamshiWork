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
    
    public partial class UserMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserMaster()
        {
            this.IsVendor = false;
        }
    
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public bool IsVendor { get; set; }
    }
}
