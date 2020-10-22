using CustomVisionTraining.Classes;
using CustomVisionTraining.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CustomVisionTraining.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ComputerVisionPage : Page
    {
        private IPropertySet localSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;

        MediaCapture mediaCapture;
        bool isPreviewing;
        DisplayRequest displayRequest = new DisplayRequest();

        public ComputerVisionPage()
        {
            this.InitializeComponent();
        }

        private async Task StartPreviewAsync()
        {
            try
            {
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();

                //displayRequest.RequestActive();
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
            }
            catch (UnauthorizedAccessException exc)
            {
                // This will be thrown if the user denied access to the camera in privacy settings
                Debug.WriteLine(exc.Message);
                return;
            }

            try
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    controlPreview.Source = mediaCapture;
                });

                await mediaCapture.StartPreviewAsync();
                isPreviewing = true;
            }
            catch (System.IO.FileLoadException)
            {
                mediaCapture.CaptureDeviceExclusiveControlStatusChanged += MediaCapture_CaptureDeviceExclusiveControlStatusChanged;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

            }

        }

        private async void MediaCapture_CaptureDeviceExclusiveControlStatusChanged(MediaCapture sender, MediaCaptureDeviceExclusiveControlStatusChangedEventArgs args)
        {
            if (args.Status == MediaCaptureDeviceExclusiveControlStatus.SharedReadOnlyAvailable)
            {
                Debug.WriteLine("The camera preview can't be displayed because another app has exclusive access");
            }
            else if (args.Status == MediaCaptureDeviceExclusiveControlStatus.ExclusiveControlAvailable && !isPreviewing)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await StartPreviewAsync();
                });
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            localSettings[Constants.SETTING_SUBSCRIPTION_KEY] = entrySubscribeKey.Text;
            Task.Run(StartPreviewAsync);
        }

        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {

            using (var captureStream = new InMemoryRandomAccessStream())
            {
                try
                {
                    await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);
                    var reader = new DataReader(captureStream.GetInputStreamAt(0));
                    var bytes = new byte[captureStream.Size];
                    await reader.LoadAsync((uint)captureStream.Size);
                    reader.ReadBytes(bytes);

                    var response = await ApiClient.Instance.MakeAnalysisRequest(bytes);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        txtResponse.Text = response;
                    });
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }
        }
    }
}
