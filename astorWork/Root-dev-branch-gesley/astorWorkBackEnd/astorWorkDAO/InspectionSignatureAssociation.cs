using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class InspectionSignatureAssociation
    {
        public int ID { get; set; }
        public int? InspectionAuditID { get; set; }
        public InspectionAudit InspectionAudit { get; set; }
        public int? UserID { get; set; }
        public UserMaster User { get; set; }
        public string Signature { get; set; }
    }
}
