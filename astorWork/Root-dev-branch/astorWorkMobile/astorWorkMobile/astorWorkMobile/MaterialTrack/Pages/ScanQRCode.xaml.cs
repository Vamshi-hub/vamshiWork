using astorWorkMobile.Shared.Utilities;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanQRCode : ContentPage
    {
        private List<string> trackerTags = new List<string>();
        public ScanQRCode()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }
        private void scanView_OnScanResult(ZXing.Result result)
        {
            string data = string.Empty;
            //if (result.Text.ToLower().Contains("tag"))
            //    data = result.Text.Substring(result.Text.LastIndexOf("/")+1).Trim();
            //else
            data = result.Text;
            if (!trackerTags.Contains(data))
            {
                trackerTags.Add(result.Text);
            }

            scanView.IsAnalyzing = false;
            Task.Run(() => ViewModelLocator.scanTrackerVM.GetTrackerAssociations(trackerTags.ToArray())).ContinueWith(t =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    GridQRList.LayoutTo(new Rectangle(0, 0, Width, Height), 300, Easing.Linear);
                    SwipeIcon.Source = ImageSource.FromFile("ic_double_down.png");
                });
            });
        }
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            SwipeIcon.Source = ImageSource.FromFile("ic_double_up.png");
            GridQRList.HeightRequest = SwipeHeader.Height;
            scanView.IsAnalyzing = true;
            ViewModelLocator.scanTrackerVM.ListMaterialItems = new System.Collections.ObjectModel.ObservableCollection<ViewModels.MaterialFrameVM>();
            ViewModelLocator.scanTrackerVM.OnPropertyChanged("ListMaterialItems");
            trackerTags.Clear();
            if (trackerTags?.Count > 0)
            {
                scanView.IsAnalyzing = false;
                Task.Run(() =>
                ViewModelLocator.scanTrackerVM.GetTrackerAssociations(trackerTags.ToArray()))
                .ContinueWith((t) =>
                {
                    scanView.IsAnalyzing = true;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                scanView.IsScanning = true;
            }
        }
        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            scanView.IsAnalyzing = false;
            ViewModelLocator.materialFrameVM.IsScanning = false;
            ViewModelLocator.materialFrameVM.OnPropertyChanged("IsVendorScanning");
            ViewModelLocator.scanTrackerVM.Reset();
        }
        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            if (e.Direction == SwipeDirection.Up)
            {
                GridQRList.LayoutTo(new Rectangle(0, 0, Width, Height), 300, Easing.Linear);
                SwipeIcon.Source = ImageSource.FromFile("ic_double_down.png");
                scanView.IsAnalyzing = false;
            }
            else if (e.Direction == SwipeDirection.Down)
            {
                GridQRList.LayoutTo(new Rectangle(0, Height - SwipeHeader.Height, Width, Height), 300, Easing.Linear);
                SwipeIcon.Source = ImageSource.FromFile("ic_double_up.png");
                scanView.IsAnalyzing = true;
            }
        }
        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }

        private void PinchGestureRecognizer_PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {

        }
    }
}