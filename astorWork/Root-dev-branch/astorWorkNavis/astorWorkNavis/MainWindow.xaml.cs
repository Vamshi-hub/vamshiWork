using astorWorkNavis.Classes;
using astorWorkNavis.Controls;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi;

namespace astorWorkNavis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer hideNoticeTimer;
        LoadingControl loadingControl;

        public MainWindow()
        {
            InitializeComponent();

            hideNoticeTimer = new Timer(5000);
            hideNoticeTimer.Elapsed += HideNoticeTimer_Elapsed;

            loadingControl = new LoadingControl();

            //ApiClient.Instance.InitClient(API_BASE_URL);

            var loginControl = new LoginControl(this);
            contentHost.Children.Add(loginControl);
            /*
            if (string.IsNullOrEmpty(Properties.Settings.Default.REFRESH_TOKEN))
                contentHost.Children.Add(loginControl);
            else
            {
                Task.Run(() => ApiClient.Instance.AuthRefresh(Properties.Settings.Default.REFRESH_TOKEN))
                    .ContinueWith((t) =>
                    {
                        if (t.Result.Status == 0)
                            contentHost.Children.Add(homeControl);
                        else
                        {
                            contentHost.Children.Add(loginControl);
                            ShowNotice("Your session has expired, please login again");
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            */
        }

        private int debugCount = 0;
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Home))
                debugCount++;

            if (debugCount >= 2)
            {
                debugCount = 0;
                DebugNavisworks();
            }
        }

        public void NavigateControl(string controlName, bool replacing)
        {

            Dispatcher.Invoke(() =>
            {
                if (replacing)
                    contentHost.Children.Clear();
                else if (contentHost.Children.Contains(loadingControl))
                    contentHost.Children.Remove(loadingControl);

                if (controlName.Equals(Constants.CONTROL_HOME))
                {
                    contentHost.Children.Add(new HomeControl(this));
                }
                else if (controlName.Equals(Constants.CONTROL_LOG_IN))
                {
                    contentHost.Children.Add(new LoginControl(this));
                }
                else if (controlName.Equals(Constants.CONTROL_SYNC))
                {
                    contentHost.Children.Add(new SyncControl(this));
                }
                else if (controlName.Equals(Constants.CONTROL_VIDEO))
                    contentHost.Children.Add(new VideoControl(this));
                else if (controlName.Equals(Constants.CONTROL_SETTING))
                    contentHost.Children.Add(new SettingsControl(this));

                Activate();
            });
        }

        public void NavigateBack(UserControl control)
        {
            Dispatcher.Invoke(() =>
            {
                if (contentHost.Children.Contains(loadingControl))
                    contentHost.Children.Remove(loadingControl);

                if (contentHost.Children.Contains(control))
                    contentHost.Children.Remove(control);
            });
        }

        private void HideNoticeTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (noticeBar.IsActive)
                    noticeBar.IsActive = false;
            });
        }

        public void ShowNotice(string message)
        {
            Dispatcher.Invoke(() =>
            {
                noticeBar.Message.Content = message;
                noticeBar.IsActive = true;
                hideNoticeTimer.Start();
            });
        }

        public void ToggleLoading()
        {
            Dispatcher.Invoke(() =>
            {
                if (contentHost.Children.Contains(loadingControl))
                    contentHost.Children.Remove(loadingControl);
                else
                    contentHost.Children.Add(loadingControl);

                Activate();
            });
        }

        private void noticeBarMessage_ActionClick(object sender, RoutedEventArgs e)
        {
            noticeBar.IsActive = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void Setup()
        {
            NavigateControl(Constants.CONTROL_SETTING, true);
        }

        private void DebugNavisworks()
        {
            ComApi.InwOpState10 state = ComApiBridge.ComApiBridge.State;
            ComApi.InwOaPropertyVec options = state.GetIOPluginOptions("lcodpanim");

            foreach (ComApi.InwOaProperty opt in options.Properties())
            {
                try
                {
                    Debug.WriteLine(opt.name);
                    string value = opt.value as string;
                    if (string.IsNullOrEmpty(value))
                        Debug.WriteLine("NA");
                    else
                        Debug.WriteLine(value);
                }
                catch { }
            }
            Debug.WriteLine("--------------------------------------------------");
        }
    }
}
