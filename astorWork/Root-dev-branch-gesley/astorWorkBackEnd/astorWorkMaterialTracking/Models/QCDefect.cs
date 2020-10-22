using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class QCDefect
    {
        public int ID { get; set; }
        public string Remarks { get; set; }
        public bool IsClosed { get; set; }

        public String CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public String UpdatedBy { get; set; }
        public bool IsRectified { get; set; }
        public bool IsOpen { get; set; }
        public int CountPhotos { get; set; }
        public int StatusCode { get; set; }

        public int? SelectedSubconID { get; set; }
        public string SubconName { get; set; }
    }
}
