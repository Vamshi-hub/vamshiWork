using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using _model = astorTrackPAPIDataModel;
using Plugin.Toasts;
using Java.Lang;
using System.Threading;

namespace astorTrackP
{
	public partial class InstalledRFID : ContentPage
	{

        InstalledRFIDViewModel _vm;
		public InstalledRFID()
		{
			InitializeComponent ();
			NavigationPage.SetTitleIcon (this, "installed.png");

            _vm = new InstalledRFIDViewModel(this.Navigation, this);
            BindingContext = _vm;           
            
        }


		protected override void OnAppearing()
		{
            base.OnAppearing();

            MessagingCenter.Subscribe<App>(this, "OnKeyDown", (s) => { Scan(); });
            MessagingCenter.Subscribe<InstalledRFIDViewModel>(this, "ScanRFID", (s) =>
            {
                Task.Run(() => Device.BeginInvokeOnMainThread(() => Scan()));
            });

            GetCount();
            
            //Task.Run(() => Scan());
        }

        //private void InitialiazeReader()
        //{
        //    App.ShowLoading("Initializing Reader...", Acr.UserDialogs.MaskType.Clear);
        //    //var isInitialized = App.rfidReader.ReaderInitialized();
        //    //if (!isInitialized)
        //    //    isInitialized = App.InitializeRFID();

        //    bool startInventory = App.rfidReader.StartInventoryTag();
        //    if (!startInventory)
        //        App.rfidReader.StopTagRead();
        //    App.HideLoading();
        //}

        private void GetCount()
        {
            TagList = App.MaterialMasterDb.GetMaterialMasters("Installed").Select(s => s.RFIDTagID).ToList();
            _vm.RFIDCount = TagList.Count();
        }

        protected override void OnDisappearing()
        {
            if (_vm.isScanning)
                App.rfidReader.StopInventory();
            MessagingCenter.Unsubscribe<App>(this, "OnKeyDown");
            MessagingCenter.Unsubscribe<InstalledRFIDViewModel>(this, "ScanRFID");

            base.OnDisappearing();

        }

        #region Scan
        private void ImageResize()
        {
            uxScan.ScaleTo(0.90, 100, Easing.CubicOut);
            uxScan.ScaleTo(1, 100, Easing.CubicIn);
        }
        private void StopInventory()
        {
            App.ShowLoading("Please wait...", Acr.UserDialogs.MaskType.Clear);
            App.rfidReader.StopInventory();
            App.HideLoading();
        }

        CancellationToken ct;
        void Scan()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            ct = cts.Token;
            //Task.Run(() => ImageResize());

            lock (_vm)
            {
                if (_vm.isLoading) return;
                _vm.isLoading = true;
            }

            if (_vm.isScanning)
            {
                cts.Cancel();
                Task.Run(() => StopInventory());
            }

            ScanType = App.SettingsDb.GetItem().RFIDScanDetection;
            _vm.isScanning = !_vm.isScanning;

            if (_vm.isScanning)
            {
                bool startInventory = App.rfidReader.StartInventoryTag();
                if (startInventory)
                {
                    Task.Run(() => RotateElement());
                    ScanTag();
                }
                else
                    Task.Run(() => StopInventory());
            }
            _vm.isLoading = false;
        }

        List<string> TagList = new List<string>();
        List<string> ReadTag = new List<string>();
        public string ScanType;
        async void ScanTag()
        {
            string strTagID;
            while (_vm.isScanning)
            {
                if (_vm.isScanning)
                {
                    _vm.RFIDCount1++;
                    strTagID = await App.rfidReader.ReadSingleTagAsync();
                    if (string.IsNullOrEmpty(strTagID))
                        continue;

                    if (strTagID.Length > 0)
                        if (!TagList.Exists(e => e.Equals(strTagID.ToString())) && !ReadTag.Exists(e => e.Equals(strTagID.ToString())))
                        {
                            var result = _vm.ProcessTag(strTagID);
                            Thread.Sleep(500);
                            if (result)
                            {
                                TagList.Add(strTagID);
                                _vm.RFIDCount = TagList.Count();
                            }
                            else
                                ReadTag.Add(strTagID);
                        }
                }
                else
                {
                    if (_vm.isScanning)
                    {
                        App.rfidReader.StopInventory();
                    }
                    break;
                }
            }

            if (TagList.Count() > 0)
            {
                await Navigation.PushAsync(new InstalledList());

                TagList.Clear();
                ReadTag.Clear();

                _vm.RFIDCount = 0;
                _vm.RFIDCount1 = 0;
            } 
        }

        #endregion

        private async void RFIDCount_Tapped(object sender, EventArgs e)
        {
            if (TagList.Count() > 0)
            {
                if (_vm.isScanning)
                    await Task.Run(() => StopInventory());

                await Navigation.PushAsync(new InstalledList());

                TagList.Clear();
                _vm.RFIDCount = 0;
            }
        }

        private async Task RotateElement()
        {
            while (!ct.IsCancellationRequested)
            {
                await uxProgress.RotateTo(360, 800, Easing.Linear);
                await uxProgress.RotateTo(0, 0); // reset to initial position
            }
        }
    }
}

