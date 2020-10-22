using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.DB
{
    public class TaskEntity: MasterEntity
    {
        public string Project { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string MarkingNo { get; set; }
        public string BeaconID { get; set; }
        public DateTime CastingDate { get; set; }
        public bool IsQCPass { get; set; }
        public int LotNo { get; set; }
    }
}
