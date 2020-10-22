using MaterialTrackApp.DB;
using MaterialTrackApp.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.ViewModel
{
    public class InstallPageVM : INotifyPropertyChanged
    {
        public List<InstallTaskEntity> PendingTasks { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public InstallPageVM()
        {
            PendingTasks = new List<InstallTaskEntity>();
        }
    }
}
