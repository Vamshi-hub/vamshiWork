using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace astorWorkMobile.Shared.Classes
{
    public class LoginVM : MasterVM
    {
        private string _tenantName;
        public string TenantName
        {
            get
            {
                return _tenantName;
            }
            set
            {
                _tenantName = value;
                OnPropertyChanged("TenantName");
                OnPropertyChanged("LoginButtonEnabled");
            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                OnPropertyChanged("UserName");
                OnPropertyChanged("LoginButtonEnabled");
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
                OnPropertyChanged("LoginButtonEnabled");
            }
        }

        public bool LoginButtonEnabled
        {
            get
            {
                return
                    !string.IsNullOrEmpty(_tenantName) &&
                    !string.IsNullOrEmpty(_userName) &&
                    !string.IsNullOrEmpty(_password);
            }
        }
        public List<Project> _projects { get; set; }
        public List<Project> Projects
        {
            get { return _projects; }
            set
            {
                _projects = value;
                if (_projects != null && _projects.Count == 1)
                {
                    SelectedProject = _projects[0];
                }
                OnPropertyChanged("Projects");
            }
        }
        public Project _selectedProject { get; set; }
        public Project SelectedProject
        {
            get { return _selectedProject; }
            set
            {
                _selectedProject = value;
                OnPropertyChanged("SelectedProject");
            }
        }
        public async Task<List<Project>> GetProjects()
        {
            var result = await ApiClient.Instance.MTGetProjects();
            Projects = new List<Project>();
            if (result.status == 0)
            {
                Projects = result.data as List<Project>;
            }
            return Projects;
        }
        public override void Reset()
        {
            _neverLoadBefore = false;
            Password = string.Empty;
        }
    }
}
