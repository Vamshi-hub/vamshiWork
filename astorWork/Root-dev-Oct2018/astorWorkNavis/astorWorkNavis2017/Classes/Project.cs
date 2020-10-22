using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkNavis2017.Classes
{
    public class Project
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> MaterialTypes { get; set; }
        public IEnumerable<string> Blocks { get; set; }
        public IEnumerable<string> Mrfs { get; set; }
    }
}
