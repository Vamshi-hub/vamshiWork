using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SiteScanRFID : ContentPage
    {
        Task<string[]> scanTask;
        List<string> listTrackerTags;
        CancellationTokenSource tokenSource;
        uint scanTimeOut = 10;

        public SiteScanRFID()
        {
            InitializeComponent();

            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                object power = 20;
                Xamarin.Forms.Application.Current.Properties.TryGetValue("c72_power", out power);
                App.scannerRFID.SetPower((int)power);
                App.scannerRFID.SubscribeKeyEvent(OnScannerKeyPressed);

                object timeOut = 10;
                Xamarin.Forms.Application.Current.Properties.TryGetValue("scan_timeout_seconds", out timeOut);
                uint.TryParse(timeOut.ToString(), out scanTimeOut);
            }
            else
            {
                DisplayAlert("Error", "Scanner not ready", "OK");
            }
        }

        void OnSelectButtonClicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var tracker = btn.BindingContext as Tracker;

            ViewModelLocator.siteUpdateStageVM.Material = tracker.material;
            if (tracker.material.IsOpenQCCase)
            {
                ViewModelLocator.siteListDefectsVM.StageAuditId = tracker.material.stageId;
                ViewModelLocator.qcDefectVM.Material = tracker.material;
                Navigation.PushAsync(new SiteListDefects());
            }
            else
            {
                Navigation.PushAsync(new SiteUpdateStage());
            }
        }

        void OnBIMButtonClicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var tracker = btn.BindingContext as Tracker;

            var tokenSource = new CancellationTokenSource();
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() => NavigateToBrowser(tracker), tokenSource.Token, TaskCreationOptions.LongRunning, scheduler);
        }

        private async void NavigateToBrowser(Tracker tracker)
        {
            var host = string.Format(App.ASTORWORK_WEB_HOST, App.Current.Properties["tenant_name"]);
            var url = $"{host}/forge-viewer";
            var forgeTokenResult = await ApiClient.Instance.MTGetForgeToken();
            var forgeTokenJson = forgeTokenResult.data as JObject;
            var forgeToken = forgeTokenJson.Value<string>("access_token");
            var forgeTokenExpireSeconds = forgeTokenJson.Value<int>("expires_in");
            var forgeTokenExpireTime = DateTime.Now.AddSeconds(forgeTokenExpireSeconds);
            var bimViewerPage = new BIMViewer(url, tracker.material.ForgeModelURN, tracker.material.ForgeElementId.Value, forgeTokenExpireTime, forgeToken);

            await Navigation.PushAsync(bimViewerPage);
            // Navigation.RemovePage(App.bimViewerPage);
        }

        void OnScanButtonClicked(object sender, EventArgs e)
        {
            ToggleScanTag();
        }

        void OnDisappearing(object sender, EventArgs e)
        {
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                App.scannerRFID.UnsubscribeKeyEvent(OnScannerKeyPressed);
            }

            ViewModelLocator.siteScanRFIDVM.Reset();
        }

        void OnAppearing(object sender, EventArgs e)
        {
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                App.scannerRFID.SubscribeKeyEvent(OnScannerKeyPressed);
            }

            if(listTrackerTags != null)
            {
                Task.Run(() => GetTrackerAssociations(listTrackerTags.ToArray()));
            }
        }

        void OnScannerKeyPressed(object sender, EventArgs e)
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
            if ((scanTask == null || scanTask.IsCompleted || scanTask.IsCanceled) && ViewModelLocator.siteScanRFIDVM.AllowScan)
            {
                tokenSource = new CancellationTokenSource();
                scanTask = StartScanTagTask();
                if (scanTask != null)
                {
                    icoScan.IsVisible = true;
                    icoScan.RelRotateTo(scanTimeOut * 60, (scanTimeOut) * 1000);
                    scanTask.ContinueWith((t) => GetTrackerAssociations(t.Result)).ContinueWith((t) =>
                    {
                        ViewExtensions.CancelAnimations(icoScan);
                        icoScan.IsVisible = false;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            else
                tokenSource.Cancel();
        }

        private async Task GetTrackerAssociations(string[] tags)
        {
            try
            {
                ViewModelLocator.siteScanRFIDVM.IsLoading = true;
                var result = await ApiClient.Instance.MTGetListTrackerInfo(tags);
                if (result.status == 0)
                {
                    ViewModelLocator.siteScanRFIDVM.AddTrackers(result.data as List<Tracker>);
                }
                else
                {
                    ViewModelLocator.siteScanRFIDVM.ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                ViewModelLocator.siteScanRFIDVM.ErrorMessage = ex.Message;
            }
            ViewModelLocator.siteScanRFIDVM.IsLoading = false;
            ViewModelLocator.siteScanRFIDVM.AllowScan = true;
        }

        private Task<string[]> StartScanTagTask()
        {
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                ViewModelLocator.siteScanRFIDVM.Reset();
               CancellationToken token = tokenSource.Token;
                return Task.Run(() =>
                {
                    ViewModelLocator.siteScanRFIDVM.AllowScan = false;
                    listTrackerTags = new List<string>();
                    byte anti = 0;
                    byte q = 0;

                    DateTime startTime = DateTime.Now;
                    if (App.scannerRFID.StartContinousScan(anti, q))
                    {
                        ViewModelLocator.siteScanRFIDVM.CountTags = 0;

                        while (!token.IsCancellationRequested && (DateTime.Now - startTime) <= TimeSpan.FromSeconds(scanTimeOut))
                        {
                            var tag = App.scannerRFID.GetTagFromBuffer();
                            if (!string.IsNullOrEmpty(tag) && !listTrackerTags.Contains(tag))
                            {
                                listTrackerTags.Add(tag);
                                ViewModelLocator.siteScanRFIDVM.CountTags++;
                            }
                            ViewModelLocator.siteScanRFIDVM.ScanIconRotation += 0.1f;
                        }
                        App.scannerRFID.StopContinousScan();
                    }
                    return listTrackerTags.ToArray();
                }, token);
            }
            else
                return null;
        }
    }
}