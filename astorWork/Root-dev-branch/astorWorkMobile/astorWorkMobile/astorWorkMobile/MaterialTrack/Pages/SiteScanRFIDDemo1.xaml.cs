using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.ViewModels;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SiteScanRFIDDemo1 : ContentPage
    {
        private Task<string[]> scanTask;
        private List<string> listTrackerTags;
        private CancellationTokenSource tokenSource;
        private uint scanTimeOut = 10;

        public SiteScanRFIDDemo1()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                Xamarin.Forms.Application.Current.Properties.TryGetValue("c72_power", out object power);
                App.scannerRFID.SetPower((int)power);
                App.scannerRFID.SubscribeKeyEvent(OnScannerKeyPressed);

                Xamarin.Forms.Application.Current.Properties.TryGetValue("scan_timeout_seconds", out object timeOut);
                uint.TryParse(timeOut.ToString(), out scanTimeOut);
            }
            else
            {
                ViewModelLocator.siteScanDemoVM.DisplaySnackBar("Scanner not ready", Shared.Classes.Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                // DisplayAlert("Error", "Scanner not ready", "OK");
            }
        }

        private void OnSelectButtonClicked(object sender, EventArgs e)
        {
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
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                App.scannerRFID.SubscribeKeyEvent(OnScannerKeyPressed);
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
            if ((scanTask == null || scanTask.IsCompleted || scanTask.IsCanceled) && ViewModelLocator.siteScanDemoVM.AllowScan)
            {
                tokenSource = new CancellationTokenSource();
                scanTask = StartScanTagTask();
                if (scanTask != null)
                {
                    ViewModelLocator.siteScanDemoVM.Materials.Clear();
                    ViewModelLocator.siteScanDemoVM.ShowBox = false;
                    ViewModelLocator.siteScanDemoVM.ShowListMaterial = false;
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
            {
                tokenSource.Cancel();
            }
        }


        private void GetTrackerAssociations(string[] tags)
        {
            ViewModelLocator.siteScanDemoVM.IsLoading = true;
            if (tags.Contains(SiteScanRFIDDemoVM.BoxTag))
            {
                ViewModelLocator.siteScanDemoVM.ShowBox = true;
                foreach (var material in ViewModelLocator.siteScanDemoVM.AllMaterials)
                {
                    ViewModelLocator.siteScanDemoVM.Materials.Add(material);
                }
            }
            else
            {
                for (int i = 0; i < SiteScanRFIDDemoVM.MaterialTags.Length; i++)
                {
                    if (tags.Contains(SiteScanRFIDDemoVM.MaterialTags[i]))
                    {
                        ViewModelLocator.siteScanDemoVM.ShowBox = false;
                        ViewModelLocator.siteScanDemoVM.Materials.Add(ViewModelLocator.siteScanDemoVM.AllMaterials.ElementAt(i));
                    }
                }
            }
            ViewModelLocator.siteScanDemoVM.IsLoading = false;
            ViewModelLocator.siteScanDemoVM.AllowScan = true;
        }

        private Task<string[]> StartScanTagTask()
        {
            if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
            {
                CancellationToken token = tokenSource.Token;
                return Task.Run(() =>
                {
                    ViewModelLocator.siteScanDemoVM.AllowScan = false;
                    listTrackerTags = new List<string>();
                    byte anti = 0;
                    byte q = 0;

                    DateTime startTime = DateTime.Now;
                    if (App.scannerRFID.StartContinousScan(anti, q))
                    {
                        while (!token.IsCancellationRequested && (DateTime.Now - startTime) <= TimeSpan.FromSeconds(scanTimeOut))
                        {
                            var tag = App.scannerRFID.GetTagFromBuffer();
                            if (!string.IsNullOrEmpty(tag) && !listTrackerTags.Contains(tag))
                            {
                                listTrackerTags.Add(tag);
                            }
                            ViewModelLocator.siteScanDemoVM.ScanIconRotation += 0.1f;
                        }
                        App.scannerRFID.StopContinousScan();
                    }
                    return listTrackerTags.ToArray();
                }, token);
            }
            else
            {
                ViewModelLocator.siteScanDemoVM.ErrorMessage = "Fail to start scanner, please reboot";
                return null;
            }
        }

        private void confirmDelivery_Clicked(object sender, EventArgs e)
        {
            foreach (var material in ViewModelLocator.siteScanDemoVM.AllMaterials)
            {
                material.stageName = "Delivered";
                material.stageColour = "#7CFC00";
                material.UpdateTime = DateTime.Now;
            }
            //ViewModelLocator.siteScanDemoVM.ShowBox = false;
            ViewModelLocator.siteScanDemoVM.ShowBoxConfirm = false;
            //ViewModelLocator.siteScanDemoVM.Materials.Clear();
        }

        private void btnUtilise_Clicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            var material = btn.BindingContext as DemoMaterial;

            material.stageName = "Utilized";
            material.stageColour = "#87cefa";
            material.UpdateTime = DateTime.Now;
        }
    }
}