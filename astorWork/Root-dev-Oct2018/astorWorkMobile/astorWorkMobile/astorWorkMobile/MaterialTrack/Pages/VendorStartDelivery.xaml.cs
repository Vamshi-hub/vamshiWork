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
    public partial class VendorStartDelivery : ContentPage
    {
        public VendorStartDelivery()
        {
            InitializeComponent();
        }
       
        async void OnSubmitButtonClicked(Object sender, EventArgs e)
        {
            bool shouldContinue = true;
            if (!ViewModelLocator.vendorStartDeliveryVM.TogglePass.Status)
            {
                shouldContinue = await DisplayAlert("Warning", "QC failed, are you sure?", "Yes", "No");
            }

            if (shouldContinue)
            {
                await Task.Run(QCAndDeliver).ContinueWith((task) => AfterQCAndDeliver(task.Result), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task AfterQCAndDeliver(int status)
        {
            if (status == 0)
            {
                await DisplayAlert("Success", string.Format("{0} with tag {1} has been updated",
                    ViewModelLocator.vendorStartDeliveryVM.TrackerInfo.markingNo,
                    ViewModelLocator.singleScanTrackerVM.Tracker.trackerLabel), 
                    "Done");

                ViewModelLocator.vendorInventoryVM.Reset();
                ViewModelLocator.singleScanTrackerVM.Reset();

                await Navigation.PopAsync();
            }
        }

        private async Task<int> QCAndDeliver()
        {
            int vendorId = int.Parse(App.Current.Properties["vendor_id"].ToString());
            ViewModelLocator.vendorStartDeliveryVM.IsLoading = true;

            var result = await ApiClient.Instance.MTQCAndDeliver(
                vendorId,
                ViewModelLocator.vendorStartDeliveryVM.TrackerInfo.id,
                ViewModelLocator.vendorStartDeliveryVM.SelectedMRF.MrfNo,
                ViewModelLocator.vendorStartDeliveryVM.TogglePass.Status,
                ViewModelLocator.vendorStartDeliveryVM.QCRemarks,
                ViewModelLocator.vendorStartDeliveryVM.SelectedLocation.Id);

            if (result.status != 0)
                ViewModelLocator.vendorStartDeliveryVM.ErrorMessage = result.message;

            ViewModelLocator.vendorStartDeliveryVM.IsLoading = false;

            return result.status;
        }

        void OnPageDisappearing(Object sender, EventArgs e)
        {
            ViewModelLocator.vendorStartDeliveryVM.Reset();
        }
    }
}