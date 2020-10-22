using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
   
    public class JobSchedule
    {
        public int ID { get; set; }

        public int MaterialID { get; set; }
        [Required]
        public MaterialMaster Material { get; set; }

        public int TradeID { get; set; }
        [Required]
        public TradeMaster Trade { get; set; }

        public int? SubconID { get; set; }
        public OrganisationMaster Subcon { get; set; }

        public DateTimeOffset? PlannedStartDate { get; set; }
        public DateTimeOffset? PlannedEndDate { get; set; }
        public DateTimeOffset? ActualStartDate { get; set; }
        public DateTimeOffset? ActualEndDate { get; set; }
        public JobStatus Status { get; set; }
        public List<ChecklistAudit> Checklists { get; set; }


    }
}
