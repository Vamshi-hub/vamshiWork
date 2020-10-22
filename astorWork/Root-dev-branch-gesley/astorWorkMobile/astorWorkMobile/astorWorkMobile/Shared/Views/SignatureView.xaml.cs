using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.JobTrack.ViewModels;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
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
        private Type PageType;
        public SignatureView(Type type)
        {
            PageType = type;
            InitializeComponent();
        }

        private ImageConstructionSettings signatureImageSettings =
            new ImageConstructionSettings
            {
                BackgroundColor = Color.Transparent,
                ShouldCrop = true
            };

        private void SignPadView_Cleared(object sender, EventArgs e)
        {
            //object bindingContext = null;
            if (PageType == typeof(JobChecklistItemVM))
            {
                var bindingContext = (BindingContext as JobChecklistItemVM);
                if (bindingContext != null)
                {
                    bindingContext.ImageBase64 = null;
                }
            }
            else if (true)
            {
                var bindingContext = (BindingContext as RTOInspection);
                if (bindingContext != null)
                {
                    bindingContext.ImageBase64 = null;
                }
            }
        }

        private async void SignPadView_StrokeCompleted(object sender, EventArgs e)
        {
            //object bindingContext = null;
            byte[] data = null;
            var signPadView = (sender as SignaturePadView);
            if (PageType != null && signPadView != null)
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    var img = await signPadView.GetImageStreamAsync(SignatureImageFormat.Png, signatureImageSettings);
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
                if (PageType == typeof(JobChecklistItemVM))
                {
                    var bindingContext = (BindingContext as JobChecklistItemVM);
                    bindingContext.ImageBase64 = Convert.ToBase64String(data);
                }
                else
                {
                    var bindingContext = (BindingContext as RTOInspection);
                    bindingContext.ImageBase64 = Convert.ToBase64String(data);
                }
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}