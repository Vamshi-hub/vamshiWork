using astorWorkNavis2016.Classes;
using astorWorkNavis2016.Utilities;
using EmergenceGuardian.FFmpeg;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace astorWorkNavis2016.Controls
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

            if (ViewModelLocator.syncPageVM.ListMaterialsMatched.Count > 0)
            {
                _wnd.ToggleLoading();

                if (NavisworksHelper.SyncTimeliner(ViewModelLocator.syncPageVM.StageNames, ViewModelLocator.syncPageVM.ListMaterialsMatched))
                {
                    var syncedIds = ViewModelLocator.syncPageVM.MaterialsMatched.Select(m => m.ID).ToArray();
                    var missedIds = ViewModelLocator.syncPageVM.MaterialsUnmatched.Select(m => m.ID).ToArray();

                    Properties.Settings.Default.LAST_SYNC_TIME = DateTimeOffset.UtcNow;

                    var data = new
                    {
                        SyncTime = Properties.Settings.Default.LAST_SYNC_TIME,
                        SyncedMaterialIds = syncedIds,
                        UnsyncedMaterialIds = missedIds,
                        ModelId = NavisworksHelper.GetProjectGuid()
                    };

                    Task.Run(() => ApiClient.Instance.InsertUpdateBimSync(ViewModelLocator.homePageVM.SelectedProject.ID, data)).ContinueWith(t =>
                    {
                        string message = string.Empty;

                        if (t.Result.Status != 0)
                        {
                            _wnd.ToggleLoading();
                            _wnd.ShowNotice(t.Result.Message);
                        }
                        else
                        {
                            int syncSessionId = (t.Result.Data as BIMSyncSession).BimSyncId;
                            Properties.Settings.Default.LAST_SYNC_ID = syncSessionId;

                            string folderPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(MainWindow)).Location);
                            FFmpegConfig.FFmpegPath = Path.Combine(folderPath, @"Binaries\ffmpeg.exe");
                            string srcPath = NavisworksHelper.ExportTimeLiner();
                            if (string.IsNullOrEmpty(srcPath))
                            {
                                _wnd.ToggleLoading();
                                _wnd.ShowNotice("Fail to export the TimeLiner");
                            }
                            else
                            {
                                ViewModelLocator.syncResultPageVM.UrlVideo = srcPath;
                                var destPath = ProcessVideo(syncSessionId, srcPath);

                                if (!string.IsNullOrEmpty(destPath))
                                {
                                    _wnd.NavigateControl(Constants.CONTROL_VIDEO, false);

                                    Task.Run(() => ApiClient.Instance.UploadBIMVideo(ViewModelLocator.homePageVM.SelectedProject.ID, syncSessionId, destPath));

                                    Properties.Settings.Default.Save();
                                }
                                else
                                {
                                    _wnd.ToggleLoading();
                                    _wnd.ShowNotice("Fail to convert video");
                                }
                            }
                        }
                    });
                }
                else
                {
                    _wnd.ToggleLoading();
                    _wnd.ShowNotice("Sync TimeLiner failed");
                }
            }
            else
            {
                _wnd.ShowNotice("Nothing to apply");
            }
        }

        private string ProcessVideo(int syncSessionId, string srcPath)
        {
            string destPath = string.Empty;

            if (!string.IsNullOrEmpty(srcPath))
            {

                ProcessStartOptions Options = new ProcessStartOptions
                {
                    DisplayMode = FFmpegDisplayMode.Interface,
                    Title = "Encoding to H264/AAC (Simple)",
                    Timeout = TimeSpan.FromSeconds(30),
                    IsMainTask = false

                };

                destPath = Path.ChangeExtension(Path.GetTempFileName(), ".mp4");
                var completion = MediaEncoder.Encode(srcPath, "h264", "aac", "-pix_fmt yuv420p", destPath, Options);

                if (!completion.Equals(CompletionStatus.Success))
                {
                    destPath = string.Empty;
                }
            }

            return destPath;
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
