using System;
using System.Collections.Generic;

namespace astorWorkDAO
{
    public class MaterialQCCase
    {
        public int ID { get; set; }
        public string CaseName { get; set; } // e.g. QC-2018-0001

        public int? CreatedById { get; set; }
        public UserMaster CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }

        public int? UpdatedById { get; set; }
        public UserMaster UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }

        public List<MaterialQCDefect> Defects { get; set; }

        public int StageAuditId { get; set; }
        public MaterialStageAudit StageAudit { get; set; }
    }
}
