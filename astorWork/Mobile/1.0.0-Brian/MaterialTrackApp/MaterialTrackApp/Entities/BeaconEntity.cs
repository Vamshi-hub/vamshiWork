using MaterialTrackApp.Utility;
using SQLite;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace MaterialTrackApp.DB
{
    public class BeaconEntity : MasterEntity, INotifyPropertyChanged
    {
        public string DisplayName { get; set; }
        public string ProximityUUID { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }

        private bool _isChecked;
        public bool IsChecked { get
            {
                return _isChecked;
            }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        [Ignore]
        public Color DistanceColor
        {
            get
            {
                if (_distance < 2)
                    return Color.Green;
                else
                    return Color.Gray;
            }
        }

        private double _distance;
        public Double Distance
        {
            get
            {
                return Math.Round(_distance, 2);
            }
            set
            {
                _distance = value;
                OnPropertyChanged("Distance");
                OnPropertyChanged("DistanceColor");
            }
        }
        public string BeaconID { get; set; }
        public string Project { get; set; }
        public string MarkingNo { get; set; }
        public Boolean InInventory { get; set; }
        public Boolean InTransit { get; set; }
        public Boolean Delivered { get; set; }
        public Boolean Installed { get; set; }
        public Boolean PassQC { get; set; }
        public Boolean FailQC { get; set; }
        public Boolean CanUse { get; set; }
        public int LotNo { get; set; }
        public DateTime CastingDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
