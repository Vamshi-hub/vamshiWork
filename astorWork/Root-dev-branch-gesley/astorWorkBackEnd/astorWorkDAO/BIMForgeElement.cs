using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class BIMForgeElement
    {
        public int ID { get; set; }

        public int DbID { get; set; }

        public int ForgeModelID { get; set; }
        public BIMForgeModel ForgeModel { get; set; }

        public int MaterialMasterID { get; set; }
        public MaterialMaster MaterialMaster { get; set; }
    }
}
