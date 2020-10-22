using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class SignedChecklist
    {
        public List<ChecklistItem> ChecklistItems { get; set; }
        public string Signature { get; set; }
        public List<Signature> Signatures { get; set; }
    }
}
