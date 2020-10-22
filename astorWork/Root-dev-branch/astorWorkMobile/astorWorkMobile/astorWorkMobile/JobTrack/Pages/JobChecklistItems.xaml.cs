using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobChecklistItems : ContentPage
    {
        public JobChecklistItems()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            ChatClient.Instance.IschecklistItemPageOpen = true;
            ViewModelLocator.jobChecklistItemVM.UpdateProperties();
        }
        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            ChatClient.Instance.IschecklistItemPageOpen = false;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}