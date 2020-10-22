using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkUserManage.Models
{
   
    public class RoleAccess
    {
        public int userId { get; set; }
        public DateTimeOffset expiryTime { get; set; }
        public PageAccessRight[] pageAccessRights { get; set; }
        public string defaultPage { get; set; }
        public long mobileEntryPoint { get; set; }
    }

    public class PageAccessRight
    {
        public string url { get; set; }
        public int right { get; set; }

    }

}
