using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class TradeMaterialTypeAssociation
    {
        public int ID { get; set; }
        public int MaterialTypeID { get; set; }
        public int TradeID { get; set; }
        public MaterialTypeMaster MaterialType { get; set; }
        public TradeMaster Trade { get; set; }
    }
}
