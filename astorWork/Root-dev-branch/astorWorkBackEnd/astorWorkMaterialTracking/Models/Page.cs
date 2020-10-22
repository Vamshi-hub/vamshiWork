using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class Page
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ModuleID { get; set; }
        public string ModuleName { get; set; }
        /*
         * 0 - Forbidden
         * 1 - Read only
         * 2 - Read and Write
         * 3 - Full access (including Delete)
         */
        public string AccessLevel { get; set; }
    }
}
