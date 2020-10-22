using Camera_NET;
using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.Rest.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
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

namespace Hackfest.Controls
{
    /// <summary>
    /// Interaction logic for TestCustomVision.xaml
    /// </summary>
    public partial class TestCustomVision : UserControl
    {
        private MainWindow _wnd;
        private PredictionEndpoint _predict;
        private Guid _projectId;

        public TestCustomVision(MainWindow wnd)
        {
            InitializeComponent();

            _wnd = wnd;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                controlCamera.CameraControl.CloseCamera();
                txtOutput.Text = string.Empty;
                _predict.Dispose();
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _wnd.NavigateBack();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _projectId = Guid.Parse(Properties.Settings.Default.CUSTOM_VISION_PROJECT_ID);
            // Init prediction endpoint
            _predict = new PredictionEndpoint
            {
                ApiKey = Properties.Settings.Default.CUSTOM_VISION_PREDICTION_KEY
            };
            _predict.HttpClient.Timeout = TimeSpan.FromSeconds(5);

            // Init camera preview
            CameraChoice _CameraChoice = new CameraChoice();
            _CameraChoice.UpdateDeviceList();

            var camera = _CameraChoice.Devices.Where(d => d.DevicePath.Equals(Properties.Settings.Default.CAMERA_UUID)).FirstOrDefault();

            if (camera != null)
            {
                var resolutions = Camera.GetResolutionList(camera.Mon);
                var res = resolutions.Where(r => r.Width == Properties.Settings.Default.CAMERA_RESOLUTION_WIDTH && r.Height == Properties.Settings.Default.CAMERA_RESOLUTION_HEIGHT).FirstOrDefault();
                if (res != null)
                    controlCamera.CameraControl.SetCamera(camera.Mon, res);
            }
        }

        private async void btnPredict_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            btn.IsEnabled = false;
            PrintOutput("Start predicting");
            try
            {
                var shotBitMap = controlCamera.CameraControl.SnapshotSourceImage();
                using (Stream shotStream = new MemoryStream())
                {
                    shotBitMap.Save(shotStream, ImageFormat.Jpeg);
                    shotStream.Position = 0;
                    var result = await _predict.PredictImageAsync(_projectId, shotStream);
                    foreach (var c in result.Predictions)
                    {
                        PrintOutput($"\t{c.Tag}: {c.Probability:P1}");
                    }
                }

            }
            catch (Exception exc)
            {
                PrintOutput(exc.Message);
            }
            PrintOutput("End predicting");
            btn.IsEnabled = true;

        }

        private void PrintOutput(string message)
        {
            txtOutput.Text += message;
            txtOutput.Text += Environment.NewLine;
        }
    }
}
