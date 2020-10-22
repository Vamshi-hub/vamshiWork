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

        private async void FigureClicked(string status)
        {
            int count = CountUnassignedJobs;
            ViewModelLocator.jobScheduleVM.ClickedTileJobStatus = int.Parse(status);
            if (status == "0" && int.Parse(Application.Current.Properties["entry_point"].ToString()) == 4)
            {
                DisplaySnackBar("Sorry! you do not have access to this", PageActions.None, MessageActions.Warning, null, null);
                return;
            }
            else if (status == "4")
            {
                count = CountOngoingJobs;
            }
            else if (status == "3")
            {
                count = CountDelayedJobs;
            }
            else if (status == "5")
            {
                count = CountQCPendingJobs;
            }
            else if (status == "6")
            {
                count = CountQCJobs;
            }
            else if (status == "12")
            {
                count = CountQCCompletedJobs;
            }
            if (count > 0)
            {
                //IsLoading = true;
                //ViewModelLocator.jobScheduleVM.Title = status != "6" ? ((JobStatus)Convert.ToInt32(status)).ToString().Replace('_', ' ') + " Jobs" : "Jobs in QC";
                //if (Convert.ToInt32(status) <= 5)
                //{
                //    ViewModelLocator.jobScheduleVM.Title = ((JobStatus)Convert.ToInt32(status)).ToString().Replace('_', ' ');
                //}
                //if (status == "5")
                //{
                //    ViewModelLocator.jobScheduleVM.Title = "QC Pending Jobs";
                //}
                //else if (status == "4")
                //{
                //    ViewModelLocator.jobScheduleVM.Title = "Ongoing Jobs";
                //}
                //else if (status == "3")
                //{
                //    ViewModelLocator.jobScheduleVM.Title = "Delayed Jobs";
                //}
                //else if (status == "0")
                //{
                //    ViewModelLocator.jobScheduleVM.Title = "Un-assigned Jobs";
                //}
                await Navigation.PushAsync(new JobSchedule(status));
                //ApiClient.Instance.MTGetOrganisations().ContinueWith(t =>
                //{
                //    if (t.Result.status == 0)
                //    {
                //        if (status == "5")
                //            ViewModelLocator.jobScheduleVM.ListMaterialJobSchedule =
                //            _jobs.Where(j => j.StatusCode == JobStatus.Job_not_started || j.StatusCode == JobStatus.Job_started || j.StatusCode == JobStatus.Job_completed).OrderBy(j => j.StatusCode).ToList();
                //        else
                //            ViewModelLocator.jobScheduleVM.ListMaterialJobSchedule =
                //                _jobs.Where(j => status != "6" ? ((int)j.StatusCode).ToString() == status : j.StatusCode > JobStatus.Job_completed && j.StatusCode < JobStatus.All_QC_passed).OrderBy(j => j.StatusCode).ToList();

                //        ViewModelLocator.jobScheduleVM.ListSubcons = t.Result.data as List<Organisation>;
                //        ViewModelLocator.jobScheduleVM.ModuleName = string.Empty;
                //        ViewModelLocator.jobScheduleVM.ListSubcons = ViewModelLocator.jobScheduleVM.ListSubcons.Where(o => o.OrganisationType == OrganisationType.Subcon).ToList();
                //        Navigation.PushAsync(new JobSchedule());
                //        IsLoading = false;
                //    }
                //    else
                //    {
                //        ErrorMessage = t.Result.message;
                //    }
                //}, TaskScheduler.FromCurrentSynchronizationContext());
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
            ApiClient.Instance.JTGetJobScheduleCount(0).ContinueWith(t =>
         {
             //_jobs = new List<JobScheduleDetails>();
             if (t.Result.status == 0)
             {
                 // _jobs = t.Result.data as JobScheduleCount;
                 //  _jobs = new List<JobScheduleDetails>(t.Result.data);
                 var counts = t.Result.data as JobScheduleCount;
                 if (counts != null)
                 {
                     CountUnassignedJobs = counts.UnassignedJobs;
                     OnPropertyChanged("CountUnassignedJobs");
                     CountOngoingJobs = counts.OngoingJobs;
                     OnPropertyChanged("CountOngoingJobs");
                     CountDelayedJobs = counts.DelayedJobs;
                     OnPropertyChanged("CountDelayedJobs");
                     CountQCPendingJobs = counts.QCPendingJobs;
                     OnPropertyChanged("CountQCPendingJobs");
                     CountQCJobs = counts.JobsinQC;
                     OnPropertyChanged("CountQCJobs");
                     CountQCCompletedJobs = counts.QCCompletedJobs;
                     OnPropertyChanged("CountQCCompletedJobs");
                 }
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
