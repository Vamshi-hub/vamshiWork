using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.JobTrack.ViewModels
{
    public class MainconHomeVM : MasterVM
    {
        private List<JobScheduleDetails> _jobs;

        public int CountUnassignedJobs { get; set; }
        public int CountOngoingJobs { get; set; }
        public int CountDelayedJobs { get; set; }
        public int CountQCPendingJobs { get; set; }
        public int CountQCCompletedJobs { get; set; }
        public int CountQCJobs { get; set; }

        public ICommand FigureCommand { get; set; }

        public ICommand ScanQRFABCommand { get; set; }

        private void FigureClicked(string status)
        {
            int count = 0;
            ViewModelLocator.jobScheduleVM.ClickedTileJobStatus = int.Parse(status);
            if (status == "0" && int.Parse(Application.Current.Properties["entry_point"].ToString()) == 4)
            {
                DisplaySnackBar("Sorry! you do not have access to this", PageActions.None, MessageActions.Warning, null, null);
                return;
            }
            if (status == "5")
            {
                count = _jobs.Where(j => j.StatusCode == JobStatus.Job_not_started || j.StatusCode == JobStatus.Job_started || j.StatusCode == JobStatus.Job_completed).Count();
            }
            else
            {
                count = _jobs.Where(j => status != "6" ? ((int)j.StatusCode).ToString() == status : j.StatusCode > JobStatus.Job_completed && j.StatusCode < JobStatus.All_QC_passed).Count();
            }
            if (count > 0)
            {
                IsLoading = true;
                ViewModelLocator.jobScheduleVM.Title = status != "6" ? ((JobStatus)Convert.ToInt32(status)).ToString().Replace('_', ' ') + " Jobs" : "Jobs in QC";
                if (Convert.ToInt32(status) <= 5)
                {
                    ViewModelLocator.jobScheduleVM.Title = ((JobStatus)Convert.ToInt32(status)).ToString().Replace('_', ' ');
                }
                if (status == "5")
                {
                    ViewModelLocator.jobScheduleVM.Title = "QC Pending Jobs";
                }
                else if (status == "4")
                {
                    ViewModelLocator.jobScheduleVM.Title = "Ongoing Jobs";
                }
                else if (status == "3")
                {
                    ViewModelLocator.jobScheduleVM.Title = "Delayed Jobs";
                }
                else if (status == "0")
                {
                    ViewModelLocator.jobScheduleVM.Title = "Un-assigned Jobs";
                }

                ApiClient.Instance.MTGetOrganisations().ContinueWith(t =>
                {
                    if (t.Result.status == 0)
                    {
                        if (status == "5")
                            ViewModelLocator.jobScheduleVM.ListMaterialJobSchedule =
                            _jobs.Where(j => j.StatusCode == JobStatus.Job_not_started || j.StatusCode == JobStatus.Job_started || j.StatusCode == JobStatus.Job_completed).OrderBy(j => j.StatusCode).ToList();
                        else
                            ViewModelLocator.jobScheduleVM.ListMaterialJobSchedule =
                                _jobs.Where(j => status != "6" ? ((int)j.StatusCode).ToString() == status : j.StatusCode > JobStatus.Job_completed && j.StatusCode < JobStatus.All_QC_passed).OrderBy(j => j.StatusCode).ToList();

                        ViewModelLocator.jobScheduleVM.ListSubcons = t.Result.data as List<Organisation>;
                        ViewModelLocator.jobScheduleVM.ModuleName = string.Empty;
                        ViewModelLocator.jobScheduleVM.ListSubcons = ViewModelLocator.jobScheduleVM.ListSubcons.Where(o => o.OrganisationType == OrganisationType.Subcon).ToList();
                        Navigation.PushAsync(new JobSchedule());
                        IsLoading = false;
                    }
                    else
                    {
                        ErrorMessage = t.Result.message;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                DisplaySnackBar("No jobs available", PageActions.None, MessageActions.Warning, null, null);
                return;
            }
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
        public void ContentAppearing()
        {
            IsLoading = true;
            ApiClient.Instance.JTGetJobSchedule(0).ContinueWith(t =>
            {
                if (t.Result.status == 0)
                {
                    _jobs = t.Result.data as List<JobScheduleDetails>;
                    if (_jobs != null)
                    {
                        CountUnassignedJobs = _jobs.Where(j => j.StatusCode == JobStatus.Job_not_assigned || j.StatusCode == JobStatus.Job_not_scheduled).Count();
                        OnPropertyChanged("CountUnassignedJobs");
                        CountOngoingJobs = _jobs.Where(j => j.StatusCode == JobStatus.Job_started).Count();
                        OnPropertyChanged("CountOngoingJobs");
                        CountDelayedJobs = _jobs.Where(j => j.StatusCode == JobStatus.Job_delayed).Count();
                        OnPropertyChanged("CountDelayedJobs");
                        CountQCPendingJobs = _jobs.Where(j => j.StatusCode == JobStatus.Job_not_started || j.StatusCode == JobStatus.Job_started || j.StatusCode == JobStatus.Job_completed).Count();
                        OnPropertyChanged("CountQCPendingJobs");
                        CountQCJobs = _jobs.Where(j => j.StatusCode > JobStatus.Job_completed && j.StatusCode < JobStatus.All_QC_passed).Count();
                        OnPropertyChanged("CountQCJobs");
                        CountQCCompletedJobs = _jobs.Where(j => j.StatusCode == JobStatus.All_QC_passed).Count();
                        OnPropertyChanged("CountQCCompletedJobs");
                    }
                    IsLoading = false;
                }
                else
                {
                    ErrorMessage = t.Result.message;
                }
            });
        }

        public MainconHomeVM() : base()
        {
            FigureCommand = new Command<string>(FigureClicked);
            ScanQRFABCommand = new Command(ScanQRFABClicked);
        }
    }
}
