using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;
using static astorWorkMobile.Shared.Utilities.ApiClient;

namespace astorWorkMobile.JobTrack.Entities
{
    public class Checklist : MasterVM
    {
        public int ProjectID { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
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
                if(_status.Contains("by Maincon"))
                {
                    _status = _status.Replace("by Maincon", "");
                }
                OnPropertyChanged("Status");
            }
        }
        public int StatusCode { get; set; }
        public int Sequence { get; set; }
        public int RTOID { get; set; }
        public string RTOName { get; set; }
        public bool IsEnabled { get; set; }
        public Color StatusColor
        {
            get
            {
                Color result = Color.Gray;
                switch ((QCStatus)StatusCode)
                {
                    case QCStatus.Pending_QC:
                        result = Color.FromHex("#CEA051");
                        break;
                    case QCStatus.QC_failed_by_Maincon:
                        result = Color.FromHex("#F00028");
                        break;
                    case QCStatus.QC_rectified_by_Subcon:
                        result = Color.FromHex("#D86D00");
                        break;
                    case QCStatus.QC_passed_by_Maincon:
                        result = Color.FromHex("#286E67");
                        break;
                    case QCStatus.QC_routed_to_RTO:
                        result = Color.FromHex("#AE59AA");
                        break;
                    case QCStatus.QC_rejected_by_RTO:
                        result = Color.FromHex("#FF5722");
                        break;
                    case QCStatus.QC_accepted_by_RTO:
                        result = Color.FromHex("#286E67");
                        break;
                    default:
                        break;
                }
                return result;
            }
        }
        public ObservableCollection<ChatMessage> ChecklistMessages { get; set; }
        public ICommand ChecklistItemCommand { get; set; }
        private async void ChecklistItemClicked()
        {
            ApiResult result;
            ViewModelLocator.jobChecklistVM.ShowChecklist = false;
            ViewModelLocator.jobChecklistVM.IsLoading = true;
            try
            {
                if (RTOID == 0)
                {
                    var lstRTO = await ApiClient.Instance.JTGetRTOList();
                    ViewModelLocator.jobChecklistItemVM.ListOFRTO = lstRTO.data as List<User>;
                }
                else
                {
                    ViewModelLocator.jobChecklistItemVM.RTOName = RTOName;
                }
                if (ViewModelLocator.jobChecklistVM.IsArchitechtural)
                {
                    result = await ApiClient.Instance.JTGetJobChecklistItems(ProjectID, ViewModelLocator.jobChecklistVM.Job.ID, ID, 0);
                }
                else
                {
                    result = await ApiClient.Instance.JTGetJobChecklistItems(0, 0, ID, ViewModelLocator.jobChecklistVM.Material.id);
                }
                if (result.status == 0)
                {
                    ViewModelLocator.jobChecklistItemVM.ImageBase64 = null;
                    ViewModelLocator.jobChecklistItemVM.Job = ViewModelLocator.jobChecklistVM.Job;
                    ViewModelLocator.jobChecklistItemVM.checklist = this;
                    ViewModelLocator.jobChecklistItemVM.Title = Name;
                    ViewModelLocator.jobChecklistItemVM.SelectedRTO = null;
                    ViewModelLocator.jobChecklistItemVM.IsImageAdded = false;
                    ViewModelLocator.jobChecklistItemVM.ChecklistItems = new List<JobChecklistItem>();
                    ViewModelLocator.jobChecklistItemVM.SignedChecklst = new SignedChecklist();
                    ViewModelLocator.jobChecklistItemVM.SignedChecklst = result.data as SignedChecklist;
                    if (ViewModelLocator.jobChecklistItemVM.ChecklistItems != null && ViewModelLocator.jobChecklistItemVM.ChecklistItems.Count > 0)
                    {
                        ViewModelLocator.jobChecklistItemVM.ShowHideContent = true;
                    }
                    await Navigation.PushAsync(new JobChecklistItems());

                }
                else
                {
                    ViewModelLocator.jobChecklistVM.ErrorMessage = result.message;
                    ViewModelLocator.jobChecklistVM.ShowChecklist = true;
                }
                ViewModelLocator.jobChecklistVM.IsLoading = false;
                if (ViewModelLocator.jobChecklistVM.Job == null && ViewModelLocator.jobChecklistVM.IsArchitechtural)
                {
                    ViewModelLocator.jobChecklistVM.ErrorMessage = "There is no Job";
                    ViewModelLocator.jobChecklistVM.ShowChecklist = true;
                    return;
                }
                else if (ViewModelLocator.jobChecklistVM.Material == null && ViewModelLocator.jobChecklistVM.IsStructural)
                {
                    ViewModelLocator.jobChecklistVM.ErrorMessage = "There is no Material";
                    ViewModelLocator.jobChecklistVM.ShowChecklist = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
        public Checklist()
        {
            ChecklistItemCommand = new Command(ChecklistItemClicked);
        }
    }
}
