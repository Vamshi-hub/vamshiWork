using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class DelayedMaterial
    {
        public int ID { get; set; }
        public string MarkingNo { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string Type { get; set; }
        public string MRFNo { get; set; }
        public string GridLine { get; set; }

        public DateTimeOffset PlannedProductionDate { get; set; }
        public DateTimeOffset ActualProductionDate { get; set; }
        public DateTimeOffset PlannedDeliveryDate { get; set; }
        public DateTimeOffset ActualDeliveryDate { get; set; }
        public DateTimeOffset PlannedInstallationDate { get; set; }
        public DateTimeOffset ActualInstallationDate { get; set; }
    }
}
