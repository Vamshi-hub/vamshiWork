using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace astorWorkMVC.Models
{
    public class BIMMaterial
    {
        public long MaterialNo { get; set; }
        public string MarkingNo { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string CurrentStatus { get; set; }
        public string Status { get; set; }
        public DateTime UpdateDT { get; set; }
    }
}