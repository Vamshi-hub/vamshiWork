using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkUserManage.Models
{
    public class EmailCompose
    {
        public string RecipientAddress { get; set; }
        public string RecipientName { get; set; }
        public string[] RecipientAddresses { get; set; }
        public string[] RecipientNames { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
