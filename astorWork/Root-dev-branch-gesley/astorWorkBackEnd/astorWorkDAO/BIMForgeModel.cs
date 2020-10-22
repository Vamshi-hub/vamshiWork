using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class BIMForgeModel
    {
        public int ID { get; set; }

        public string BucketKey { get; set; }
        public string ObjectKey { get; set; }
        public string ObjectID{ get; set; }
        public string Sha1 { get; set; }
        public int Size { get; set; }
        public string Location { get; set; }

        public List<BIMForgeElement> Elements { get; set; }
    }
}
