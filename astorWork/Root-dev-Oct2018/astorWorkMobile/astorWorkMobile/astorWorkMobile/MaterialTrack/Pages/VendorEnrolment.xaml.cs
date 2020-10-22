using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VendorEnrolment : ContentPage
    {
        public VendorEnrolment()
        {
            InitializeComponent();
        }

        void OnEnrolButtonClicked(Object sender, EventArgs e)
        {
            if (ViewModelLocator.vendorEnrolmentVM.SN <= ViewModelLocator.vendorEnrolmentVM.MaxSN)
            {
                DisplayAlert("Error", "Serial No. cannot be less than " + ViewModelLocator.vendorEnrolmentVM.MaxSN, "OK");
            }
            else
            {
                int vendorId = int.Parse(Application.Current.Properties["vendor_id"].ToString());
                ViewModelLocator.vendorEnrolmentVM.IsLoading = true;
                Task.Run(() => CreateInventory(vendorId)).ContinueWith((task) => AfterEnrolled(task.Result), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        void OnMarkingNoTapped(Object sender, ItemTappedEventArgs e)
        {
            ViewModelLocator.vendorEnrolmentVM.SetMarkingNo(e.Item as string);
        }

        private async Task AfterEnrolled(int status)
        {
            if (status == 0)
            {
                var shouldContinue = await DisplayAlert("Success", string.Format("{0} is associated with tag {1}",
            ViewModelLocator.vendorEnrolmentVM.MarkingNo,
            ViewModelLocator.singleScanTrackerVM.Tracker.trackerLabel), "Start Delivery", "Done");

                if (shouldContinue)
                {
                    ViewModelLocator.singleScanTrackerVM.Tracker.inventory = new Entities.Inventory
                    {
                        markingNo = ViewModelLocator.vendorEnrolmentVM.MarkingNo,
                        sn = ViewModelLocator.vendorEnrolmentVM.SN,
                        castingDate = ViewModelLocator.vendorEnrolmentVM.CastingDate
                    };
                    ViewModelLocator.vendorStartDeliveryVM.TrackerInfo = ViewModelLocator.singleScanTrackerVM.Tracker;
                    ViewModelLocator.vendorStartDeliveryVM.Reset();
                    await Navigation.PushAsync(new VendorStartDelivery());
                    Navigation.RemovePage(this);
                }
                else
                {
                    ViewModelLocator.vendorInventoryVM.Reset();
                    //ViewModelLocator.singleScanTrackerVM.Reset();
                    await Navigation.PopAsync();
                }
            }
        }

        private async Task<int> CreateInventory(int vendorId)
        {
            var result = await ApiClient.Instance.MTCreateInventory(
                ViewModelLocator.vendorEnrolmentVM.Project.id,
                vendorId,
                ViewModelLocator.singleScanTrackerVM.Tracker.id,
                ViewModelLocator.vendorEnrolmentVM.MarkingNo,
                ViewModelLocator.vendorEnrolmentVM.SN,
                ViewModelLocator.vendorEnrolmentVM.CastingDate);

            if (result.status != 0)
                ViewModelLocator.vendorEnrolmentVM.ErrorMessage = result.message;

            ViewModelLocator.vendorEnrolmentVM.IsLoading = false;

            return result.status;
        }

        void OnPageDisappearing(Object sender, EventArgs e)
        {
            ViewModelLocator.vendorEnrolmentVM.Reset();
        }
    }
}