using astorTrackPAPI;
using MaterialTrackApp.DB;
using MaterialTrackApp.Interface;
using MaterialTrackApp.PartialView;
using MaterialTrackApp.Utility;
using MaterialTrackApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

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

        public static astorTrackPAPIHelper astorTrackAPI
        {
            get;
            private set;
        }

        public static readonly string TOPIC_LOADING_RESULT = "MaterialTrackApp.LOADING_RESULT";

        private readonly string ASTORWORK_API_END_POINT = "http://brian-hp-snc3e7s/astorWorkDev/api/";
        private readonly string MT_API_END_POINT = "192.168.1.149";
        private readonly string PB_API_END_POINT = "https://api.powerbi.com/v1.0/{0}/";

        public static string PB_TENANT_ID = "3156e991-a773-429e-a59a-df6faa02e474";
        public static string PB_USER_NAME = "powerbiadmin@astoriasolutions.com";
        public static string PB_USER_PWD = "Acpl123!@#";
        public static string PBClientId = "7ad390e8-ac68-4c32-aca5-cb00da9ff42f";
        public static string PBClientSecret = "HA29yEdxUaFYHJgHcBzVlITPtcm9t41XaTy/3U+v8oI=";
        public static string PB_NATIVE_CLIENT_ID = "902c893f-8436-4ffb-9697-368cd96d4291";

        public App()
        {
            InitializeComponent();

            // Init API
            astorTrackAPI = new astorTrackPAPIHelper();
            //astorTrackAPI.Endpoint = string.Format("http://{0}/astorTrackPAPI", MT_API_END_POINT);
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

            Task.Run(() => LocalDBHandler.Instance.InitDB("astorWorkApp.db3"));                       

            mainPage = new MainPage();
            indexPage = new IndexPage();

            if (Current.Properties.ContainsKey("user_name"))
                MainPage = new NavigationPage(mainPage);
            else
                MainPage = indexPage;
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
    }
}
