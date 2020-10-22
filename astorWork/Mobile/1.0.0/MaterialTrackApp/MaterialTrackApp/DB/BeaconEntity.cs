using MaterialTrackApp.Utility;
using System;

namespace MaterialTrackApp.DB
{
    public class BeaconEntity : MasterEntity
    {
        public string DisplayName { get; set; }
        public string ProximityUUID { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public string PrevStatus { get; set; }
        public string NextStatus { get; set; }

        public string BeaconID { get; set; }

        private MaterialEntity _material;
        public MaterialEntity Material
        {
            get
            {
                return _material;
            }
            set
            {
                if (_material != null)
                {
                    _material.IsInUse = false;

                    if (ViewModelLocator.EnrollPageVM.PendingMaterials != null && !ViewModelLocator.EnrollPageVM.PendingMaterials.Contains(_material))
                        ViewModelLocator.EnrollPageVM.PendingMaterials.Add(_material);
                }


                _material = value;

                if (_material != null)
                {
                    _material.IsInUse = true;

                    if (ViewModelLocator.EnrollPageVM.PendingMaterials != null && ViewModelLocator.EnrollPageVM.PendingMaterials.Contains(_material))
                        ViewModelLocator.EnrollPageVM.PendingMaterials.Remove(_material);
                }
            }
        }

        public DateTime CastingDate { get; set; }
        public int LotNo { get; set; }
    }
}
