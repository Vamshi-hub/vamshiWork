using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class ChatTakePhotoVM : TakePhotoVM
    {
        public bool _loading { get; set; }
        public bool Loading { get { return _loading; } set { _loading = value; OnPropertyChanged("Loading"); } }
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
        public async void ConfirmClicked()
        {
            try
            {
                IsLoading = true;
                string _userName = string.Empty;
                if (Application.Current.Properties.ContainsKey("user_name"))
                    _userName = Application.Current.Properties["user_name"] as string;

                var imageBytes = await GetThumbnailBytes();
                var originalImageBytes = GetOriginalPhotoBytes();
                var imageLengthBytes = BitConverter.GetBytes(imageBytes.Length);

                var imageBase64 = Convert.ToBase64String(imageBytes);
                var qcPhoto = new QCPhoto
                {
                    ImageBase64 = imageBase64,
                    Remarks = Remarks
                };
                string tenantName = string.Empty;
                if (!string.IsNullOrEmpty(Application.Current.Properties["tenant_name"] as string))
                    tenantName = Application.Current.Properties["tenant_name"] as string;
                var msg = new MessageData
                {
                    TenantName = tenantName,
                    ThumbnailImagebase64 = qcPhoto.ImageBase64,
                    OriginalImagebase64 = Convert.ToBase64String(originalImageBytes),
                    Message = qcPhoto.Remarks,
                    UserName = _userName,
                    Timestamp = DateTime.UtcNow,
                    HasImage = true,
                    MaterialID = !ViewModelLocator.chatVM.IsJobChat ? ViewModelLocator.jobChecklistVM.Material.id : 0,
                    MarkingNo = !ViewModelLocator.chatVM.IsJobChat ? ViewModelLocator.jobChecklistVM.Material.markingNo : "",
                    ModuleName = ViewModelLocator.jobChecklistVM.CheckListSubHeader,
                    JobID = ViewModelLocator.chatVM.IsJobChat ? ViewModelLocator.jobChecklistItemVM.Job.ID : 0,
                    JobName = ViewModelLocator.jobChecklistItemVM.Job != null ? ViewModelLocator.jobChecklistItemVM.Job.TradeName : "",
                    ChecklistID = ViewModelLocator.jobChecklistItemVM.checklist.ID,
                    ChecklistName = ViewModelLocator.jobChecklistItemVM.checklist.Name,
                    ChecklistItemID = !ViewModelLocator.chatVM.IsChecklistChat ? ViewModelLocator.chatVM.ChecklistItem.ID : 0,
                    ChecklistItemName = !ViewModelLocator.chatVM.IsChecklistChat ? ViewModelLocator.chatVM.ChecklistItem.Name : ""
                };
                if (ChatClient.Instance.IsConnected)
                {

                    ChatClient.Instance.Message = msg;
                    ChatClient.Instance.SendMessageAsync();

                }
                else
                {
                    await ChatClient.Instance.Connect();
                    ChatClient.Instance.Message = msg;
                    ChatClient.Instance.SendMessageAsync();
                }
                ViewModelLocator.jobChecklistItemVM.IsImageAdded = true;
                //ViewModelLocator.chatVM.UpdateProperty();
                Reset();
                IsLoading = false;
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                IsLoading = false;
            }
        }
        public async void CancelClicked()
        {
            Reset();
            await Navigation.PopAsync();
        }
        public ChatTakePhotoVM()
        {
            ConfirmCommand = new Command(ConfirmClicked);
            CancelCommand = new Command(CancelClicked);
        }
    }
}
