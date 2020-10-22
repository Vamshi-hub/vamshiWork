using MaterialTrackApp.DB;
using SQLite;
using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace MaterialTrackApp.Entities
{
    public class EnrollTaskEntity : MasterEntity, INotifyPropertyChanged
    {
        [Ignore]
        public BeaconEntity Beacon { get; set; }
        public string Project { get; set; }

        private string _markingNo;
        public string MarkingNo
        {
            get
            {
                return _markingNo;
            }
            set
            {
                _markingNo = value;
                OnPropertyChanged("MarkingNo");
                OnPropertyChanged("CanClearMarkingNo");
            }
        }

        private string _mrfNo;
        public string MRFNo
        {
            get
            {
                return _mrfNo;
            }
            set
            {
                _mrfNo = value;
                OnPropertyChanged("MRFNo");
                OnPropertyChanged("CanClearMRFNo");
                OnPropertyChanged("CanQC");
            }
        }
        
        public int LotNo { get; set; }

        public DateTime CastingDate { get; set; }

        public Boolean NoUpdate
        {
            get
            {
                return 
                    (string.IsNullOrEmpty(_markingNo) && string.IsNullOrEmpty(_mrfNo)) ||
                    (string.IsNullOrEmpty(_mrfNo) && InTransit);
            }
        }

        public Boolean CanClearMarkingNo
        {
            get
            {
                return !InTransit && !string.IsNullOrEmpty(_markingNo);
            }
        }

        public Boolean CanClearMRFNo
        {
            get
            {
                return !string.IsNullOrEmpty(_mrfNo);
            }
        }
        public Boolean InTransit { get; set; }
        public Boolean NotInTransit {
            get {
                return !InTransit;
            }
        }

        private bool _passQC;
        public Boolean PassQC
        {
            get
            {
                return _passQC;
            }
            set
            {
                _passQC = value;
                OnPropertyChanged("PassQC");
            }
        }
        public Boolean CanQC {
            get {
                return !string.IsNullOrEmpty(_markingNo) && !string.IsNullOrEmpty(_mrfNo);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
