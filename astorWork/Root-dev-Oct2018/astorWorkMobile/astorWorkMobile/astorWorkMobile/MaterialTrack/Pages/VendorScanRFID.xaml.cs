using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VendorScanRFID : ContentPage
    {
        private string currTag;

        public VendorScanRFID()
        {
            InitializeComponent();
            ViewModelLocator.singleScanTrackerVM.Reset();
            ViewModelLocator.singleScanTrackerVM.Navigation = Navigation;

            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                object power = 10;
                Application.Current.Properties.TryGetValue("c72_power", out power);
                App.scannerRFID.SetPower((int)power);
                App.scannerRFID.SubscribeKeyEvent(OnScannerKeyPressed);
            }
            else
            {
                DisplayAlert("Error", "Scanner not ready", "OK");
            }
        }

        void OnDisappearing(object sender, EventArgs e)
        {
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                App.scannerRFID.UnsubscribeKeyEvent(OnScannerKeyPressed);
            }
        }

        void OnAppearing(object sender, EventArgs e)
        {
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                App.scannerRFID.SubscribeKeyEvent(OnScannerKeyPressed);
            }

            if (!string.IsNullOrEmpty(currTag))
            {
                Task.Run(() => ViewModelLocator.singleScanTrackerVM.SetTag(currTag));
            }
        }

        void OnScanButtonClicked(object sender, EventArgs e)
        {
            ScanTag();
        }

        void OnScannerKeyPressed(object sender, EventArgs e)
        {
            var evt = e as AndroidKeyEventArgs;
            if (evt.KeyCode == 139 || evt.KeyCode == 280)
            {
                if (evt.RepeatCount == 0)
                {
                    ScanTag();
                }
            }
        }

        private void ScanTag()
        {
            if (App.scannerRFID != null && ViewModelLocator.singleScanTrackerVM.AllowScan)
            {
                ViewModelLocator.singleScanTrackerVM.AllowScan = false;
                ViewModelLocator.singleScanTrackerVM.IsLoading = true;

                icoScan.IsVisible = true;
                icoScan.RelRotateTo(180, 3000);

                Task.Run(() => App.scannerRFID.GetSingleTag()).ContinueWith((task) =>
                {
                    currTag = task.Result;
                    ViewModelLocator.singleScanTrackerVM.SetTag(task.Result);
                }).ContinueWith((t) =>
                {
                    ViewExtensions.CancelAnimations(icoScan);
                    icoScan.IsVisible = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private async Task GetTrackerAssociation(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                // If the association history already has the tag, and the last API call is within 5 minutes
                // Then bypass the API call
                var tracker = ViewModelLocator.singleScanTrackerVM.ListTrackers.Where(t => t.tag == tag).FirstOrDefault();
                if (tracker == null || (DateTime.Now - tracker.UpdatedTime) >= TimeSpan.FromMinutes(5))
                {
                    ViewModelLocator.singleScanTrackerVM.ListTrackers.Remove(tracker);

                    try
                    {
                        var result = await ApiClient.Instance.MTGetListTrackerInfo(new string[] { tag });
                        if (result.status == 0)
                        {
                            var trackers = result.data as List<Tracker>;
                            if (trackers.Count > 0)
                            {
                                ViewModelLocator.singleScanTrackerVM.Tracker = trackers.ElementAt(0);
                                ViewModelLocator.singleScanTrackerVM.Tracker.UpdatedTime = DateTime.Now;
                                ViewModelLocator.singleScanTrackerVM.ListTrackers.Add(ViewModelLocator.singleScanTrackerVM.Tracker);
                            }
                            else
                            {
                                ViewModelLocator.singleScanTrackerVM.ErrorMessage = "This tracker is not in our system";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ViewModelLocator.singleScanTrackerVM.ErrorMessage = ex.Message;
                    }
                }
                else
                {
                    ViewModelLocator.singleScanTrackerVM.Tracker = tracker;
                }
            }
            else
            {
                ViewModelLocator.singleScanTrackerVM.ErrorMessage = "No RFID tag found";
            }
            ViewModelLocator.singleScanTrackerVM.IsLoading = false;
            ViewModelLocator.singleScanTrackerVM.AllowScan = true;
        }
    }
}