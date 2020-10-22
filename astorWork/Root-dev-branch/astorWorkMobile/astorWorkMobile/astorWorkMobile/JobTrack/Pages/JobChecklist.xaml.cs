using astorWorkMobile.Shared.Utilities;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobChecklist : ContentPage
    {
        public JobChecklist()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (ViewModelLocator.jobChecklistVM.Job != null)
            {
                ViewModelLocator.jobChecklistVM.ShowChecklist = false;
                ViewModelLocator.jobChecklistVM.IsLoading = true;
                Task.Run(ViewModelLocator.jobChecklistVM.GetChecklist);
            }
            else
            {
                ViewModelLocator.jobChecklistVM.ShowChecklist = false;
                ViewModelLocator.jobChecklistVM.IsLoading = true;
                Task.Run(ViewModelLocator.jobChecklistVM.GetChecklist);
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}