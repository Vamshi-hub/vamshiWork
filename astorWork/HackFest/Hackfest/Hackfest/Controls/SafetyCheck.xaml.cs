using Camera_NET;
using Hackfest.ViewModels;
using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace Hackfest.Controls
{
    /// <summary>
    /// Interaction logic for SafetyCheck.xaml
    /// </summary>
    public partial class SafetyCheck : UserControl
    {
        private MainWindow _wnd;
        private PredictionEndpoint _customVisionClient;
        private IFaceServiceClient _faceApiClient;

        private Guid _customVisionProjectId;

        private Timer detectTimer;

        public SafetyCheck(MainWindow wnd)
        {
            InitializeComponent();

            DataContext = Locator.safetyCheckVM;

            _wnd = wnd;
            _customVisionProjectId = Guid.Parse(Properties.Settings.Default.CUSTOM_VISION_PROJECT_ID);

            detectTimer = new Timer
            {
                Interval = Properties.Settings.Default.SAFETY_CHECK_INTERVAL * 1000,
                AutoReset = false
            };
            detectTimer.Elapsed += DetectTimer_Elapsed;
        }

        private async void DetectTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                
                Locator.safetyCheckVM.PersonName = "NA";
                Locator.safetyCheckVM.StatusMessage = "Detecting worker and safety";
                Locator.safetyCheckVM.StatusColor = "SkyBlue";
                

                IEnumerable<FaceAttributeType> faceAttributes =
                    new FaceAttributeType[] {
                        FaceAttributeType.Gender,
                        FaceAttributeType.Age,
                        FaceAttributeType.Smile,
                        FaceAttributeType.Emotion,
                        FaceAttributeType.Glasses,
                        FaceAttributeType.Hair
                    };

                Bitmap bmp = controlCamera.CameraControl.SnapshotSourceImage();
                Candidate candidate = null;

                using (var shotStream = new MemoryStream(1024 * 1024))
                {
                    bmp.Save(shotStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    shotStream.Position = 0;
                    var faces = await _faceApiClient.DetectAsync(shotStream, returnFaceId: true, returnFaceLandmarks: false, returnFaceAttributes: faceAttributes);

                    if (faces.Length > 0)
                    {
                        var personInfo = await _faceApiClient.IdentifyAsync(
                            faces.Select(f => f.FaceId).ToArray(),
                            personGroupId: Properties.Settings.Default.FACE_API_PROJECT_ID,
                            confidenceThreshold: (float)Properties.Settings.Default.FACE_API_THRESHHOLD);

                        foreach (var person in personInfo)
                        {
                            candidate = person.Candidates.Where(c => c.Confidence > Properties.Settings.Default.FACE_API_THRESHHOLD).OrderByDescending(c => c.Confidence).FirstOrDefault();

                            if (candidate != null)
                            {
                                var actualPerson = await _faceApiClient.GetPersonInPersonGroupAsync(Properties.Settings.Default.FACE_API_PROJECT_ID, candidate.PersonId);
                                Locator.safetyCheckVM.PersonName = actualPerson.Name;
                            }
                        }
                    }
                }

                if (candidate != null)
                {
                    using (var shotStream = new MemoryStream(1024 * 1024))
                    {
                        bmp.Save(shotStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        shotStream.Position = 0;
                        var result = _customVisionClient.PredictImage(_customVisionProjectId, shotStream);
                        Locator.safetyCheckVM.StatusMessage = "Unable to confirm safety";
                        Locator.safetyCheckVM.StatusColor = "LightSalmon";
                        foreach (var c in result.Predictions)
                        {
                            Debug.WriteLine($"\t{c.Tag}: {c.Probability}");
                            if (c.Tag == "without_helmet" && c.Probability > Properties.Settings.Default.CUSTOM_VISION_UNSAFE_THRESHOLD)
                            {
                                Debug.WriteLine("Safety breach");
                                Locator.safetyCheckVM.StatusMessage = "Safety breach detected";
                                Locator.safetyCheckVM.StatusColor = "LightCoral";
                                break;
                            }
                            else if (c.Tag == "with_helmet" && c.Probability > Properties.Settings.Default.CUSTOM_VISION_SAFE_THRESHOLD)
                            {
                                Debug.WriteLine("Safety confirmed");
                                Locator.safetyCheckVM.StatusMessage = "Safety confirmed";
                                Locator.safetyCheckVM.StatusColor = "LightGreen";
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Locator.safetyCheckVM.StatusMessage = "Unable to confirm safety";
                    Locator.safetyCheckVM.StatusColor = "LightSalmon";
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
            }

            detectTimer.Start();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _faceApiClient = new FaceServiceClient(Properties.Settings.Default.FACE_API_PRIMARY_KEY,
                string.Format("https://{0}/face/v1.0", Properties.Settings.Default.FACE_API_ENDPOINT));
            // Init prediction endpoint
            _customVisionClient = new PredictionEndpoint
            {
                ApiKey = Properties.Settings.Default.CUSTOM_VISION_PREDICTION_KEY
            };
            _customVisionClient.HttpClient.Timeout = TimeSpan.FromSeconds(5);

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

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                controlCamera.CameraControl.CloseCamera();
                _customVisionClient.Dispose();
                _faceApiClient = null;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
            }
        }

        private void btnToggleDetect_Checked(object sender, RoutedEventArgs e)
        {
            Locator.safetyCheckVM.StatusIcon = "Record";
            Locator.safetyCheckVM.StatusMessage = "Detecting worker and safety";
            Locator.safetyCheckVM.StatusColor = "SkyBlue";

            detectTimer.Start();
        }

        private void btnToggleDetect_Unchecked(object sender, RoutedEventArgs e)
        {
            Locator.safetyCheckVM.StatusIcon = "BlockHelper";
            Locator.safetyCheckVM.StatusMessage = "Idle";
            Locator.safetyCheckVM.StatusColor = "LightGray";

            detectTimer.Stop();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            _wnd.NavigateBack();
        }
    }
}
