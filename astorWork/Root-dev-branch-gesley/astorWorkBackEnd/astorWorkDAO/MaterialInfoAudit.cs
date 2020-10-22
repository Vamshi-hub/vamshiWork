using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class MaterialInfoAudit
    {
        public int ID { get; set; }

        public DateTimeOffset ExpectedDeliveryDate { get; set; }
        public string Remarks { get; set; }
        public MaterialMaster Material { get; set; }

        public UserMaster CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public UserMaster UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
