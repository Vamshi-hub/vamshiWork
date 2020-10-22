using astorWork_Navisworks.Classes;
using astorWork_Navisworks.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace astorWork_Navisworks.Controls
{
    /// <summary>
    /// Interaction logic for LoadingControl.xaml
    /// </summary>
    public partial class VideoControl : UserControl
    {
        MainWindow _wnd;
        public VideoControl(MainWindow wnd)
        {
            _wnd = wnd;
            DataContext = ViewModelLocator.syncResultPageVM;

            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            _wnd.NavigateControl(Constants.CONTROL_HOME, false);
        }

        private void DisposeVideo()
        {
            try
            {
                mediaVideo.Stop();
                mediaVideo.Close();
                mediaVideo.Source = null;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                File.Delete(ViewModelLocator.syncResultPageVM.UrlVideo);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string result = string.Empty;
            var dlg = new System.Windows.Forms.SaveFileDialog();

            dlg.Filter = "AVI Video (*.avi)|*.avi";
            dlg.RestoreDirectory = false;

            try
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    File.Copy(ViewModelLocator.syncResultPageVM.UrlVideo, dlg.FileName, true);
                    Process.Start(Path.GetDirectoryName(dlg.FileName));
                }
                else
                    result = "Fail to create file";
            }
            catch (Exception exc)
            {
                result = exc.Message;
            }

            if (!string.IsNullOrEmpty(result))
                _wnd.ShowNotice(result);

            _wnd.NavigateControl(Constants.CONTROL_HOME, false);
        }

        private void mediaVideo_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() => {
                _wnd.Activate();
            });
        }
    }
}
