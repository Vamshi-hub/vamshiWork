using astorWorkMobile.JobTrack.Entities;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile.Shared.Utilities
{
    public sealed class ChatClient : INotifyPropertyChanged
    {
        private static volatile ChatClient instance;
        private static object syncRoot = new Object();
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public static ChatClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new ChatClient();
                        }
                    }
                }

                return instance;
            }
        }
        private HubConnection connection;
        private MessageData _message;
        public MessageData Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }
        private ObservableCollection<ChatMessage> _jobMessages { get; set; }
        public ObservableCollection<ChatMessage> JobMessages { get => _jobMessages; set { _jobMessages = value; OnPropertyChanged("JobMessages"); } }
        public ObservableCollection<MessageData> ChecklistItemMessages { get; set; }

        private bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            set => Device.BeginInvokeOnMainThread(() =>
                 {
                     isConnected = value;
                     OnPropertyChanged("IsConnected");
                 });
        }
        public Command SendMessageCommand { get; }
        public Command ConnectCommand { get; }
        public Command DisconnectCommand { get; }

        private Random random;
        private ChatClient()
        {
            JobMessages = new ObservableCollection<ChatMessage>();
            SendMessageCommand = new Command(async () => await SendMessageAsync());
            ConnectCommand = new Command(async () => await Connect());
            DisconnectCommand = new Command(async () => await Disconnect());
            random = new Random();
            connection = new HubConnectionBuilder()
                .WithUrl(App.ASTORWORK_SIGNALR_ENDPOINT).Build();
            //.WithUrl("https://astorworksignalrhub20190613124953.azurewebsites.net/chat").Build();
            //connection = new HubConnectionBuilder()
            //   .WithUrl("http://tenant1.astorworkqa.com/signalrhub/chat").Build();
            //connection = new HubConnectionBuilder()
            //   .WithUrl("http://192.168.1.114:5000/chat").Build();

            connection.Closed += async (error) =>
            {
                MessageData Message = new MessageData();
                Message.Message = "Connection closed";
                Message.IsSystem = true;
                //string username = string.Empty;
                //if (Application.Current.Properties.ContainsKey("user_name"))
                //    username = Application.Current.Properties["user_name"] as string;
                //if (username != Message.UserName)
                //{
                //    SendLocalMessage(Message);
                //}
                IsConnected = false;
                await Task.Delay(random.Next(0, 5) * 1000);
                await Connect();
            };
            connection.On<MessageData>("SendMessage", (message) =>
            {
                string username = string.Empty;
                if (Application.Current.Properties.ContainsKey("user_name"))
                    username = Application.Current.Properties["user_name"] as string;
                if (username != Message.UserName)
                {
                    SendLocalMessage(Message);
                }
                //SendLocalMessage(message);
            });
        }

        public async Task SendMessageAsync()
        {
            try
            {
                SendLocalMessage(Message).Wait();
                string username = string.Empty;
                if (Application.Current.Properties.ContainsKey("user_name"))
                {
                    username = Application.Current.Properties["user_name"] as string;
                    PostChat(username, Message);
                    await connection.InvokeAsync("SendMessage", Message);
                }

            }
            catch (Exception ex)
            {
                MessageData msg = new MessageData();
                msg.Message = $"send failed: {ex.Message}";
                msg.IsSystem = true;
                //SendLocalMessage(msg);
            }
        }

        public async Task Connect()
        {
            try
            {
                await connection.StartAsync();
                IsConnected = true;
                //MessageData msg = new MessageData();
                //msg.Message = "Connected..";
                //msg.IsSystem = true;
                //SendLocalMessage(msg);
            }
            catch (Exception ex)
            {
                MessageData msg = new MessageData();
                msg.Message = $"Connection error: {ex.Message}";
                msg.IsSystem = true;
                //SendLocalMessage(msg);
            }
        }
        public async Task Disconnect()
        {
            await connection.StopAsync();
            IsConnected = false;
            MessageData msg = new MessageData();
            msg.Message = "Disconnected..";
            msg.IsSystem = true;
            //SendLocalMessage(msg);
        }
        public async Task Close()
        {
            await connection.StopAsync();
        }
        public void GetMessages()
        {
            JobMessages = new ObservableCollection<ChatMessage>();
            Task.Run(ApiClient.Instance.GetChats).ContinueWith((t) =>
            {
                if (t.Result.status == 0)
                {
                    var chats = t.Result.data as List<ChatMessage>;
                    chats.ForEach(msg => JobMessages.Add(new ChatMessage()
                    {
                        Header = msg.Header,
                        Message = msg.Message?.Replace('_', ' '),
                        UserName = msg.UserName,
                        Timestamp = msg.Timestamp.ToLocalTime(),
                        MaterialID = msg.MaterialID,
                        MarkingNo = msg.MarkingNo,
                        ModuleName = msg.ModuleName,
                        JobID = msg.JobID,
                        JobName = msg.JobName,
                        ChecklistID = msg.ChecklistID,
                        ChecklistName = msg.ChecklistName,
                        ChecklistItemID = msg.ChecklistItemID,
                        ChecklistItemName = msg.ChecklistItemName,
                        IsSystem = msg.IsSystem,
                        HasImage = msg.HasImage,
                        ThumbnailImagebase64 = msg.ThumbnailImagebase64,
                        ThumbnailUrl = msg.ThumbnailUrl,
                        OriginalImagebase64 = string.Empty,
                        OriginalAttachmentUrl = msg.OriginalAttachmentUrl,
                        SeenUsers = msg.SeenUsers
                    }));
                }
            });


        }
        public bool IsChatOpen { get; set; }
        public bool IschecklistItemPageOpen { get; set; }
        private async Task SendLocalMessage(MessageData msg)
        {
            try
            {
                ChatMessage massage;
                Device.BeginInvokeOnMainThread(() =>
                {
                    massage = new ChatMessage
                    {
                        Header = msg.Header,
                        Message = msg.Message?.Replace('_', ' '),
                        UserName = msg.UserName,
                        Timestamp = msg.Timestamp.ToLocalTime(),
                        MaterialID = msg.MaterialID,
                        MarkingNo = msg.MarkingNo,
                        ModuleName = msg.ModuleName,
                        JobID = msg.JobID,
                        JobName = msg.JobName,
                        ChecklistID = msg.ChecklistID,
                        ChecklistName = msg.ChecklistName,
                        ChecklistItemID = msg.ChecklistItemID,
                        ChecklistItemName = msg.ChecklistItemName,
                        IsSystem = msg.IsSystem,
                        HasImage = msg.HasImage,
                        ThumbnailImagebase64 = msg.ThumbnailImagebase64,
                        ThumbnailUrl = msg.ThumbnailUrl,
                        OriginalAttachmentUrl = msg.OriginalAttachmentUrl,
                        OriginalImagebase64 = msg.OriginalImagebase64
                    };

                    string username = string.Empty;
                    if (Application.Current.Properties.ContainsKey("user_name"))
                        username = Application.Current.Properties["user_name"] as string;
                    if (!string.IsNullOrEmpty(username))
                    {
                        if (massage.SeenUsers == null)
                            massage.SeenUsers = new List<string>();
                        if (IsChatOpen)
                        {
                            massage.SeenUsers.Add(username);
                        }
                        else
                        {
                            massage.SeenUsers = null;
                        }
                    }
                    JobMessages.Add(massage);
                    OnPropertyChanged("JobMessages");
                    //if (IsChatOpen)
                    //{
                    ViewModelLocator.chatVM.UpdateProperty();
                    //}
                    if (IschecklistItemPageOpen)
                    {
                        ViewModelLocator.jobChecklistItemVM.UpdateProperties();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageData msg1 = new MessageData();
                msg1.Message = $"Connection error: {ex.Message}";
                msg1.IsSystem = true;
            }
        }

        private async void PostChat(string username, MessageData msg)
        {
            if (msg != null && !string.IsNullOrEmpty(msg.UserName) && username == msg.UserName)
            {
                ChatMessage message = new ChatMessage
                {
                    Header = msg.Header,
                    Message = msg.Message,
                    UserName = msg.UserName,
                    Timestamp = DateTime.UtcNow,
                    MaterialID = msg.MaterialID,
                    MarkingNo = msg.MarkingNo,
                    ModuleName = msg.ModuleName,
                    JobID = msg.JobID,
                    JobName = msg.JobName,
                    ChecklistID = msg.ChecklistID,
                    ChecklistName = msg.ChecklistName,
                    ChecklistItemID = msg.ChecklistItemID,
                    ChecklistItemName = msg.ChecklistItemName,
                    IsSystem = msg.IsSystem,
                    HasImage = msg.HasImage,
                    ThumbnailImagebase64 = msg.ThumbnailImagebase64,
                    ThumbnailUrl = msg.ThumbnailUrl,

                };
                await Task.Run(async () =>
                {
                    var result = await ApiClient.Instance.PostChat(username, message);
                    if (result.status == 0 && msg.HasImage && !string.IsNullOrEmpty(msg.OriginalImagebase64))
                    {
                        var chatID = Convert.ToInt32(result.data);
                        var status = await ApiClient.Instance.PutChatPhoto(msg.OriginalImagebase64, chatID);
                    }
                });
            }
        }
    }
}
