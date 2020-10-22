using System;

namespace astorWorkJobTracking.Models
{
    public class Checklist
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public int Sequence { get; set; }
        public bool IsEnabled { get; set; }
        public int RTOID { get; set; }
        public string RTOName { get; set; }
    }
}
