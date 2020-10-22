using System;
using Xamarin.Forms;
using Plugin.DeviceInfo;
using Plugin.Toasts;
using astorTrackPAPI;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace astorTrackP
{
    public partial class App : Application
    {
        #region Instantiate
        public static astorTrackPAPIHelper api = new astorTrackPAPIHelper();
        public static int RFIDPower = 10;
        public static string RFIDScanDetection;
        public static IRFIDUHFReader rfidReader = null;
        public static IBarcodeReader barcodeReader = null;
        public static DeviceInfo deviceInfo = new DeviceInfo();
        //		public static string EndPoint = "http://192.168.1.89/astorTrackAPI";
        //public static string EndPoint = "http://192.168.0.115/astorTrackAPI";
        //		public static string EndPoint = "http://172.30.1.87/astorTrackAPI";
        //public static string EndPoint = "http://astortrack.cloudapp.net/astorTrackAPI";
        public static string EndPoint = "http://astortrack.cloudapp.net/astorWorkAPI_Demo";
        static LoginDb _loginDb;
        static ModuleDb _modulesDb;
        static SettingsDb _settingsDb;
        static ObjectMasterDb _objectMasterDb;
        public static IToastNotificator toast;
        static MaterialMasterDb _MaterialMasterDb;
        static LocationMasterDb _locationMasterDb;
        #endregion

        public App()
        {
            toast = DependencyService.Get<IToastNotificator>();            
            MainPage = new NavigationPage(new astorTrackP.LoginPage());
            //MainPage = new NavigationPage(new astorTrackP.Dashboard());
            //InitializeReader();
            //Task.Run(() => InitializeReader());
        }

        #region Db

        public static LoginDb LoginDb { get { if (_loginDb == null) { _loginDb = new LoginDb(); } return _loginDb; } }

        public static MaterialMasterDb MaterialMasterDb { get { if (_MaterialMasterDb == null) { _MaterialMasterDb = new MaterialMasterDb(); } return _MaterialMasterDb; } }

        public static SettingsDb SettingsDb { get { if (_settingsDb == null) { _settingsDb = new SettingsDb(); } return _settingsDb; } }

        public static ModuleDb ModuleDb { get { if (_modulesDb == null) { _modulesDb = new ModuleDb(); } return _modulesDb; } }

        public static LocationMasterDb LocationMasterDb { get { if (_locationMasterDb == null) { _locationMasterDb = new LocationMasterDb(); } return _locationMasterDb; } }

        public static ObjectMasterDb ObjectMasterDb { get { if (_objectMasterDb == null) { _objectMasterDb = new ObjectMasterDb(); } return _objectMasterDb; } }

        #endregion

        public static void ShowMessage(ToastNotificationType type, string title, string description)
        {
            UserDialogs.Instance.Alert(description, title, "OK");
            //App.toast.Notify(type, title, description, TimeSpan.FromSeconds(2));
        }

        public static void ShowLoading(string title, MaskType maskType)
        {
            UserDialogs.Instance.ShowLoading(title, maskType);            
        }

        public static void HideLoading()
        {
            UserDialogs.Instance.HideLoading();
        }

        protected override void OnResume()
        {
            base.OnResume();
           
        }

        public static void GetMaterialList(string status)
        {
            App.ShowLoading("Syncing...", Acr.UserDialogs.MaskType.Clear);
            App.MaterialMasterDb.ClearMaterialMaster();

            var data = App.api.GetMaterialMasters(status);
            if (data != null)
            {
                foreach (var item in data)
                {
                    App.MaterialMasterDb.InsertMaterialMaster(new MaterialMaster
                    {
                        MaterialNo = item.MaterialNo,
                        MarkingNo = item.MarkingNo,
                        Project = item.Project,
                        Block = item.Block,
                        Level = item.Level,
                        Zone = item.Zone,
                        DrawingNo = item.DrawingNo,
                        MaterialType = item.MaterialType,
                        MaterialSize = item.MaterialSize,
                        EstimatedLength = item.EstimatedLength,
                        ActualLength = item.ActualLength,
                        Status = item.Status,
                        LocationID = item.LocationID,
                        Contractor = item.Contractor,
                        MRFNo = item.MRFNo,
                        RFIDTagID = item.RFIDTagID,
                        Location = item.Project + " > " + item.Block + " > " + item.Level + " > " + item.Zone,
                        DeliveryDate = item.DeliveryDate,
                        DeliveryRemarks = item.DeliveryRemarks,
                        DeliveredLocation = item.Location,
                        Remarks = item.Remarks,
                        QAStatus = "check",
                        LotNo = item.LotNo,
                        CastingDate = item.CastingDate
                    });
                }
            }

            App.HideLoading();
        }

        public static void GetMaterialMasterMarkingNo(bool IsActive)
        {
            App.ShowLoading("Syncing...", Acr.UserDialogs.MaskType.Clear);
            App.MaterialMasterDb.ClearMaterialMasterMarkingNo();

            var data = App.api.GetMaterialMasterMarkingNo(IsActive);
            if (data != null)
            {
                foreach (var item in data)
                {
                    App.MaterialMasterDb.InsertMaterialMasterMarkingNo(new MaterialMasterMarkingNo
                    {
                        ID = item.ID,
                        MarkingNo = item.MarkingNo,
                        MaterialType = item.MaterialType,
                        Prefix = item.Prefix,
                        Counter = item.Counter,
                        RFIDTagID = item.RFIDTagID,
                        BeaconID = item.BeaconID,
                        LotNo = item.LotNo,
                        CastingDate = item.CastingDate,
                        Project = "BPN6C12"
                    });
                }
            }

            App.HideLoading();
        }

        public static void GetMaterialMasterDashboard()
        {
            App.ShowLoading("Syncing...", Acr.UserDialogs.MaskType.Clear);
            App.MaterialMasterDb.ClearMaterialMasterDashboard();

            var data = App.api.GetMaterialMasterDashboard();
            if (data != null)
            {
                foreach (var item in data)
                {
                    App.MaterialMasterDb.InsertMaterialMasterDashboard(new MaterialMasterDashboard
                    {
                        Progress = item.Progress,
                        Produced = int.Parse(item.Produced),
                        Delivered = int.Parse(item.Delivered),
                        Installed = int.Parse(item.Installed),
                    });
                }
            }

            App.HideLoading();
        }

        //public static bool InitializeRFID()
        //{
        //    bool result = false;

        //    if (rfidReader != null)
        //    {
        //        result = rfidReader.InitializeReaderAsync().Result;

        //        var rfid = App.SettingsDb.GetItem();
        //        if (rfid != null)
        //            if (rfid.RFIDPower > 0)
        //                App.rfidReader.SetRFIDPowerAsync(rfid.RFIDPower);

        //    }
        //    return result;
        //}

        //public static void CloseReader()
        //{
        //    if (App.rfidReader != null)
        //        App.rfidReader.CloseReader();
        //}

        //public static void InitializeReader()
        //{   
        //    var isInitialized = App.rfidReader.ReaderInitialized();
        //    if (!isInitialized)
        //    {
        //        ShowLoading("initializing reader...", Acr.UserDialogs.MaskType.Clear);
        //        //isInitialized = App.InitializeRFID();
        //        var result = rfidReader.InitializeReaderAsync().Result;
        //        var rfid = App.SettingsDb.GetItem();
        //        if (rfid != null)
        //            if (rfid.RFIDPower > 0)
        //                App.rfidReader.SetRFIDPowerAsync(rfid.RFIDPower);
        //        HideLoading();
        //    }

        //}
    }

}


