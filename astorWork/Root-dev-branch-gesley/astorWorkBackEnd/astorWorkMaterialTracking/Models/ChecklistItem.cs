using System;

namespace astorWorkMaterialTracking.Models
{
    public class ChecklistItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string TimeFrame { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public int Sequence { get; set; }
    }
}
