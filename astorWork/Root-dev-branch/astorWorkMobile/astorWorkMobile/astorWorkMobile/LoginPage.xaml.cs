using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile
{
    public partial class LoginPage : ContentPage
    {
        private bool isLogingin;
        public LoginPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            ViewModelLocator.loginVM.Reset();
            InitializeComponent();
        }

        private void ToggleButtonAnimation(bool animToggle)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                isLogingin = animToggle;
                if (animToggle)
                {
                    btnLogin.IsEnabled = false;
                    btnLogin.BackgroundColor = Color.FromHex("#43a047");
                    lblLogin.Text = "";
                    btnLogin.RotateXTo(180, 200, Easing.Linear);
                    lblLogin.RotateXTo(-180);
                    lblLogin.Text = "Please wait...";
                }
                else
                {
                    btnLogin.IsEnabled = true;
                    btnLogin.BackgroundColor = Color.FromHex("#8196aa");
                    lblLogin.Text = "";
                    btnLogin.RotateXTo(0, 200, Easing.Linear);
                    lblLogin.RotateXTo(0);
                    lblLogin.Text = "Login";
                }
            });
        }
        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            bool status = false;
            if (!isLogingin)
            {
                ToggleButtonAnimation(true);
                string userName = ViewModelLocator.loginVM.UserName.Trim();
                string password = ViewModelLocator.loginVM.Password;
                string tenantName = ViewModelLocator.loginVM.TenantName.Trim();

                try
                {
                    //ApiClient.Instance.InitClient(string.Format(PB_API_END_POINT, PB_TENANT_ID));
                    var apiEndPoint = string.Format(App.ASTORWORK_API_END_POINT, tenantName);
                    //  if (ApiClient.Instance == null)
                    ApiClient.Instance.InitClient(apiEndPoint, App.ASTORWORK_TENANT_DUMMY, App.ASTORWORK_API_TIMEOUT_SECONDS);
                    Application.Current.Properties["tenant_name"] = tenantName;
                    Application.Current.Properties["user_name"] = userName;

                    ViewModelLocator.loginVM.IsLoading = true;

                    var t1 = Task.Run(ApiClient.Instance.GetTenantSettings)
                        .ContinueWith(t => ProcessTenantSettings(t.Result));
                    var t2 = Task.Run(() => ApiClient.Instance.AuthLogin(userName, password)).ContinueWith(async (t) => ProcessAuthResult(t.Result));

                    await Task.WhenAll(t1, t2).ContinueWith(t =>
                    {
                        // Application.Current.SavePropertiesAsync();
                        //Application.Current.MainPage = new MainContentPage();
                        //ViewModelLocator.loginVM.IsLoading = false;
                        ////ChatClient.Instance.Connect();
                        //ChatClient.Instance.GetMessages();
                    });
                    ToggleButtonAnimation(false);
                    ViewModelLocator.loginVM.Reset();
                }
                catch (Exception exc)
                {
                    ToggleButtonAnimation(false);
                    ViewModelLocator.loginVM.Reset();
                    Debug.WriteLine(exc.Message);
                    ViewModelLocator.loginVM.ErrorMessage = exc.Message;
                }
                //ToggleButtonAnimation(false);
            }
        }

        private void ProcessTenantSettings(ApiClient.ApiResult apiResult)
        {
            if (apiResult.status != 0)
            {
                ViewModelLocator.loginVM.ErrorMessage = apiResult.message;
            }
            else
            {
                var tenantSettings = apiResult.data as TenantSettings;
                Application.Current.Properties["enabled_modules"] = tenantSettings.EnabledModules;
            }
        }

        private async Task ProcessAuthResult(ApiClient.ApiResult apiResult)
        {
            if (apiResult.status != 0)
            {
                ViewModelLocator.loginVM.ErrorMessage = apiResult.message;
            }
            else
            {
                var authResult = apiResult.data as AuthResult;
                try
                {
                    if (authResult.Payload != null)
                    {
                        Application.Current.Properties["entry_point"] = authResult.Payload.Role.MobileEntryPoint;
                        Application.Current.Properties["user_id"] = authResult.UserID;
                        Application.Current.Properties["organisationID"] = authResult.Payload.OrganisationId;
                        Application.Current.Properties["organisationType"] = authResult.OrganisationType;
                        switch (authResult.Payload.Role.MobileEntryPoint)
                        {
                            case 0:
                                Application.Current.Properties["c72_power"] = 8;
                                Application.Current.Properties["scan_timeout_seconds"] = 10;
                                break;
                            case 1:
                                Application.Current.Properties["c72_power"] = 20;
                                Application.Current.Properties["scan_timeout_seconds"] = 10;
                                break;
                            case 2:
                                Application.Current.Properties["c72_power"] = 8;
                                Application.Current.Properties["scan_timeout_seconds"] = 10;
                                break;
                            case 3:
                                Application.Current.Properties["c72_power"] = 8;
                                Application.Current.Properties["scan_timeout_seconds"] = 10;
                                break;
                            default:
                                break;
                        }
                        await Application.Current.SavePropertiesAsync();
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current.MainPage = new MainContentPage();
                            ViewModelLocator.loginVM.IsLoading = false;
                        });
                        //ChatClient.Instance.Connect();
                        ChatClient.Instance.GetMessages();
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                    ViewModelLocator.loginVM.ErrorMessage = "Authentication failed, please try again later";
                }
            }
        }

        private void tenantName_Completed(object sender, EventArgs e)
        {
            userName.Focus();
        }

        private void userName_Completed(object sender, EventArgs e)
        {
            password.Focus();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            var isPortrait = false;

            if (width < height)
            {
                isPortrait = true;
            }

            if (Device.RuntimePlatform == Device.iOS)
            {
                if (isPortrait)
                {   // Ipad
                    if (width >= 1024)
                        this.BackgroundImageSource = "bg-Portrait@2x.png";
                    else if (width >= 768)
                        this.BackgroundImageSource = "bg-Portrait.png";
                    // iPhone
                    else if (width >= 414 && height >= 896) //iphone 11, 11 Pro Max, 
                        this.BackgroundImageSource = "bg-736h@3x_new.png";
                    else if (width >= 414) //iphone 6 Plus, 7 Plus, 8 Plus
                        this.BackgroundImageSource = "bg-736h@3x.png";
                    else if (width >= 375 && height >= 812) //iphone 11 Pro
                        this.BackgroundImageSource = "bg-736h@3x_new.png";
                    else if (width >= 375) //iphone 6, 7, 8
                        this.BackgroundImageSource = "bg-667h@2x.png";
                    else if (width >= 320 && height >= 568) //iphone 5, 5s, SE
                        this.BackgroundImageSource = "bg-568h@2x.png";
                    else if (width >= 320)
                        this.BackgroundImageSource = "bg.png";
                }
            }
        }

        //private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        //{

        //}

        //private void BtnSaveProject_Clicked(object sender, EventArgs e)
        //{
        //    if (pickerProjects.SelectedIndex == -1)
        //    {
        //        ViewModelLocator.loginVM.ErrorMessage = "Please select a project";
        //        return;
        //    }
        //    Application.Current.Properties["SelectedProject"] = (Project)pickerProjects.SelectedItem;
        //    Application.Current.MainPage = new MainContentPage();
        //}
    }
}
