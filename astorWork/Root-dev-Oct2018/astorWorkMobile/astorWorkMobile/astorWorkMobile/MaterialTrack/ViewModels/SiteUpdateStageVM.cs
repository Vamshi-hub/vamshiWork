using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class SiteUpdateStageVM : MasterVM
    {
        private ObservableCollection<Location> _locations;
        public ObservableCollection<Location> Locations { get
            {
                return _locations;
            }
            set
            {
                _locations = value;
                OnPropertyChanged("Locations");
            }
        }

        private Location _selectedLocation;
        public Location SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                _selectedLocation = value;
                OnPropertyChanged("ShowUpdateForm");
                OnPropertyChanged("SelectedLocation");
            }
        }

        private bool _updateLocationOnly;
        public bool UpdateLocationOnly { get
            {
                return _updateLocationOnly;
            }
            set
            {
                _updateLocationOnly = value;
                OnPropertyChanged("UpdateLocationOnly");
                OnPropertyChanged("UpdateModeLabel");
            }
        }

        public string UpdateModeLabel
        {
            get
            {
                if (_updateLocationOnly)
                    return "Update Location";
                else
                    return "Update Stage";
            }
        }

        public bool ShowUpdateForm
        {
            get
            {
                return _selectedLocation != null;
            }
        }

        public Material Material { get; set; }

        private string _nextStageName;
        public string NextStageName
        {
            get
            {
                return _nextStageName;
            }
            set
            {
                _nextStageName = value;
                OnPropertyChanged("NextStageName");
            }
        }

        private Color _nextStageColor;
        public Color NextStageColor
        {
            get
            {
                return _nextStageColor;
            }
            set
            {
                _nextStageColor = value;
                OnPropertyChanged("NextStageColor");
            }
        }

        private bool _nextStageQC;
        public bool NextStageQC
        {
            get
            {
                return _nextStageQC;
            }
            set
            {
                _nextStageQC = value;
                OnPropertyChanged("NextStageQC");
                OnPropertyChanged("IsSubmit");
                OnPropertyChanged("IsNewCase");
            }
        }
        private TogglePassVM _TogglePass;
        public TogglePassVM TogglePass
        {
            get
            {
                return _TogglePass;
            }
            set
            {
                _TogglePass = value;
                if(_TogglePass != null)
                    _TogglePass.PropertyChanged += _TogglePass_PropertyChanged;

                OnPropertyChanged("TogglePass");
            }
        }

        private void _TogglePass_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("IsSubmit");
            OnPropertyChanged("IsNewCase");
        }

        public string QCRemarks { get; set; }

        private ObservableCollection<QCPhoto> _qcPhotos;
        public ObservableCollection<QCPhoto> QCPhotos
        {
            get
            {
                return _qcPhotos;
            }
            set
            {
                _qcPhotos = value;
                OnPropertyChanged("QCPhotos");
            }
        }
        public bool IsNewCase
        {
            get
            {
                return !IsSubmit;
            }
        }
        public bool IsSubmit
        {
            get
            {
                return (!NextStageQC || TogglePass.Status);
            }
        }

        public int CountQCPhotos
        {
            get
            {
                return QCPhotos == null ? 0 : QCPhotos.Count;
            }
            set
            {
                OnPropertyChanged("QCPhotos");
                OnPropertyChanged("CountQCPhotos");
            }
        }

        public SiteUpdateStageVM() : base()
        {
            TogglePass = new TogglePassVM();
            _qcPhotos = new ObservableCollection<QCPhoto>();
            _neverLoadBefore = false;
        }

        public override void Reset()
        {
            TogglePass = new TogglePassVM();
            _neverLoadBefore = true;
            QCRemarks = string.Empty;
            SelectedLocation = null;
            QCPhotos.Clear();
            UpdateLocationOnly = false;
            OnPropertyChanged("CountQCPhotos");
        }
    }
}
