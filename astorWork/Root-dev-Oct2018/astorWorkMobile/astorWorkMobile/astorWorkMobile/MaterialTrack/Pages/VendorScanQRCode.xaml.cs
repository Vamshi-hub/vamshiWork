using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VendorScanQRCode : ContentPage
    {
        private string currTag = string.Empty;

        public VendorScanQRCode()
        {
            ViewModelLocator.singleScanTrackerVM.Reset();
            ViewModelLocator.singleScanTrackerVM.Navigation = Navigation;
            InitializeComponent();
        }

        private void scanView_OnScanResult(ZXing.Result result)
        {
            // Stop analysis until we navigate away so we don't keep reading barcodes
            currTag = result.Text;
            ViewModelLocator.singleScanTrackerVM.SetTag(result.Text);
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currTag))
            {
                Task.Run(() => ViewModelLocator.singleScanTrackerVM.SetTag(currTag));
            }
        }
    }
}