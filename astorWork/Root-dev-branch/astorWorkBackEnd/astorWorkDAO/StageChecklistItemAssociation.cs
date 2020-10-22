using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    class StageChecklistItemAssociation
    {
        public int ID { get; set; }
        public int ChecklistItemID { get; set; }
        public int StageID { get; set; }
        public ChecklistItemMaster ChecklistItem { get; set; }
        public MaterialStageMaster Stage { get; set; }
        public int Sequence { get; set; }
    }
}
