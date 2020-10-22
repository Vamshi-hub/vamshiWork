using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class Project
    {
        public int id { get; set; }
        public string name { get; set; }
        public IEnumerable<string> materialTypes { get; set; }
        public IEnumerable<string> blocks { get; set; }
        public IEnumerable<string> mrfs { get; set; }
    }
}
