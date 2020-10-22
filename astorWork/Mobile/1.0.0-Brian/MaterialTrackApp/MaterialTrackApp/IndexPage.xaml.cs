using MaterialTrackApp.Class;
using MaterialTrackApp.Utility;
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
            string userName = entryUserName.Text;
            string password = entryPassword.Text;

            entryUserName.Text = string.Empty;
            entryPassword.Text = string.Empty;

            try
            {
                App.InvokeLoadingPage(this, App.mainPage, "Try to sign in, please wait...", true);
            }
            catch(Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
            }
            Task.Run(() => SignInAsync(userName, password));
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
                var result = await ApiClient.Instance.LoginUser(userName, password);
                if (result.Status == 0)
                {
                    var user = result.Data.FirstOrDefault() as ApiClient.BeaconAppUser;
                    if (user != null)
                    {
                        Application.Current.Properties["user_name"] = user.UserName;
                        Application.Current.Properties["user_role"] = user.RoleID;
                        Application.Current.Properties["user_location"] = user.LocationAssociationID.Value;
                        await Application.Current.SavePropertiesAsync();

                        ViewModelLocator.homePageVM.CanManualUpdate = user.RoleID.Equals(Constants.ROLE_SITE_SUPERVISOR);

                        MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, "");
                    }
                    else
                        MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, "User doesn't exist or password is incorrect");
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