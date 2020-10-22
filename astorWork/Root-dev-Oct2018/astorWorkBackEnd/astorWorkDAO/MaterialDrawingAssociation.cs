using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class MaterialDrawingAssociation
    {
        public int MaterialID { get; set; }
        public int DrawingID { get; set; }
        public MaterialMaster Material { get; set; }
        public MaterialDrawingAudit Drawing { get; set; }
    }
}
