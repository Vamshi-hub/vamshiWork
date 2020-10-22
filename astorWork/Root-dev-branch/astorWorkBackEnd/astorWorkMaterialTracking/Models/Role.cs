using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkDAO;

namespace astorWorkMaterialTracking.Models
{
    public class Role
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int DefaultPageID { get; set; }
        public string DefaultPageName { get; set; }
        public List<Page> Pages { get; set; }
    }
}
