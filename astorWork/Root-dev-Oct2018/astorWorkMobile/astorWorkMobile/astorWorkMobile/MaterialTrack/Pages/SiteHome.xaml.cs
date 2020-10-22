using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.ViewModels;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SiteHome : ContentPage
    {
        public SiteHome()
        {
            InitializeComponent();
        }

        // Disable back button so that it won't go back to BIM viewer
        /*
        protected override bool OnBackButtonPressed()
        {
            Navigation.PopToRootAsync();
            return true;
        }
        */

        private void OnScanQRCodeClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SiteScanQRCode());
        }

        private void OnScanRFIDClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SiteScanRFID());

            // ViewModelLocator.siteScanDemoVM = new SiteScanRFIDDemoVM();
            // Navigation.PushAsync(new SiteScanRFIDDemo1());
        }
    }
}