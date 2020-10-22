using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class QCCase : MasterVM
    {
        public int ID { get; set; }
        public string CaseName { get; set; }
        public string CreatedBy { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public bool IsOpen { get; set; }
        public bool IsClosed { get { return !IsOpen; } }
        public int CountOpenDefects { get; set; }
        public int CountClosedDefects { get; set; }

        public bool _showAddDefect { get; set; }
        public bool ShowAddDefect
        {
            get
            {
                if (Application.Current.Properties.ContainsKey("entry_point") && (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 1 || Convert.ToInt32(Application.Current.Properties["entry_point"]) == 4))
                {
                    _showAddDefect = true;
                }
                else
                {
                    _showAddDefect = false;
                }
               // OnPropertyChanged("ShowAddDefect");
                return _showAddDefect;
            }
        }
        private bool _expanded;
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                _expanded = value;
                OnPropertyChanged("Expanded");
                OnPropertyChanged("ExpansionIcon");

                Task.Run(RefreshDefects);
            }
        }

        public void RefreshDefects()
        {
            int OrganisationType = Convert.ToInt32(Application.Current.Properties["organisationType"]);
            if (ListQCDefect == null)
            {
                try
                {
                    IsLoading = true;
                    Task.Run(async () =>
                    {
                        var Result = await ApiClient.Instance.MTGetQCDefects(ID);
                        if (Result.data != null)
                        {
                            List<QCDefect> listDefects = Result.data as List<QCDefect>;
                            if (listDefects != null)
                            {
                                if (Application.Current.Properties["entry_point"].Equals(0) || Application.Current.Properties["entry_point"].Equals(2))
                                {
                                    listDefects = listDefects.Where(p => p.SelectedSubconID == Convert.ToInt32(Application.Current.Properties["organisationID"])).ToList();
                                }
                                ListQCDefect = new ObservableCollection<QCDefect>(listDefects);
                            }
                        }
                        else
                        {
                            ListQCDefect = null;
                            ErrorMessage = Result.message;
                        }

                        IsLoading = false;
                    });
                }
                catch (Exception ex)
                {

                    throw;
                }

            }
        }

        void OnRowTapped()
        {
            Expanded = !Expanded;
        }

        void AddDefect()
        {
            ViewModelLocator.qcDefectVM.QCOpenCase = this;
            ViewModelLocator.qcDefectVM.QCDefectDetails = new QCDefect
            {
                ID = 0,
                IsOpen = true
            };
            ViewModelLocator.qcDefectVM.QCPhotos.Clear();

            ViewModelLocator.qcDefectVM.Navigation.PushAsync(new DefectDetails());
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
                Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("ListQCDefect"); });
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
