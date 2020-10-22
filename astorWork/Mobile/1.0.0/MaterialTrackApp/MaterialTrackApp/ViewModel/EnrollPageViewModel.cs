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
    public class EnrollPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<BeaconEntity> _beacons;
        private ObservableCollection<string> _blocks;
        private ObservableCollection<string> _levels;
        private ObservableCollection<string> _zones;
        private ObservableCollection<MaterialEntity> _materials;
        private ObservableCollection<MaterialEntity> _pendingMaterials;

        public ObservableCollection<BeaconEntity> Beacons
        {
            get { return _beacons; }
            set
            {
                _beacons = value;
                OnPropertyChanged("Beacons");
            }
        }

        public ObservableCollection<string> Blocks
        {
            get { return _blocks; }
            set
            {
                _blocks = value;
                OnPropertyChanged("Blocks");
            }
        }

        public ObservableCollection<string> Levels
        {
            get { return _levels; }
            set
            {
                _levels = value;
                OnPropertyChanged("Levels");
            }
        }

        public ObservableCollection<string> Zones
        {
            get { return _zones; }
            set
            {
                _zones = value;
                OnPropertyChanged("Zones");
            }
        }

        public ObservableCollection<MaterialEntity> Materials
        {
            get { return _materials; }
            set
            {
                _materials = value;
                OnPropertyChanged("Materials");
            }
        }

        public ObservableCollection<MaterialEntity> PendingMaterials
        {
            get { return _pendingMaterials; }
            set
            {
                _pendingMaterials = value;
                OnPropertyChanged("PendingMaterials");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public EnrollPageViewModel()
        {
            Beacons = new ObservableCollection<BeaconEntity>();
            Materials = new ObservableCollection<MaterialEntity>();
            Blocks = new ObservableCollection<string>();
            Levels = new ObservableCollection<string>();
            Zones = new ObservableCollection<string>();
        }
    }
}
