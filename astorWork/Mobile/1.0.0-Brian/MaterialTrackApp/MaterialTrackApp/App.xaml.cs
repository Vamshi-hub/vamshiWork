using MaterialTrackApp.Interface;
using MaterialTrackApp.PartialView;
using MaterialTrackApp.Utility;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using MaterialTrackApp.Class;

namespace MaterialTrackApp
{
    public partial class App : Application
    {
        public static MainPage mainPage
        {
            get;
            private set;
        }
        public static IndexPage indexPage
        {
            get;
            private set;
        }

        public static readonly string TOPIC_LOADING_RESULT = "MaterialTrackApp.LOADING_RESULT";

        private static string ASTORWORK_API_END_POINT = "http://Brian-HP-SNC3E7S/astorWorkMVC/api/";

        public App()
        {
#if __QA__
            ASTORWORK_API_END_POINT = "http://astorwork-qa.cloudapp.net/astorWorkMVC/api/";
#endif
            InitializeComponent();
            // Dummy init of API client
            try
            {
                //ApiClient.Instance.InitClient(string.Format(PB_API_END_POINT, PB_TENANT_ID));
                ApiClient.Instance.InitClient(ASTORWORK_API_END_POINT);
            }
            catch (Exception exc)
            {
                Debug.WriteLine("Init HTTP client failed");
                Debug.WriteLine(exc.Message);
            }

            var dbInited = Task.Run(() => LocalDBHandler.Instance.InitDB("astorWorkApp.db3")).Result;

            if (dbInited)
            {
                mainPage = new MainPage();
                indexPage = new IndexPage();

                if (Current.Properties.ContainsKey("user_name"))
                {
                    ViewModelLocator.homePageVM.CanManualUpdate =
                        Current.Properties["user_role"].Equals(Constants.ROLE_SITE_SUPERVISOR);
                    MainPage = new NavigationPage(mainPage);
                }
                else
                    MainPage = indexPage;
            }
            else
            {
                MainPage.DisplayAlert("Alert", "Fail to initialize the application", "OK");
                var applicationHelper = DependencyService.Get<IApplicationHelper>();
                if (applicationHelper != null)
                    applicationHelper.closeApplication();
            }
        }

        public static void InvokeLoadingPage(Page prevPage, Page nextPage, string message, bool replaceMainPage)
        {
            var loadingPage = new LoadingPage();
            loadingPage.InitLoadingPage(prevPage, nextPage, message, replaceMainPage);
            if (replaceMainPage)
                Current.MainPage = loadingPage;
            else
                Current.MainPage.Navigation.PushAsync(loadingPage);
        }

        public static void InvokeLoadingView(Page prevPage, Layout<View> layoutParent, Task loadingTask, string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var loadingView = new LoadingView(message);
                layoutParent.Children.Add(loadingView);
                layoutParent.RaiseChild(loadingView);
                Task.Run(() => loadingView.WaitForLoadingComplete(layoutParent, loadingTask));
            });
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
