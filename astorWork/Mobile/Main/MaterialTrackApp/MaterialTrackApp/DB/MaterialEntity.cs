using MaterialTrackApp.Utility;

namespace MaterialTrackApp.DB
{
    public class MaterialEntity: MasterEntity
    {
        public int MaterialId { get; set; }
        public string MarkingNo { get; set; }
        public string QCStatus { get; set; }
        public string Status { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }

        public string BeaconID { get; set; }

        private bool _isInUse;
        public bool IsInUse { get { return _isInUse; }
            set {
                _isInUse = value;

                /*
                if (ViewModelLocator.EnrollPageVM.PendingMaterials != null && ViewModelLocator.EnrollPageVM.PendingMaterials.Contains(this) && _isInUse)
                    ViewModelLocator.EnrollPageVM.PendingMaterials.Remove(this);

                if (ViewModelLocator.EnrollPageVM.PendingMaterials != null && !ViewModelLocator.EnrollPageVM.PendingMaterials.Contains(this) && !_isInUse)
                    ViewModelLocator.EnrollPageVM.PendingMaterials.Add(this);
                    */
            }
        }
    }
}
