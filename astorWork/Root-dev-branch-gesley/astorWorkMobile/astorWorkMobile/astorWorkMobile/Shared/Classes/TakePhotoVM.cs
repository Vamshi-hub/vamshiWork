using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class TakePhotoVM : MasterVM
    {
        public ICommand ConfirmCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        public override void Reset()
        {
            base.Reset();
            photoBytes = null;
            Photo = null;
            Remarks = null;
            OnPropertyChanged("Photo");
            OnPropertyChanged("Remarks");
            OnPropertyChanged("ShowConfirmButton");
        }

        public ImageSource Photo { get; set; }
        public string Remarks { get; set; }
        public bool ShowConfirmButton
        {
            get
            {
                try
                {
                    return photoBytes != null && photoBytes.LongLength > 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private byte[] photoBytes;
        public Task<byte[]> GetThumbnailBytes()
        {
            // Compress the original photo
            var bytes = PhotoResizer.ResizeImage(photoBytes, 350, 350, 92);
            return bytes;
        }
        public byte[] GetOriginalPhotoBytes()
        {
            return photoBytes;
        }

        public async Task<bool> CapturePhoto()
        {
            IsLoading = true;
            photoBytes = null;
            bool status = false;
            try
            {
                await CrossMedia.Current.Initialize();
                Func<object> func = () =>
                {
                    var obj = DependencyService.Get<IPhotoOverlay>().GetImageOverlay();
                    return obj;
                };
                if (CrossMedia.Current.IsCameraAvailable && CrossMedia.Current.IsTakePhotoSupported)
                {
                    var originalPhoto = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                    {
                        OverlayViewProvider = func,
                        Directory = "QC-Photos",
                        Name = string.Format("qc.jpg"),
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        //SaveToAlbum = true,
                        MaxWidthHeight = 1920,
                        //MaxWidthHeight = 500,
                        CompressionQuality = 95
                    });

                    if (originalPhoto != null)
                    {
                        
                        Photo = ImageSource.FromStream(() =>
                        {
                            photoBytes = File.ReadAllBytes(originalPhoto.Path);
                            var stream = originalPhoto.GetStream();
                            OnPropertyChanged("ShowConfirmButton");
                            return stream;
                        });
                        status = true;
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
            catch (Exception exc)
            {
                ErrorMessage = exc.Message;
            }
            IsLoading = false;
            return status;
        }

        public async Task<bool> GetPhotoFromGallery()
        {
            IsLoading = true;
            photoBytes = null;
            bool status = false;
            try
            {
                await CrossMedia.Current.Initialize();

                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    var originalPhoto = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                    {
                        PhotoSize = PhotoSize.MaxWidthHeight,
                        MaxWidthHeight = 1920,
                        CompressionQuality = 95
                        //MaxWidthHeight = 500,
                        //CompressionQuality = 50
                    });

                    if (originalPhoto != null)
                    {
                        Photo = ImageSource.FromStream(() =>
                        {
                            photoBytes = File.ReadAllBytes(originalPhoto.Path);
                            var stream = originalPhoto.GetStream();
                            OnPropertyChanged("ShowConfirmButton");
                            return stream;
                        });
                        status = true;
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
            catch (Exception exc)
            {
                ErrorMessage = exc.Message;
            }
            IsLoading = false;
            return status;
        }

        public TakePhotoVM()
        {
            CrossMedia.Current.Initialize();
        }
    }
}
