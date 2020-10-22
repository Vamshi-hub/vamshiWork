using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class Module
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Page> Pages { get; set; }
    }
}
