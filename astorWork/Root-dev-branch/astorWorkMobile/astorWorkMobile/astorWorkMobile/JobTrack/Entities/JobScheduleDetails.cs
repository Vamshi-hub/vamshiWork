using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.JobTrack.Entities
{
    public class JobScheduleDetails : MasterVM
    {
        public int ID { get; set; }
        public int MaterialID { get; set; }
        public string MarkingNo { get; set; }
        public string Zone { get; set; }
        public string Level { get; set; }
        public string Block { get; set; }
        public string MaterialType { get; set; }
        public int TradeID { get; set; }
        public string _tradeName { get; set; }
        public string TradeName { get { return _tradeName; } set { _tradeName = value; } }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }

        public int SubConID { get; set; }
        public string SubConName { get; set; }
        public DateTimeOffset? Start { get; set; }
        private DateTime _plannedSD = DateTime.Now;
        [JsonIgnore]
        public DateTime PlannedSD
        {
            get => _plannedSD;
            set
            {
                _plannedSD = value;
                OnPropertyChanged("PlannedSD");
            }
        }
        private DateTime _plannedED = DateTime.Now;
        [JsonIgnore]
        public DateTime PlannedED
        {
            get => _plannedED;
            set
            {
                _plannedED = value;
                OnPropertyChanged("PlannedED");
            }
        }
        public DateTimeOffset? End { get; set; }
        public DateTimeOffset? ActualStartDate { get; set; }
        public DateTimeOffset? ActualEndDate { get; set; }

        public JobStatus StatusCode { get; set; }
        private string _status { get; set; }
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                if (_status.Contains('_'))
                {
                    _status = _status.Replace('_', ' ');
                }
            }
        }
        public bool RouteToRTO { get; set; }
        [JsonIgnore]
        public Color StatusColor
        {
            get
            {
                Color result = Color.Gray;
                switch (StatusCode)
                {
                    case JobStatus.Job_delayed:
                        result = Color.FromHex("#FF7F50");
                        break;
                    case JobStatus.Job_not_started:
                        result = Color.FromHex("#3D3D3D");
                        break;
                    case JobStatus.Job_started:
                        result = Color.FromHex("#80C3D8");
                        break;
                    case JobStatus.Job_completed:
                        result = Color.FromHex("#00008B");
                        break;
                    case JobStatus.QC_failed_by_Maincon:
                        result = Color.FromHex("#F00028");
                        break;
                    case JobStatus.QC_passed_by_Maincon:
                        result = Color.FromHex("#286E67");
                        break;
                    case JobStatus.QC_routed_to_RTO:
                        result = Color.FromHex("#00BFFF");
                        break;
                    case JobStatus.QC_rejected_by_RTO:
                        result = Color.FromHex("#FF5722");
                        break;
                    case JobStatus.QC_accepted_by_RTO:
                        result = Color.FromHex("#286E67");
                        break;
                    case JobStatus.QC_rectified_by_Subcon:
                        result = Color.FromHex("#3F51B5");
                        break;
                    case JobStatus.All_QC_passed:
                        result = Color.FromHex("#D86D00");
                        break;
                    default:
                        break;
                }
                StatusChanged();
                return result;
            }
        }
        [JsonIgnore]
        public string ModuleName => $"{Block}-L{Level}-{Zone}-{MarkingNo}";

        [JsonIgnore]
        public int FrameHeight
        {
            get
            {
                if (IsExpanded)
                {
                    if (StatusCode == JobStatus.Job_not_assigned || StatusCode == JobStatus.Job_completed)
                    {
                        return 300;
                    }
                    else
                    {
                        return 250;
                    }
                }
                else
                {
                    return 50;
                }
            }
        }
        [JsonIgnore]
        public DateTime Today => DateTime.Today.Date;
        [JsonIgnore]
        public bool ShowSubcon => StatusCode != JobStatus.Job_not_assigned;
        [JsonIgnore]
        public bool ShowSubconSelection => StatusCode == JobStatus.Job_not_assigned;
        [JsonIgnore]
        public bool ShowAssignButton => StatusCode == JobStatus.Job_not_assigned && SelectedSubcon != null && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 4 && (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 5 || Convert.ToInt32(Application.Current.Properties["entry_point"]) == 1);
        [JsonIgnore]
        public bool ShowChecklistButton
        {
            get
            {
                bool status = false;
                //status = StatusCode >= JobStatus.Not_Started && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 1;
                if (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 3)
                {
                    if (StatusCode >= JobStatus.QC_routed_to_RTO)
                    {
                        status = true;
                    }
                }
                else if (StatusCode == JobStatus.QC_failed_by_Maincon && Convert.ToInt32(Application.Current.Properties["entry_point"]) == 2)
                {
                    status = true;
                }
                else if (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 4 && StatusCode >= JobStatus.Job_not_started)
                {
                    status = true;
                }
                else if (StatusCode >= JobStatus.Job_not_started && Convert.ToInt32(Application.Current.Properties["entry_point"]) == 5)
                {
                    status = true;
                }
                else
                    status = false;
                return status;
            }
        }
        [JsonIgnore]
        public bool ShowStartDate => Start != null && Start.HasValue;
        [JsonIgnore]
        public bool ShowEndDate => End != null && End.HasValue;
        [JsonIgnore]
        public bool ShowActualStartDate => ActualStartDate != null && ActualStartDate.HasValue;
        [JsonIgnore]
        public bool ShowActualEndDate => ActualEndDate != null && ActualEndDate.HasValue;
        [JsonIgnore]
        public bool IsExpanded { get; set; }
        [JsonIgnore]
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

        public bool ShowWarning => StatusCode == JobStatus.Job_delayed ||
                    StatusCode == JobStatus.QC_failed_by_Maincon ||
                    StatusCode == JobStatus.QC_rejected_by_RTO;

        [JsonIgnore]
        public ICommand ExpandCommand { get; set; }

        private void ExpandClicked()
        {
            IsExpanded = !IsExpanded;
            OnPropertyChanged("IsExpanded");
            OnPropertyChanged("ExpansionIcon");
            OnPropertyChanged("FrameHeight");
            if (ViewModelLocator.jobScanVM.ScannedItemsGrid != null)
            {
                ViewModelLocator.jobScanVM.ScannedItemsGrid.LayoutTo(new Rectangle(0, 0, ViewModelLocator.jobScanVM.Width, ViewModelLocator.jobScanVM.Height), 300, Easing.Linear);
            }
        }

        private Organisation _selectedSubcon;
        [JsonIgnore]
        public Organisation SelectedSubcon
        {
            get
            {
                return _selectedSubcon;
            }
            set
            {
                _selectedSubcon = value;
                OnPropertyChanged("ShowAssignButton");
                OnPropertyChanged("SelectedSubcon");
            }
        }
        [JsonIgnore]

        public bool ShowQCButton => StatusCode == JobStatus.QC_failed_by_Maincon || StatusCode == JobStatus.QC_rejected_by_RTO && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 3;

        public bool ShowStartJobButton => MaterialID != 0 && !ShowActualStartDate && Convert.ToInt32(Application.Current.Properties["entry_point"]) == 2 && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 5;

        public bool ShowCompleteJobButton => ShowActualStartDate && !ShowActualEndDate && Convert.ToInt32(Application.Current.Properties["entry_point"]) == 2 && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 5;

        public bool ShowHardStop => StatusCode == JobStatus.QC_failed_by_Maincon || StatusCode == JobStatus.QC_rejected_by_RTO;

        public ICommand QCCommand { get; set; }


        public ICommand AssignJobCommand { get; set; }

        private void AssignJobClicked()
        {
            IsLoading = true;
            SubConID = SelectedSubcon.ID;
            SubConName = SelectedSubcon.Name;
            Start = PlannedSD;
            End = PlannedED;
            ApiClient.Instance.JTAssignJob(0, this).ContinueWith(t =>
            {
                if (t.Result.status == 0)
                {
                    StatusCode = JobStatus.Job_not_started;
                    Status = JobStatus.Job_not_started.ToString().Replace('_', ' ');
                    OnPropertyChanged("SubConName");
                    OnPropertyChanged("StatusCode");
                    OnPropertyChanged("Status");
                    OnPropertyChanged("StatusColor");
                    OnPropertyChanged("ShowAssignButton");
                    OnPropertyChanged("Start");
                    OnPropertyChanged("End");
                    OnPropertyChanged("ShowStartDate");
                    OnPropertyChanged("ShowEndDate");
                    OnPropertyChanged("ShowSubcon");
                    OnPropertyChanged("ShowSubconSelection");
                    IsLoading = false;
                }
                else
                {
                    ErrorMessage = t.Result.message;
                }
            });
        }

        public ICommand StartJobCommand { get; set; }

        private void StartJobClicked()
        {
            IsLoading = true;
            var jobStatus = JobStatus.Job_started;
            ApiClient.Instance.JTUpdateJobSchedule(ProjectID, ID, Convert.ToInt32(jobStatus)).ContinueWith(t =>
             {
                 if (t.Result.status == 0)
                 {
                     StatusCode = jobStatus;
                     Status = jobStatus.ToString();
                     ActualStartDate = DateTime.Now;
                     OnPropertyChanged("ActualStartDate");
                     OnPropertyChanged("ShowActualStartDate");
                     OnPropertyChanged("StatusCode");
                     OnPropertyChanged("Status");
                     OnPropertyChanged("StatusColor");
                     OnPropertyChanged("ShowAssignButton");
                     OnPropertyChanged("Start");
                     OnPropertyChanged("End");
                     OnPropertyChanged("ShowStartDate");
                     OnPropertyChanged("ShowEndDate");
                     OnPropertyChanged("ShowSubcon");
                     OnPropertyChanged("ShowSubconSelection");
                     IsLoading = false;
                 }
                 else
                 {
                     ErrorMessage = t.Result.message;
                 }
             });
        }

        public ICommand CompleteJobCommand { get; set; }

        private void CompleteJobClicked()
        {
            IsLoading = true;
            var jobStatus = JobStatus.Job_completed;
            ApiClient.Instance.JTUpdateJobSchedule(ProjectID, ID, Convert.ToInt32(jobStatus)).ContinueWith(t =>
            {
                if (t.Result.status == 0)
                {
                    StatusCode = jobStatus;
                    Status = jobStatus.ToString();
                    ActualEndDate = DateTime.Now;
                    OnPropertyChanged("ActualEndDate");
                    OnPropertyChanged("ShowActualEndDate");
                    OnPropertyChanged("StatusCode");
                    OnPropertyChanged("Status");
                    OnPropertyChanged("StatusColor");
                    OnPropertyChanged("ShowAssignButton");
                    OnPropertyChanged("Start");
                    OnPropertyChanged("End");
                    OnPropertyChanged("ShowStartDate");
                    OnPropertyChanged("ShowEndDate");
                    OnPropertyChanged("ShowSubcon");
                    OnPropertyChanged("ShowSubconSelection");
                    OnPropertyChanged("ShowCompleteJobButton");
                    IsLoading = false;
                }
                else
                {
                    ErrorMessage = t.Result.message;
                }
            });
        }

        public ICommand ChecklistCommand { get; set; }

        private async void CheckListClicked()
        {
            ViewModelLocator.jobChecklistVM.Job = this;
            await Navigation.PushAsync(new JobChecklist());

        }

        private void StatusChanged()
        {
            OnPropertyChanged("ShowStartJobButton");
            OnPropertyChanged("ShowCompleteJobButton");
        }

        public JobScheduleDetails()
        {
            _neverLoadBefore = false;
            ExpandCommand = new Command(ExpandClicked);
            AssignJobCommand = new Command(AssignJobClicked);
            StartJobCommand = new Command(StartJobClicked);
            CompleteJobCommand = new Command(CompleteJobClicked);
            ChecklistCommand = new Command(CheckListClicked);
            //QCCommand = new Command(QCButtonClicked);
        }
    }
}
