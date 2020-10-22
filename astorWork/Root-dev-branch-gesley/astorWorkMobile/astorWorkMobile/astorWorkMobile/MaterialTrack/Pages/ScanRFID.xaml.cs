using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanRFID : ContentPage
    {
        private Task<string[]> scanTask;
        private List<string> listTrackerTags;
        private CancellationTokenSource tokenSource;
        private uint scanTimeOut = 10;

        public ScanRFID()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
            listTrackerTags = new List<string>();
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                Application.Current.Properties.TryGetValue("c72_power", out object power);
                App.scannerRFID.SetPower((int)power);
                App.scannerRFID.SubscribeKeyEvent(OnScannerKeyPressed);

                Application.Current.Properties.TryGetValue("scan_timeout_seconds", out object timeOut);
                uint.TryParse(timeOut.ToString(), out scanTimeOut);
            }
        }

        private void OnScanButtonClicked(object sender, EventArgs e)
        {
            ToggleScanTag();
        }

        private void OnDisappearing(object sender, EventArgs e)
        {
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                App.scannerRFID.UnsubscribeKeyEvent(OnScannerKeyPressed);
            }

            ViewModelLocator.scanTrackerVM.Reset();
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                App.scannerRFID.SubscribeKeyEvent(OnScannerKeyPressed);
            }

            if (listTrackerTags != null)
            {
                Task.Run(() => ViewModelLocator.scanTrackerVM.GetTrackerAssociations(listTrackerTags.ToArray()));
            }
        }

        private void OnScannerKeyPressed(object sender, EventArgs e)
        {
            var evt = e as AndroidKeyEventArgs;
            if (evt.KeyCode == 139 || evt.KeyCode == 280)
            {
                if (evt.RepeatCount == 0)
                {
                    ToggleScanTag();
                }
            }
        }

        private void ToggleScanTag()
        {
            if ((scanTask == null || scanTask.IsCompleted || scanTask.IsCanceled) && ViewModelLocator.scanTrackerVM.AllowScan)
            {
                tokenSource = new CancellationTokenSource();
                scanTask = StartScanTagTask();
                if (scanTask != null)
                {
                    icoScan.IsVisible = true;
                    icoScan.RelRotateTo(scanTimeOut * 60, (scanTimeOut) * 1000);
                    scanTask.ContinueWith((t) =>
                    ViewModelLocator.scanTrackerVM.GetTrackerAssociations(t.Result))
                    .ContinueWith((t) =>
                    {
                        ViewExtensions.CancelAnimations(icoScan);
                        icoScan.IsVisible = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            else
            {
                tokenSource.Cancel();
            }
        }

        private Task<string[]> StartScanTagTask()
        {
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                CancellationToken token = tokenSource.Token;
                return Task.Run(() =>
                {
                    ViewModelLocator.scanTrackerVM.AllowScan = false;
                    byte anti = 0;
                    byte q = 0;

                    DateTime startTime = DateTime.Now;

                    if (App.scannerRFID.StartContinousScan(anti, q))
                    {
                        ViewModelLocator.scanTrackerVM.CountTags = 0;
                        while (!token.IsCancellationRequested && (DateTime.Now - startTime) <= TimeSpan.FromSeconds(scanTimeOut))
                        {
                            var tag = App.scannerRFID.GetTagFromBuffer();
                            if (!string.IsNullOrEmpty(tag) && !listTrackerTags.Contains(tag))
                            {
                                listTrackerTags.Add(tag);
                                ViewModelLocator.scanTrackerVM.CountTags++;
                            }
                            ViewModelLocator.scanTrackerVM.ScanIconRotation += 0.1f;
                        }
                        App.scannerRFID.StopContinousScan();
                    }
                    return listTrackerTags.ToArray();
                }, token);
            }
            else
            {
                return null;
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}