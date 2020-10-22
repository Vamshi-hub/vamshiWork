using System;
using Xamarin.Forms;
using Plugin.DeviceInfo;
using Plugin.Toasts;
using astorCableAPI;

namespace astorCable
{
    public class App : Application
    {
        public static astorCableAPIHelper api = new astorCableAPIHelper();        
        public static int RFIDPower = 10;
        public static string RFIDScanDetection;
        public static IRFIDUHFReader rfidReader = null;
        public static IBarcodeReader barcodeReader = null;
        public static DeviceInfo deviceInfo = new DeviceInfo();
        //		public static string EndPoint = "http://192.168.1.89/astorTrackAPI";
        //public static string EndPoint = "http://192.168.0.115/astorTrackAPI";
        //		public static string EndPoint = "http://localhost/astorTrackAPI";
        //		public static string EndPoint = "http://172.30.1.87/astorTrackAPI";
        static LoginDb _loginDb;
        //static DocumentTypeDb _documentTypeDb;
        //      static DocumentDb _documentDb;        
        static ModuleDb _modulesDb;
        static SettingsDb _settingsDb;
        //      static ReceivingSetupDb _receivingSetupDb;
        //static ReleasingSetupDb _releasingSetupDb;
        
        static ObjectMasterDb _objectMasterDb;
        //static RFIDSetupDb _rfidSetupDb;
        //static DocumentItemDb _documentItemDb;
        //static ScanItemDb _scanItemDb;
        //		static ScanItemSerialNoDb _scanItemSerialNoDb;
        //static DocumentSerialNoDb _documentSerialNoDb;
        public static IToastNotificator toast;

        static CableMasterDb _cableMasterDb;
        //static CableDetailDb _cableDetailDb;
        static CableIssueDetailDb _cableIssueDetailDb;
        static LocationMasterDb _locationMasterDb;
        static CableRequestFormDb _CableRequestFormDb;
        static CableInspectionDb _cableInspectionDb;
        public App()
        {
            toast = DependencyService.Get<IToastNotificator>();

            if (rfidReader != null) //for demo
            {
                rfidReader.InitializeReaderAsync().Wait();                
            }
            //
            //
            //			if (barcodeReader != null) {
            ////				System.Threading.Tasks.Task.Factory.StartNew(() => barcodeReader.InitializeReaderAsync ());
            //				barcodeReader.InitializeReaderAsync ().Wait();
            //			}

            //api.Endpoint = EndPoint;

            MainPage = new NavigationPage(new astorCable.LoginPage());
            //MainPage = new TabbedPage(new astorCable.LoginPage());

            //MainPage = new astorCable.Login ();
            // The root page of your application
            //MainPage = new ContentPage {
            //	Content  = new StackLayout {
            //		VerticalOptions = LayoutOptions.Center,
            //		Children = {
            //			new Label {
            //				HorizontalTextAlignment = TextAlignment.Center,
            //				Text = "Welcome to Xamarin Forms!"
            //			}
            //		}
            //	}
            //};

        }



        public static LoginDb LoginDb { get { if (_loginDb == null) { _loginDb = new LoginDb(); } return _loginDb; } }

        public static CableMasterDb CableMasterDb { get { if (_cableMasterDb == null) { _cableMasterDb = new CableMasterDb(); } return _cableMasterDb; } }

        //public static CableDetailDb CableDetailDb { get { if (_cableDetailDb == null) { _cableDetailDb = new CableDetailDb(); } return _cableDetailDb; } }

        public static CableIssueDetailDb CableIssueDetailDb { get { if (_cableIssueDetailDb == null) { _cableIssueDetailDb = new CableIssueDetailDb(); } return _cableIssueDetailDb; } }

        
        //      public static DocumentTypeDb DocumentTypeDb { get { if (_documentTypeDb == null) { _documentTypeDb = new DocumentTypeDb ();} return _documentTypeDb; }}

        //public static DocumentDb DocumentDb {get { if (_documentDb == null) {_documentDb = new DocumentDb ();} return _documentDb; }}


        //public static DocumentItemDb DocumentItemDb {get { if (_documentItemDb == null) {_documentItemDb = new DocumentItemDb ();} return _documentItemDb; }}

        public static SettingsDb SettingsDb { get { if (_settingsDb == null) { _settingsDb = new SettingsDb(); } return _settingsDb; } }

        //      public static ReceivingSetupDb ReceivingSetupDb { get { if (_receivingSetupDb == null) { _receivingSetupDb = new ReceivingSetupDb ();} return _receivingSetupDb; }}

        //public static ReleasingSetupDb ReleasingSetupDb { get { if (_releasingSetupDb == null) { _releasingSetupDb = new ReleasingSetupDb ();} return _releasingSetupDb; }}

        public static ModuleDb ModuleDb { get { if (_modulesDb == null) { _modulesDb = new ModuleDb(); } return _modulesDb; } }

        public static LocationMasterDb LocationMasterDb { get { if (_locationMasterDb == null) { _locationMasterDb = new LocationMasterDb();} return _locationMasterDb; }}

        public static ObjectMasterDb ObjectMasterDb { get { if (_objectMasterDb == null) { _objectMasterDb = new ObjectMasterDb(); } return _objectMasterDb; } }

        public static CableRequestFormDb CableRequestFormDb { get { if (_CableRequestFormDb == null) { _CableRequestFormDb = new CableRequestFormDb(); } return _CableRequestFormDb; } }

        public static CableInspectionDb CableInspectionDb { get { if (_cableInspectionDb == null) { _cableInspectionDb = new CableInspectionDb(); } return _cableInspectionDb; } }

        //public static RFIDSetupDb RFIDSetupDb {get { if (_rfidSetupDb == null) {_rfidSetupDb = new RFIDSetupDb ();}return _rfidSetupDb; }}

        //public static ScanItemDb ScanItemDb {get { if (_scanItemDb == null) {_scanItemDb = new ScanItemDb ();}return _scanItemDb; }}

        //		public static ScanItemSerialNoDb ScanItemSerialNoDb {get { if (_scanItemSerialNoDb == null) {_scanItemSerialNoDb = new ScanItemSerialNoDb ();}return _scanItemSerialNoDb; }}

        //public static DocumentSerialNoDb DocumentSerialNoDb {get { if (_documentSerialNoDb == null) {_documentSerialNoDb = new DocumentSerialNoDb ();}return _documentSerialNoDb; }}

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static async void ShowMessage(ToastNotificationType type, string title, string description)
        {
            await App.toast.Notify(type, title, description, TimeSpan.FromSeconds(2));
        }
    }

}


