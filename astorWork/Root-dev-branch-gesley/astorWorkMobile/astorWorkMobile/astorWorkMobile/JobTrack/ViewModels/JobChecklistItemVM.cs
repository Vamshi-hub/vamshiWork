using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using astorWorkMobile.Shared.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;
using static astorWorkMobile.Shared.Utilities.ApiClient;

namespace astorWorkMobile.JobTrack.ViewModels
{
    public class JobChecklistItemVM : MasterVM
    {
        ApiResult result = null;
        public bool DisplayButton
        {
            get
            {
                if (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 5)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        private int _badgeNumber { get; set; }
        public bool ToClearSignature { get; set; }
        public int BadgeNumber
        {
            get
            {
                try
                {
                    var msgs = new List<ChatMessage>(ChatClient.Instance.JobMessages);
                    string username = string.Empty;
                    if (Application.Current.Properties.ContainsKey("user_name"))
                        username = Application.Current.Properties["user_name"] as string;
                    List<ChatMessage> vmsgs = null;
                    if (msgs != null && msgs.Count > 0)
                    {
                        if (!ViewModelLocator.jobChecklistVM.IsStructural)
                            vmsgs = msgs.Where(m => m.JobID == ViewModelLocator.jobChecklistVM.Job.ID
                                     && m.ChecklistID == ViewModelLocator.jobChecklistItemVM.checklist.ID && (m.ChecklistItemID == null || m.ChecklistItemID == 0)).ToList();
                        else
                        {
                            vmsgs = msgs.Where(m => m.MaterialID == ViewModelLocator.jobChecklistVM.Material.id
                             && m.ChecklistID == ViewModelLocator.jobChecklistItemVM.checklist.ID && (m.ChecklistItemID == null || m.ChecklistItemID == 0)).ToList();
                        }
                    }
                    if (vmsgs != null && vmsgs.Count > 0)
                    {
                        var seenmsgs = vmsgs.Where(m => m.SeenUsers != null && m.SeenUsers.Count > 0 && m.SeenUsers.Contains(username)).ToList();
                        if (seenmsgs != null && seenmsgs.Count > 0)
                            _badgeNumber = vmsgs.Except(seenmsgs).Count();
                        else
                        {
                            _badgeNumber = vmsgs.Count;
                        }
                    }
                    else
                    {
                        _badgeNumber = 0;
                    }
                    if (_badgeNumber > 0)
                    {
                        IsUnreadMsg = true;
                    }
                    else
                        IsUnreadMsg = false;
                    OnPropertyChanged("IsUnreadMsg");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return _badgeNumber;

            }
        }
        public bool IsUnreadMsg { get; set; }
        public bool IsImageAdded { get; set; }
        public string CheckListHeader { get; set; }
        public string CheckListSubHeader { get; set; }
        public string Title { get; set; }
        public JobScheduleDetails Job { get; set; }
        public Checklist checklist { get; set; }
        private List<JobChecklistItem> _checklistItems;
        public List<JobChecklistItem> ChecklistItems
        {
            get
            {
                try
                {
                    return SignedChecklst?.ChecklistItems;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            set
            {
                try
                {
                    _checklistItems = value;
                    if (ViewModelLocator.jobChecklistVM.IsArchitechtural)
                    {
                        CheckListHeader = Job.TradeName;
                        CheckListSubHeader = Job.ModuleName;
                    }
                    else
                    {
                        CheckListHeader = ViewModelLocator.jobChecklistVM.Material.markingNo;
                        CheckListSubHeader = ViewModelLocator.jobChecklistVM.Material.destination;
                    }
                    OnPropertyChanged("CheckListHeader");
                    OnPropertyChanged("CheckListSubHeader");
                    OnPropertyChanged("ChecklistItems");
                    OnPropertyChanged("ShowSubmitButton");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }
        public SignedChecklist SignedChecklst { get; set; }
        private List<User> _listOfRTO { get; set; }
        public List<User> ListOFRTO
        {
            get => _listOfRTO;
            set
            {
                try
                {
                    _listOfRTO = value;
                    OnPropertyChanged("ListOfRTO");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private User _selectedRTO { get; set; }
        public User SelectedRTO
        {
            get => _selectedRTO;
            set
            {
                try
                {
                    _selectedRTO = value;
                    OnPropertyChanged("SelectedRTO");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public string RTOName { get; set; }
        public bool ShowRTOAssigned
        {
            get
            {
                try
                {
                    return checklist.RTOID != 0 && Job != null && Job.RouteToRTO || checklist.RTOID != 0 && ViewModelLocator.jobChecklistVM.Material != null && !string.IsNullOrEmpty(ViewModelLocator.jobChecklistVM.Material.routeTo);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public string Remarks { get; set; }
        public bool ShowSubmitButton
        {
            get
            {
                try
                {
                    bool success = false;

                    if (MobileEntryPoint == 4)
                    {
                        success = checklist.StatusCode < (int)QCStatus.QC_passed_by_Maincon && checklist.StatusCode != (int)QCStatus.QC_failed_by_Maincon ? true : false;
                    }
                    return success;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool ShowNotifySubConButton
        {
            get
            {
                try
                {
                    bool success = false;
                    if (MobileEntryPoint == 4)
                    {
                        success = checklist.StatusCode == (int)QCStatus.QC_rejected_by_RTO ? true : false;
                    }
                    return success;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool ShowAcceptButton
        {
            get
            {
                try
                {
                    bool success = false;
                    if (MobileEntryPoint == 3)
                    {
                        success = checklist.StatusCode != (int)QCStatus.QC_accepted_by_RTO ? true : false;
                    }
                    return success;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool ShowRejectButton
        {
            get
            {
                try
                {
                    bool success = false;
                    if (MobileEntryPoint == 3)
                    {
                        success = checklist.StatusCode != (int)QCStatus.QC_accepted_by_RTO &&
                            checklist.StatusCode != (int)QCStatus.QC_rejected_by_RTO
                            ? true : false;
                    }
                    return success;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool ShowRectifiedButton
        {
            get
            {
                try
                {
                    bool success = false;
                    if (MobileEntryPoint == 2 || MobileEntryPoint == 0)
                    {
                        success = checklist.StatusCode == (int)QCStatus.QC_failed_by_Maincon ? true : false;
                    }
                    return success;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public int MobileEntryPoint
        {
            get
            {
                int data = 0;
                try
                {
                    if (Application.Current.Properties["entry_point"] != null)
                    {
                        data = Convert.ToInt32(Application.Current.Properties["entry_point"]);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return data;
            }
        }
        private string _imageBase64 { get; set; }
        public string ImageBase64
        {
            get
            {
                return _imageBase64;
            }
            set
            {
                try
                {
                    _imageBase64 = value;
                    OnPropertyChanged("ImageBase64");
                    OnPropertyChanged("SignatureImageSource");
                    OnPropertyChanged("ShowSignature");
                    OnPropertyChanged("ShowSignatureButton");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public string Url
        {
            get
            {
                try
                {
                    if (SignedChecklst != null && SignedChecklst.Signatures != null) //&& MobileEntryPoint != 0 why this condition was added?, removed as of now.
                        return SignedChecklst.Signatures.Where(sgn => sgn.MobileEntryPoint == MobileEntryPoint)?.FirstOrDefault()?.SignatureURL;
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public ImageSource _signatureImageSource { get; set; }
        public ImageSource SignatureImageSource
        {
            get
            {
                try
                {
                    if (!string.IsNullOrEmpty(ImageBase64))
                    {
                        _signatureImageSource = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(ImageBase64)));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Url))

                            _signatureImageSource = Url;
                        else
                            _signatureImageSource = null;
                    }
                    OnPropertyChanged("ShowSignatureButton");
                    return _signatureImageSource;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }
        public bool ShowSignature
        {
            get
            {
                try
                {
                    return !string.IsNullOrEmpty(Url) || !string.IsNullOrEmpty(ImageBase64);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool ShowSignatureButton
        {
            get
            {
                try
                {
                    bool status = false;
                    if (MobileEntryPoint == 4)
                    {
                        if (checklist.StatusCode < (int)QCStatus.QC_passed_by_Maincon && checklist.StatusCode != (int)QCStatus.QC_failed_by_Maincon)
                            status = true;
                    }
                    else if (MobileEntryPoint == 3)
                    {
                        if (checklist.StatusCode != (int)QCStatus.QC_accepted_by_RTO)
                            status = true;
                    }
                    else if (MobileEntryPoint == 2 || MobileEntryPoint == 0)
                    {
                        if (checklist.StatusCode == (int)QCStatus.QC_failed_by_Maincon)
                            status = true;
                    }

                    return status;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool _showHideContent { get; set; }
        public bool ShowHideContent
        {
            get => _showHideContent;
            set
            {
                _showHideContent = value;
                OnPropertyChanged("ShowHideContent");
            }
        }
        private bool _showRTO { get; set; }
        public bool ShowRTO
        {
            get
            {
                try
                {
                    if (ViewModelLocator.jobChecklistVM.IsArchitechtural)
                    {
                        _showRTO = MobileEntryPoint == 4 && Job.RouteToRTO && checklist.StatusCode < (int)QCStatus.QC_passed_by_Maincon
                       && checklist.RTOID == 0 && ChecklistItems != null && ChecklistItems.Count > 0
                       && ChecklistItems.Where(chk => chk.StatusCode == (int)ChecklistItemStatus.Pending || chk.StatusCode == (int)ChecklistItemStatus.Fail).Count() == 0;
                    }
                    else if (ViewModelLocator.jobChecklistVM.IsStructural)
                    {
                        _showRTO = MobileEntryPoint == 4 && !string.IsNullOrEmpty(ViewModelLocator.jobChecklistVM.Material.routeTo) && checklist.StatusCode < (int)QCStatus.QC_passed_by_Maincon
                            && checklist.RTOID == 0 && ChecklistItems != null && ChecklistItems.Count > 0
                       && ChecklistItems.Where(chk => chk.StatusCode == (int)ChecklistItemStatus.Pending || chk.StatusCode == (int)ChecklistItemStatus.Fail).Count() == 0;
                    }
                    if (_showRTO && ListOFRTO != null && ListOFRTO.Count == 1)
                    {
                        SelectedRTO = ListOFRTO[0];
                        OnPropertyChanged("SelectedRTO");
                    }
                    return _showRTO;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            set
            {
                _showRTO = value;
                OnPropertyChanged("ShowRTO");
            }
        }
        public ICommand SubmitCommand { get; set; }
        public ICommand QCAcceptedCommand { get; set; }
        public ICommand QCRejectedCommand { get; set; }
        public ICommand QCRectifiedCommand { get; set; }
        public ICommand NotifySubConCommand { get; set; }
        public ICommand SignButtonCommand { get; set; }
        public ICommand ChatCommand { get; set; }
        void ChatClicked()
        {
            try
            {
                ViewModelLocator.chatVM.Header = checklist.Name;
                ViewModelLocator.chatVM.Checklist = checklist;
                ViewModelLocator.chatVM.IsChecklistChat = true;
                ViewModelLocator.chatVM.IsJobChat = !ViewModelLocator.jobChecklistVM.IsStructural;
                //ViewModelLocator.chatVM.Messages = new ObservableCollection<ChatMessage>();
                //if (checklist.ChecklistMessages != null && checklist.ChecklistMessages.Count > 0)
                //ViewModelLocator.chatVM.Messages = checklist.ChecklistMessages;
                Navigation.PushAsync(new Chat());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        async void SignButtonClicked()
        {
            try
            {
                var signPadPage = new SignatureView(this.GetType());
                signPadPage.BindingContext = this;
                if (ShowSignatureButton)
                {
                    await Navigation.PushModalAsync(signPadPage);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async void SubmitClicked()
        {
            try
            {
                int RTOID = 0;
                if (ChecklistItems != null && ChecklistItems.Count > 0 && ChecklistItems.Where(ci => ci.StatusCode == (int)
                 ChecklistItemStatus.Pending).Count() == ChecklistItems.Count)
                {
                    DisplaySnackBar("Inspection not done", PageActions.None, MessageActions.Warning, null, null);
                    return;
                }

                if (string.IsNullOrEmpty(Url) && string.IsNullOrEmpty(ImageBase64))
                {
                    DisplaySnackBar("Please Sign before submitting", PageActions.None, MessageActions.Warning, null, null);
                    return;
                }

                if ((Job != null && Job.RouteToRTO) || (ViewModelLocator.jobChecklistVM.Material != null && !string.IsNullOrEmpty(ViewModelLocator.jobChecklistVM.Material.routeTo)))
                {
                    if (ChecklistItems.Where(chk => chk.StatusCode == (int)ChecklistItemStatus.Fail || chk.StatusCode == (int)ChecklistItemStatus.Pending).Count() == 0)
                    {

                        if (SelectedRTO != null || checklist.RTOID != 0)
                        {
                            RTOID = checklist.RTOID != 0 ? checklist.RTOID : SelectedRTO.UserID;
                        }
                        else
                        {
                            if (ListOFRTO == null || ListOFRTO.Count == 0)
                            {
                                ViewModelLocator.jobChecklistItemVM.ErrorMessage = "There is no RTO user";
                            }
                            else
                            {
                                //await Application.Current.MainPage.DisplayAlert("Info", "Please select RTO", "OK");
                                ViewModelLocator.jobChecklistItemVM.DisplaySnackBar("Please select RTO", PageActions.None, MessageActions.Warning, null, null);
                            }
                            return;

                        }
                    }
                }
                if (ViewModelLocator.jobChecklistVM.IsStructural && ChecklistItems.Where(chk => chk.StatusCode == (int)ChecklistItemStatus.Fail || chk.StatusCode == (int)ChecklistItemStatus.Pass).Count() > 0)
                {
                    if (!IsImageAdded)
                    {
                        DisplaySnackBar("Please add image to proceed", PageActions.None, MessageActions.Warning, null, null);
                        return;
                    }
                }
                else if (ChecklistItems.Where(chk => chk.StatusCode == (int)ChecklistItemStatus.Fail).Count() > 0)
                {
                    if (!IsImageAdded)
                    {
                        DisplaySnackBar("Please add image to proceed", PageActions.None, MessageActions.Warning, null, null);
                        return;
                    }
                }

                ShowHideContent = false;
                IsLoading = true;
                SignedChecklst.ChecklistItems = ChecklistItems;
                SignedChecklst.Signature = ImageBase64;
                if (ViewModelLocator.jobChecklistVM.IsArchitechtural)
                {
                    result = await ApiClient.Instance.JTPutJobChecklistItems(Job.ProjectID, Job.ID, RTOID, checklist.ID, SignedChecklst);
                }
                else
                {
                    result = await ApiClient.Instance.MTPutJobChecklistItems(checklist.ID, ViewModelLocator.jobChecklistVM.Material.id, RTOID, SignedChecklst);
                }
                if (result.status == 0)
                {
                    IsLoading = false;
                    // await Application.Current.MainPage.DisplayAlert("Success", "Updated successfully", "OK");
                    ViewModelLocator.jobChecklistVM.Checklists.Where(chk => chk.ID == checklist.ID).FirstOrDefault().StatusCode = Convert.ToInt32(result.data);
                    ViewModelLocator.jobChecklistVM.Checklists.Where(chk => chk.ID == checklist.ID).FirstOrDefault().Status = ((QCStatus)Convert.ToInt32(result.data)).ToString();
                    if (Convert.ToInt32(result.data) != (int)QCStatus.Pending_QC)
                    {
                        SendMessage(((QCStatus)Convert.ToInt32(result.data)).ToString().Replace('_', ' '));
                    }
                    DisplaySnackBar("QC Updated", PageActions.PopAsync, MessageActions.Success, null, null);
                    // await Navigation.PopAsync();
                    ShowHideContent = true;
                    ViewModelLocator.jobChecklistItemVM.ToClearSignature = true;
                }
                else
                {
                    ShowHideContent = true;
                    ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async void QCAcceptedClicked()
        {
            try
            {
                if (string.IsNullOrEmpty(Url) && string.IsNullOrEmpty(ImageBase64))
                {
                    DisplaySnackBar("Please Sign before submitting", PageActions.None, MessageActions.Warning, null, null);
                    return;
                }
                ShowHideContent = false;
                IsLoading = true;
                SignedChecklst.Signature = ImageBase64;
                if (ViewModelLocator.jobChecklistVM.IsArchitechtural)
                {
                    result = await ApiClient.Instance.JTAcceptRejectChecklistRTO(0, checklist.ID, Job.ID, (int)QCStatus.QC_accepted_by_RTO, SignedChecklst);
                }
                else
                {
                    result = await ApiClient.Instance.MTAcceptRejectChecklistRTO(checklist.ID, ViewModelLocator.jobChecklistVM.Material.id, (int)QCStatus.QC_accepted_by_RTO, SignedChecklst);
                }
                if (result.status == 0)
                {
                    IsLoading = false;
                    //  await Application.Current.MainPage.DisplayAlert("Success", "Request submitted successfully", "Ok");
                    ViewModelLocator.jobChecklistVM.Checklists.Where(chk => chk.ID == checklist.ID).FirstOrDefault().StatusCode = Convert.ToInt32(result.data);
                    ViewModelLocator.jobChecklistVM.Checklists.Where(chk => chk.ID == checklist.ID).FirstOrDefault().Status = ((QCStatus)Convert.ToInt32(result.data)).ToString();
                    SendMessage(QCStatus.QC_accepted_by_RTO.ToString());
                    DisplaySnackBar("QC Accepted ", PageActions.PopAsync, MessageActions.Success, null, null);
                    // await Navigation.PopAsync();
                    ShowHideContent = true;
                    ViewModelLocator.jobChecklistItemVM.ToClearSignature = true;
                }
                else
                {
                    ShowHideContent = true;
                    ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async void QCRejectedClicked()
        {
            try
            {
                if (string.IsNullOrEmpty(Url) && string.IsNullOrEmpty(ImageBase64))
                {
                    DisplaySnackBar("Please Sign before submitting", PageActions.None, MessageActions.Warning, null, null);
                    return;
                }
                if (!IsImageAdded)
                {
                    DisplaySnackBar("Please add image for Reject reason", PageActions.None, MessageActions.Warning, null, null);
                    return;
                }
                ShowHideContent = false;
                IsLoading = true;
                SignedChecklst.Signature = ImageBase64;

                if (!ViewModelLocator.jobChecklistVM.IsStructural)
                {
                    result = await ApiClient.Instance.JTAcceptRejectChecklistRTO(0, checklist.ID, Job.ID, (int)QCStatus.QC_rejected_by_RTO, SignedChecklst);
                }
                else
                {
                    result = await ApiClient.Instance.MTAcceptRejectChecklistRTO(checklist.ID, ViewModelLocator.jobChecklistVM.Material.id, (int)QCStatus.QC_rejected_by_RTO, SignedChecklst);
                }
                if (result.status == 0)
                {
                    IsLoading = false;
                    // await Application.Current.MainPage.DisplayAlert("Success", "Request submitted successfully", "Ok");
                    ViewModelLocator.jobChecklistVM.Checklists.Where(chk => chk.ID == checklist.ID).FirstOrDefault().StatusCode = Convert.ToInt32(result.data);
                    ViewModelLocator.jobChecklistVM.Checklists.Where(chk => chk.ID == checklist.ID).FirstOrDefault().Status = ((QCStatus)Convert.ToInt32(result.data)).ToString();
                    SendMessage(QCStatus.QC_rejected_by_RTO.ToString());
                    DisplaySnackBar("QC Rejected", PageActions.PopAsync, MessageActions.Success, null, null);
                    // await Navigation.PopAsync();
                    ShowHideContent = true;
                    ViewModelLocator.jobChecklistItemVM.ToClearSignature = true;
                }
                else
                {
                    ShowHideContent = true;
                    ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async void QCRectifiedClicked()
        {
            try
            {
                if (string.IsNullOrEmpty(Url) && string.IsNullOrEmpty(ImageBase64))
                {
                    DisplaySnackBar("Please Sign before submitting", PageActions.None, MessageActions.Warning, null, null);
                    return;
                }
                if (!IsImageAdded)
                {
                    DisplaySnackBar("Please add image for Rectify reason", PageActions.None, MessageActions.Warning, null, null);
                    return;
                }
                ShowHideContent = false;
                IsLoading = true;
                SignedChecklst.Signature = ImageBase64;
                if (ViewModelLocator.jobChecklistVM.IsArchitechtural)
                {
                    result = await ApiClient.Instance.JTAcceptRejectChecklistRTO(0, checklist.ID, Job.ID, (int)QCStatus.QC_rectified_by_Subcon, SignedChecklst);
                }
                else
                {
                    result = await ApiClient.Instance.MTAcceptRejectChecklistRTO(checklist.ID, ViewModelLocator.jobChecklistVM.Material.id, (int)QCStatus.QC_rectified_by_Subcon, SignedChecklst);
                }
                if (result.status == 0)
                {
                    IsLoading = false;
                    ViewModelLocator.jobChecklistVM.Checklists.Where(chk => chk.ID == checklist.ID).FirstOrDefault().StatusCode = Convert.ToInt32(result.data);
                    ViewModelLocator.jobChecklistVM.Checklists.Where(chk => chk.ID == checklist.ID).FirstOrDefault().Status = ((QCStatus)Convert.ToInt32(result.data)).ToString();
                    SendMessage(QCStatus.QC_rectified_by_Subcon.ToString());
                    DisplaySnackBar("QC Rectified", PageActions.PopAsync, MessageActions.Success, null, null);
                    ShowHideContent = true;
                    ViewModelLocator.jobChecklistItemVM.ToClearSignature = true;
                }
                else
                {
                    ShowHideContent = true;
                    ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async void QCNotifyToSubConClicked()
        {
            try
            {
                if (string.IsNullOrEmpty(Url) && string.IsNullOrEmpty(ImageBase64))
                {
                    DisplaySnackBar("Please Sign before submitting", PageActions.None, MessageActions.Warning, null, null);
                    return;
                }
                ShowHideContent = false;
                IsLoading = true;
                SignedChecklst.Signature = ImageBase64;
                if (ViewModelLocator.jobChecklistVM.IsArchitechtural)
                {
                    result = await ApiClient.Instance.JTAcceptRejectChecklistRTO(0, checklist.ID, Job.ID, (int)QCStatus.QC_failed_by_Maincon, SignedChecklst);
                }
                else
                {
                    result = await ApiClient.Instance.MTAcceptRejectChecklistRTO(checklist.ID, ViewModelLocator.jobChecklistVM.Material.id, (int)QCStatus.QC_failed_by_Maincon, SignedChecklst);
                }

                if (result.status == 0)
                {
                    IsLoading = false;
                    ViewModelLocator.jobChecklistVM.Checklists.Where(chk => chk.ID == checklist.ID).FirstOrDefault().StatusCode = Convert.ToInt32(result.data);
                    ViewModelLocator.jobChecklistVM.Checklists.Where(chk => chk.ID == checklist.ID).FirstOrDefault().Status = ((QCStatus)Convert.ToInt32(result.data)).ToString();
                    SendMessage(QCStatus.QC_failed_by_Maincon.ToString());
                    DisplaySnackBar("QC Updated", PageActions.PopAsync, MessageActions.Success, null, null);
                    ShowHideContent = true;
                    ViewModelLocator.jobChecklistItemVM.ToClearSignature = true;
                }
                else
                {
                    ShowHideContent = true;
                    ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ResetProperties()
        {
            ImageBase64 = null;
            _imageBase64 = null;
            _signatureImageSource = null;
            //_signatureImageSource.ClearValue();
            //_signatureImageSource.
        }

        public void UpdateProperties()
        {
            OnPropertyChanged("BadgeNumber");
        }
        private async void SendMessage(string status)
        {
            try
            {
                string username = string.Empty;
                if (Application.Current.Properties.ContainsKey("user_name"))
                    username = Application.Current.Properties["user_name"] as string;
                string tenantName = string.Empty;
                if (!string.IsNullOrEmpty(Application.Current.Properties["tenant_name"] as string))
                    tenantName = Application.Current.Properties["tenant_name"] as string;

                var msg = new MessageData
                {
                    TenantName = tenantName,
                    Header = string.Empty,
                    Message = $"QC status updated to {status}",
                    UserName = username,
                    Timestamp = DateTime.UtcNow,
                    MaterialID = ViewModelLocator.jobChecklistVM.IsStructural ? ViewModelLocator.jobChecklistVM.Material.id : 0,
                    MarkingNo = ViewModelLocator.jobChecklistVM.IsStructural ? ViewModelLocator.jobChecklistVM.Material.markingNo : "",
                    ModuleName = ViewModelLocator.jobChecklistVM.CheckListSubHeader,
                    JobID = !ViewModelLocator.jobChecklistVM.IsStructural ? ViewModelLocator.jobChecklistItemVM.Job.ID : 0,
                    JobName = ViewModelLocator.jobChecklistItemVM.Job != null ? ViewModelLocator.jobChecklistItemVM.Job.TradeName : "",
                    ChecklistID = ViewModelLocator.jobChecklistItemVM.checklist.ID,
                    ChecklistName = ViewModelLocator.jobChecklistItemVM.checklist.Name,
                    ChecklistItemID = 0,
                    ChecklistItemName = "",
                    HasImage = false,
                    IsSystem = true,
                    ThumbnailImagebase64 = "",
                    ThumbnailUrl = ""
                };

                if (ChatClient.Instance.IsConnected)
                {
                    ChatClient.Instance.Message = msg;
                    await ChatClient.Instance.SendMessageAsync();
                }
                else
                {
                    await ChatClient.Instance.Connect();
                    ChatClient.Instance.Message = msg;
                    await ChatClient.Instance.SendMessageAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JobChecklistItemVM() : base()
        {
            try
            {
                ResetProperties();
                SignButtonCommand = new Command(SignButtonClicked);
                SubmitCommand = new Command(SubmitClicked);
                QCAcceptedCommand = new Command(QCAcceptedClicked);
                QCRejectedCommand = new Command(QCRejectedClicked);
                QCRectifiedCommand = new Command(QCRectifiedClicked);
                NotifySubConCommand = new Command(QCNotifyToSubConClicked);
                ChatCommand = new Command(ChatClicked);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
