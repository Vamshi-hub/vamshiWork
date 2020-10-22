using Camera_NET;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Hackfest.Controls
{
    /// <summary>
    /// Interaction logic for TestCustomVision.xaml
    /// </summary>
    public partial class TestFaceAPI : UserControl
    {
        private MainWindow _wnd;
        private IFaceServiceClient _client;

        public TestFaceAPI(MainWindow wnd)
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
                _client = null;
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
            _client = new FaceServiceClient(Properties.Settings.Default.FACE_API_PRIMARY_KEY, 
                string.Format("https://{0}/face/v1.0", Properties.Settings.Default.FACE_API_ENDPOINT));
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
            IEnumerable<FaceAttributeType> faceAttributes =
                new FaceAttributeType[] {
                    FaceAttributeType.Gender,
                    FaceAttributeType.Age,
                    FaceAttributeType.Smile,
                    FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses,
                    FaceAttributeType.Hair
                };
            try
            {
                var shotBitMap = controlCamera.CameraControl.SnapshotSourceImage();
                using (Stream shotStream = new MemoryStream())
                {
                    shotBitMap.Save(shotStream, ImageFormat.Jpeg);
                    shotStream.Position = 0;
                    var faces = await _client.DetectAsync(shotStream, returnFaceId: true, returnFaceLandmarks: false, returnFaceAttributes: faceAttributes);
                    PrintOutput("--------------------------");
                    foreach (var face in faces)
                    {
                        PrintOutput(FaceDescription(face));
                        PrintOutput("-----------------");
                    }
                    var personInfo = await _client.IdentifyAsync(
                        faces.Select(f => f.FaceId).ToArray(),
                        personGroupId: Properties.Settings.Default.FACE_API_PROJECT_ID,
                        confidenceThreshold: (float) Properties.Settings.Default.FACE_API_THRESHHOLD);

                    foreach (var person in personInfo)
                    {
                        foreach (var candidate in person.Candidates)
                        {
                            var actualPerson = await _client.GetPersonInPersonGroupAsync(Properties.Settings.Default.FACE_API_PROJECT_ID, candidate.PersonId);

                            PrintOutput(actualPerson.Name + " with confidence: " + candidate.Confidence);
                            PrintOutput("-----------------");
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                PrintOutput(exc.Message);
                PrintOutput(exc.StackTrace);
            }
            PrintOutput("End predicting");
            btn.IsEnabled = true;

        }

        private void PrintOutput(string message)
        {
            txtOutput.Text += message;
            txtOutput.Text += Environment.NewLine;
        }

        private string FaceDescription(Face face)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Face: ");

            // Add the gender, age, and smile.
            sb.Append(face.FaceAttributes.Gender);
            sb.Append(", ");
            sb.Append(face.FaceAttributes.Age);
            sb.Append(", ");
            sb.Append(String.Format("smile {0:F1}%, ", face.FaceAttributes.Smile * 100));

            // Add the emotions. Display all emotions over 10%.
            sb.Append("Emotion: ");
            EmotionScores emotionScores = face.FaceAttributes.Emotion;
            if (emotionScores.Anger >= 0.1f) sb.Append($"anger {emotionScores.Anger * 100:F1}%, ");
            if (emotionScores.Contempt >= 0.1f) sb.Append($"contempt {emotionScores.Contempt * 100:F1}%, ");
            if (emotionScores.Disgust >= 0.1f) sb.Append($"disgust {emotionScores.Disgust * 100:F1}%, ");
            if (emotionScores.Fear >= 0.1f) sb.Append($"fear {emotionScores.Fear * 100:F1}%, ");
            if (emotionScores.Happiness >= 0.1f) sb.Append($"happiness {emotionScores.Happiness * 100:F1}%, ");
            if (emotionScores.Neutral >= 0.1f) sb.Append($"neutral {emotionScores.Neutral * 100:F1}%, ");
            if (emotionScores.Sadness >= 0.1f) sb.Append($"sadness {emotionScores.Sadness * 100:F1}%, ");
            if (emotionScores.Surprise >= 0.1f) sb.Append($"surprise {emotionScores.Surprise * 100:F1}%, ");

            // Add glasses.
            sb.Append(face.FaceAttributes.Glasses);
            sb.Append(", ");

            // Add hair.
            sb.Append("Hair: ");

            // Display baldness confidence if over 1%.
            if (face.FaceAttributes.Hair.Bald >= 0.01f)
                sb.Append($"bald {face.FaceAttributes.Hair.Bald * 100:F1}% ");

            // Display all hair color attributes over 10%.
            HairColor[] hairColors = face.FaceAttributes.Hair.HairColor;
            foreach (HairColor hairColor in hairColors)
            {
                if (hairColor.Confidence >= 0.1f)
                {
                    sb.Append(hairColor.Color.ToString());
                    sb.Append($" {hairColor.Confidence * 100:F1}% ");
                }
            }

            // Return the built string.
            return sb.ToString();
        }
    }
}
