using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using astorWorkMobile.iOS.Services;
using astorWorkMobile.Shared.Classes;
using Foundation;
using UIKit;
[assembly: Xamarin.Forms.Dependency(typeof(PhotoOverlay))]
namespace astorWorkMobile.iOS.Services
{
    public class PhotoOverlay:IPhotoOverlay
    {
        public object GetImageOverlay()
        {
            var imageView = new UIImageView(UIImage.FromBundle("qc.jpg"));
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var screen = UIScreen.MainScreen.Bounds;
            imageView.Frame = screen;

            return imageView;
        }
    }
}