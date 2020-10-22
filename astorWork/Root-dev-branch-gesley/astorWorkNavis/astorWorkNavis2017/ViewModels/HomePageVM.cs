using astorWorkNavis2017.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkNavis2017.ViewModels
{
    public class HomePageVM : INotifyPropertyChanged
    {
        public string UserName { get; set; }
        public List<Project> Projects { get; set; }

        private Project _selectedProject;
        public Project SelectedProject
        {
            get
            {
                return _selectedProject;
            }
            set
            {
                _selectedProject = value;
                if (_selectedProject != null)
                {
                    Properties.Settings.Default.PROJECT_ID = _selectedProject.ID;
                    CanProceed = true;
                }
                else
                {
                    CanProceed = false;
                }
            }
        }

        private bool _canProceed;
        public bool CanProceed
        {
            get
            {
                return _canProceed;
            }
            set
            {
                _canProceed = value;
                RaisePropertyChanged("CanProceed");
            }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public HomePageVM()
        {
        }
    }
}
