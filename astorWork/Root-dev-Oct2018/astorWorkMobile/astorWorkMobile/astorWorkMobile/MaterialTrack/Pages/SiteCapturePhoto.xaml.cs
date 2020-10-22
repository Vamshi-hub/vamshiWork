using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SiteCapturePhoto : ContentPage
    {
        public SiteCapturePhoto()
        {
            InitializeComponent();
        }

        private void OnConfirmClicked(object sender, EventArgs e)
        {
            var imageBase64 = Convert.ToBase64String(ViewModelLocator.takePhotoVM.GetImageBytes());
            var qcPhoto = new QCPhoto
            {
                ImageBase64 = imageBase64,
                Remarks = ViewModelLocator.takePhotoVM.Remarks
            };
            ViewModelLocator.qcDefectVM.QCPhotos.Add(qcPhoto);

            ViewModelLocator.takePhotoVM.Reset();
            Navigation.PopAsync();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            ViewModelLocator.takePhotoVM.Reset();
            Navigation.PopAsync();
        }
    }
}