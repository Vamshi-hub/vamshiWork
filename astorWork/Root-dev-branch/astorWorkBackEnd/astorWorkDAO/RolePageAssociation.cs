using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace astorWorkDAO
{
    public class RolePageAssociation
    {
        public int RoleId { get; set; }
        public int PageId { get; set; }
        public RoleMaster Role { get; set; }
        public PageMaster Page { get; set; }

        /*
         * 0 - Forbidden
         * 1 - Read only
         * 2 - Read and Write
         * 3 - Full access (including Delete)
         */
        public int AccessLevel { get; set; }
    }
}
