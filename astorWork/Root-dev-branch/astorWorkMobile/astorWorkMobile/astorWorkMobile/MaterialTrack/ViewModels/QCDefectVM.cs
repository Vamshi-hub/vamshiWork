using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class QCDefectVM : MasterVM
    {
        public QCDefectVM() : base()
        {
            _qcPhotos = new ObservableCollection<QCPhoto>();
            QCRectifyCommand = new Command(RectifyButtonClicked);
        }
        public bool ShowAssignedTo
        {
            get
            {
                return QCDefectDetails != null && QCDefectDetails.ID != 0;
            }
        }

        private Organisation _selectedSubcon { get; set; }
        public Organisation SelectedSubcon
        {
            get { return _selectedSubcon; }
            set
            {
                if (value != null)
                {
                    _selectedSubcon = value;
                    QCDefectDetails.SelectedSubconID = _selectedSubcon.ID;
                }
                OnPropertyChanged("SelectedSubcon");
            }
        }
        private List<Organisation> _listSubcons { get; set; }
        public List<Organisation> ListSubcons
        {
            get
            {
                return _listSubcons;
            }
            set
            {
                _listSubcons = value;
                if (_listSubcons != null && _listSubcons.Count == 1)
                {
                    SelectedSubcon = _listSubcons[0];
                    QCDefectDetails.SelectedSubconID = SelectedSubcon.ID;
                }
                OnPropertyChanged("ListSubcons");
            }
        }

        private Material _material;
        public Material Material
        {
            get => _material;
            set
            {
                _material = value;
                OnPropertyChanged("Material");
            }
        }

        public PPVCJob Job => ViewModelLocator.siteListDefectsVM.Job;

        private QCCase _QCOpenCase;
        public QCCase QCOpenCase
        {
            get => _QCOpenCase;
            set
            {
                _QCOpenCase = value;
                OnPropertyChanged("QCOpenCase");
            }
        }

        public bool ShowSubmitButton => (Convert.ToInt32(Application.Current.Properties["entry_point"]) != 5 && (QCDefectDetails != null && QCDefectDetails.IsOpen && (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 1 || Convert.ToInt32(Application.Current.Properties["entry_point"]) == 4)));

        public bool ShowCloseButton => (Convert.ToInt32(Application.Current.Properties["entry_point"]) != 5 && (_QCDefectDetails != null && _QCDefectDetails.IsRectified && _QCDefectDetails.ID > 0 && (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 1 || Convert.ToInt32(Application.Current.Properties["entry_point"]) == 4)));

        public bool ShowRectifyButton => (Convert.ToInt32(Application.Current.Properties["entry_point"]) != 5 && (_QCDefectDetails != null && _QCDefectDetails.IsOpen && (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 2 || Convert.ToInt32(Application.Current.Properties["entry_point"]) == 0)));

        public bool ShowCapturePhotoButton => (Convert.ToInt32(Application.Current.Properties["entry_point"]) != 5 && (_QCDefectDetails == null || _QCDefectDetails.IsOpen || _QCDefectDetails.IsRectified));

        public bool ShowViewPhotoButton => (QCPhotos != null && QCPhotos.Count > 0);

        private ObservableCollection<QCPhoto> _qcPhotos;

        public ObservableCollection<QCPhoto> QCPhotos
        {
            get => _qcPhotos;
            set
            {

                _qcPhotos = value;
                OnPropertyChanged("QCPhotos");
                OnPropertyChanged("ShowViewPhotoButton");
            }
        }

        public void AddPhoto(QCPhoto photo)
        {
            if (_qcPhotos != null && photo != null)
            {
                _qcPhotos.Add(photo);
                OnPropertyChanged("ShowViewPhotoButton");
            }
        }

        private QCDefect _QCDefectDetails;
        public QCDefect QCDefectDetails
        {
            get => _QCDefectDetails;
            set
            {
                _QCDefectDetails = value;
                OnPropertyChanged("QCDefectDetails");
                OnPropertyChanged("ShowCloseButton");
                OnPropertyChanged("ShowSubmitButton");
                OnPropertyChanged("ShowDefectInfo");
                OnPropertyChanged("UpdateButtonLabel");
                OnPropertyChanged("DefectStatusColor");
                OnPropertyChanged("DefectQCStatusText");

                if (_QCDefectDetails != null)
                {
                    IsLoading = true;
                    if (_QCDefectDetails.ID != 0 && !_QCDefectDetails.IsDummyDefects)
                    {
                        Task.Run(() => ApiClient.Instance.MTGetQCDefectPhots(_QCDefectDetails.ID)).ContinueWith((t) =>
                        {
                            if (t.Result.data != null)
                                QCPhotos = new ObservableCollection<QCPhoto>(t.Result.data as List<QCPhoto>);
                            else
                            {
                                QCPhotos = null;
                                ErrorMessage = t.Result.message;
                            }


                        });
                    }
                    IsLoading = false;
                }
            }
        }

        private QCPhoto _selectedPhoto;
        public QCPhoto SelectedPhoto
        {
            get => _selectedPhoto;
            set
            {
                _selectedPhoto = value;
                OnPropertyChanged("SelectedPhoto");
            }
        }

        private List<User> _listOfSubcon { get; set; }

        public List<User> ListOfSubcon
        {
            get => _listOfSubcon;
            set
            {
                _listOfSubcon = value;
                OnPropertyChanged("ListOfSubcon");
            }
        }

        //private User _selectedSubcon { get; set; }

        //public User SelectedSubcon
        //{
        //    get => _selectedSubcon;
        //    set
        //    {
        //        _selectedSubcon = value;
        //        OnPropertyChanged("SelectedSubcon");
        //    }
        //}

        //public bool ShowOrganisationAssigned
        //{
        //    get
        //    {
        //        if (QCDefectDetails == null || QCDefectDetails.SubconName == null)
        //            return false;

        //        return QCDefectDetails.SubconName.Length > 0;
        //    }
        //}

        public bool ShowOrganisations
        {
            get
            {
                return QCDefectDetails != null && QCDefectDetails.ID == 0;
            }
        }

        public bool ShowDefectInfo => QCDefectDetails != null && QCDefectDetails.ID > 0;

        public string UpdateButtonLabel => ShowDefectInfo ? "Update Defect" : "Create Defect";

        private void RectifyButtonClicked()
        {
            var htmlTemplate = @"<p>Hi,</p>
<p>QC Case No.: <strong>[QC_CASE_NAME]</strong><br>
A defect under this QC case has been rectified by <strong>[SUBCON_USER]</strong> at <strong>[TIMESTAMP]</strong>.</p><p>Remarks: <em>[QC_REMARKS]</em></p>
<p>Regards,<br>astorWork Team</p>";
            var bodyHtml = htmlTemplate.Replace("[QC_CASE_NAME]", QCOpenCase.CaseName)
                .Replace("[SUBCON_USER]", "Michelangelo")
                .Replace("[TIMESTAMP]", DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"))
                .Replace("[QC_REMARKS]", QCDefectDetails.Remarks);
            Task.Run(() => EmailClient.Instance.SendSingleAsync("benjamin.chua@astoriasolutions.com", "Benjamin", "QC Defect Rectified", bodyHtml));
            QCDefectDetails.StatusCode = 1;

            // Navigation.NavigationStack[Navigation.NavigationStack.Count - 1].DisplayAlert("Success", "We have notified main-con about this rectification", "OK");
            // Navigation.PopAsync();
            DisplaySnackBar("We have notified main-con about this rectification", Enums.PageActions.PopAsync, Enums.MessageActions.Success, null, null);
        }

        public ICommand QCRectifyCommand { get; set; }


        public override void Reset()
        {
            base.Reset();
            _neverLoadBefore = false;
            Material = null;
            QCOpenCase = new QCCase
            {
                ID = 0,
                IsOpen = true
            };
            QCDefectDetails = new QCDefect
            {
                IsOpen = true,
                ID = 0
            };

            if (QCPhotos != null)
            {
                QCPhotos.Clear();
            }
        }
    }
}
