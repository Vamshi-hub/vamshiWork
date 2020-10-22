using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class RolePowerBIReport
    {
        public int ID { get; set; }
        public int RoleId { get; set; }
        public RoleMaster Role { get; set; }
        public string PowerBIReportId { get; set; }
    }
}
