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
    public class BeaconInfoPageVM : INotifyPropertyChanged
    {
        public List<BeaconEntity> ListBeacons { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public BeaconInfoPageVM()
        {            
            ListBeacons = new List<BeaconEntity>();
        }
    }
}
