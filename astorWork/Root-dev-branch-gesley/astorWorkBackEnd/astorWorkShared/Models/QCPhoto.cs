using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkShared.Models
{
    public class QCPhoto
    {
        public int ID { get; set; }
        public string ImageContent { get; set; }
        public string Remarks { get; set; }
        public bool Closed { get; set; }

        public string URL { get; set; }
        public bool IsOpen { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
