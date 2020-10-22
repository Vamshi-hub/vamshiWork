using astorWorkMobile.JobTrack.ViewModels;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages.Partials
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ChatInputBarView : ContentView
	{
		public ChatInputBarView ()
		{
			InitializeComponent ();

            //if (Device.RuntimePlatform == Device.iOS)
            //{
            //    this.SetBinding(HeightRequestProperty, new Binding("Height", BindingMode.OneWay, null, null, null, chatTextInput));
            //}
        }

        public void Handle_Completed(object sender, EventArgs e)
        {
            //chatTextInput.Focus();
        }

        public void UnFocusEntry(object sender, EventArgs e)
        {
            chatTextInput?.Unfocus();
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            Task.Run(ViewModelLocator.mtTakePhotoVM.CapturePhoto).ContinueWith((t) =>
                ConfirmPhoto(), TaskScheduler.FromCurrentSynchronizationContext());
        }
        async Task ConfirmPhoto()
        {
            var capturePhotoPage = new CapturePhoto();
            //capturePhotoPage.Disappearing += CapturePhotoPage_Disappearing;
            await Navigation.PushAsync(capturePhotoPage);
        }
    }
}