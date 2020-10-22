using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkBackgroundService.Models
{
    public class DeliveryMaterial
    {
        public int ID { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string MarkingNo { get; set; }
        public string MaterialType { get; set; }
        public DateTimeOffset ExpectedDeliveryDate { get; set; }
        public int TimeZoneOffset { get; set; }
    }
}
