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

        public static IScannerRFID scannerRFID;

        public App()
        {
#if DEBUG
            ASTORWORK_TENANT_DUMMY = "localhost";
#endif

#if LOCALHOST
            ASTORWORK_TENANT_DUMMY = "localhost";
#endif
            InitializeComponent();

#if BRIAN_DEBUG
            ASTORWORK_TENANT_DUMMY = "localhost";
            ASTORWORK_API_END_POINT = "http://brian-hp2018:9000/api/";
            ASTORWORK_WEB_HOST = "http://brian-hp2018:4200";
            //ASTORWORK_API_END_POINT = "http://192.168.0.134:9000/api/";
            //ASTORWORK_WEB_HOST = "http://192.168.0.134:4200";
#elif BEN_DEBUG
            ASTORWORK_TENANT_DUMMY = "localhost";
            //ASTORWORK_API_END_POINT = "http://brian-hp2018:9000/api/";
            //ASTORWORK_WEB_HOST = "http://brian-hp2018:4200";
            ASTORWORK_API_END_POINT = "http://192.168.1.69:9000/api/";
            ASTORWORK_WEB_HOST = "http://192.168.1.69:4200";
#elif QA_RELEASE
            ASTORWORK_API_END_POINT = "http://{0}.astorworkqa.com/api/";
            ASTORWORK_WEB_HOST = "http://{0}.astorworkqa.com";
            ASTORWORK_TENANT_DUMMY = string.Empty;
#elif QA_DEBUG
            ASTORWORK_API_END_POINT = "http://{0}.astorworkqa.com/api/";
            ASTORWORK_WEB_HOST = "http://{0}.astorworkqa.com";
            ASTORWORK_TENANT_DUMMY = string.Empty;
#endif
            bool needLogin = true;
            // Pre-load the browser page

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
                            UserID = Current.Properties["user_id"] as string,
                            AccessToken = Current.Properties["access_token"] as string,
                            ExpiresIn = expiresIn,
                            RefreshToken = Current.Properties["refresh_token"] as string
                        };
                        try
                        {
                            //ApiClient.Instance.InitClient(string.Format(PB_API_END_POINT, PB_TENANT_ID));
                            var apiEndPoint = string.Format(ASTORWORK_API_END_POINT, Current.Properties["tenant_name"]);
                            ApiClient.Instance.InitClient(apiEndPoint, ASTORWORK_TENANT_DUMMY);
                            needLogin = false;
                            MainPage = new MainContentPage(authResult.Payload.Role.MobileEntryPoint);
                        }
                        catch (Exception exc)
                        {
                            Debug.WriteLine("Init HTTP client failed");
                            Debug.WriteLine(exc.Message);
                        }
                    }
                }
            }

            if (needLogin)
            {
                MainPage = new NavigationPage(new MainPage());
                (MainPage as NavigationPage).BarBackgroundColor = Color.FromHex("#373b99");
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
            scannerRFID = DependencyService.Get<IScannerRFID>();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            scannerRFID = DependencyService.Get<IScannerRFID>();

            if (scannerRFID != null)
            {
                scannerRFID.Dispose();
            }
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
            scannerRFID = DependencyService.Get<IScannerRFID>();
            scannerRFID.Init();
        }
    }
}
