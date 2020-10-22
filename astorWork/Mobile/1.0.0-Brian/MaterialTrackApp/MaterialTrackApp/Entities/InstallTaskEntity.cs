using MaterialTrackApp.DB;
using SQLite;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace MaterialTrackApp.Entities
{
    public class InstallTaskEntity : MasterEntity, INotifyPropertyChanged
    {
        [Ignore]
        public BeaconEntity Beacon { get; set; }

        public string BeaconName
        {
            get
            {
                if (Beacon == null)
                    return "N.A.";
                else
                    return Beacon.DisplayName;
            }
        }

        public int MaterialId { get; set; }
        public string Project { get; set; }
        public string Block { get; set; }
        public string Level { get; set; }
        public string Zone { get; set; }
        public string MarkingNo { get; set; }

        private bool _passQC;
        public Boolean PassQC { get
            {
                return _passQC;
            }
            set
            {
                _passQC = value;
                OnPropertyChanged("PassQC");
            }
        }

        private bool _selected;
        public Boolean Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
