using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainconHome : ContentPage
    {
        public MainconHome()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
            if (int.Parse(Application.Current.Properties["entry_point"].ToString()) == 2 || int.Parse(Application.Current.Properties["entry_point"].ToString()) == 1)
            {
                btnQCInspection.IsVisible = false;
            }
        }

        // Disable back button so that it won't go back to BIM viewer
        /*
        protected override bool OnBackButtonPressed()
        {
            Navigation.PopToRootAsync();
            return true;
        }
        */

        private async void OnScanQRCodeClicked(object sender, EventArgs e)
        {
            await Task.Run(ViewModelLocator.mainconHomeVM.GetCameraPermission)
                      .ContinueWith(async (t) =>
                      {
                          ViewModelLocator.scanTrackerVM.CameraReady = t.Result;
                          if (t.Result)
                              await Navigation.PushAsync(new ScanQRCode());
                          else
                              ViewModelLocator.scanTrackerVM.DisplaySnackBar("No camera available", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                      },
                      TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnScanRFIDClicked(object sender, EventArgs e)
        {
            if (App.scannerRFID == null || App.scannerRFID != null && !App.scannerRFID.InitSuccess())
            {
                ViewModelLocator.scanTrackerVM.DisplaySnackBar("Scanner not ready", Shared.Classes.Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
            }
            else if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                Navigation.PushAsync(new ScanRFID());
            }
            // DisplayAlert("Error", "Scanner not ready", "OK");

            // ViewModelLocator.siteScanDemoVM = new SiteScanRFIDDemoVM();
            // Navigation.PushAsync(new SiteScanRFIDDemo1());
        }
        private void OnQCInspectionClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new MaterialInspection());
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}