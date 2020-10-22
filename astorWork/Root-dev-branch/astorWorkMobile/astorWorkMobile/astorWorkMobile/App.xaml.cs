using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Diagnostics;

using Xamarin.Forms;

namespace astorWorkMobile
{
    public partial class App : Application
    {
        public static string ASTORWORK_API_END_POINT = string.Empty;
        public static string ASTORWORK_WEB_HOST = string.Empty;
        public static string ASTORWORK_TENANT_DUMMY = string.Empty;
        public static string ASTORWORK_WS_HOST = string.Empty;
        public static string ASTORWORK_SIGNALR_ENDPOINT = string.Empty;
        public static int ASTORWORK_API_TIMEOUT_SECONDS = Constants.SETTING_API_TIMEOUT_SECONDS;
        public static string SEND_GRID_USER_NAME = "azure_f9578f68858cb3f3eed147efa1d76f36@azure.com";
        public static string SEND_GRID_USER_PWD = "abc123456";
        public static string CHAT_PROTOCOL = "astorwork_chat";

        public static IScannerRFID scannerRFID;
        public static double ScreenWidth;
        public static double ScreenHeight;
        public App()
        {
#if DEBUG
            ASTORWORK_API_END_POINT = "http://{0}.astorworkqa.com/api/";
            ASTORWORK_WEB_HOST = "http://{0}.astorworkqa.com";
            ASTORWORK_TENANT_DUMMY = string.Empty;
            ASTORWORK_WS_HOST = "wss://astorwork-rt-dev.azurewebsites.net";
            ASTORWORK_SIGNALR_ENDPOINT = "https://astorworksignalrhub20190613124953.azurewebsites.net/chat";
            //ASTORWORK_TENANT_DUMMY = "localhost";
            ASTORWORK_API_TIMEOUT_SECONDS = 15;
#endif

#if LOCALHOST
            ASTORWORK_TENANT_DUMMY = "localhost";
#endif
            InitializeComponent();

#if BRIAN_DEBUG
            ASTORWORK_TENANT_DUMMY = "localhost";
            ASTORWORK_API_END_POINT = "http://brian-hp2018:9000/api/";
            ASTORWORK_WEB_HOST = "http://brian-hp2018:4200";
            ASTORWORK_WS_HOST = "ws://brian-hp2018:9003";
#elif BRIAN_DEBUG_HOME
            ASTORWORK_TENANT_DUMMY = "localhost";
            ASTORWORK_API_END_POINT = "http://192.168.0.78:9000/api/";
            ASTORWORK_WEB_HOST = "http://192.168.0.78:4200";
#elif BEN_DEBUG
            ASTORWORK_TENANT_DUMMY = "localhost";
            ASTORWORK_API_END_POINT = "http://192.168.1.69:9000/api/";
            ASTORWORK_WEB_HOST = "http://192.168.1.69:4200";
#elif PRAD_DEBUG
            ASTORWORK_TENANT_DUMMY = "localhost";
            ASTORWORK_API_END_POINT = "http://192.168.4.129:9000/api/";
            ASTORWORK_WEB_HOST = "http://192.168.4.129:4200";
            ASTORWORK_API_TIMEOUT_SECONDS = 20;
            ASTORWORK_SIGNALR_ENDPOINT = "https://astorworksignalrhubdev.azurewebsites.net/chat";
#elif QA_RELEASE
            ASTORWORK_API_END_POINT = "http://{0}.astorwork.com/api/";
            ASTORWORK_WEB_HOST = "http://{0}.astorwork.com";
            ASTORWORK_TENANT_DUMMY = string.Empty;
            ASTORWORK_WS_HOST = "wss://astorwork-rt-dev.azurewebsites.net";
            ASTORWORK_SIGNALR_ENDPOINT = "https://astorworksignalrhub20190613124953.azurewebsites.net/chat";
#elif QA_DEBUG
            ASTORWORK_API_END_POINT = "http://{0}.astorworkqa.com/api/";
            ASTORWORK_WEB_HOST = "http://{0}.astorworkqa.com";
            ASTORWORK_TENANT_DUMMY = string.Empty;
            ASTORWORK_WS_HOST = "wss://astorwork-rt-dev.azurewebsites.net";
            ASTORWORK_SIGNALR_ENDPOINT = "https://astorworksignalrhub20190613124953.azurewebsites.net/chat";
#endif
            try
            {
                NavigationPage.SetHasNavigationBar(this, false);

                //ApiClient.Instance.InitClient(string.Format(PB_API_END_POINT, PB_TENANT_ID));
                var apiEndPoint = string.Format(ASTORWORK_API_END_POINT, Current.Properties["tenant_name"]);
                ApiClient.Instance.InitClient(apiEndPoint, ASTORWORK_TENANT_DUMMY, ASTORWORK_API_TIMEOUT_SECONDS);
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Init HTTP client failed");
                Debug.WriteLine(exc.Message);
            }

            bool needLogin = true;
            if (Current.Properties.ContainsKey("access_token_expires") &&
                Current.Properties.ContainsKey("tenant_name") &&
                Current.Properties.ContainsKey("user_id") &&
                Current.Properties.ContainsKey("refresh_token"))
            {
                var tokenExpires = Current.Properties["access_token_expires"] as DateTime?;
                if (tokenExpires.HasValue)
                {
                    int expiresIn = (int)(tokenExpires.Value - DateTime.UtcNow).TotalSeconds;
                    if (expiresIn > 60)
                    {
                        var authResult = new AuthResult
                        {
                            UserID = (Current.Properties["user_id"] as int?).Value,
                            AccessToken = Current.Properties["access_token"] as string,
                            ExpiresIn = expiresIn,
                            RefreshToken = Current.Properties["refresh_token"] as string
                        };
                        needLogin = false;
                        MainPage = new MainContentPage();
                    }
                }
            }

            if (needLogin)
            {
                MainPage = new NavigationPage(new LoginPage());
                /*
                (MainPage as NavigationPage).BarBackgroundColor = Color.FromHex("#373b99");
                */
            }
        }

        protected async override void OnStart()
        {
            // Handle when your app starts
            scannerRFID = DependencyService.Get<IScannerRFID>();
            if (Application.Current.Properties.ContainsKey("tenant_name"))
            {
                await ChatClient.Instance.Connect();
                ChatClient.Instance.GetMessages();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            scannerRFID = DependencyService.Get<IScannerRFID>();

            if (scannerRFID != null)
            {
                scannerRFID.Dispose();
            }
            ApiClient.Instance.ResetClient();
            //ChatClient.Instance.Disconnect();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            scannerRFID = DependencyService.Get<IScannerRFID>();
            if (scannerRFID != null)
            {
                scannerRFID.Init();
            }

            //ChatClient.Instance.InitClient();
            //ChatClient.Instance.Connect();
            if (Current.Properties.ContainsKey("tenant_name"))
            {
                var apiEndPoint = string.Format(ASTORWORK_API_END_POINT, Current.Properties["tenant_name"]);
                ApiClient.Instance.InitClient(apiEndPoint, ASTORWORK_TENANT_DUMMY, ASTORWORK_API_TIMEOUT_SECONDS);
            }
        }
    }
}
