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
    public partial class DeliveredRFID : ContentPage
    {

        DeliveredRFIDViewModel _vm;
        public DeliveredRFID()
        {
            InitializeComponent();
            NavigationPage.SetTitleIcon(this, "delivered.png");

            _vm = new DeliveredRFIDViewModel(this.Navigation, this);
            BindingContext = _vm;
            
        }


        protected override void OnAppearing()
        {
            
            base.OnAppearing();

            MessagingCenter.Subscribe<App>(this, "OnKeyDown", (s) => { Scan(); });
            MessagingCenter.Subscribe<DeliveredRFIDViewModel>(this, "ScanRFID", (s) =>
            {
                Task.Run(() => Device.BeginInvokeOnMainThread(()=>Scan()));
                //Task.Run(() => Scan());
                //Task scanTask = Task.Run(() => Scan());
                //scanTask.Wait();
            });
            GetCount();
            
            _vm.GetLocation();
            if (_vm.Location == "" || _vm.Location == null)
            {
                App.ShowMessage(ToastNotificationType.Warning, "Delivery Location", "Please select default location...");
            }
            //else
            //{   
            //    var item = this.ToolbarItems.Where(w => w.Icon == "location.png").FirstOrDefault();
            //    this.ToolbarItems.Remove(item);
            //    //Task.Run(() => Scan());
            //}
        }

        //private void StopTagRead()
        //{
        //    App.ShowLoading("Closing Reader...", Acr.UserDialogs.MaskType.None);
        //    App.rfidReader.StopTagRead();
        //    App.HideLoading();
        //}
        
        protected override void OnDisappearing()
        {
            if (_vm.isScanning)
                App.rfidReader.StopInventory();
            MessagingCenter.Unsubscribe<App>(this, "OnKeyDown");
            MessagingCenter.Unsubscribe<DeliveredRFIDViewModel>(this, "ScanRFID");

            base.OnDisappearing();

        }

        //private void InitialiazeReader()
        //{
        //    //App.ShowLoading("Initializing Reader...", Acr.UserDialogs.MaskType.Clear);
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
            TagList = _vm.GetCount();
            
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
            CancellationTokenSource cts = new CancellationTokenSource();
            ct = cts.Token;
            //Task.Run(() => ImageResize());

            App.ShowLoading("Please wait...", Acr.UserDialogs.MaskType.Clear);
            if (App.LoginDb.GetItem().DeliveredAssociationID == 0)
            {
                App.ShowMessage(ToastNotificationType.Warning, "Delivery Location", "Please select default location...");
                return;
            }
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

            App.HideLoading();

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
                await Navigation.PushAsync(new DeliveredList());
                TagList.Clear();
                ReadTag.Clear();
                _vm.RFIDCount1 = 0;
                _vm.RFIDCount = 0;
            }
        }

        #endregion

        private async void RFIDCount_Tapped(object sender, EventArgs e)
        {
            if (TagList.Count() > 0)
            {
                await Navigation.PushAsync(new DeliveredList());
                TagList.Clear();
                _vm.RFIDCount = 0;
            }
        }

        private void Location_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new DeliveredLocation());
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

