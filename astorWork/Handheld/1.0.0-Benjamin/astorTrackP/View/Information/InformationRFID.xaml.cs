using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using _model = astorTrackPAPIDataModel;
using Plugin.Toasts;
using System.Threading;

namespace astorTrackP
{
	public partial class InformationRFID : ContentPage
	{

        InformationRFIDViewModel _vm;
		public InformationRFID()
		{
			InitializeComponent ();
			NavigationPage.SetTitleIcon (this, "information.png");            
            _vm = new InformationRFIDViewModel(this.Navigation);
            BindingContext = _vm;
            //NavigationPage.SetHasBackButton(this, false);
        }


		protected override void OnAppearing()
		{
            MessagingCenter.Subscribe<App>(this, "OnKeyDown", (s) => { Scan(); });
            MessagingCenter.Subscribe<InformationRFIDViewModel>(this, "ScanRFID", (s) =>
            {
                Task.Run(() => Device.BeginInvokeOnMainThread(() => Scan()));
            });            
            base.OnAppearing();
            _vm.MaterialNo = null;
            //Scan();
        }
        
        protected override void OnDisappearing()
        {
            if (_vm.isScanning)
                App.rfidReader.StopInventory();
            MessagingCenter.Unsubscribe<App>(this, "OnKeyDown");
            MessagingCenter.Unsubscribe<InformationRFIDViewModel>(this, "ScanRFID");            
            base.OnDisappearing();
        }

        #region Scan
        private void StopInventory()
        {
            App.ShowLoading("Please wait...", Acr.UserDialogs.MaskType.Clear);
            App.rfidReader.StopInventory();
            App.HideLoading();
        }
        private void ImageResize()
        {
            uxScan.ScaleTo(0.90, 100, Easing.CubicOut);
            uxScan.ScaleTo(1, 100, Easing.CubicIn);
        }

        CancellationToken ct;
        void Scan() 
        {
            //Task.Run(() => ImageResize());
            CancellationTokenSource cts = new CancellationTokenSource();
            ct = cts.Token;

            lock (_vm)
            {
                if (_vm.isLoading) return;
                _vm.isLoading = true;
            }

            if (_vm.isScanning)
            {
                cts.Cancel();
                Task.Run(() => StopInventory());
                Navigation.PopAsync();
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

                   
                    strTagID = await App.rfidReader.ReadSingleTagAsync(); //<-- this is running on the background
                    
                    if (string.IsNullOrEmpty(strTagID))
                        continue;

                    if (strTagID.Length > 0)
                        if (!ReadTag.Exists(e => e.Equals(strTagID.ToString())))
                        {
                            var result = _vm.ProcessTag(strTagID);
                            if (result)
                            {
                                _vm.isScanning = !_vm.isScanning;
                                App.rfidReader.StopInventory();
                                break;
                            }
                            else
                                ReadTag.Add(strTagID);
                        }
                }
                else
                {
                    if (_vm.isScanning)
                    {
                        _vm.isScanning = !_vm.isScanning;
                        App.rfidReader.StopInventory();
                    }
                    break;
                }
            }

            ReadTag.Clear();
            _vm.RFIDCount1 = 0;

            if (_vm.MaterialNo != null)
            {
                Device.BeginInvokeOnMainThread(() =>
                Navigation.PushAsync(new InformationView(_vm.MaterialNo)));
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

        #endregion

    }
}

