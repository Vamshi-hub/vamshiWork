using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.JobTrack.ViewModels;
using astorWorkMobile.Shared.Classes;
using SignaturePad.Forms;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignatureView : ContentPage
    {
        public SignatureView()
        {
            InitializeComponent();
        }

        private ImageConstructionSettings signatureImageSettings =
            new ImageConstructionSettings
            {
                BackgroundColor = Color.Transparent,
                ShouldCrop = false
            };

        private void SignPadView_Cleared(object sender, EventArgs e)
        {
            var bindingContext = (BindingContext as JobChecklistItemVM);
            if (bindingContext != null)
            {
                bindingContext.ImageBase64 = null;
            }
        }

        private async void SignPadView_StrokeCompleted(object sender, EventArgs e)
        {
            var bindingContext = (BindingContext as JobChecklistItemVM);
            byte[] data = null;
            var signPadView = (sender as SignaturePadView);
            if (bindingContext != null && signPadView != null)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var img = await signPadView.GetImageStreamAsync(SignatureImageFormat.Png);
                    var signatureMemoryStream = new MemoryStream();
                    img.CopyTo(signatureMemoryStream);
                    data = signatureMemoryStream.ToArray();
                }
                else
                {
                    var img = await signPadView.GetImageStreamAsync(SignatureImageFormat.Png, signatureImageSettings);
                    var signatureMemoryStream = (MemoryStream)img;
                    data = signatureMemoryStream.ToArray();
                }

                //Func<Stream> streamFunc = () => {
                //    return signPadView.GetImageStreamAsync(SignatureImageFormat.Png, signatureImageSettings).Result;
                //};
                
                //bindingContext.SignatureImageSource = ImageSource.FromStream(streamFunc);
                bindingContext.ImageBase64 = Convert.ToBase64String(data);
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}