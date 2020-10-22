using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class QCPhoto
    {
        public QCPhoto()
        {
            PrevPhotoCommand = new Command(PrevButtonClicked);
            NextPhotoCommand = new Command(NextButtonClicked);
        }

        public int ID { get; set; }
        public string Url { get; set; }
        public string Remarks { get; set; }
        public bool IsOpen { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageBase64 { get; set; }

        private Xamarin.Forms.ImageSource image;
        public Xamarin.Forms.ImageSource Image
        {
            get
            {
                if (image == null)
                {
                    if (!string.IsNullOrEmpty(ImageBase64))
                        image = Xamarin.Forms.ImageSource.FromStream(
                            () => new MemoryStream(Convert.FromBase64String(ImageBase64)));
                    else
                        image = Url;
                }
                return image;
            }
        }

        public int PhotoIndex
        {
            get
            {
                return ViewModelLocator.qcDefectVM.QCPhotos.IndexOf(this);
            }
        }

        public bool HasNext
        {
            get
            {
                bool result = false;
                if(ViewModelLocator.qcDefectVM.QCPhotos != null && ViewModelLocator.qcDefectVM.QCPhotos.Contains(this))
                {
                    if (PhotoIndex < ViewModelLocator.qcDefectVM.QCPhotos.Count - 1)
                        result = true;
                }
                return result;
            }
        }

        public bool HasPrev
        {
            get
            {
                bool result = false;
                if (ViewModelLocator.qcDefectVM.QCPhotos != null && ViewModelLocator.qcDefectVM.QCPhotos.Contains(this))
                {
                    if (PhotoIndex > 0)
                        result = true;
                }
                return result;
            }
        }

        public string IndexLabel
        {
            get
            {
                string result = string.Empty;
                if (ViewModelLocator.qcDefectVM.QCPhotos != null && ViewModelLocator.qcDefectVM.QCPhotos.Contains(this))
                {
                    result = string.Format("{0}/{1}",
                        PhotoIndex + 1,
                        ViewModelLocator.qcDefectVM.QCPhotos.Count);
                }
                return result;
            }
        }

        void PrevButtonClicked()
        {
            if (ViewModelLocator.qcDefectVM.SelectedPhoto == this && 
                ViewModelLocator.qcDefectVM.QCPhotos != null)
            {
                if (PhotoIndex > 0)
                    ViewModelLocator.qcDefectVM.SelectedPhoto = ViewModelLocator.qcDefectVM.QCPhotos[PhotoIndex - 1];
            }
        }

        void NextButtonClicked()
        {
            if (ViewModelLocator.qcDefectVM.SelectedPhoto == this &&
                ViewModelLocator.qcDefectVM.QCPhotos != null)
            {
                if (PhotoIndex < ViewModelLocator.qcDefectVM.QCPhotos.Count - 1)
                    ViewModelLocator.qcDefectVM.SelectedPhoto = ViewModelLocator.qcDefectVM.QCPhotos[PhotoIndex + 1];
            }
        }

        public ICommand PrevPhotoCommand { get; set; }
        public ICommand NextPhotoCommand { get; set; }
    }
}
