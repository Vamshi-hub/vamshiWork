using astorWork_Navisworks.Classes;
using astorWork_Navisworks.Controls;
using astorWork_Navisworks.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi;

namespace astorWork_Navisworks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Timer hideNoticeTimer;
        LoginControl loginControl;
        LoadingControl loadingControl;
        HomeControl homeControl;
        SyncControl syncControl;

        public MainWindow()
        {
            InitializeComponent();
            
            hideNoticeTimer = new Timer(5000);
            hideNoticeTimer.Elapsed += HideNoticeTimer_Elapsed;

            loginControl = new LoginControl(this);
            homeControl = new HomeControl(this);
            syncControl = new SyncControl(this);
            loadingControl = new LoadingControl();

            if (Properties.Settings.Default.USER_ID > 0)
                contentHost.Children.Add(homeControl);
            else
                contentHost.Children.Add(loginControl);

        }

        public void NavigateControl(string controlName, bool replacing)
        {
            Dispatcher.Invoke(() =>
            {
                if (contentHost.Children.Contains(loadingControl))
                    contentHost.Children.Remove(loadingControl);

                if (replacing)
                    contentHost.Children.Clear();

                if (controlName.Equals(Constants.CONTROL_HOME))
                {
                    if (contentHost.Children.Contains(homeControl))
                        contentHost.Children.Remove(homeControl);

                    contentHost.Children.Add(homeControl);
                }
                else if (controlName.Equals(Constants.CONTROL_LOG_IN))
                {
                    if (contentHost.Children.Contains(loginControl))
                        contentHost.Children.Remove(loginControl);

                    contentHost.Children.Add(loginControl);
                }
                else if (controlName.Equals(Constants.CONTROL_SYNC))
                {
                    if (contentHost.Children.Contains(syncControl))
                        contentHost.Children.Remove(syncControl);

                    contentHost.Children.Add(syncControl);
                }
                else if (controlName.Equals(Constants.CONTROL_VIDEO))
                    contentHost.Children.Add(new VideoControl(this));

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
        }

        private int debugCount = 0;
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Home))
                debugCount++;

            if (debugCount >= 2)
            {
                debugCount = 0;
                DebugNavisworks();
            }
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
