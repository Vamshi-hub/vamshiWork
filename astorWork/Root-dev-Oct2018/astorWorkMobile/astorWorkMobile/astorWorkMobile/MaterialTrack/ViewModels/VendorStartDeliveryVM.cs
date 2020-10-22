using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class VendorStartDeliveryVM : MasterVM
    {
        public Project Project { get; set; }
        public List<Location> Locations { get; set; }

        private Location _selectedLocation;
        public Location SelectedLocation
        {
            get
            {
                return _selectedLocation;
            }
            set
            {
                _selectedLocation = value;
                OnPropertyChanged("ShowUpdateForm");
            }
        }

        public bool ShowUpdateForm
        {
            get
            {
                return _selectedLocation != null;
            }
        }

        private Tracker _trackerInfo;
        public Tracker TrackerInfo { get
            {
                return _trackerInfo;
            }
            set
            {
                _trackerInfo = value;
                if (_trackerInfo != null)
                {
                    Task.Run(() => ApiClient.Instance.MTGetListMRF(Project.id, _trackerInfo.markingNo).ContinueWith(t =>
                    {
                        if (t.Result.status == 0)
                        {
                            var listMRFs = t.Result.data as List<MRF>;
                            if (listMRFs != null)
                            {
                                ListMRF = new ObservableCollection<MRF>(
                                    listMRFs.Where(mrf => mrf.OrderDate <= DateTimeOffset.Now));

                                if (ListMRF.Count == 0)
                                    ErrorMessage = "No MRF available";
                                else
                                {
                                    if (_trackerInfo.displayMaterial)
                                    {
                                        SelectedMRF = ListMRF.FirstOrDefault(mrf => mrf.MrfNo == _trackerInfo.material.mrfNo);
                                        SelectMRFEnabled = false;
                                        OnPropertyChanged("SelectMRFEnabled");
                                    }
                                }
                            }
                        }
                        else
                        {
                            ErrorMessage = t.Result.message;
                            ListMRF = null;
                        }
                    }));
                }

                OnPropertyChanged("TrackerInfo");
            }
        }

        private ObservableCollection<MRF> _listMRF;
        public ObservableCollection<MRF> ListMRF
        {
            get { return _listMRF; }
            set { _listMRF = value; OnPropertyChanged("ListMRF"); }
        }

        private MRF _selectedMRF;
        public MRF SelectedMRF
        {
            get
            {
                return _selectedMRF;
            }
            set
            {
                _selectedMRF = value;
                OnPropertyChanged("SelectedMRF");
                OnPropertyChanged("SubmitButtonEnabled");
                OnPropertyChanged("ShowQCField");
            }
        }

        public bool SelectMRFEnabled { get; set; }

        private string _qcRemarks;
        public string QCRemarks { get
            {
                return _qcRemarks;
            }
            set
            {
                _qcRemarks = value;
                OnPropertyChanged("QCRemarks");
            }
        }

        public bool ShowQCField
        {
            get
            {
                return _selectedMRF != null;
            }
        }

        public bool SubmitButtonEnabled
        {
            get
            {
                return _selectedMRF != null;
            }
        }

        public TogglePassVM TogglePass { get; set; }

        public VendorStartDeliveryVM() : base()
        {
            TogglePass = new TogglePassVM();
            _neverLoadBefore = false;
        }

        public override void Reset()
        {
            base.Reset();

            SelectMRFEnabled = true;
            SelectedMRF = null;
            QCRemarks = string.Empty;
            SelectedLocation = null;
            _neverLoadBefore = false;
            TogglePass = new TogglePassVM();
        }
    }
}
