using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Toasts;
using System.Linq;

namespace astorTrackP
{
	public partial class EnrollMarkingNo : ContentPage
	{
        EnrollMarkingNoViewModel _vm;
        public EnrollMarkingNo()
        {
            InitializeComponent();
            _vm = new EnrollMarkingNoViewModel(this.Navigation);            
            BindingContext = _vm;    
            NavigationPage.SetTitleIcon(this, "inventory.png");
        }
        
        //void Scan()
        //{
        //    lock (_vm)
        //    {
        //        if (_vm.isLoading) return;
        //        _vm.isLoading = true;
        //    }

        //    if (_vm.isScanning) App.rfidReader.StopTagRead();

        //    ScanType = App.SettingsDb.GetItem().RFIDScanDetection;
        //    _vm.isScanning = !_vm.isScanning;
        //    if (_vm.isScanning)
        //    {
        //        bool startInventory = App.rfidReader.StartInventoryTag();
        //        if (startInventory)
        //            Device.BeginInvokeOnMainThread(() => ScanTag());
        //        else
        //            App.rfidReader.StopTagRead();
        //    }
        //    _vm.isLoading = false;
        //}

        //public string ScanType;
        //void ScanTag()
        //{
        //    string strTagID;
        //    while (true)
        //    {
        //        if (_vm.isScanning)
        //        {
        //            strTagID = App.rfidReader.ReadSingleTag();

        //            if (string.IsNullOrEmpty(strTagID))
        //                continue;

        //            //if (strTagID.Length > 9)
        //            //    strTagID = strTagID.Substring(strTagID.Length - 9, 9);

        //            _vm.SearchDocument = strTagID;

        //            _vm.DoSearchCommand();

        //            _vm.isScanning = !_vm.isScanning;
        //            _vm.isLoading = false;
        //            App.rfidReader.StopTagRead();
        //        }
        //        else
        //        {
        //            if (_vm.isScanning)
        //                App.rfidReader.StopTagRead();
        //            break;
        //        }
        //    }
        //}
      
        protected override void OnAppearing()
        {
            //MessagingCenter.Subscribe<App>(this, "OnKeyDown", (s) => {Scan();});
            _vm.DoSearchCommand();
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            //MessagingCenter.Unsubscribe<App>(this, "OnKeyDown");
            base.OnDisappearing();
        }

    }
}

