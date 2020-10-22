using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class VendorInventoryVM : MasterVM
    {
        private List<Project> _projects;
        public List<Project> Projects {
            get
            {
                return _projects;
            }
            set
            {
                _projects = value;

                OnPropertyChanged("Projects");
            }
        }

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

                ScanMenuVisible = false;

                OnPropertyChanged("SelectedProject");

                if (_selectedProject != null)
                {
                    IsLoading = true;
                    Task.Run(GetInventory);
                }
                else
                {
                    Inventories = null;
                    OnPropertyChanged("ShowScanButton");
                }
            }
        }

        public bool ShowScanButton
        {
            get
            {
                return _selectedProject != null && !IsLoading;
            }
        }

        private List<Inventory> _inventories;
        public List<Inventory> Inventories
        {
            get
            {
                return _inventories;
            }
            set
            {
                _inventories = value;

                OnPropertyChanged("Inventories");
            }
        }

        public async Task GetProjects()
        {
            try
            {
                var result = await ApiClient.Instance.MTGetProjects();
                if (result.status == 0)
                {
                    Projects = result.data as List<Project>;
                }
                else
                {
                    ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            IsLoading = false;
        }

        public async Task GetInventory()
        {
            try
            {
                var result1 = await ApiClient.Instance.MTGetProjectDetails(_selectedProject.id);

                if (result1.status == 0)
                {
                    _selectedProject = result1.data as Project;
                    int vendorId = int.Parse(Application.Current.Properties["vendor_id"].ToString());
                    var result = await ApiClient.Instance.MTGetInventory(_selectedProject.id, vendorId);
                    if (result.status == 0)
                    {
                        Inventories = result.data as List<Inventory>;
                    }
                    else
                    {
                        ErrorMessage = result.message;
                    }
                }
                else
                {
                    ErrorMessage = result1.message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            IsLoading = false;
            OnPropertyChanged("ShowScanButton");
        }

        private bool _scanMenuVisible;
        public bool ScanMenuVisible { get
            {
                return _scanMenuVisible;
            }
            set
            {
                _scanMenuVisible = value;
                OnPropertyChanged("ScanMenuVisible");
            }
        }

        public VendorInventoryVM() : base()
        {
            Inventories = new List<Inventory>();
            SelectedProject = null;
        }

        public override void Reset()
        {
            base.Reset();
            SelectedProject = null;
        }
    }
}
