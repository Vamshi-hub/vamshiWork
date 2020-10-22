using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class MaterialListItem
    {
        public int ID { get; set; }
        public string MarkingNo { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string Type { get; set; }
        public string MRFNo { get; set; }
        public DateTimeOffset? ExpectedDelivery { get; set; }
    }
}
