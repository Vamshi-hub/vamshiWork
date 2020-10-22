using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.JobTrack.Entities
{
    public class ChatMessage
    {
        public string TenantName { get; set; }
        public string Header { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string DisplayTimestamp { 
            get 
            {
                return Timestamp.ToString("dd/MM/yyyy HH:mm");
            } 
        }
        private QCPhoto _image { get; set; }
        public QCPhoto Image
        {
            get
            {
                if (HasImage)
                {
                    if (!string.IsNullOrEmpty(ThumbnailImagebase64))
                    {
                        _image = new QCPhoto()
                        {
                            ImageBase64 = ThumbnailImagebase64,
                            Remarks = Message
                        };
                    }
                    else if(!string.IsNullOrEmpty(ThumbnailUrl))
                    {
                        _image = new QCPhoto()
                        {
                            Url = ThumbnailUrl,
                            Remarks = Message
                        };
                    }
                    else
                    {
                        _image = null;
                    }
                }
                else
                {
                    _image = null;
                }
                return _image;
            }
        }
        public QCPhoto OriginalImage
        {
            get
            {
                QCPhoto originalImage = new QCPhoto();
                if (HasImage)
                {

                    if (!string.IsNullOrEmpty(OriginalImagebase64))
                    {
                        originalImage = new QCPhoto()
                        {
                            ImageBase64 = OriginalImagebase64,
                            Remarks = Message
                        };
                    }
                    else if (!string.IsNullOrEmpty(OriginalAttachmentUrl))
                    {
                        originalImage = new QCPhoto()
                        {
                            Url = OriginalAttachmentUrl,
                            Remarks = Message
                        };
                    }
                    else
                        originalImage = null;
                }
                else
                {
                    originalImage = null;
                }
                return originalImage;
            }
        }
        public string ThumbnailUrl { get; set; }
        public string OriginalAttachmentUrl { get; set; }
        public string ThumbnailImagebase64 { get; set; }
        public string OriginalImagebase64 { get; set; }
        public bool HasImage { get; set; }
        public bool IsSystem { get; set; }
        public int? MaterialID { get; set; }
        public string MarkingNo { get; set; }
        public string ModuleName { get; set; }
        public string JobName { get; set; }
        public int? JobID { get; set; }
        public string ChecklistName { get; set; }
        public int? ChecklistID { get; set; }
        public int? ChecklistItemID { get; set; }
        public string ChecklistItemName { get; set; }
        public List<string> SeenUsers { get; set; }
        public string UserColor
        {
            get
            {
                //if (UserName.Contains("user"))
                //{
                return "#fc0377";
                //}
                //else
                //{
                //    var i = Convert.ToInt32(Application.Current.Properties["entry_point"]);
                //    return userColors[i.ToString()];
                //}
            }
        }
        public ICommand ShowImageCommand { get; set; }
        void ShowImageClicked()
        {
            ViewModelLocator.qcDefectVM.QCPhotos.Clear();
            ViewModelLocator.qcDefectVM.QCPhotos.Add(OriginalImage);
            ViewModelLocator.chatVM.Navigation.PushAsync(new ViewQCPhoto());
        }
        public ChatMessage()
        {
            ShowImageCommand = new Command(ShowImageClicked);
        }
        //static Dictionary<string, string> userColors = new Dictionary<string, string>()
        //{
        //    { "1", "Blue" },
        //    { "2", "Red" },
        //    {"3", "Green" }
        //};
    }

    public class MessageData
    {
        public string TenantName { get; set; }
        public string Header { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ThumbnailUrl { get; set; }
        public string OriginalAttachmentUrl { get; set; }
        public string ThumbnailImagebase64 { get; set; }
        public string OriginalImagebase64 { get; set; }
        public bool HasImage { get; set; }
        public bool IsSystem { get; set; }
        public int? MaterialID { get; set; }
        public string MarkingNo { get; set; }
        public string ModuleName { get; set; }
        public string JobName { get; set; }
        public int? JobID { get; set; }
        public string ChecklistName { get; set; }
        public int? ChecklistID { get; set; }
        public int? ChecklistItemID { get; set; }
        public string ChecklistItemName { get; set; }
    }
}
