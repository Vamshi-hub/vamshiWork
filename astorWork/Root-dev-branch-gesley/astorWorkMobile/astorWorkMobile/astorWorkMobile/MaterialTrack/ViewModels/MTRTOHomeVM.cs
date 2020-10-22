using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using astorWorkMobile.Shared.Utilities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.JobTrack.Pages;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class MTRTOHomeVM : MasterVM
    {
        int ProjectID = -1;
        public List<Project> _projects { get; set; }
        public List<Project> Projects
        {
            get
            {
                if (_projects != null && _projects.Count == 1)
                {
                    SelectedProject = _projects[0];
                }
                return _projects;
            }
            set
            {
                if (value != null)
                {
                    _projects = value;
                }
            }
        }
        private bool _showHideQCStatus { get; set; }
        public bool ShowHideQCStatus { get { return _showHideQCStatus; } set { _showHideQCStatus = value; } }
        private bool _IsQCCompleted { get; set; }
        public bool IsQCCompleted
        {
            get { return _IsQCCompleted; }
            set
            {
                _IsQCCompleted = value;
            }
        }
        private Project _selectedProject { get; set; }
        public Project SelectedProject
        {
            get
            {
                return _selectedProject;
            }
            set
            {
                if (value != null && _selectedProject != value)
                {
                    ShowHideQCStatus = false;
                    _selectedProject = value;
                    ProjectID = _selectedProject.id;
                    GetRTOMaterials();

                }
                OnPropertyChanged("HideShowQCStatus");
                OnPropertyChanged("RTOMaterials");
                OnPropertyChanged("SelectedProject");
            }
        }
        private Project _selectedProjQCCompltd { get; set; }
        public Project SelectedProjQCCompltd
        {
            get
            {
                return _selectedProjQCCompltd;
            }
            set
            {
                if (value != null && _selectedProjQCCompltd != value)
                {
                    ShowHideQCStatus = false;
                    _selectedProjQCCompltd = value;
                    ProjectID = SelectedProjQCCompltd.id;
                    GetRTOMaterials();

                }
                OnPropertyChanged("HideShowQCStatus");
                OnPropertyChanged("RTOMaterials");
            }
        }

        private Project _selectedProjQCPending { get; set; }
        public Project SelectedProjQCPending
        {
            get
            {
                return _selectedProjQCPending;
            }
            set
            {
                if (value != null && _selectedProjQCPending != value)
                {
                    ShowHideQCStatus = true;
                    _selectedProjQCPending = value;
                    ProjectID = SelectedProjQCPending.id;
                    GetRTOMaterials();
                }
                OnPropertyChanged("HideShowQCStatus");
                OnPropertyChanged("RTOMaterials");
            }
        }

        private List<MaterialFrameVM> _RTOMaterials { get; set; }
        public List<MaterialFrameVM> RTOMaterials
        {
            get { return _RTOMaterials; }
            set
            {
                if (value != null)
                {
                    _RTOMaterials = value;
                    if (_IsQCCompleted)
                    {
                        RTOQCCompletedMaterials = new List<MaterialFrameVM>();
                        RTOQCCompletedMaterials = RTOMaterials?.Where(mm => mm.Material.projectID == ProjectID && mm.Material.QCStatusCode == (int)Enums.JobStatus.All_QC_passed).ToList();
                        //RTOQCCompletedMaterials = RTOMaterials?.Where(mm => mm.Material.projectID == ProjectID && mm.Material.QCStatusCode == (int)Enums.JobStatus.QC_Completed).ToList();
                        if (RTOQCCompletedMaterials == null || RTOQCCompletedMaterials.Count == 0)
                        {
                            ErrorMessage = "No materials found";
                        }
                    }
                    else
                    {
                        RTOQCPendingMaterials = new List<MaterialFrameVM>();
                        RTOQCPendingMaterials = RTOMaterials?.Where(mm => mm.Material.projectID == ProjectID && mm.Material.QCStatusCode >= (int)Enums.JobStatus.QC_routed_to_RTO && mm.Material.QCStatusCode != (int)Enums.JobStatus.All_QC_passed).ToList();
                        //RTOQCPendingMaterials = RTOMaterials?.Where(mm => mm.Material.projectID == ProjectID && mm.Material.QCStatusCode >= (int)Enums.JobStatus.QC_Routed && mm.Material.QCStatusCode != (int)Enums.JobStatus.QC_Completed).ToList();
                        if (RTOQCPendingMaterials == null || RTOQCPendingMaterials.Count == 0)
                        {
                            ErrorMessage = "No materials found";
                        }
                    }
                }
            }
        }
        private List<MaterialFrameVM> _RTOQCCompletedMaterials { get; set; }
        public List<MaterialFrameVM> RTOQCCompletedMaterials
        {
            get { return _RTOQCCompletedMaterials; }
            set
            {
                _RTOQCCompletedMaterials = value;
            }
        }

        private List<MaterialFrameVM> _RTOQCPendingMaterials { get; set; }
        public List<MaterialFrameVM> RTOQCPendingMaterials
        {
            get { return _RTOQCPendingMaterials; }
            set
            {
                _RTOQCPendingMaterials = value;
            }
        }

        public ICommand ChecklistButtonCommand { get; set; }
        public ICommand ScanRFIDFABCommand { get; set; }
        public ICommand ScanQRFABCommand { get; set; }

        public async Task GetProjects()
        {
            IsLoading = true;
            try
            {
                //if (Application.Current.Properties.ContainsKey("SelectedProject") && Application.Current.Properties["SelectedProject"] != null)
                //{
                //    ProjectID = ((Project)Application.Current.Properties["SelectedProject"]).id;
                //    SelectedProject = ((Project)Application.Current.Properties["SelectedProject"]);
                //}
                var result = await ApiClient.Instance.MTGetProjects();
                if (result.status == 0)
                {
                    Projects = result.data as List<Project>;
                    if (Projects == null && Projects.Count == 0)
                    {
                        ErrorMessage = "Failed to get projects";
                    }
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
            OnPropertyChanged("Projects");
            IsLoading = false;
        }

        void ScanRFIDFABClicked()
        {
            // Navigation.PushAsync(new ScanRFID());
        }
        async void ScanQRFABClicked()
        {
            await Task.Run(GetCameraPermission)
                .ContinueWith(async (t) =>
                {
                    ViewModelLocator.scanTrackerVM.CameraReady = t.Result;
                    if (t.Result)
                    {
                        await Navigation.PushAsync(new ScanQRCode());
                    }
                    else
                    {
                        ViewModelLocator.scanTrackerVM.DisplaySnackBar("No camera available", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        public async Task GetRTOMaterials()
        {
            //api call to get list of materials associated to RTO for the Selected Project.
            IsLoading = true;
            try
            {
                if (ProjectID == -1)
                {
                    IsLoading = false;
                    return;
                }
                var result = await ApiClient.Instance.MTGetRTOMaterials(ProjectID);
                if (result.status == 0)
                {
                    RTOMaterials = result.data as List<MaterialFrameVM>;
                    if (RTOMaterials == null || RTOMaterials.Count() == 0)
                    {
                        ErrorMessage = "Failed to get materials";
                    }
                }
                else
                {
                    ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ErrorMessage = ex.Message;
            }
            OnPropertyChanged("SelectedProject");
            OnPropertyChanged("RTOMaterials");
            OnPropertyChanged("RTOQCCompletedMaterials");
            OnPropertyChanged("RTOQCPendingMaterials");
            IsLoading = false;
        }
        public void CheckListClicked(Material Material)
        {
            ViewModelLocator.jobChecklistVM.Material = Material;
            Navigation.PushAsync(new JobChecklist());
        }

        public MTRTOHomeVM()
        {
            ChecklistButtonCommand = new Command<Material>(CheckListClicked);
            ScanRFIDFABCommand = new Command(ScanRFIDFABClicked);
            ScanQRFABCommand = new Command(ScanQRFABClicked);
        }
    }
}
