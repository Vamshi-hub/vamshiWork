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
    public class EnrollPageVM : INotifyPropertyChanged
    {
        private List<EnrollTaskEntity> _pendingTasks;
        public List<EnrollTaskEntity> PendingTasks
        {
            get { return _pendingTasks; }
            set
            {
                _pendingTasks = value;
                OnPropertyChanged("PendingTasks");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public EnrollPageVM()
        {
            PendingTasks = new List<EnrollTaskEntity>();
        }
    }
}
