using System;
using System.Collections.Generic;
using System.Text;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
    public class MaterialQCDefect
    {
        public int ID { get; set; }

        public QCStatus Status { get; set; }

        public OrganisationMaster Organisation { get; set; }

        public int? OrganisationID { get; set; }
        public int? CreatedByID { get; set; }
        public UserMaster CreatedBy { get; set; }

        public DateTimeOffset CreatedDate { get; set; }

        public int? UpdatedByID { get; set; }
        public UserMaster UpdatedBy { get; set; }

        public DateTimeOffset? UpdatedDate { get; set; }

        public int QCCaseID { get; set; }
        public MaterialQCCase QCCase { get; set; }

        public List<MaterialQCPhotos> Photos { get; set; }

        public string Remarks { get; set; }
    }
}
