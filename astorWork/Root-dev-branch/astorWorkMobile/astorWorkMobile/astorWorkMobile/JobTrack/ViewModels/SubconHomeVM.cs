using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.JobTrack.ViewModels
{
    public class SubconHomeVM : MasterVM
    {
        int ProjectID = -1;
        public string QRScanSource { get; set; }
        private List<Project> _projects { get; set; }
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
                _projects = value;
            }
        }

        private bool _showScanButton;
        public bool ShowScanButton
        {
            get
            {
                if (JobNotifications != null && JobNotifications.Count > 0)
                {
                    ShowScanButton = true;
                }
                else
                {
                    ShowScanButton = false;
                }
                return _showScanButton;
            }
            set
            {
                _showScanButton = value;
            }
        }
        public bool IsJobSummaryPage { get; set; }

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
                    _selectedProject = value;
                    ProjectID = _selectedProject.id;
                    Task.Run(GetNotifications);
                }
                OnPropertyChanged("SelectedProject");
            }
        }
        private Project _selectedJobSummaryProj { get; set; }
        public Project SelectedJobSummaryProj
        {
            get
            {
                return _selectedJobSummaryProj;
            }
            set
            {
                if (value != null && _selectedJobSummaryProj != value)
                {
                    _selectedJobSummaryProj = value;
                    ProjectID = _selectedJobSummaryProj.id;
                    Task.Run(GetNotifications);
                }
                OnPropertyChanged("SelectedJobSummaryProj");
            }
        }


        private Project _selectedQCNotifyProj { get; set; }
        public Project SelectedQCNotifyProj
        {
            get
            {
                return _selectedQCNotifyProj;
            }
            set
            {
                if (value != null && _selectedQCNotifyProj != value)
                {
                    _selectedQCNotifyProj = value;
                    ProjectID = _selectedQCNotifyProj.id;
                    Task.Run(GetNotifications);
                }
                OnPropertyChanged("SelectedQCNotifyProj");
            }
        }

        public List<JobScheduleDetails> QCNotifications { get; set; }
        public List<JobScheduleDetails> JobNotifications { get; set; }
        public bool HasQCNotification { get; set; }
        public async Task<bool> GetProjects()
        {

            IsLoading = true;
            bool success = false;
            try
            {
                JobNotifications = new List<JobScheduleDetails>();
                QCNotifications = new List<JobScheduleDetails>();
                var result = await ApiClient.Instance.MTGetProjects();
                if (result.status == 0)
                {
                    Projects = result.data as List<Project>;
                    if (Projects != null && Projects.Count > 0)
                    {
                        success = true;
                    }
                    else
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
            IsLoading = false;
            OnPropertyChanged("Projects");
            return success;

        }

        public async Task GetNotifications()
        {
            JobNotifications = new List<JobScheduleDetails>();
            QCNotifications = new List<JobScheduleDetails>();
            HasQCNotification = false;
            try
            {
                if (ProjectID == -1)
                {
                    return;
                }
                IsLoading = true;
                int SubConId = int.Parse(Application.Current.Properties["organisationID"].ToString());
                var result = await ApiClient.Instance.JTGetJobScheduleBySubCon(ProjectID, SubConId, null);
                if (result.status == 0)
                {
                    var lstJobSchedule = result.data as List<JobScheduleDetails>;
                    JobNotifications = lstJobSchedule.Where(js => js.StatusCode != Enums.JobStatus.QC_failed_by_Maincon).OrderBy(p => p.StatusCode).ToList();
                    QCNotifications = lstJobSchedule.Where(js => js.StatusCode == Enums.JobStatus.QC_failed_by_Maincon).ToList();
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
            OnPropertyChanged("ShowScanButton");
            OnPropertyChanged("QCNotifications");
            OnPropertyChanged("JobNotifications");
            OnPropertyChanged("SelectedProject");
            IsLoading = false;
        }

        public ICommand AssociateQRCommand { get; set; }

        public ICommand ScanRFIDFABCommand { get; set; }
        public ICommand ScanQRFABCommand { get; set; }
        async void AssociateQRClicked()
        {
            ViewModelLocator.scanTrackerVM.Reset();
            await Task.Run(GetCameraPermission)
                .ContinueWith(async (t) =>
               {
                   ViewModelLocator.jobScanVM.CameraReady = t.Result;
                   if (t.Result)
                   {
                       await Navigation.PushAsync(new JobScanQR());
                   }
                   else
                   {
                       DisplaySnackBar("No camera available", Shared.Classes.Enums.PageActions.None, Shared.Classes.Enums.MessageActions.Warning, null, null);
                   }
               }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        void ScanRFIDFABClicked()
        {
            //    Navigation.PushAsync(new ScanRFID());
        }
        async void ScanQRFABClicked()
        {
            await Task.Run(GetCameraPermission)
                .ContinueWith(async (t) =>
                {
                    ViewModelLocator.scanTrackerVM.CameraReady = t.Result;
                    if (t.Result)
                    {
                        await Navigation.PushAsync(new JobScanQR());
                    }
                    else
                    {
                        ViewModelLocator.scanTrackerVM.DisplaySnackBar("No camera available", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public SubconHomeVM() : base()
        {
            AssociateQRCommand = new Command(AssociateQRClicked);
            ScanRFIDFABCommand = new Command(ScanRFIDFABClicked);
            ScanQRFABCommand = new Command(ScanQRFABClicked);
            QCNotifications = new List<JobScheduleDetails>();
            JobNotifications = new List<JobScheduleDetails>();

            var plannedDate = DateTime.Today;

            //foreach (var ppvc in DummyPPVCs)
            //{
            //    /*
            //    DummyJobs.Add(new PPVCJob
            //    {
            //        JobName = "Plastering",
            //        Start = plannedDate,
            //        PPVC = ppvc,
            //        JobStatus = 2
            //    });
            //    DummyJobs.Add(new PPVCJob
            //    {
            //        JobName = "Waterproofing",
            //        Start = plannedDate,
            //        PPVC = ppvc,
            //        JobStatus = 2
            //    });
            //    DummyJobs.Add(new PPVCJob
            //    {
            //        JobName = "Floor Screeeding",
            //        Start = plannedDate,
            //        PPVC = ppvc,
            //        JobStatus = 1
            //    });
            //    */
            //    if (ppvc.id % 3 ==0)
            //    {
            //        DummyJobs.Add(new PPVCJob
            //        {
            //            JobName = "Window Frame Installation",
            //            Start = plannedDate.AddDays(1),
            //            PPVC = ppvc,
            //            JobStatus = 4
            //        });
            //    }
            //    else
            //    {
            //        DummyJobs.Add(new PPVCJob
            //        {
            //            JobName = "Window Frame Installation",
            //            Start = plannedDate,
            //            PPVC = ppvc,
            //            JobStatus = 2
            //        });
            //        if (ppvc.materialType.StartsWith("PPVC-Bedroom"))
            //        {
            //            DummyJobs.Add(new PPVCJob
            //            {
            //                JobName = "Wardrobe Installation",
            //                Start = plannedDate,
            //                PPVC = ppvc,
            //                JobStatus = 0
            //            });
            //        }
            //        if (!ppvc.materialType.StartsWith("PPVC-Kitchen"))
            //        {
            //            DummyJobs.Add(new PPVCJob
            //            {
            //                JobName = "Door Frame Installation",
            //                Start = plannedDate,
            //                PPVC = ppvc,
            //                JobStatus = 0
            //            });
            //        }
            //        if (ppvc.materialType.EndsWith("PBU"))
            //        {
            //            DummyJobs.Add(new PPVCJob
            //            {
            //                JobName = "Sanitary Fixtures Installation",
            //                Start = plannedDate,
            //                PPVC = ppvc,
            //                JobStatus = 3
            //            });
            //        }
            //    }
            //}
        }
    }
}
