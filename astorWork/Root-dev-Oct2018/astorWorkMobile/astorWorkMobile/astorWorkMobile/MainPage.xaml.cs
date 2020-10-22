using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            /*
            ViewModelLocator.loginVM.IsLoading = true;
            Task.Run(RefreshSession).ContinueWith(t => ProcessAuthResult(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
            */
            InitializeComponent();
        }

        private void LoginButton_Clicked(object sender, EventArgs e)
        {
            string userName = ViewModelLocator.loginVM.UserName;
            string password = ViewModelLocator.loginVM.Password;
            string tenantName = ViewModelLocator.loginVM.TenantName;
            ViewModelLocator.loginVM.Reset();

            try
            {
                //ApiClient.Instance.InitClient(string.Format(PB_API_END_POINT, PB_TENANT_ID));
                var apiEndPoint = string.Format(App.ASTORWORK_API_END_POINT, tenantName);
                ApiClient.Instance.InitClient(apiEndPoint, App.ASTORWORK_TENANT_DUMMY);
                Application.Current.Properties["tenant_name"] = tenantName;

                ViewModelLocator.loginVM.IsLoading = true;

                Task.Run(() => ApiClient.Instance.AuthLogin(userName, password)).ContinueWith(t => ProcessAuthResult(t.Result), TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                ViewModelLocator.loginVM.ErrorMessage = "Can't find server";
            }

        }

        private void ProcessAuthResult(ApiClient.ApiResult apiResult)
        {
            bool login = false;
            if (apiResult.status != 0)
                ViewModelLocator.loginVM.ErrorMessage = apiResult.message;
            else
            {
                var authResult = apiResult.data as AuthResult;
                try
                {
                    if (authResult.Payload != null)
                    {
                        Application.Current.SavePropertiesAsync();

                        Navigation.PushAsync(new MainContentPage(authResult.Payload.Role.MobileEntryPoint));
                     
                        login = true;
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            }
            ViewModelLocator.loginVM.IsLoading = false;

            if(!login)
                InitializeComponent();
        }
    }
}
