using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using astorWorkMobile.iOS.Services;
using astorWorkMobile.Shared.Utilities;
using CoreGraphics;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhotoResizerService))]
namespace astorWorkMobile.iOS.Services
{
    class PhotoResizerService : IPhotoResizer
    {
        public Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality)
        {
            UIImage originalImage = ImageFromByteArray(imageData);

            float oldWidth = (float)originalImage.Size.Width;
            float oldHeight = (float)originalImage.Size.Height;
            float scaleFactor = 0f;

            if (oldWidth > oldHeight)
            {
                scaleFactor = width / oldWidth;
            }
            else
            {
                scaleFactor = height / oldHeight;
            }

            float newHeight = oldHeight * scaleFactor;
            float newWidth = oldWidth * scaleFactor;

            UIGraphics.BeginImageContext(new SizeF(newWidth, newHeight));
            originalImage.Draw(new RectangleF(0, 0, newWidth, newHeight));
            var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            var bytesImagen = resizedImage.AsJPEG((float)quality).ToArray();
            resizedImage.Dispose();
            return Task.FromResult(bytesImagen);

            ////create a 24bit RGB image
            //using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
            //    (int)newWidth, (int)newHeight, 8,
            //    (int)(4 * newWidth), CGColorSpace.CreateDeviceRGB(),
            //    CGImageAlphaInfo.PremultipliedFirst))
            //{
            //    RectangleF imageRect = new RectangleF(0, 0, newWidth, newHeight);

            //    // draw the image
            //    context.DrawImage(imageRect, originalImage.CGImage);

            //    UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage());

            //    // save the image as a jpeg
            //    return Task.FromResult(resizedImage.AsJPEG((float)quality).ToArray());
            //}
        }

        public static UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }
            return image;
        }
    }
}