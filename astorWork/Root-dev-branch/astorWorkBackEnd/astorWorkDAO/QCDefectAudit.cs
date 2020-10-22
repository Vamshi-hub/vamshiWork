using System;
using System.Collections.Generic;
using System.Text;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
    class QCDefectAudit
    {
        public int ID { get; set; }
        public MaterialQCDefect QCDefect { get; set; }
        public int? QCDefectID { get; set; }

        public DateTime CreatedDate { get; set; }
        public UserMaster CreatedBy { get; set; }
        public string Remarks { get; set; }
        public QCStatus Status { get; set; }
    }
}
