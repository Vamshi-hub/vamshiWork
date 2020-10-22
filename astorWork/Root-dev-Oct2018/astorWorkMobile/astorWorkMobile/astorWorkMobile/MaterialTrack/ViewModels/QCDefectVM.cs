using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class QCDefectVM : MasterVM
    {
        public QCDefectVM() : base()
        {
            _qcPhotos = new ObservableCollection<QCPhoto>();
        }

        private Material _material;
        public Material Material { get
            {
                return _material;
            }
            set
            {
                _material = value;
                OnPropertyChanged("Material");
            }
        }
        private QCCase _QCOpenCase;
        public QCCase QCOpenCase
        {
            get
            {
                return _QCOpenCase;
            }
            set
            {
                _QCOpenCase = value;
                OnPropertyChanged("QCOpenCase");
            }
        }

        private int _StageAuditID;
        public int StageAuditID
        {
            get
            {
                return _StageAuditID;
            }
            set
            {
                _StageAuditID = value;
                OnPropertyChanged("StageAuditID");
            }
        }

        public bool IsCloseButtonVisible
        {
            get
            {
                return (_QCDefectDetails != null && _QCDefectDetails.IsOpen && _QCDefectDetails.ID != 0);
            }
        }

        public bool IsSubmitButtonVIsible
        {
            get
            {
                return (_QCDefectDetails != null && _QCDefectDetails.IsOpen);
            }
        }

        private ObservableCollection<QCPhoto> _qcPhotos;

        public ObservableCollection<QCPhoto> QCPhotos
        {
            get
            {
                return _qcPhotos;
            }
            set
            {

                _qcPhotos = value;
                OnPropertyChanged("QCPhotos");
            }
        }

        private QCDefect _QCDefectDetails;
        public QCDefect QCDefectDetails
        {
            get
            {
                return _QCDefectDetails;
            }
            set
            {
                _QCDefectDetails = value;
                OnPropertyChanged("QCDefectDetails");
                OnPropertyChanged("IsCloseButtonVisible");
                OnPropertyChanged("IsSubmitButtonVIsible");
                OnPropertyChanged("ShowDefectInfo");
                OnPropertyChanged("UpdateButtonLabel");

                if (_QCDefectDetails != null)
                {
                    IsLoading = true;
                    if (_QCDefectDetails.ID != 0)
                    {
                        Task.Run(() => ApiClient.Instance.MTGetQCDefectPhots(_QCDefectDetails.ID)).ContinueWith((t) =>
                        {
                            if (t.Result.data != null)
                            {
                                QCPhotos = new ObservableCollection<QCPhoto>(t.Result.data as List<QCPhoto>);
                            }
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
            get
            {
                return _selectedPhoto;
            }
            set
            {
                _selectedPhoto = value;
                OnPropertyChanged("SelectedPhoto");
            }
        }

        public bool ShowDefectInfo
        {
            get
            {
                return QCDefectDetails.ID > 0;
            }
        }

        public string UpdateButtonLabel
        {
            get
            {
                return ShowDefectInfo ? "Update Defect" : "Create Defect";
            }
        }
        
    }
}
