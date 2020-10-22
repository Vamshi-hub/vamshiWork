using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkDAO;

namespace astorWorkBackEnd.Models
{
    public class RoleDetails
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }
        public int DefaultPageID { get; set; }
        public List<ListofPages> ListofPages { get; set; }
    }

    public class ListofPages
    {
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        public int PageId { get; set; }
        public string PageName { get; set; }

        /*
         * 0 - Forbidden
         * 1 - Read only
         * 2 - Read and Write
         * 3 - Full access (including Delete)
         */
        public string AccessLevel { get; set; }
    }
}
