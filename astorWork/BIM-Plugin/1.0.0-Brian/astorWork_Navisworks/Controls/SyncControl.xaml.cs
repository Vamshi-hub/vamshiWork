using astorWork_Navisworks.Classes;
using astorWork_Navisworks.Utilities;
using astorWork_Navisworks.ViewModels;
using EmergenceGuardian.FFmpeg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace astorWork_Navisworks.Controls
{
    /// <summary>
    /// Interaction logic for SyncControl.xaml
    /// </summary>
    public partial class SyncControl : UserControl
    {
        private MainWindow _wnd;
        public SyncControl(MainWindow wnd)
        {
            _wnd = wnd;
            DataContext = ViewModelLocator.syncPageVM;

            InitializeComponent();
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(MainWindow)).Location);
            FFmpegConfig.FFmpegPath = Path.Combine(folderPath, @"Binaries\ffmpeg.exe");

            if (NavisworksHelper.SyncTimeliner(ViewModelLocator.syncPageVM.ListMaterialsMatched))
            {
                _wnd.ToggleLoading();
                Task.Run(() =>
                {
                    string srcPath = NavisworksHelper.ExportTimeLiner();
                    if (string.IsNullOrEmpty(srcPath))
                    {
                        _wnd.ShowNotice("Fail to export the TimeLiner");
                    }
                    else
                    {
                        ViewModelLocator.syncResultPageVM.UrlVideo = srcPath;
                        ProcessVideo(srcPath);
                    }
                });
            }
        }

        private void ProcessVideo(string srcPath)
        {
            string message = string.Empty;
            string visualizeUrl = string.Empty;
            DateTime lastSyncDT = DateTime.Now;

            if (string.IsNullOrEmpty(srcPath))
            {
                message = "Cannot upload current video";
            }
            else
            {
                if (Properties.Settings.Default.IS_UPLOAD_CLOUD)
                {
                    string destPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");

                    ProcessStartOptions Options = new ProcessStartOptions
                    {
                        DisplayMode = FFmpegDisplayMode.Interface,
                        Title = "Encoding to H264/AAC (Simple)",
                        Timeout = TimeSpan.FromSeconds(30),
                        IsMainTask = false
                    };

                    var completion = MediaEncoder.Encode(srcPath, "h264", "aac", null, destPath, Options);
                    if (!completion.Equals(CompletionStatus.Success))
                    {
                        message = "Cannot convert video";
                    }

                    else
                    {
                        var result = ApiClient.Instance.UploadBIMVideo(Properties.Settings.Default.API_KEY, destPath).Result;
                        try
                        {
                            File.Delete(destPath);
                        }
                        catch (Exception exc)
                        {
                            Debug.WriteLine(exc.Message);
                        }

                        if (string.IsNullOrEmpty(result.Message))
                        {
                            visualizeUrl = result.Data.ToString();
                        }
                        else
                            message = result.Message;
                    }
                }
            }

            int status = 0;
            if (ViewModelLocator.syncPageVM.MaterialsUnmatched.Count > 0)
                status = 1;

            var syncedIds = string.Join(",", ViewModelLocator.syncPageVM.MaterialsMatched.Select(m => m.ID));
            var missedIds = string.Join(",", ViewModelLocator.syncPageVM.MaterialsUnmatched.Select(m => m.ID));

            if (string.IsNullOrEmpty(message))
            {
                _wnd.NavigateControl(Constants.CONTROL_VIDEO, false);
                message = NavisworksHelper.GetProjectTitle();

                Properties.Settings.Default.LAST_SYNC_TIME = lastSyncDT;
                Properties.Settings.Default.Save();
            }
            else
            {
                status = 2;
                _wnd.ToggleLoading();
                _wnd.ShowNotice("Failed to sync TimeLiner");
            }

            var data = new
            {
                LastSyncDate = lastSyncDT.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Status = status,
                SyncedMaterialIds = syncedIds,
                MissingMaterialIds = missedIds,
                BIMProjectID = NavisworksHelper.GetProjectGuid(),
                VisualizeURL = visualizeUrl,
                Remarks = message
            };

            ApiClient.Instance.InsertUpdateBimSync(Properties.Settings.Default.API_KEY, data).Wait();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _wnd.NavigateBack(this);
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                if (ViewModelLocator.syncPageVM.MaterialsMatched.Count == 0)
                    btnApply.IsEnabled = false;
                else
                    btnApply.IsEnabled = true;
            }
        }
    }
}
