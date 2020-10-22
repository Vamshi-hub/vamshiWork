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

namespace astorWorkMobile.JobTrack.ViewModels
{
    public class ChatVM : MasterVM
    {
        public Page Page { get; set; }
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
        private string _userName;
        public bool IsJobChat { get; set; }
        public bool IsChecklistChat { get; set; }
        public Checklist Checklist { get; set; }
        public JobChecklistItem ChecklistItem { get; set; }
        public string Header { get; set; }
        public List<ChatMessage> Messages
        {
            get
            {
                List<ChatMessage> chatMessages = null;
                var msgs = new List<ChatMessage>(ChatClient.Instance.JobMessages);
                try
                {
                    if (msgs != null)
                    {

                        if (IsJobChat)
                        {
                            if (IsChecklistChat)
                            {
                                chatMessages = msgs.Where(m => m.JobID == ViewModelLocator.jobChecklistVM.Job.ID
                                 && m.ChecklistID == ViewModelLocator.jobChecklistItemVM.checklist.ID && (m.ChecklistItemID == null || m.ChecklistItemID == 0)).ToList();
                            }
                            else
                            {
                                chatMessages = msgs.Where(m => m.JobID == ViewModelLocator.jobChecklistVM.Job.ID
                             && m.ChecklistID == ViewModelLocator.jobChecklistItemVM.checklist.ID && m.ChecklistItemID == ChecklistItem.ID).ToList();
                            }
                        }
                        else
                        {
                            if (IsChecklistChat)
                            {
                                chatMessages = msgs.Where(m => m.MaterialID == ViewModelLocator.jobChecklistVM.Material.id
                                 && m.ChecklistID == ViewModelLocator.jobChecklistItemVM.checklist.ID && (m.ChecklistItemID == null || m.ChecklistItemID == 0)).ToList();
                            }
                            else
                            {
                                chatMessages = msgs.Where(m => m.MaterialID == ViewModelLocator.jobChecklistVM.Material.id
                             && m.ChecklistID == ViewModelLocator.jobChecklistItemVM.checklist.ID && m.ChecklistItemID == ChecklistItem.ID).ToList();
                            }
                        }

                        if (chatMessages == null)
                        {
                            chatMessages = new List<ChatMessage>();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return chatMessages;
            }
        }
        public string TextToSend { get; set; }
        public ICommand SendMessageCommand { get; set; }
        public ICommand CloseChatCommand { get; set; }

        private async void SendMessageClicked()
        {
            try
            {
                string username = string.Empty;
                if (Application.Current.Properties.ContainsKey("user_name"))
                    username = Application.Current.Properties["user_name"] as string;
                string tenantName = string.Empty;
                if (!string.IsNullOrEmpty(Application.Current.Properties["tenant_name"] as string))
                    tenantName = Application.Current.Properties["tenant_name"] as string;
                if (!string.IsNullOrEmpty(TextToSend))
                {
                    var msg = new MessageData
                    {
                        TenantName = tenantName,
                        Header = Header,
                        Message = TextToSend,
                        UserName = username,
                        Timestamp = DateTime.UtcNow,
                        MaterialID = !IsJobChat ? ViewModelLocator.jobChecklistVM.Material.id : 0,
                        MarkingNo = !IsJobChat ? ViewModelLocator.jobChecklistVM.Material.markingNo : "",
                        ModuleName = ViewModelLocator.jobChecklistVM.CheckListSubHeader,
                        JobID = IsJobChat ? ViewModelLocator.jobChecklistItemVM.Job.ID : 0,
                        JobName = ViewModelLocator.jobChecklistItemVM.Job != null ? ViewModelLocator.jobChecklistItemVM.Job.TradeName : "",
                        ChecklistID = ViewModelLocator.jobChecklistItemVM.checklist.ID,
                        ChecklistName = ViewModelLocator.jobChecklistItemVM.checklist.Name,
                        ChecklistItemID = !IsChecklistChat ? ChecklistItem.ID : 0,
                        ChecklistItemName = !IsChecklistChat ? ChecklistItem.Name : "",
                        HasImage = false,
                        IsSystem = false,
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

                    TextToSend = string.Empty;
                    OnPropertyChanged("TextToSend");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateProperty()
        {
            OnPropertyChanged("Messages");
            MessagingCenter.Send(this, "SCROLL_BOTTOM");
        }
        public ICommand AddPhotoCommand { get; set; }
        private async void AddPhotoClicked()
        {
            try
            {
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ICommand SendChatAuditCommand { get; set; }
        private void SendChatAuditClicked()
        {
            //            var htmlTemplate = @"<p>Hi,</p>
            //<p>Module: <strong>[MODULE_NAME]</strong><br>
            //Job: <strong>[JOB_NAME]</strong><br>
            //CheckList Item: <strong>[CHECKLIST_ITEM]</strong><br>
            //.</p>
            //<p>Regards,<br>astorWork Team</p>";
            //            var bodyHtml = htmlTemplate.Replace("[MODULE_NAME]", ViewModelLocator.jobChecklistVM.Job.ModuleName)
            //                .Replace("[JOB_NAME]", ViewModelLocator.jobChecklistVM.Job.JobName)
            //                .Replace("CHECKLIST_ITEM]",Header);

            //            Task.Run(() => EmailClient.Instance.SendSingleAsync("bibekananda.pradhan@astoriasolutions.com", "Pradhan", "Chat History", bodyHtml));
        }
        private async Task ConfirmPhoto()
        {
            try
            {
                var capturePhotoPage = new CapturePhoto();
                await Navigation.PushAsync(capturePhotoPage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ICommand ViewPhotosCommand { get; set; }
        private async void ViewPhotosClicked()
        {
            try
            {
                ViewModelLocator.qcDefectVM.QCPhotos.Clear();
                var messages = Messages;
                foreach (var msg in messages)
                {
                    if (msg.OriginalImage != null)
                    {
                        ViewModelLocator.qcDefectVM.QCPhotos.Add(msg.OriginalImage);
                    }
                }
                if (ViewModelLocator.qcDefectVM.QCPhotos != null && ViewModelLocator.qcDefectVM.QCPhotos.Count > 0)
                {
                    await Navigation.PushAsync(new MaterialTrack.Pages.ViewQCPhoto());
                }
                else
                {
                    //await ViewModelLocator.chatVM.DisplaySnackBar("No Photos", Enums.PageActions.None, Enums.MessageActions.Warning, Page, Enums.PageType.Modal, null);
                    ViewModelLocator.chatVM.DisplaySnackBar("No Photos", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void UpdateSeenBy()
        {
            try
            {
                string username = string.Empty;
                if (Application.Current.Properties.ContainsKey("user_name"))
                    username = Application.Current.Properties["user_name"] as string;
                if (Messages != null && Messages.Count > 0)
                {
                    var seenmsgs = Messages.Where(m => m.SeenUsers != null && m.SeenUsers.Count > 0 && m.SeenUsers.Contains(username.ToLower())).ToList();
                    List<ChatMessage> unSeenmsgs = null;
                    if (seenmsgs != null && seenmsgs.Count > 0)
                        unSeenmsgs = Messages.Except(seenmsgs).ToList();
                    else
                    {
                        unSeenmsgs = Messages.ToList();
                    }
                    if (unSeenmsgs != null && unSeenmsgs.Count > 0)
                    {
                        List<ChatMessage> unseenMessages = new List<ChatMessage>();
                        foreach (var unmsgs in unSeenmsgs)
                        {
                            //unmsgs.HasImage = false;
                            //unmsgs.Image = null;
                            unseenMessages.Add(unmsgs);
                        }

                        Task.Run(() => ApiClient.Instance.PutChat(username, unseenMessages)).ContinueWith((t) =>
                        {
                            if (t.Result.status == 0)
                            {
                                foreach (var msg in unSeenmsgs)
                                {
                                    if (!string.IsNullOrEmpty(msg.ThumbnailImagebase64) || !string.IsNullOrEmpty(msg.ThumbnailUrl))
                                    {
                                        msg.HasImage = true;
                                    }
                                    if (msg.SeenUsers == null)
                                        msg.SeenUsers = new List<string>();
                                    msg.SeenUsers.Add(username);
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ChatVM()
        {
            try
            {
                if (Application.Current.Properties.ContainsKey("user_name"))
                    _userName = Application.Current.Properties["user_name"] as string;

                SendMessageCommand = new Command(SendMessageClicked);
                AddPhotoCommand = new Command(AddPhotoClicked);
                SendChatAuditCommand = new Command(SendChatAuditClicked);
                ViewPhotosCommand = new Command(ViewPhotosClicked);
                CloseChatCommand = new Command(() => { Navigation.PopModalAsync(); });
                //UpdateProperty();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
