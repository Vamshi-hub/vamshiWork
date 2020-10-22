using MaterialTrackApp.Utility;

namespace MaterialTrackApp.DB
{
    public class MaterialEntity: MasterEntity
    {
        public int MaterialID { get; set; }
        public string MarkingNo { get; set; }

        public string MRFNo { get; set; }
        public int LotNo { get; set; }
        public string Project { get; set; }
        public string MaterialType { get; set; }
        public string Status { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }

        public string BeaconID { get; set; }
        public string CastingDate { get; set; }

        private bool _isInUse;
        public bool IsInUse { get { return _isInUse; }
            set {
                _isInUse = value;
            }
        }
    }
}
