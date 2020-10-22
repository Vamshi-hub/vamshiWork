using astorWorkMobile.JobTrack.ViewModels;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.ObjectModel;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobSchedule : ContentPage
    {
        bool IsScanBtnsVisible;
        public ObservableCollection<string> Items { get; set; }

        public JobSchedule()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var vm = BindingContext as JobScheduleVM;
            vm.QCButtonClicked();
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            ViewModelLocator.jobScheduleVM.DisplaySnackBar("Job Assigned Successfully", Enums.PageActions.None, Enums.MessageActions.Success, null, null);

        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}
