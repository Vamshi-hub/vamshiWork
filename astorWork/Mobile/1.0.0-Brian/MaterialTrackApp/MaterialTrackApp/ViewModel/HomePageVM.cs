using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MaterialTrackApp.ViewModel
{
    public class HomePageVM : INotifyPropertyChanged
    {
        private bool _canManualUpdate;
        public Boolean CanManualUpdate
        {
            get { return _canManualUpdate; }
            set
            {
                _canManualUpdate = value;
                OnPropertyChanged("CanManualUpdate");
            }
        }

        private int _countPendingTask;
        public int CountPendingTask
        {
            get { return _countPendingTask; }
            set
            {
                _countPendingTask = value;
                OnPropertyChanged("CountPendingTask");
                OnPropertyChanged("PendingTaskButtonText");
                OnPropertyChanged("PendingTaskButtonVisible");
            }
        }

        public string PendingTaskButtonText
        {
            get { return string.Format("{0} unsynced tasks", _countPendingTask); }

        }
        public Boolean PendingTaskButtonVisible
        {
            get { return _countPendingTask > 0; }
        }

        private int _countPendingEnroll;
        public int CountPendingEnroll
        {
            get { return _countPendingEnroll; }
            set
            {
                _countPendingEnroll = value;
                OnPropertyChanged("CountPendingEnroll");
            }
        }

        private int _countPendingQC;
        public int CountPendingQC
        {
            get { return _countPendingQC; }
            set
            {
                _countPendingQC = value;
                OnPropertyChanged("CountPendingQC");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public HomePageVM()
        {
            _canManualUpdate = false;
        }
    }
}
