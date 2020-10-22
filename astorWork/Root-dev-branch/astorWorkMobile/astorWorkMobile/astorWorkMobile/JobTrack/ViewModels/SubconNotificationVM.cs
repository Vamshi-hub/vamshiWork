using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.JobTrack.ViewModels
{
    public class SubconNotificationVM : MasterVM
    {
        public PPVCJob Job { get; set; }
        public PPVCQC QC { get; set; }

        public bool ShowQCButton => Job != null && Job.JobStatus == 3;

        public bool ShowStartJobButton => Job != null && Job.JobStatus == 0;

        public bool ShowCompleteJobButton => Job != null && Job.JobStatus == 1;

        public bool ShowHardStop => Job != null && Job.JobStatus == 4;

        public string StatusText
        {
            get
            {
                var text = string.Empty;
                if (Job != null)
                {
                    switch (Job.JobStatus)
                    {
                        case 0:
                            text = "Not Started";
                            break;
                        case 1:
                            text = "Started";
                            break;
                        case 2:
                            text = "Completed";
                            break;
                        case 3:
                            text = "QC Open";
                            break;
                        case 4:
                            text = "Cannot start job because previous job QC failed";
                            break;
                        default:
                            break;
                    }
                }
                return text;
            }
        }

        public Color StatusColor
        {
            get
            {
                var color = Color.Black;
                if (Job != null)
                {
                    switch (Job.JobStatus)
                    {
                        case 0:
                            color = Color.FromHex("#455A64"); // Blue gray
                            break;
                        case 1:
                            color = Color.FromHex("#3F51B5"); // Indigo
                            break;
                        case 2:
                            color = Color.FromHex("#009688"); // Teal
                            break;
                        case 3:
                            color = Color.FromHex("#FF5722"); // Deep organge
                            break;
                        case 4:
                            color = Color.FromHex("#FF3D00"); // Depp orange A400
                            break;
                        default:
                            break;
                    }
                }
                return color;
            }
        }

        public ICommand QCCommand { get; set; }

        private void QCButtonClicked()
        {
            ViewModelLocator.siteListDefectsVM.Job = Job;
            ViewModelLocator.siteListDefectsVM.OpenCase = new MaterialTrack.Entities.QCCase
            {
                CaseName = Job.ModuleName + "-" + Job.JobName + "-2019-001",
                CountOpenDefects = 2,
                CreatedBy = "Susan",
                CreatedDate = DateTime.Today.AddDays(-5),
                ListQCDefect = new ObservableCollection<QCDefect>
                {
                    new QCDefect
                    {
                        ID = 0,
                        IsOpen = false,
                        CreatedBy = "Susan",
                        CreatedDate = DateTime.Today.AddDays(-5),
                        UpdatedBy = "Oscar",
                        UpdatedDate = DateTime.Today.AddDays(-3),
                        CountPhotos = 2,
                        Remarks = "Sink in wrong angle, fixed"
                    },new QCDefect
                    {
                        ID = 0,
                        IsOpen = true,
                        CreatedBy = "Susan",
                        CreatedDate = DateTime.Today.AddDays(-2),
                        CountPhotos = 1,
                        Remarks = "Sink leaking water"
                    }
                }
            };
            Navigation.PushAsync(new JobQCCase());
        }

        public ICommand StartJobCommand { get; set; }

        private void StartJobClicked()
        {
            Job.JobStatus = 1;
            StatusChanged();
            //Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].DisplayAlert("Success", $"You have started {Job.JobName} for {Job.ModuleName}", "OK");
            DisplaySnackBar($"You have started {Job.JobName} for {Job.ModuleName}", Enums.PageActions.None, Enums.MessageActions.Success, null, null);
        }

        public ICommand CompleteJobCommand { get; set; }

        private void CompleteJobClicked()
        {
            var htmlTemplate = @"<p>Hi,</p>
<p>Module: <strong>[MODULE_NAME]</strong><br>
Job: <strong>[JOB_NAME]</strong><br>This job has been completed by <strong>[SUBCON_USER]</strong> at <strong>[TIMESTAMP]</strong>.</p>
<p>Regards,<br>astorWork Team</p>";
            var bodyHtml = htmlTemplate.Replace("[MODULE_NAME]", Job.ModuleName)
                .Replace("[JOB_NAME]", Job.JobName)
                .Replace("[SUBCON_USER]", "Michelangelo")
                .Replace("[TIMESTAMP]", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));

            Task.Run(() => EmailClient.Instance.SendSingleAsync("benjamin.chua@astoriasolutions.com", "Benjamin", "Job completed", bodyHtml));

            Job.JobStatus = 2;
            StatusChanged();

            // Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].DisplayAlert("Success", $"You have completed {Job.JobName} for {Job.ModuleName}. We have notified main-con.", "OK");
            DisplaySnackBar($"You have completed {Job.JobName} for {Job.ModuleName}. We have notified main-con", Enums.PageActions.None, Enums.MessageActions.Success, null, null);
        }

        private void StatusChanged()
        {
            OnPropertyChanged("StatusText");
            OnPropertyChanged("StatusColor");
            OnPropertyChanged("ShowStartJobButton");
            OnPropertyChanged("ShowCompleteJobButton");
        }

        public SubconNotificationVM() : base()
        {
            QCCommand = new Command(QCButtonClicked);
            StartJobCommand = new Command(StartJobClicked);
            CompleteJobCommand = new Command(CompleteJobClicked);
        }
    }
}
