using Camera_NET;
using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.Rest.TransientFaultHandling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class TestCNTK : UserControl
    {
        private MainWindow _wnd;

        public TestCNTK(MainWindow wnd)
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

            var shotBitMap = controlCamera.CameraControl.SnapshotSourceImage();

            using (var stream = new MemoryStream())
            {
                shotBitMap.Save(stream, ImageFormat.Jpeg);
                stream.Position = 0;
                using (var client = new HttpClient())
                {
                    using (var content =
                        new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        content.Add(new StreamContent(stream), "file", "upload.jpg");

                        using (
                           var message =
                               await client.PostAsync("http://localhost:5555/api_upload", content)) {
                            
                            var bytes = await message.Content.ReadAsByteArrayAsync();
                            var memoryStream = new MemoryStream(bytes);
                            memoryStream.Position = 0;
                            //var output = await message.Content.ReadAsStreamAsync();
                            //output.Position = 0;
                            //var str = await message.Content.ReadAsStringAsync();
                            //var bytes = Convert.FromBase64String(str);
                            BitmapImage bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = memoryStream;
                            bitmap.EndInit();

                            // Assign the Source property of your image
                            resultImg.Source = bitmap;
                            
                        }
                    }
                }
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
            if (emotionScores.Anger >= 0.1f) sb.Append(String.Format("anger {0:F1}%, ", emotionScores.Anger * 100));
            if (emotionScores.Contempt >= 0.1f) sb.Append(String.Format("contempt {0:F1}%, ", emotionScores.Contempt * 100));
            if (emotionScores.Disgust >= 0.1f) sb.Append(String.Format("disgust {0:F1}%, ", emotionScores.Disgust * 100));
            if (emotionScores.Fear >= 0.1f) sb.Append(String.Format("fear {0:F1}%, ", emotionScores.Fear * 100));
            if (emotionScores.Happiness >= 0.1f) sb.Append(String.Format("happiness {0:F1}%, ", emotionScores.Happiness * 100));
            if (emotionScores.Neutral >= 0.1f) sb.Append(String.Format("neutral {0:F1}%, ", emotionScores.Neutral * 100));
            if (emotionScores.Sadness >= 0.1f) sb.Append(String.Format("sadness {0:F1}%, ", emotionScores.Sadness * 100));
            if (emotionScores.Surprise >= 0.1f) sb.Append(String.Format("surprise {0:F1}%, ", emotionScores.Surprise * 100));

            // Add glasses.
            sb.Append(face.FaceAttributes.Glasses);
            sb.Append(", ");

            // Add hair.
            sb.Append("Hair: ");

            // Display baldness confidence if over 1%.
            if (face.FaceAttributes.Hair.Bald >= 0.01f)
                sb.Append(String.Format("bald {0:F1}% ", face.FaceAttributes.Hair.Bald * 100));

            // Display all hair color attributes over 10%.
            HairColor[] hairColors = face.FaceAttributes.Hair.HairColor;
            foreach (HairColor hairColor in hairColors)
            {
                if (hairColor.Confidence >= 0.1f)
                {
                    sb.Append(hairColor.Color.ToString());
                    sb.Append(String.Format(" {0:F1}% ", hairColor.Confidence * 100));
                }
            }

            // Return the built string.
            return sb.ToString();
        }
    }
}
