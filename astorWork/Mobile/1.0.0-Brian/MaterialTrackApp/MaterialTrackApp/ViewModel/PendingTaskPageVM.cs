using MaterialTrackApp.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.ViewModel
{
    public class PendingTaskPageVM : INotifyPropertyChanged
    {
        private ObservableCollection<EnrollTaskEntity> _listEnrollTasks;
        public ObservableCollection<EnrollTaskEntity> ListEnrollTasks {
            get
            {
                return _listEnrollTasks;
            }
            set
            {
                _listEnrollTasks = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public PendingTaskPageVM()
        {
            _listEnrollTasks = new ObservableCollection<EnrollTaskEntity>();
        }
    }
}
