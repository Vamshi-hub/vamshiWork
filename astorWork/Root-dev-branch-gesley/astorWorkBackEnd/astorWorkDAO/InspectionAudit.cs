using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class InspectionAudit
    {
        public int ID { get; set; }
        public string RoomType { get; set; }
        public string ReferenceNo { get; set; }
        public DateTime? InspectionDate { get; set; }
        public TimeSpan? InspectionTime { get; set; }
        public List<InspectionSignatureAssociation> Signatures { get; set; }
        public string Remarks { get; set; }
    }
}
