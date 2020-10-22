using Microsoft.Cognitive.CustomVision.Prediction;
using Microsoft.Cognitive.CustomVision.Training;
using Microsoft.Cognitive.CustomVision.Training.Models;
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
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.FileProperties;
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
    public sealed partial class TrainingPage : Page
    {
        public TrainingPage()
        {
            this.InitializeComponent();

            captureTimer = new DispatcherTimer();
            captureTimer.Interval = TimeSpan.FromSeconds(1);
            captureTimer.Tick += CaptureTimer_Tick;

            trainTimer = new DispatcherTimer();
            trainTimer.Interval = TimeSpan.FromSeconds(1);
            trainTimer.Tick += TrainTimer_Tick;

            trainingImages = new List<string>();

            trainingApi = new TrainingApi()
            {
                ApiKey = TRAINING_KEY
            };

            var account = trainingApi.GetAccountInfo();
            var predictionKey = account.Keys.PredictionKeys.PrimaryKey;
            // Create a prediction endpoint, passing in the obtained prediction key
            predictionEndpoint = new PredictionEndpoint() { ApiKey = predictionKey };

            projectId = Guid.Parse("8adc16dd-ea35-47d7-9ed0-260711a3acff");
            //entrySubscribeKey.Text = localSettings[Constants.SETTING_SUBSCRIPTION_KEY] as string;
        }

        private IPropertySet localSettings = ApplicationData.Current.LocalSettings.Values;
        private DispatcherTimer captureTimer;
        private DispatcherTimer trainTimer;

        private static List<string> trainingImages;

        MediaCapture mediaCapture;
        bool isPreviewing;
        DisplayRequest displayRequest = new DisplayRequest();
        StorageFolder saveFolder;

        TrainingApi trainingApi;
        PredictionEndpoint predictionEndpoint;
        string tagName;
        Guid projectId;
        Guid iterationId;

        private const string TRAINING_KEY = "706a5802a79044f3be6a94cfa1463013";

        private async void TrainTimer_Tick(object sender, object e)
        {
            try
            {
                var iteration = await trainingApi.GetIterationAsync(projectId, iterationId);
                if (iteration.Status == "Training")
                {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        listResponse.Items.Add("Training...");
                    });
                }
                else
                {
                    trainTimer.Stop();
                    if (iteration.TrainedAt.HasValue)
                    {
                        iteration.IsDefault = true;
                        iteration = await trainingApi.UpdateIterationAsync(projectId, iterationId, iteration);

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            listResponse.Items.Add(string.Format("Training is done with {0} at {1}", iteration.Name, iteration.TrainedAt));
                        });
                    }
                    else
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            listResponse.Items.Add("Training failed " + iteration.Status);
                        });
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

            }
        }

        private int imageCount = 0;
        private void CaptureTimer_Tick(object sender, object e)
        {
            if (imageCount < 40)
            {
                Task.Run(GenerateImage);
            }
        }

        private async Task StartPreviewAsync()
        {
            try
            {
                mediaCapture = new MediaCapture();
                await mediaCapture.InitializeAsync();
                var resolutions = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).Select(x => x as VideoEncodingProperties);

                var maxRes = resolutions.OrderByDescending(x => x.Height * x.Width).FirstOrDefault();

                await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, maxRes);

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
            catch (FileLoadException)
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
            Task.Run(StartPreviewAsync);
        }

        private async void btnCapture_Click(object sender, RoutedEventArgs e)
        {
            var myPictures = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
            if (saveFolder == null)
                saveFolder = await myPictures.RequestAddFolderAsync();

            imageCount = 0;
            //trainingImages.Clear();
            captureTimer.Start();
        }

        private async Task GenerateImage()
        {
            try
            {
                using (var captureStream = new InMemoryRandomAccessStream())
                {
                    var fileTime = DateTime.Now.ToFileTimeUtc();
                    string fileName = string.Format("cv_{0}.jpg", fileTime);
                    StorageFile file = await saveFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);

                    await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);

                    using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var decoder = await BitmapDecoder.CreateAsync(captureStream);
                        var encoder = await BitmapEncoder.CreateForTranscodingAsync(fileStream, decoder);

                        var properties = new BitmapPropertySet {
            { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) }
        };
                        await encoder.BitmapProperties.SetPropertiesAsync(properties);

                        await encoder.FlushAsync();

                        trainingImages.Add(file.Path);

                        imageCount++;
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            txtNumImages.Text = imageCount.ToString();
                        });
                    }
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
            }
        }

        private async Task UploadImages()
        {
            bool success = false;
            try
            {
                var project = await trainingApi.GetProjectAsync(projectId);
                if (project != null)
                {
                    var tags = await trainingApi.GetTagsAsync(projectId);
                    var tag = tags.Tags.Where(t => t.Name == tagName).FirstOrDefault();
                    if (tag == null)
                        tag = await trainingApi.CreateTagAsync(projectId, tagName);

                    var imageFiles = trainingImages.Select(img => new ImageFileCreateEntry(Path.GetFileName(img), File.ReadAllBytes(img))).ToList();

                    var summary = await trainingApi.CreateImagesFromFilesAsync(project.Id, new ImageFileCreateBatch(imageFiles, new List<Guid>() { tag.Id }));
                    success = summary.IsBatchSuccessful;

                    trainingImages.Clear();
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    listResponse.Items.Add(exc.Message);
                });
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (success)
                {
                    listResponse.Items.Add("Uploaded images successfully at " + DateTime.Now.ToString());
                }
                else
                    listResponse.Items.Add("Uploaded images failed at " + DateTime.Now.ToString());
            });
        }

        private async Task TrainModel()
        {
            try
            {
                var iteration = await trainingApi.TrainProjectAsync(projectId);
                iterationId = iteration.Id;

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    listResponse.Items.Add("Begin training");

                    trainTimer.Start();
                });
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    listResponse.Items.Add(exc.Message);
                });
            }
        }

        private async Task PredictImage()
        {
            try
            {
                using (var captureStream = new InMemoryRandomAccessStream())
                {
                    await mediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), captureStream);
                    StorageFolder folder = ApplicationData.Current.TemporaryFolder;
                    StorageFile file = await folder.CreateFileAsync(DateTime.Now.ToFileTime() + ".jpg");

                    using (var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        var decoder = await BitmapDecoder.CreateAsync(captureStream);
                        var encoder = await BitmapEncoder.CreateForTranscodingAsync(fileStream, decoder);

                        var properties = new BitmapPropertySet {
            { "System.Photo.Orientation", new BitmapTypedValue(PhotoOrientation.Normal, PropertyType.UInt16) }
        };
                        await encoder.BitmapProperties.SetPropertiesAsync(properties);

                        await encoder.FlushAsync();
                    }

                    using (var stream = File.Open(file.Path, FileMode.Open))
                    {
                        var result = await predictionEndpoint.PredictImageAsync(projectId, stream);


                        // Loop over each prediction and write out the results
                        List<string> results = new List<string>();
                        foreach (var c in result.Predictions)
                        {
                            results.Add($"\t{c.Tag}: {c.Probability:P1}");
                        }

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            foreach (string line in results)
                            {
                                listResponse.Items.Add(line);
                            }
                        });
                    }
                }
            }
            catch (Exception exc)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    listResponse.Items.Add(exc.Message);
                });
            }
        }

        private void btnTrain_Click(object sender, RoutedEventArgs e)
        {
            tagName = entryTag.Text;

            Task.Run(TrainModel);
        }

        private void entryTag_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(entryTag.Text))
                btnTrain.IsEnabled = false;
            else
                btnTrain.IsEnabled = true;
        }

        private void btnPredict_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(PredictImage);
        }

        private void btnCLear_Click(object sender, RoutedEventArgs e)
        {
            trainingImages.Clear();
            saveFolder = null;
        }

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            tagName = entryTag.Text;
            if (trainingImages.Count == 0)
            {
                var myPictures = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Pictures);
                if (saveFolder == null)
                    saveFolder = await myPictures.RequestAddFolderAsync();

                if (saveFolder != null)
                {
                    var files = await saveFolder.GetFilesAsync();
                    trainingImages.AddRange(files.Select(f => f.Path));
                }
            }

            if (trainingImages.Count == 0)
            {
                listResponse.Items.Add("No images selected");
            }
            else
            {
                await Task.Run(UploadImages);
            }

        }
    }
}
