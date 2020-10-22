using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile.Shared.Utilities
{
    public static class PhotoResizer
    {
        public static async Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality)
        {
            byte[] result = null;
            result = await DependencyService.Get<IPhotoResizer>().ResizeImage(imageData, width, height, quality);
            //#if __IOS__
            //          result = ResizeImageIOS ( imageData, width, height, quality);
            //#endif
            //#if __ANDROID__
            //          result = ResizeImageAndroid ( imageData, width, height, quality );
            //#endif
            //#if WINDOWS_PHONE
            //          result = ResizeImageWinPhone ( imageData, width, height, quality );
            //#endif
            return await Task.FromResult(result);
        }


        //#if __IOS__
        //      public static byte[] ResizeImageIOS (byte[] imageData, float width, float height, int quality)
        //      {
        //          UIImage originalImage = ImageFromByteArray (imageData);


        //          float oldWidth = (float)originalImage.Size.Width;
        //          float oldHeight = (float)originalImage.Size.Height;
        //          float scaleFactor = 0f;

        //          if (oldWidth > oldHeight)
        //          {
        //              scaleFactor = width / oldWidth;
        //          }
        //          else
        //          {
        //              scaleFactor = height / oldHeight;
        //          }

        //          float newHeight = oldHeight * scaleFactor;
        //          float newWidth = oldWidth * scaleFactor;

        //          //create a 24bit RGB image
        //          using (CGBitmapContext context = new CGBitmapContext (IntPtr.Zero,
        //              (int)newWidth, (int)newHeight, 8,
        //              (int)(4 * newWidth), CGColorSpace.CreateDeviceRGB (),
        //              CGImageAlphaInfo.PremultipliedFirst)) {

        //              RectangleF imageRect = new RectangleF (0, 0, newWidth, newHeight);

        //              // draw the image
        //              context.DrawImage (imageRect, originalImage.CGImage);

        //              UIKit.UIImage resizedImage = UIKit.UIImage.FromImage (context.ToImage ());

        //              // save the image as a jpeg
        //              return resizedImage.AsJPEG((float)quality).ToArray();
        //          }
        //      }

        //      public static UIKit.UIImage ImageFromByteArray(byte[] data)
        //      {
        //          if (data == null) {
        //              return null;
        //          }

        //          UIKit.UIImage image;
        //          try {
        //              image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
        //          } catch (Exception e) {
        //              Console.WriteLine ("Image load failed: " + e.Message);
        //              return null;
        //          }
        //          return image;
        //      }
        //#endif

        //#if __ANDROID__

        //      public static byte[] ResizeImageAndroid (byte[] imageData, float width, float height, int quality)
        //      {
        //      // Load the bitmap
        //      Bitmap originalImage = BitmapFactory.DecodeByteArray (imageData, 0, imageData.Length);

        //      float oldWidth = (float)originalImage.Width;
        //      float oldHeight = (float)originalImage.Height;
        //      float scaleFactor = 0f;

        //      if (oldWidth > oldHeight)
        //      {
        //          scaleFactor = width / oldWidth;
        //      }
        //      else
        //      {
        //          scaleFactor = height / oldHeight;
        //      }

        //      float newHeight = oldHeight * scaleFactor;
        //      float newWidth = oldWidth * scaleFactor;

        //      Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);

        //      using (MemoryStream ms = new MemoryStream())
        //      {
        //      resizedImage.Compress (Bitmap.CompressFormat.Jpeg, quality, ms);
        //      return ms.ToArray ();
        //      }
        //      }

        //#endif

        //#if WINDOWS_PHONE

        //      public static byte[] ResizeImageWinPhone (byte[] imageData, float width, float height)
        //      {
        //      byte[] resizedData;

        //      using (MemoryStream streamIn = new MemoryStream (imageData))
        //      {
        //      WriteableBitmap bitmap = PictureDecoder.DecodeJpeg (streamIn, (int)width, (int)height);

        //      using (MemoryStream streamOut = new MemoryStream ())
        //      {
        //      bitmap.SaveJpeg(streamOut, (int)width, (int)height, 0, 100);
        //      resizedData = streamOut.ToArray();
        //      }
        //      }
        //      return resizedData;
        //      }

        //#endif
    }
}
