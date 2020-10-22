using astorWorkMobile.Shared.Utilities;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobScanQR : ContentPage
    {
        string tracker = string.Empty;
        public JobScanQR()
        {
            ViewModelLocator.jobScanVM.SubConId = 0;
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }
        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            ViewModelLocator.jobScanVM.Jobs = new List<Entities.JobScheduleDetails>();
            ViewModelLocator.jobScanVM.OnPropertyChanged("Jobs");
            ViewModelLocator.jobScanVM.OnPropertyChanged("ShowJobsList");
            SwipeIcon.Source = ImageSource.FromFile("ic_double_up.png");
            GridQRList.HeightRequest = SwipeHeader.Height;
            if (tracker?.Length > 0)
            {
                scanView.IsAnalyzing = false;
                Task.Run(() =>
                ViewModelLocator.jobScanVM.GetJobScheduleByTracker(tracker))
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
        private void scanView_OnScanResult(ZXing.Result result)
        {
            if (!string.IsNullOrEmpty(result.Text))
            {
                tracker = result.Text;
                scanView.IsAnalyzing = false;
                if (!string.IsNullOrEmpty(tracker))
                {
                    Task.Run(() => ViewModelLocator.jobScanVM.GetJobScheduleByTracker(tracker)).ContinueWith(t =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            ViewModelLocator.jobScanVM.ScannedItemsGrid = GridQRList;
                            ViewModelLocator.jobScanVM.Height = Height;
                            ViewModelLocator.jobScanVM.Width = Width;
                            GridQRList.LayoutTo(new Rectangle(0, 0, Width, Height), 300, Easing.Linear);
                            SwipeIcon.Source = ImageSource.FromFile("ic_double_down.png");
                        });
                    });
                }
            }
        }
        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            scanView.IsAnalyzing = false;
            ViewModelLocator.jobScanVM.ScannedItemsGrid = null;
            ViewModelLocator.jobScanVM.Reset();
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
    }
}