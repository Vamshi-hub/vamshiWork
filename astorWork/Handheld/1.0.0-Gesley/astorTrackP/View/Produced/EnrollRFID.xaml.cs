using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using _model = astorTrackPAPIDataModel;
using Plugin.Toasts;

namespace astorTrackP
{
	public partial class EnrollRFID : ContentPage
	{

        EnrollRFIDViewModel _vm;
		public EnrollRFID(MaterialMaster materialMaster)
		{
			InitializeComponent ();
			NavigationPage.SetTitleIcon (this, "produced.png");

			_vm = new EnrollRFIDViewModel(this.Navigation, materialMaster);
            BindingContext = _vm;
        }


		protected override void OnAppearing()
		{
            //App.InitialiazeReader();
            ReadTag.Clear();
            MessagingCenter.Subscribe<App>(this, "OnKeyDown", (s) => { Scan(); });
            MessagingCenter.Subscribe<EnrollRFIDViewModel>(this, "ScanRFID", (s) =>
            {
                Task.Run(() => Device.BeginInvokeOnMainThread(() => Scan()));
            });
            base.OnAppearing();          
        }

        protected override void OnDisappearing()
        {
            if (_vm.isScanning)
                App.rfidReader.StopInventory();

            MessagingCenter.Unsubscribe<App>(this, "OnKeyDown");
            MessagingCenter.Unsubscribe<EnrollRFIDViewModel>(this, "ScanRFID");
            base.OnDisappearing();
        }

        #region Scan
        private void StopInventory()
        {
            App.ShowLoading("Please wait...", Acr.UserDialogs.MaskType.Clear);
            App.rfidReader.StopInventory();
            App.HideLoading();
        }
        void Scan()
        {
            App.ShowLoading("Please wait...", Acr.UserDialogs.MaskType.Clear);
            lock (_vm)
            {
                if (_vm.isLoading) return;
                _vm.isLoading = true;
            }

            if (_vm.isScanning)
                Task.Run(() => StopInventory());

            ScanType = App.SettingsDb.GetItem().RFIDScanDetection;
            _vm.isScanning = !_vm.isScanning;

            App.HideLoading();

            if (_vm.isScanning)
            {
                bool startInventory = App.rfidReader.StartInventoryTag();
                if (startInventory)
                    ScanTag();
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
                    strTagID = await App.rfidReader.ReadSingleTagAsync();

                    if (string.IsNullOrEmpty(strTagID))
                        continue;
                    
                    if (!ReadTag.Exists(e => e.Equals(strTagID.ToString())))
                    {
                        var result = _vm.ProcessTag(strTagID);
                        Java.Lang.Thread.Sleep(500);
                        if (result)
                        {
                            _vm.isScanning = !_vm.isScanning;
                            _vm.isLoading = false;
                            _vm.ScanSaveText = "Save";
                            uxCastingDate.Date = _vm.MaterialMaster.CastingDate.Value;
                            uxLotNo.Text = _vm.MaterialMaster.LotNo.ToString();
                            uxRFIDTagID.Text = _vm.MaterialMaster.RFIDTagID;

                            uxLotNo.IsEnabled = _vm.IsEnabled;                            
                            uxCastingDate.IsEnabled = _vm.IsEnabled;

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
                        App.rfidReader.StopInventory();
                    }
                    break;
                }
            }

            ReadTag.Clear();
        }

        #endregion

        private void uxQA_Tapped(object sender, EventArgs e)
        {
            _vm.OnQACommand();
            uxQA.Source = string.Format("{0}.png'", _vm.MaterialMaster.QAStatus);

        }
    }
}

