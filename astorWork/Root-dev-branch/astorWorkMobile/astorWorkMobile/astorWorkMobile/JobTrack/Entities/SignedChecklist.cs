using astorWorkMobile.Shared.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkMobile.JobTrack.Entities
{
    public class SignedChecklist
    {
        public List<JobChecklistItem> ChecklistItems { get; set; }
        public string Signature { get; set; }
        public List<Signature> Signatures { get; set; }
    }
}
