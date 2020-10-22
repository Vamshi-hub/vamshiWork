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
    public partial class MainconHome : ContentPage
    {
        bool IsScanBtnsVisible;

        public MainconHome()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            ViewModelLocator.jobScheduleVM = new ViewModels.JobScheduleVM();
            ViewModelLocator.mainconHomeVM.IsLoading = true;
            ViewModelLocator.mainconHomeVM.ContentAppearing();
            ViewModelLocator.mainconHomeVM.IsLoading = false;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }

        //private async void BtnScan_Clicked(object sender, EventArgs e)
        //{
        //    if (IsScanBtnsVisible)
        //    {
        //        btnQRScan.IsVisible = false;
        //        btnRFIDScan.IsVisible = false;
        //        IsScanBtnsVisible = false;
        //    }
        //    else
        //    {
        //        btnRFIDScan.IsVisible = true;
        //        await btnRFIDScan.ScaleTo(1.25, 100);
        //        await btnRFIDScan.ScaleTo(1, 100);

        //        btnQRScan.IsVisible = true;
        //        await btnQRScan.ScaleTo(1.25, 100);
        //        await btnQRScan.ScaleTo(1, 100);

        //        IsScanBtnsVisible = true;
        //    }
        //}
    }
}