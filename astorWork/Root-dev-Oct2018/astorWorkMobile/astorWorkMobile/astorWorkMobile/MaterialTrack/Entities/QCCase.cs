using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class QCCase : MasterVM
    {
        public int ID { get; set; }
        public string CaseName { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public bool IsOpen { get; set; }
        public bool IsClosed { get { return !IsOpen; } }
        public int CountOpenDefects { get; set; }
        public int CountClosedDefects { get; set; }

        private bool _expanded;
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                _expanded = value;
                OnPropertyChanged("Expanded");
                OnPropertyChanged("ExpansionIcon");

                RefreshDefects();
            }
        }

        public void RefreshDefects()
        {
            if (ListQCDefect == null)
            {
                IsLoading = true;

                Task.Run(() => ApiClient.Instance.MTGetQCDefects(ID))
                    .ContinueWith(t =>
                    {
                        if (t.Result.data != null)
                        {
                            var listDefects = t.Result.data as List<QCDefect>;
                            if (listDefects != null)
                                ListQCDefect = new ObservableCollection<QCDefect>(listDefects);
                        }
                        else
                        {
                            ListQCDefect = null;
                            ErrorMessage = t.Result.message;
                        }

                        IsLoading = false;
                    });
            }
        }

        void OnRowTapped()
        {
            Expanded = !Expanded;
        }

        void AddDefect()
        {
            ViewModelLocator.qcDefectVM.QCOpenCase = this;
            ViewModelLocator.qcDefectVM.QCDefectDetails = new QCDefect();
            ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen = true;
            ViewModelLocator.qcDefectVM.QCPhotos.Clear();
            ViewModelLocator.singleScanTrackerVM.Navigation.PushAsync(new DefectDetails());
        }

        public string ExpansionIcon
        {
            get
            {
                if (_expanded)
                    return "ic_keyboard_arrow_up.png";
                else
                    return "ic_keyboard_arrow_down.png";
            }
        }
        private ObservableCollection<QCDefect> _listQCDefect;
        public ObservableCollection<QCDefect> ListQCDefect
        {
            get
            {
                return _listQCDefect;
            }
            set
            {
                _listQCDefect = value;
                OnPropertyChanged("ListQCDefect");
            }
        }

        /*
        private QCDefect _selectedDefect;
        public QCDefect SelectedDefect
        {
            get
            {
                return _selectedDefect;
            }
            set
            {
                if ((_selectedDefect == null && value != null) || (_selectedDefect != null && value != null && _selectedDefect.ID != value.ID))
                {
                }

                _selectedDefect = null;
                OnPropertyChanged("SelectedDefect");
            }
        }
        */

        public ICommand ExpandCommand { get; set; }
        public ICommand AddDefectCommand { get; set; }

        public QCCase()
        {
            ExpandCommand = new Command(OnRowTapped);
            AddDefectCommand = new Command(AddDefect);
        }
    }
}
