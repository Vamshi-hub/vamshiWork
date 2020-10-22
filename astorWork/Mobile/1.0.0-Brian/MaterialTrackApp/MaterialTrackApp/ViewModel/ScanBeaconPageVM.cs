using MaterialTrackApp.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MaterialTrackApp.ViewModel
{
    public class ScanBeaconPageVM : INotifyPropertyChanged
    {
        public int CountBeacons { get; set; }

        private ObservableCollection<BeaconEntity> _beacons;
        public ObservableCollection<BeaconEntity> Beacons
        {
            get { return _beacons; }
            set
            {
                _beacons = value;
                OnPropertyChanged("Beacons");

                CountBeacons = value == null ? 0 : value.Count;
                OnPropertyChanged("CountBeacons");
            }
        }

        private bool _bluetoothEnabled;
        public Boolean BluetoothEnabled
        {
            get { return _bluetoothEnabled; }
            set
            {
                _bluetoothEnabled = value;
                OnPropertyChanged("BluetoothEnabled");
                OnPropertyChanged("BluetoothDisabled");
            }
        }

        public Boolean BluetoothDisabled
        {
            get { return !_bluetoothEnabled; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ScanBeaconPageVM()
        {
            _beacons = new ObservableCollection<BeaconEntity>();
            _beacons.CollectionChanged += _beacons_CollectionChanged;
        }

        private void _beacons_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _bluetoothEnabled = false;
            CountBeacons = _beacons == null ? 0 : _beacons.Count;
            OnPropertyChanged("CountBeacons");
            OnPropertyChanged("Beacons");
        }
    }
}
