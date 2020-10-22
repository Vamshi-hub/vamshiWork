using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Distribute;
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

        const string LogTag = "astorWorkAppCenter";
        public const string androidKey = "a840fcd3-d36b-4619-8486-f3d15773d667";
        public const string iosKey = "69aa05de-d798-4a63-802f-9341dc85ca82";

        public static IScannerRFID scannerRFID;
        public static double ScreenWidth;
        public static double ScreenHeight;

        static App()
        {
            Crashes.SendingErrorReport += SendingErrorReportHandler;
            Crashes.SentErrorReport += SentErrorReportHandler;
            Crashes.FailedToSendErrorReport += FailedToSendErrorReportHandler;
        }
        public App()
        {
#if DEBUG
            ASTORWORK_API_END_POINT = "https://{0}.astorworkqa.com/api/";
            ASTORWORK_WEB_HOST = "https://{0}.astorworkqa.com";
            ASTORWORK_TENANT_DUMMY = string.Empty;
            ASTORWORK_WS_HOST = "wss://astorwork-rt-dev.azurewebsites.net";
            ASTORWORK_SIGNALR_ENDPOINT = "https://astorworksignalrhub20190613124953.azurewebsites.net/chat";
            //ASTORWORK_TENANT_DUMMY = "localhost";
            ASTORWORK_API_TIMEOUT_SECONDS = 15;    
#elif RELEASE
            ASTORWORK_API_END_POINT = "https://{0}.astorwork.com/api/";
            ASTORWORK_WEB_HOST = "https://{0}.astorwork.com";
            ASTORWORK_TENANT_DUMMY = string.Empty;
            ASTORWORK_WS_HOST = "wss://astorwork-rt-dev.azurewebsites.net";
            ASTORWORK_SIGNALR_ENDPOINT = "https://astorworksignalrhub20190613124953.azurewebsites.net/chat";
            ASTORWORK_API_TIMEOUT_SECONDS = 15;
#elif QA_RELEASE
            ASTORWORK_API_END_POINT = "https://{0}.astorworkqa.com/api/";
            ASTORWORK_WEB_HOST = "https://{0}.astorworkqa.com";
            ASTORWORK_TENANT_DUMMY = string.Empty;
            ASTORWORK_WS_HOST = "wss://astorwork-rt-dev.azurewebsites.net";
            ASTORWORK_SIGNALR_ENDPOINT = "https://astorworksignalrhub20190613124953.azurewebsites.net/chat";
#elif QA_DEBUG
            ASTORWORK_API_END_POINT = "https://{0}.astorworkqa.com/api/";
            ASTORWORK_WEB_HOST = "https://{0}.astorworkqa.com";
            ASTORWORK_TENANT_DUMMY = string.Empty;
            ASTORWORK_WS_HOST = "wss://astorwork-rt-dev.azurewebsites.net";
            ASTORWORK_SIGNALR_ENDPOINT = "https://astorworksignalrhub20190613124953.azurewebsites.net/chat";
#endif
            InitializeComponent();
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
            }
        }

        protected async override void OnStart()
        {
            AppCenter.LogLevel = LogLevel.Verbose;
            Crashes.ShouldAwaitUserConfirmation = ConfirmationHandler;
            //Distribute.ReleaseAvailable = OnReleaseAvailable;

            AppCenter.Start($"android={androidKey};ios={iosKey}",
                               typeof(Analytics),
                               //typeof(Distribute),
                               typeof(Crashes)
                               );
            await Crashes.HasCrashedInLastSessionAsync().ContinueWith(hasCrashed =>
            {
                AppCenterLog.Info(LogTag, "Crashes.HasCrashedInLastSession=" + hasCrashed.Result);
            });
            await Crashes.GetLastSessionCrashReportAsync().ContinueWith(report =>
            {
                AppCenterLog.Info(LogTag, "Crashes.LastSessionCrashReport.Exception=" + report.Result?.StackTrace);
            });
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                scannerRFID = DependencyService.Get<IScannerRFID>();
            }
            if (Application.Current.Properties.ContainsKey("tenant_name"))
            {
                await ChatClient.Instance.Connect();
                ChatClient.Instance.GetMessages();
            }
        }

        protected async override void OnSleep()
        {
            // Handle when your app sleeps
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                scannerRFID = DependencyService.Get<IScannerRFID>();

                if (scannerRFID != null)
                {
                    scannerRFID.Dispose();
                }
            }
            ApiClient.Instance.ResetClient();
            await ChatClient.Instance.Disconnect();
        }

        protected async override void OnResume()
        {
            // Handle when your app resumes 
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
            {
                scannerRFID = DependencyService.Get<IScannerRFID>();
                if (scannerRFID != null)
                {
                    scannerRFID.Init();
                }
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
                    if (expiresIn > 0)
                    {
                        needLogin = false;
                    }
                }
            }
            else
            {
                needLogin = false; // yet to login
            }

            if (needLogin)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    Current.MainPage.DisplayAlert("Session Expired", "Your session has expired, Please login again.", "OK");
                });

                ViewModelLocator.loginVM.Reset();
                ViewModelLocator.vendorHomeVM.Reset();
                ViewModelLocator.scanTrackerVM.Reset();
                Current.Properties.Clear();
                await Current.SavePropertiesAsync();
                Current.MainPage = new NavigationPage(new LoginPage());
            }
            else
            {
                if (Current.Properties.ContainsKey("tenant_name"))
                {
                    var apiEndPoint = string.Format(ASTORWORK_API_END_POINT, Current.Properties["tenant_name"]);
                    ApiClient.Instance.InitClient(apiEndPoint, ASTORWORK_TENANT_DUMMY, ASTORWORK_API_TIMEOUT_SECONDS);

                    await ChatClient.Instance.Connect();
                    ChatClient.Instance.GetMessages();
                }
            }
        }

        static void SendingErrorReportHandler(object sender, SendingErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sending error report");

            var args = e as SendingErrorReportEventArgs;
            ErrorReport report = args.Report;

            if (report.StackTrace != null)
            {
                AppCenterLog.Info(LogTag, report.StackTrace.ToString());
            }
            else if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }
        }

        static void SentErrorReportHandler(object sender, SentErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Sent error report");

            var args = e as SentErrorReportEventArgs;
            ErrorReport report = args.Report;

            if (report.StackTrace != null)
            {
                AppCenterLog.Info(LogTag, report.StackTrace.ToString());
            }
            else
            {
                AppCenterLog.Info(LogTag, "No system exception was found");
            }

            if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }
        }

        static void FailedToSendErrorReportHandler(object sender, FailedToSendErrorReportEventArgs e)
        {
            AppCenterLog.Info(LogTag, "Failed to send error report");

            var args = e as FailedToSendErrorReportEventArgs;
            ErrorReport report = args.Report;

            //test some values
            if (report.StackTrace != null)
            {
                AppCenterLog.Info(LogTag, report.StackTrace.ToString());
            }
            else if (report.AndroidDetails != null)
            {
                AppCenterLog.Info(LogTag, report.AndroidDetails.ThreadName);
            }

            if (e.Exception != null)
            {
                AppCenterLog.Info(LogTag, "There is an exception associated with the failure");
            }
        }

        bool ConfirmationHandler()
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Current.MainPage.DisplayActionSheet("Crash detected. Send crash report?", null, null, "Send", "Always Send", "Don't Send").ContinueWith((arg) =>
                {
                    var answer = arg.Result;
                    UserConfirmation userConfirmationSelection;
                    if (answer == "Send")
                    {
                        userConfirmationSelection = UserConfirmation.Send;
                    }
                    else if (answer == "Always Send")
                    {
                        userConfirmationSelection = UserConfirmation.AlwaysSend;
                    }
                    else
                    {
                        userConfirmationSelection = UserConfirmation.DontSend;
                    }
                    AppCenterLog.Debug(LogTag, "User selected confirmation option: \"" + answer + "\"");
                    Crashes.NotifyUserConfirmation(userConfirmationSelection);
                });
            });

            return true;
        }

        bool OnReleaseAvailable(ReleaseDetails releaseDetails)
        {
            AppCenterLog.Info("AppCenterDemo", "OnReleaseAvailable id=" + releaseDetails.Id
                                            + " version=" + releaseDetails.Version
                                            + " releaseNotesUrl=" + releaseDetails.ReleaseNotesUrl);
            var custom = releaseDetails.ReleaseNotes?.ToLowerInvariant().Contains("custom") ?? false;
            if (custom)
            {
                var title = "Version " + releaseDetails.ShortVersion + " available!";
                Task answer;
                if (releaseDetails.MandatoryUpdate)
                {
                    answer = Current.MainPage.DisplayAlert(title, releaseDetails.ReleaseNotes, "Update now!");
                }
                else
                {
                    answer = Current.MainPage.DisplayAlert(title, releaseDetails.ReleaseNotes, "Update now!", "Maybe tomorrow...");
                }
                answer.ContinueWith((task) =>
                {
                    if (releaseDetails.MandatoryUpdate || (task as Task<bool>).Result)
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Update);
                    }
                    else
                    {
                        Distribute.NotifyUpdateAction(UpdateAction.Postpone);
                    }
                });
            }
            return custom;
        }
    }
}
