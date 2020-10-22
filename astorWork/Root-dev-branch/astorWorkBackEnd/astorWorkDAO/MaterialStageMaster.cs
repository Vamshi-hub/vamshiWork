using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace astorWorkDAO
{
    public class MaterialStageMaster
    {
        public int ID { get; set; }

        [Required]
        //[MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public int Order { get; set; }

        [Required]
        public string Colour { get; set; }

        /*
         * 1 - Produced
         * 2 - Delivered
         * 3 - Installed
         */
        public int MilestoneId { get; set; }

        public bool IsEditable { get; set; }

        public string MaterialTypes { get; set; }

        public bool CanIgnoreQC { get; set; }
        public List<ChecklistMaster> checklists { get; set; }
    }
}
