using astorWork_Navisworks.Classes;
using astorWork_Navisworks.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace astorWork_Navisworks.Controls
{
    /// <summary>
    /// Interaction logic for LoginControl.xaml
    /// </summary>
    public partial class LoginControl : UserControl
    {
        private MainWindow _wnd;
        public LoginControl(MainWindow wnd)
        {
            _wnd = wnd;
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            string userName = entryUserName.Text;
            string password = entryPwd.Password;
            if (string.IsNullOrEmpty(userName))
                _wnd.ShowNotice("User name cannot be empty");
            else if (string.IsNullOrEmpty(password))
                _wnd.ShowNotice("Password cannot be empty");
            else
            {
                _wnd.ToggleLoading();
                Task.Run(() => LoginAsync(userName, password));
            }
        }

        private async Task LoginAsync(string user_name, string password)
        {
            var result = await ApiClient.Instance.LoginUser(user_name, password);

            if (result.Status != 0)
            {
                _wnd.ShowNotice(result.Message);
                _wnd.ToggleLoading();
            }
            else
            {
                try
                {
                    int userId = (int)result.Data["ID"];
                    string userName = (string)result.Data["UserName"];
                    string apiKey = (string)result.Data["APIKey"];

                    if(result.Data["CanUpload"] != null)
                    {
                        if ((bool)result.Data["CanUpload"])
                            Properties.Settings.Default.IS_UPLOAD_CLOUD = true;
                        else
                            Properties.Settings.Default.IS_UPLOAD_CLOUD = false;
                    }

                    Properties.Settings.Default.API_KEY = apiKey;
                    Properties.Settings.Default.USER_ID = userId;
                    Properties.Settings.Default.USER_NAME = userName;
                    Properties.Settings.Default.Save();

                    _wnd.ShowNotice("Welcome, " + userName);
                    _wnd.NavigateControl(Constants.CONTROL_HOME, true);
                }
                catch (Exception exc)
                {
                    _wnd.ShowNotice(exc.Message);
                    _wnd.ToggleLoading();
                }
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            _wnd.NavigateControl(Constants.CONTROL_SETTING, true);
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            _wnd.Close();
        }
    }
}
