using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class MaterialQCDefect
    {
        public int ID { get; set; }

        public bool IsOpen { get; set; }

        public int? CreatedById { get; set; }
        public UserMaster CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public int? UpdatedById { get; set; }
        public UserMaster UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }

        public int QCCaseId { get; set; }
        public MaterialQCCase QCCase { get; set; }

        public List<MaterialQCPhotos> Photos { get; set; }

        public string Remarks { get; set; }
    }
}
