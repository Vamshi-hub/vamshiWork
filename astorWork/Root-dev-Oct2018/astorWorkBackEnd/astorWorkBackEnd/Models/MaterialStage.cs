using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class MaterialStage
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Colour { get; set; }
        public bool IsQCStage { get; set; }
        public List<string> MaterialTypes { get; set; }
        public int NextStageID { get; set; }

        public int Order { get; set; }
        public bool isEditable { get; set; }
    }
}
