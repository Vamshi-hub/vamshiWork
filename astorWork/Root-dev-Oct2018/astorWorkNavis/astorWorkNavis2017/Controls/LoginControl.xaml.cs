using astorWorkNavis2017.Classes;
using astorWorkNavis2017.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace astorWorkNavis2017.Controls
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
            string tenantName = entryTenantName.Text;
            string userName = entryUserName.Text;
            string password = entryPwd.Password;
            if (string.IsNullOrEmpty(userName))
                _wnd.ShowNotice("User name cannot be empty");
            else if (string.IsNullOrEmpty(password))
                _wnd.ShowNotice("Password cannot be empty");
            else if (string.IsNullOrEmpty(tenantName))
                _wnd.ShowNotice("Tenant name cannot be empty");
            else
            {
                _wnd.ToggleLoading();

                ApiClient.Instance.InitClient(tenantName);

                Task.Run(() => ApiClient.Instance.LoginUser(userName, password)).ContinueWith(async t =>
                {
                    if (t.Result.Status != 0)
                    {
                        _wnd.ShowNotice(t.Result.Message);
                        _wnd.ToggleLoading();
                    }
                    else
                    {
                        var authResult = t.Result.Data as AuthResult;

                        ApiClient.Instance.SetAuthProperties(authResult);
                        ApiClient.Instance.StartRefreshTimer(authResult);

                        ViewModelLocator.homePageVM.UserName = userName;
                        var result = await ApiClient.Instance.MTGetProjects();

                        if (result.Status != 0)
                        {
                            _wnd.ShowNotice(t.Result.Message);
                            _wnd.ToggleLoading();
                        }
                        else
                        {
                            ViewModelLocator.homePageVM.Projects = result.Data as List<Project>;
                            if (Properties.Settings.Default.PROJECT_ID > 0)
                            {
                                ViewModelLocator.homePageVM.SelectedProject = ViewModelLocator.homePageVM.Projects.FirstOrDefault(p => p.ID == Properties.Settings.Default.PROJECT_ID);
                            }
                            _wnd.NavigateControl(Constants.CONTROL_HOME, true);
                        }
                    }
                });
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
