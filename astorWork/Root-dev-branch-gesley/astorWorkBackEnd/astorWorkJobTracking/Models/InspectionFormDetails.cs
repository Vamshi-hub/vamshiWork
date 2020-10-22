using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class InspectionFormDetails
    {
        public string ReferenceNo { get; set; }
        public string ModuleName { get; set; }
        public string DrawingNo { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Time { get; set; }
        public List<Signatures> Signatures { get; set; }
        public int StatusCode { get; set; }
        public String Remarks { get; set; }
    }
}
