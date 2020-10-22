using System;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class QCDefect
    {
        public int ID { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public bool IsOpen { get; set; }
        public bool IsClosed { get { return !IsOpen; } }
        public int CountPhotos { get; set; }
    }
}
