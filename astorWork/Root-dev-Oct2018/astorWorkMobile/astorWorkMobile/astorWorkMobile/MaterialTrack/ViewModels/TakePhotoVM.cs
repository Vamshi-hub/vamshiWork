using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class TakePhotoVM : MasterVM
    {
        public override void Reset()
        {
            base.Reset();
            imageBytes = null;
            Photo = null;
            Remarks = null;
            OnPropertyChanged("Photo");
            OnPropertyChanged("Remarks");
        }

        public ImageSource Photo { get; set; }
        public string Remarks { get; set; }
        public bool ShowConfirmButton {
            get {
                return imageBytes != null && imageBytes.LongLength > 0;
            }
        }

        private byte[] imageBytes;
        public byte[] GetImageBytes()
        {
            return imageBytes;
        }

        public async Task CapturePhoto()
        {
            IsLoading = true;
            imageBytes = null;

            try
            {
                await CrossMedia.Current.Initialize();

                if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                {
                    var photo = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "QC-Photos",
                        Name = string.Format("qc.jpg"),
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1920,
                        CompressionQuality = 92
                    });

                    if (photo != null)
                    {
                        Photo = ImageSource.FromStream(() =>
                        {
                            imageBytes = File.ReadAllBytes(photo.Path);
                            var stream = photo.GetStream();

                            OnPropertyChanged("ShowConfirmButton");

                            return stream;
                        });
                        OnPropertyChanged("Photo");
                    }
                    else
                        ErrorMessage = "Photo has not been taken";
                }
                else
                {
                    ErrorMessage = "No camera available.";
                }
            }
            catch(Exception exc)
            {
                ErrorMessage = exc.Message;
            }
            IsLoading = false;
        }
    }
}
