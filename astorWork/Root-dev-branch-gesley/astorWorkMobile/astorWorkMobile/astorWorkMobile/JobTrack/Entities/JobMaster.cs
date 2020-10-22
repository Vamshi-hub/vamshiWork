using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.Shared.Classes
{
    public class JobMaster : MasterVM
    {
        public Material PPVC { get; set; }
        public int JobID { get; set; }
        public string JobName { get; set; }
        public string SubCon { get; set; }
        private DateTime _start{ get; set; }
        public DateTime Start
        {
            get
            {
                if (_start == null)
                    _start = DateTime.Today;

                return _start;
            }
            set
            {
                if (value != null)
                    _start = value;
                OnPropertyChanged("Start");
            }
        }
        public DateTime _end { get; set; }
        public DateTime End
        {
            get
            {
                if (_end == null)
                    _end = DateTime.Today;
                return _end;
            }
            set
            {
                if (value != null)
                    _end = value;
                OnPropertyChanged("End");
            }
        }
        public DateTime ActualStartDate { get; set; }
        public DateTime ActualEndDate { get; set; }

        private int _status;
        public int Status { get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
                OnPropertyChanged("JobStatus");
                OnPropertyChanged("StatusColor");
            }
        }

        public string CompletionPercentage
        {
            get; set;
        }

        public string JobStatus
        {
            get
            {
                var text = string.Empty;
                switch (Status)
                {
                    case -1:
                        text = "Un-assigned";
                        break;
                    case 0:
                        if (Start <= DateTime.Today)
                        {
                            text = "Delayed";
                        }
                        else
                        {
                            text = "Not Started";
                        }
                        break;
                    case 1:
                        text = "Started";
                        break;
                    case 2:
                        text = "Completed";
                        break;
                    case 3:
                        text = "QC Failed";
                        break;
                    case 4:
                        text = "Previous Job QC Open";
                        break;
                    case 5:
                        text = "QC Passed";
                        break;
                    case 6:
                        text = "QC Accepted";
                        break;
                    case 7:
                        text = "QC Rejected";
                        break;
                    case 8:
                        text = "QC Rectified";
                        break;
                    default:
                        break;
                }
                return text;
            }
        }
        public string ModuleName
        {
            get
            {
                return $"{PPVC.block}-L{PPVC.level}-{PPVC.zone}-{PPVC.markingNo}";
            }
        }

        public string Description
        {
            get
            {
                if (PPVC != null)
                    return $"{PPVC.block}-L{PPVC.level}-{PPVC.zone}-{PPVC.markingNo}-{JobName}";
                else
                    return string.Empty;
            }
        }

        public Color StatusColor
        {
            get
            {
                Color result = Color.Gray;
                switch (_status)
                {
                    case 0:
                        if (Start <= DateTime.Today)
                        {
                            result = Color.FromHex("#FF4500");
                        }
                        else
                        {
                            result = Color.FromHex("#455A64");
                        }
                        break;
                    case 1:
                        result = Color.LightBlue;
                        break;
                    case 2:
                        result = Color.Blue;
                        break;
                    case 3:
                        result = Color.FromHex("#FF5722");
                        break;
                    case 4:
                        result = Color.FromHex("#FF5722");
                        break;
                    case 5:
                        result = Color.FromHex("#009688");
                        break;
                    case 7:
                        result = Color.FromHex("#FF5722");
                        break;
                    case 6:
                        result = Color.FromHex("#009688");
                        break;
                    default:
                        break;
                }

                return result;
            }
        }

        public DateTime Today
        {
            get
            {
                return DateTime.Today.Date;
            }
        }
        public bool ShowChecklistButton
        {
            get
            {
                int myId = 0;
                if (Application.Current.Properties.ContainsKey("user_id"))
                    myId = (int)Application.Current.Properties["user_id"];

                return Status == 2 || (Status == 5 && myId == 3) || Status == 7 || Status == 3 || Status == 8;
            }
        }

        public bool ShowAssignButton
        {
            get
            {
                return Status == -1;
            }
        }

        public bool ShowPendingData
        {
            get
            {
                return Status != -1;
            }
        }

        public bool ShowStartDate
        {
            get
            {
                return Status != -1 && Status != 0;
            }
        }

        public bool IsDelayed
        {
            get
            {
                return Status == 0 && Start <= DateTime.Today;
            }
        }

        public bool IsExpanded { get; set; }

        public string ExpansionIcon
        {
            get
            {
                if (IsExpanded)
                {
                    return "ic_keyboard_arrow_up.png";
                }
                else
                {
                    return "ic_keyboard_arrow_down.png";
                }
            }
        }

        public List<string> DummySubcons
        {
            get
            {
                List<string> v = new List<string> { "WaterPro Pte Ltd", "EZ Flooring Inc", "Install Express Corp" };
                return v;
            }
        }
        public string SelectedSubcon
        {
            get
            {
                return SubCon;
            }
            set
            {
                if (value != null)
                {
                    SubCon = value;
                    OnPropertyChanged("SubCon");
                }
            }
        }

        public int FrameHeight
        {
            get
            {
                if (IsExpanded)
                {
                    if (Status == -1 || Status == 2)
                        return 350;
                    else
                        return 260;
                }
                else
                    return (5 <= Status && Status <= 7)?100:70;
            }
        }

        public ICommand ChecklistCommand { get; set; }
        void ChecklistClicked()
        {
            //ViewModelLocator.jobChecklistVM.Job = this;
            Navigation.PushAsync(new JobChecklist());
        }

        public ICommand ExpandCommand { get; set; }
        void ExpandClicked()
        {
            IsExpanded = !IsExpanded;
            OnPropertyChanged("IsExpanded");
            OnPropertyChanged("ExpansionIcon");
            OnPropertyChanged("FrameHeight");
        }

        public ICommand AssignJobCommand { get; set; }
        void AssignJobClicked()
        {
            Status = 0;
            OnPropertyChanged("Status");
            OnPropertyChanged("JobStatus");
            OnPropertyChanged("StatusColor");
            OnPropertyChanged("ShowPendingData");
            OnPropertyChanged("ShowAssignButton");
            OnPropertyChanged("ShowStartDate");
            OnPropertyChanged("Start");
            OnPropertyChanged("End");


            var htmlTemplate = @"<p>Hi,</p>
<p>Module: <strong>[MODULE_NAME]</strong><br>
Job: <strong>[JOB_NAME]</strong><br>
Planned Start Date: <strong>[START]</strong><br>
Planned End Date: <strong>[END]</strong></p>
<p>
This job has been assigned to <strong>[SUBCON_NAME]</strong>
</p>
<p>Regards,<br>astorWork Team</p>";
            var bodyHtml = htmlTemplate.Replace("[MODULE_NAME]", ModuleName)
                .Replace("[JOB_NAME]", JobName)
                .Replace("[SUBCON_NAME]", SelectedSubcon)
                .Replace("[START]", Start.ToString("MM/dd/yyyy"))
                .Replace("[END]", End.ToString("MM/dd/yyyy"));

            Task.Run(() => EmailClient.Instance.SendSingleAsync("brian.yang@astoriasolutions.com", "Brian Yang", "Job assigned", bodyHtml));
        }

        public ObservableCollection<ChatMessage> Messages { get; set; }

        public JobMaster()
        {
            Messages = new ObservableCollection<ChatMessage>();

            AssignJobCommand = new Command(AssignJobClicked);
            ExpandCommand = new Command(ExpandClicked);
            ChecklistCommand = new Command(ChecklistClicked);

            ChatClient.Instance.PropertyChanged += ChatClientPropertyChanged;
        }

        private void ChatClientPropertyChanged(object sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ReceivedMessage")
            {
                //if (ChatClient.Instance.Message != null &&
                //    ChatClient.Instance.Message.Header == Description)
                //{
                //    Messages.Add(ChatClient.Instance.Message);
                //}
            }
        }
    }
}
