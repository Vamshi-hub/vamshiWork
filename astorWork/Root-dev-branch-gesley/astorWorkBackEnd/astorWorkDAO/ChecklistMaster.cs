using System;
using System.Collections.Generic;
using System.Text;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
    public class ChecklistMaster
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public TradeMaster Trade { get; set; }
        public int? TradeID { get; set; }
        public int Sequence { get; set; }
        public MaterialStageMaster MaterialStage { get; set; }
        public int? MaterialStageID { get; set; }
        public bool IsActive { get; set; }
        public ChecklistType Type { get; set; }
    }
}
