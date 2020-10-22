using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class MRFMaster
    {
        public int ID { get; set; }
        public string MRFNo { get; set; }
        public List<MaterialMaster> Materials { get; set; }
        public List<UserMRFAssociation> UserMRFAssociations { get; set; }
        
        public DateTimeOffset OrderDate { get; set; }
        public DateTimeOffset PlannedCastingDate { get; set; }
        public DateTimeOffset ExpectedDeliveryDate { get; set; }
        public string Remarks { get; set; }

        public double MRFCompletion { get; set; }

        public UserMaster CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public UserMaster UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
