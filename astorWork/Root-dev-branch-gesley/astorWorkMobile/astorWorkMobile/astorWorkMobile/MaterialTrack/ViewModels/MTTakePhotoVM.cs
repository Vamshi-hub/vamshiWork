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
    public class MTTakePhotoVM : TakePhotoVM
    {
        public bool DisplayButton
        {
            get
            {
                if (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 5)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public async void ConfirmClicked()
        {
            IsLoading = true;
            byte[] imageBytes = GetOriginalPhotoBytes();
            if (imageBytes != null && imageBytes.Length > 0)
            {
                var imageBase64 = Convert.ToBase64String(imageBytes);
                var qcPhoto = new QCPhoto
                {
                    ImageBase64 = imageBase64,
                    Remarks = Remarks
                };
                ViewModelLocator.qcDefectVM.AddPhoto(qcPhoto);

                Reset();
                IsLoading = false;
                await Navigation.PopAsync();
            }
            else
            {
                ErrorMessage = "Image not available";
            }
            IsLoading = false;
        }

        public async void CancelClicked()
        {
            Reset();
            await Navigation.PopAsync();
        }

        public MTTakePhotoVM()
        {
            ConfirmCommand = new Command(ConfirmClicked);
            CancelCommand = new Command(CancelClicked);
        }
    }
}
