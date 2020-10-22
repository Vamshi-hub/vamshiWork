using astorWorkMobile.Shared.Utilities;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SiteScanQRCode : ContentPage
    {
        private string currTag = string.Empty;

        public SiteScanQRCode()
        {
            ViewModelLocator.singleScanTrackerVM.Reset();
            ViewModelLocator.singleScanTrackerVM.Navigation = Navigation;
            InitializeComponent();
        }

        private void scanView_OnScanResult(ZXing.Result result)
        {
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