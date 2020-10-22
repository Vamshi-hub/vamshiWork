using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class Inventory
    {
        public int ID { get; set; }
        public string MarkingNo { get; set; }
        public int SN { get; set; }
        public DateTimeOffset CastingDate { get; set; }
        public string TrackerLabel { get; set; }
        public int TrackerID { get; set; }
    }
}
