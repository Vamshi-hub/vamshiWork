using System;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
    public class MaterialQCPhotos
    {
        public int ID { get; set; }
        public string URL { get; set; }
        public string Remarks { get; set; }
        public QCStatus Status { get; set; }

        public int? CreatedById { get; set; }
        public UserMaster CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
