using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class BIMForgeElement
    {
        public int ID { get; set; }

        public int DbId { get; set; }

        public int ForgeModelId { get; set; }
        public BIMForgeModel ForgeModel { get; set; }

        public int MaterialMasterId { get; set; }
        public MaterialMaster MaterialMaster { get; set; }
    }
}
