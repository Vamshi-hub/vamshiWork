using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IndexPage : ContentPage
    {
        public IndexPage()
        {
            InitializeComponent();
        }

        private void toggleContainer()
        {
            if (containerChooseType.IsEnabled)
            {
                containerChooseType.IsEnabled = false;
                containerChooseType.ScaleTo(0);

                containerSignIn.ScaleTo(1);
                containerSignIn.IsEnabled = true;

                containerMaster.RaiseChild(containerSignIn);
            }
            else
            {
                containerSignIn.IsEnabled = false;
                containerSignIn.ScaleTo(0);

                containerChooseType.ScaleTo(1);
                containerChooseType.IsEnabled = true;
                containerMaster.RaiseChild(containerChooseType);
            }
        }

        private void btnVendor_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties["user_role"] = 0;
            toggleContainer();
        }

        private void btnSupervisor_Clicked(object sender, EventArgs e)
        {
            Application.Current.Properties["user_role"] = 1;
            toggleContainer();
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            toggleContainer();
        }

        private void btnSignIn_Clicked(object sender, EventArgs e)
        {
            try
            {
                App.InvokeLoadingPage(this, App.mainPage, "Try to sign in, please wait...", true);
            }
            catch(Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
            }
            Task.Run(() => SignInAsync(entryUserName.Text, entryPassword.Text));
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            entryUserName.Text = string.Empty;
            entryPassword.Text = string.Empty;
            /*
            if (Application.Current.Properties.ContainsKey("user_role"))
                Application.Current.Properties["user_role"] = 0;
            else
                Application.Current.Properties.Add("user_role", 0);

            containerMaster.RaiseChild(containerChooseType);
            */
        }

        private void entryUserName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(entryUserName.Text) || string.IsNullOrEmpty(entryPassword.Text))
                btnSignIn.IsEnabled = false;
            else
                btnSignIn.IsEnabled = true;
        }

        private async Task SignInAsync(string userName, string password)
        {
            try
            {
                if (App.astorTrackAPI.ValidateUserExistForMobile(userName, password))
                {
                    var userDetails = App.astorTrackAPI.GetUserDetailsAsyncForMobile(userName);

                    
                    Application.Current.Properties["role_id"] = userDetails.RoleID;
                    Application.Current.Properties["role_name"] = userDetails.RoleName;
                    Application.Current.Properties["role_location_id"] = userDetails.RoleLocationID;
                    Application.Current.Properties["user_name"] = userName;
                    await Application.Current.SavePropertiesAsync();

                    MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, "");
                }
                else
                    MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, "User doesn't exist or password is incorrect");
            }
            catch(Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
                MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, "Network error, please try again later");
            }
        }
    }
}