using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace astorWorkMVC.Models
{
    public class EchoMessage
    {
        public string Source { get; set; }
        public string Method { get; set; }
        public DateTime ReceiveTime { get; set; }
        public string Message { get; set; }
    }
}