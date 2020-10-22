using MaterialTrackApp.Class;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.ViewModel
{
    public class ReportPageViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ListViewGroup<string, DashboardInfo>> _groups;
        public ObservableCollection<ListViewGroup<string, DashboardInfo>> Groups
        {
            get { return _groups; }
            set
            {
                _groups = value;
                OnPropertyChanged("Groups");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ReportPageViewModel()
        {
            Groups = new ObservableCollection<ListViewGroup<string, DashboardInfo>>();
        }
    }
}
