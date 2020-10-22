using astorWorkMobile.JobTrack.ViewModels;
using astorWorkMobile.Shared.Utilities;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Chat : ContentPage
    {
        public ICommand ScrollListCommand { get; set; }
        public Chat()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
            ViewModelLocator.chatVM.TextToSend = string.Empty;
            ViewModelLocator.chatVM.UpdateSeenBy();
        }
        private void ChatList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // don't do anything if we just de-selected the row.
            if (e.Item == null) return;
            // Optionally pause a bit to allow the preselect hint.
            Task.Delay(500);
            // Deselect the item.
            if (sender is ListView lv) lv.SelectedItem = null;
        }
        private void ContentPage_Appearing(object sender, System.EventArgs e)
        {
            MessagingCenter.Subscribe<ChatVM>(this, "SCROLL_BOTTOM", (obj) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (ViewModelLocator.chatVM.Messages != null && ViewModelLocator.chatVM.Messages.Count > 0)
                    {
                        Task.Delay(100);
                        ChatList.ScrollTo(ViewModelLocator.chatVM.Messages[ViewModelLocator.chatVM.Messages.Count - 1], ScrollToPosition.End, false);
                    }
                });
            });

            ChatClient.Instance.IsChatOpen = true;
            //ViewModelLocator.chatVM.OnPropertyChanged("Messages");
        }
        private void ContentPage_Disappearing(object sender, System.EventArgs e)
        {
            ChatClient.Instance.IsChatOpen = false;
        }
    }
}