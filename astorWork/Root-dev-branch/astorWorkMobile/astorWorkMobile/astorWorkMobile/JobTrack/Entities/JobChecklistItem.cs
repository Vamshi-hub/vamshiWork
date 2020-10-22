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

namespace astorWorkMobile.JobTrack.Entities
{
    public class JobChecklistItem : MasterVM
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string TimeFrame { get; set; }
        public string SelectedBackgroundColor => "#00b57c";
        public string NonSelectedBacgroundcolor => "#ebebeb";
        public string SelectedForeColor => "white";
        public string NonSelectedForecolor => "black";
        public string YesbgColor => StatusCode == (int)ChecklistItemStatus.Pass ? "#00b57c" : NonSelectedBacgroundcolor;
        public string NobgColor => StatusCode == (int)ChecklistItemStatus.Fail ? "#e85059" : NonSelectedBacgroundcolor;
        public string NAbgColor => StatusCode == (int)ChecklistItemStatus.NA ? "#707070" : NonSelectedBacgroundcolor;
        public string YesTextColor => StatusCode == (int)ChecklistItemStatus.Pass ? SelectedForeColor : NonSelectedForecolor;
        public string NoTextColor => StatusCode == (int)ChecklistItemStatus.Fail ? SelectedForeColor : NonSelectedForecolor;
        public string NATextColor => StatusCode == (int)ChecklistItemStatus.NA ? SelectedForeColor : NonSelectedForecolor;
        public ICommand YesCommand { get; set; }
        private void YesClicked()
        {
            if (YesbgColor == NonSelectedBacgroundcolor)
            {
                StatusCode = (int)ChecklistItemStatus.Pass;
                Status = ChecklistItemStatus.Pass.ToString();
            }
            else
            {
                StatusCode = (int)ChecklistItemStatus.Pending;
                Status = ChecklistItemStatus.Pending.ToString();
            }
            CommonProperties();
            buttonClicked();
        }
        public ICommand NoCommand { get; set; }
        private void NoClicked()
        {
            if (NobgColor == NonSelectedBacgroundcolor)
            {
                StatusCode = (int)ChecklistItemStatus.Fail;
                Status = ChecklistItemStatus.Fail.ToString();
            }
            else
            {
                StatusCode = (int)ChecklistItemStatus.Pending;
                Status = ChecklistItemStatus.Pending.ToString();
            }
            CommonProperties();
            buttonClicked();
        }
        public ICommand NACommand { get; set; }
        private void NAClicked()
        {
            if (NAbgColor == NonSelectedBacgroundcolor)
            {
                StatusCode = (int)ChecklistItemStatus.NA;
                Status = ChecklistItemStatus.NA.ToString();
            }
            else
            {
                StatusCode = (int)ChecklistItemStatus.Pending;
                Status = ChecklistItemStatus.Pending.ToString();
            }
            CommonProperties();
            buttonClicked();
        }
        private void buttonClicked()
        {
            ViewModelLocator.jobChecklistItemVM.ShowRTO = true;
        }
        public bool IsButtonsenabled
        {
            get
            {
                bool status = false;
                if (ViewModelLocator.jobChecklistItemVM.checklist.StatusCode < (int)QCStatus.QC_passed_by_Maincon && ViewModelLocator.jobChecklistItemVM.checklist.StatusCode != (int)QCStatus.QC_failed_by_Maincon && Convert.ToInt32(Application.Current.Properties["entry_point"]) == 4)
                {
                    status = true;
                }
                return status;

            }
        }
        private void CommonProperties()
        {
            OnPropertyChanged("StatusCode");
            OnPropertyChanged("Status");
            OnPropertyChanged("YesbgColor");
            OnPropertyChanged("NobgColor");
            OnPropertyChanged("NAbgColor");
            OnPropertyChanged("YesTextColor");
            OnPropertyChanged("NoTextColor");
            OnPropertyChanged("NATextColor");
        }
        public ICommand ChatCommand { get; set; }
        void ChatClicked()
        {
            ViewModelLocator.chatVM.Header = Name;
            ViewModelLocator.chatVM.Checklist = ViewModelLocator.jobChecklistItemVM.checklist;
            ViewModelLocator.chatVM.ChecklistItem = this;
            ViewModelLocator.chatVM.IsChecklistChat = false;
            ViewModelLocator.chatVM.IsJobChat = !ViewModelLocator.jobChecklistVM.IsStructural;
            //ViewModelLocator.chatVM.Messages = new ObservableCollection<ChatMessage>();
            //if (checklist.ChecklistMessages != null && checklist.ChecklistMessages.Count > 0)
            //ViewModelLocator.chatVM.Messages = checklist.ChecklistMessages;
            Navigation.PushAsync(new Chat());
        }
        public ICommand AddPhotoCommand { get; set; }
        private async void AddPhotoClicked()
        {
            ViewModelLocator.chatVM.Checklist = ViewModelLocator.jobChecklistItemVM.checklist;
            ViewModelLocator.chatVM.ChecklistItem = this;
            ViewModelLocator.chatVM.IsChecklistChat = false;
            ViewModelLocator.chatVM.IsJobChat = !ViewModelLocator.jobChecklistVM.IsStructural;
            var action = await MainPage.DisplayActionSheet("Attach a photo", "Cancel", null, "Gallery", "Camera");
            if (action == "Gallery")
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await ViewModelLocator.chatTakePhotoVM.GetPhotoFromGallery();
                    if (result)
                        await ConfirmPhoto();
                    //TaskScheduler.FromCurrentSynchronizationContext();
                });

            }
            else if (action == "Camera")
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var result = await ViewModelLocator.chatTakePhotoVM.CapturePhoto();
                    if (result)
                        await ConfirmPhoto();
                    //TaskScheduler.FromCurrentSynchronizationContext();
                });
            }
        }
        private async Task ConfirmPhoto()
        {
            var capturePhotoPage = new CapturePhoto();
            await Navigation.PushAsync(capturePhotoPage);
        }
        public ICommand ViewPhotosCommand { get; set; }
        private void ViewPhotosClicked()
        {
            ViewModelLocator.qcDefectVM.QCPhotos.Clear();
            var msgs = new List<ChatMessage>(ChatClient.Instance.JobMessages);
            string username = string.Empty;
            if (Application.Current.Properties.ContainsKey("user_name"))
                username = Application.Current.Properties["user_name"] as string;
            //_badgeNumber = msgs.Count;
            var messages = msgs.Where(m => (!ViewModelLocator.jobChecklistVM.IsStructural ? m.JobID == ViewModelLocator.jobChecklistVM.Job.ID : m.MaterialID == ViewModelLocator.jobChecklistVM.Material.id)
                     && m.ChecklistID == ViewModelLocator.jobChecklistItemVM.checklist.ID && m.ChecklistItemID == ID).ToList();
            foreach (var msg in messages)
            {
                if (msg.Image != null)
                {
                    ViewModelLocator.qcDefectVM.QCPhotos.Add(msg.OriginalImage);
                }
            }
            if (ViewModelLocator.qcDefectVM.QCPhotos != null && ViewModelLocator.qcDefectVM.QCPhotos.Count > 0)
            {
                Navigation.PushAsync(new MaterialTrack.Pages.ViewQCPhoto());
            }
            else
            {
                ViewModelLocator.chatVM.DisplaySnackBar("No Photos", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
            }
        }
        public JobChecklistItem() : base()
        {

            YesCommand = new Command(YesClicked);
            NoCommand = new Command(NoClicked);
            NACommand = new Command(NAClicked);
            ChatCommand = new Command(ChatClicked);
            AddPhotoCommand = new Command(AddPhotoClicked);
            ViewPhotosCommand = new Command(ViewPhotosClicked);

        }
    }
}
