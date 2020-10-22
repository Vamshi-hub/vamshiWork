using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.DB
{
    public class EnrollEntity : MasterEntity
    {
        public string MaterialNo{ get; set; }
        public string BeaconId { get; set; }
        public DateTime CastingDate { get; set; }
        public int LotNo { get; set; }
        public bool Submitted { get; set; }
        public DateTime SubmitDate { get; set; }
    }
}
