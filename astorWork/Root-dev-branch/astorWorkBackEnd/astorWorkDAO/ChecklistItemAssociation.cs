using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class ChecklistItemAssociation
    {
        public int ID { get; set; }
        public int ChecklistItemID { get; set; }
        public int ChecklistID { get; set; }
        public ChecklistItemMaster ChecklistItem { get; set; }
        public ChecklistMaster Checklist { get; set; }
        public int ChecklistItemSequence { get; set; }
        public bool IsActive { get; set; }
    }
}
