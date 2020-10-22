using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RTOHome : ContentPage
    {
        bool IsScanBtnsVisible;
        public ObservableCollection<string> Items { get; set; }

        public RTOHome()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //var vm = BindingContext as RTOHomeVM;
            //var jobSchedule = e.Item as JobMaster;
            //vm.ShowHideMaterial(jobSchedule);
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            IsScanBtnsVisible = false;
            btnQRScan.IsVisible = false;
            //btnRFIDScan.IsVisible = false;
            ViewModelLocator.rtoHomeVM.JTGetJobScheduleForRTO(0);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //var vm = BindingContext as RTOHomeVM;
            //vm.QCButtonClicked();
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            //DisplayAlert("Success", "Job Assigned Successfully", "OK");

        }

        private async void BtnScan_Clicked(object sender, EventArgs e)
        {
            if (IsScanBtnsVisible)
            {
                btnQRScan.IsVisible = false;
                //    btnRFIDScan.IsVisible = false;
                IsScanBtnsVisible = false;
            }
            else
            {
                // btnRFIDScan.IsVisible = true;
                //  await btnRFIDScan.ScaleTo(1.25, 100);
                //  await btnRFIDScan.ScaleTo(1, 100);

                btnQRScan.IsVisible = true;
                await btnQRScan.ScaleTo(1.25, 100);
                await btnQRScan.ScaleTo(1, 100);

                IsScanBtnsVisible = true;
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}
