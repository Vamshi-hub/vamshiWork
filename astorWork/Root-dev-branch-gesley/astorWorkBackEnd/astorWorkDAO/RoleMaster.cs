using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace astorWorkDAO
{
    public class RoleMaster
    {
        public int ID { get; set; }
        public string Name { get; set; }

        /*
         * 0 - Web
         * 1 - Mobile
         * 2 - Desktop
         * 3 - Non-expire role
         * e.g. 01 means both web and mobile
         */
        public string PlatformCode { get; set; }
        public List<RolePageAssociation> RolePageAssociations { get; set; }

        public PageMaster DefaultPage { get; set; }

        /*
         * 0 - Vendor Inventory
         * 1 - Maincon
         * 2 - Sub-con
         * 3 - RTO
         */
        public int MobileEntryPoint { get; set; }        
        public bool IsEditable { get; set; }

        public List<RolePowerBIReport> PowerBIReports { get; set; }
    }
}
