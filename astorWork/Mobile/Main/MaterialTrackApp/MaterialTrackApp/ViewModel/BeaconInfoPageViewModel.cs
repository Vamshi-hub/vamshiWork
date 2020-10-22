using MaterialTrackApp.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.ViewModel
{
    public class BeaconInfoPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BeaconEntity> _beaconsNotInUse;
        private ObservableCollection<BeaconEntity> _beaconsInUse;

        public ObservableCollection<BeaconEntity> BeaconsNotInUse
        {
            get { return _beaconsNotInUse; }
            set
            {
                _beaconsNotInUse = value;
                OnPropertyChanged("BeaconsNotInUse");
            }
        }

        public ObservableCollection<BeaconEntity> BeaconsInUse
        {
            get { return _beaconsInUse; }
            set
            {
                _beaconsInUse = value;
                OnPropertyChanged("BeaconsInUse");
            }
        }
        

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public BeaconInfoPageViewModel()
        {
            BeaconsNotInUse = new ObservableCollection<BeaconEntity>();
            BeaconsInUse = new ObservableCollection<BeaconEntity>();
        }
    }
}
