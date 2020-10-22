using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Utilities.ApiClient;

namespace astorWorkMobile.JobTrack.ViewModels
{
    public class RTOHomeVM : MasterVM
    {
        public List<JobScheduleDetails> _rtoJobSchedules { get; set; }
        public List<JobScheduleDetails> RTOJobSchedules { get => _rtoJobSchedules; set { _rtoJobSchedules = value; OnPropertyChanged("RTOJobSchedules"); } }
        private List<JobMaster> _listMaterialJobSchedule { get; set; }


        public List<JobMaster> ListMaterialJobSchedule
        {
            get => _listMaterialJobSchedule;
            set
            {
                _listMaterialJobSchedule = value;
                OldJob = null;
                OnPropertyChanged("ListMaterialJobSchedule");
            }
        }

        public string ModuleName
        {
            get
            {
                if (_listMaterialJobSchedule != null && _listMaterialJobSchedule.Count > 0)
                {
                    var ppvc = _listMaterialJobSchedule.First().PPVC;
                    return $"{ppvc.block}-L{ppvc.level}-{ppvc.zone}-{ppvc.markingNo}";
                }
                else
                {
                    return "N.A.";
                }
            }
        }

        public ICommand ScanRFIDFABCommand { get; set; }
        public ICommand ScanQRFABCommand { get; set; }

        private void ScanRFIDFABClicked()
        {
            //  Navigation.PushAsync(new ScanRFID());
        }

        private async void ScanQRFABClicked()
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
        public void QCButtonClicked()
        {
            var Job = new PPVCJob();
            Job.JobName = OldJob.JobName;
            Job.JobStatus = OldJob.Status;
            Job.PPVC = OldJob.PPVC;
            Job.Start = OldJob.Start;
            ViewModelLocator.siteListDefectsVM.Job = Job;
            //Navigation.PushAsync(new JobQCCase());
        }

        private JobMaster OldJob { get; set; }

        internal void ShowHideMaterial(JobMaster objJobSchedule)
        {
            if (OldJob == objJobSchedule)
            {
                //click twice in the same item will hide it
                objJobSchedule.IsExpanded = !objJobSchedule.IsExpanded;
                UpdateMaterialVisible(objJobSchedule);
            }
            else
            {
                if (OldJob != null && OldJob.JobID != 0)
                {
                    //Hide previous selected Item
                    OldJob.IsExpanded = false;
                    UpdateMaterialVisible(OldJob);
                }
                //expand selected item
                objJobSchedule.IsExpanded = true;
                UpdateMaterialVisible(objJobSchedule);
            }
            OldJob = objJobSchedule;

        }

        private void UpdateMaterialVisible(JobMaster objJobSchedule)
        {
            int index = ListMaterialJobSchedule.IndexOf(objJobSchedule);
            ListMaterialJobSchedule.Remove(objJobSchedule);
            ListMaterialJobSchedule.Insert(index, objJobSchedule);
        }

        public async void JTGetJobScheduleForRTO(int ProjectId)
        {
            IsLoading = true;
            ApiResult result = await ApiClient.Instance.JTGetJobScheduleForRTO(ProjectId);
            if (result.status == 0)
            {
                List<JobScheduleDetails> jobs = (result.data as List<JobScheduleDetails>);
                List<JobScheduleDetails> lstjobs = new List<JobScheduleDetails>();
                foreach (JobScheduleDetails job in jobs)
                    if (lstjobs.Count == 0 || lstjobs.Where(j => j.ID == job.ID).Count() == 0)
                        lstjobs.Add(job);

                RTOJobSchedules = new List<JobScheduleDetails>();
                RTOJobSchedules = lstjobs.Where(p => p.StatusCode >= Enums.JobStatus.QC_routed_to_RTO).OrderBy(p => p.StatusCode).ToList();
                //RTOJobSchedules = lstjobs.Where(p => p.StatusCode >= Enums.JobStatus.QC_Routed && (Application.Current.Properties.ContainsKey("SelectedProject") && ((Project)Application.Current.Properties["SelectedProject"]).id == p.ProjectID)).OrderBy(p => p.StatusCode).ToList();
                if (RTOJobSchedules == null || RTOJobSchedules.Count == 0)
                {
                    ErrorMessage = "No Jobs found";
                }
            }
            else
                ErrorMessage = result.message;

            IsLoading = false;
        }

        private void ToggleSectionPopup()
        {
            //IsSectionPopupVisible = !IsSectionPopupVisible;
            //for (int i = 0; i < 10; i++)
            //{
            //    _jobSectionList.Add(new JobSections() { SectionName = "Section" + i });
            //}
            //JobSectionList = _jobSectionList;
        }
        public RTOHomeVM() : base()
        {
            RTOJobSchedules = new List<JobScheduleDetails>();
            ListMaterialJobSchedule = new List<JobMaster>();
            ScanRFIDFABCommand = new Command(ScanRFIDFABClicked);
            ScanQRFABCommand = new Command(ScanQRFABClicked);
            OldJob = new JobMaster();
            //JTGetJobScheduleForRTO(5);
            //Notifications = new ObservableCollection<SubconNotificationVM>();
            //var plannedDate = DateTime.Today;
        }
    }
    //public class JobSections : MasterVM
    //{
    //    private string _sectionName { get; set; }
    //    public string SectionName { get => _sectionName; set { _sectionName = value; OnPropertyChanged("SectionName"); } }
    //    private string _sectionID { get; set; }
    //    public string SectionID { get => _sectionID; set { _sectionID = value; OnPropertyChanged("SectionID"); } }
    //    private string _tradeID { get; set; }
    //    public string TradeID { get => _tradeID; set { _tradeID = value; OnPropertyChanged("TradeID"); } }
    //}
}