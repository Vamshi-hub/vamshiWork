using astorWorkMobile.JobTrack.Entities;
using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.JobTrack.ViewModels
{
    public class JobScheduleVM : MasterVM
    {
        public string Title { get; set; }
        public List<Organisation> ListSubcons { get; set; }
        //public List<Organisation> _listSubcons { get; set; }
        //public List<Organisation> ListSubcons
        //{
        //    get
        //    {
        //        if (_listSubcons != null && _listSubcons.Count == 1)
        //        {
        //            foreach (var item in JobSchedules)
        //            {
        //                item.SelectedSubcon = _listSubcons[0];
        //            }
        //        }
        //        return _listSubcons;
        //    }
        //    set { _listSubcons = value; }
        //}
        public bool IsRunning { get; set; }
        public int ClickedTileJobStatus { get; set; }
        private bool _allowScan;
        public bool AllowScan
        {
            get
            {
                return _allowScan;
            }
            set
            {
                _allowScan = value;
                OnPropertyChanged("AllowScan");
            }
        }
        public int FilterHeight
        {
            get
            {
                return FilterExpanded ? 250 : 50;
            }
        }
        public Thickness HeaderMargin
        {
            get
            {
                return FilterExpanded ? new Thickness(0, 260, 0, 0) : new Thickness(0, 60, 0, 0);
            }
        }
        public Thickness JobScheduleMargin
        {
            get
            {
                return FilterExpanded ? new Thickness(0, 300, 0, 0) : new Thickness(0, 100, 0, 0);
            }
        }
        public bool FilterExpanded { get; set; }
        public string FilterExpandIcon
        {
            get
            {
                return FilterExpanded ? "ic_keyboard_arrow_up" : "ic_keyboard_arrow_down";
            }
        }

        public ICommand FilterExpandedCommand { get; set; }
        public ICommand ScanQRFABCommand { get; set; }
        void ToggleFilterExpanded()
        {
            FilterExpanded = !FilterExpanded;
            OnPropertyChanged("FilterExpanded");
            OnPropertyChanged("FilterExpandIcon");
            OnPropertyChanged("HeaderMargin");
            OnPropertyChanged("JobScheduleMargin");
        }
        async void ScanQRFABClicked()
        {
            await Task.Run(GetCameraPermission)
                .ContinueWith(async (t) =>
                {
                    ViewModelLocator.scanTrackerVM.CameraReady = t.Result;
                    if (t.Result)
                    {
                        await Navigation.PushAsync(new JobScanQR());
                    }
                    else
                    {
                        ViewModelLocator.scanTrackerVM.DisplaySnackBar("No camera available", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        public List<string> ListBlocks { get; set; }
        //private List<string> _listBlocks { get; set; }
        //public List<string> ListBlocks
        //{
        //    get
        //    {
        //        if (_listBlocks != null && _listBlocks.Count == 1)
        //        {
        //            SelectedBlock = _listBlocks[0];
        //        }
        //        return _listBlocks;
        //    }
        //    set { _listBlocks = value; JobSchedules = new ObservableCollection<JobScheduleDetails>(); }
        //}
        private string _selectedBlock;
        public string SelectedBlock
        {
            get
            {
                return _selectedBlock;
            }
            set
            {
                _selectedBlock = value;
                ListZones = new List<string>();
                ListLevels = new List<string>();
                ListMarkingNos = new List<string>();
                SelectedLevel = null;
                SelectedZone = null;
                SelectedMarkingNo = null;
                if (!string.IsNullOrEmpty(_selectedBlock))
                {
                    ListLevels = _listMaterialJobSchedule.Where(js => js.Block == _selectedBlock).OrderBy(js => js.Level).Select(js => js.Level).Distinct().ToList();
                }
                OnPropertyChanged("ListZones");
                OnPropertyChanged("ListLevels");
                OnPropertyChanged("ListMarkingNos");
                OnPropertyChanged("SelectedBlock");
            }
        }
        public List<string> ListLevels { get; set; }
        //private List<string> _listLevels { get; set; }
        //public List<string> ListLevels
        //{
        //    get
        //    {
        //        if (_listLevels != null && _listLevels.Count == 1)
        //        {
        //            SelectedLevel = _listLevels[0];
        //        }
        //        return _listLevels;
        //    }
        //    set { _listLevels = value; }
        //}
        private string _selectedLevel;
        public string SelectedLevel
        {
            get
            {
                return _selectedLevel;
            }
            set
            {
                _selectedLevel = value;
                ListZones = new List<string>();
                ListMarkingNos = new List<string>();
                SelectedZone = null;
                SelectedMarkingNo = null;
                if (!string.IsNullOrEmpty(_selectedLevel))
                {
                    ListZones = _listMaterialJobSchedule.Where(js => js.Block == _selectedBlock && js.Level == _selectedLevel).OrderBy(js => js.Zone).Select(js => js.Zone).Distinct().ToList();
                }
                OnPropertyChanged("ListZones");
                OnPropertyChanged("ListMarkingNos");
                OnPropertyChanged("SelectedLevel");
            }
        }
        public List<string> ListZones { get; set; }
        //private List<string> _listZones { get; set; }
        //public List<string> ListZones
        //{
        //    get
        //    {
        //        if (_listZones != null && _listZones.Count == 1)
        //        {
        //            SelectedZone = _listZones[0];
        //        }
        //        return _listZones;
        //    }
        //    set { _listZones = value; }
        //}
        private string _selectedZone;
        public string SelectedZone
        {
            get
            {
                return _selectedZone;
            }
            set
            {
                _selectedZone = value;
                ListMarkingNos = new List<string>();
                SelectedMarkingNo = null;
                if (!string.IsNullOrEmpty(_selectedZone))
                {
                    ListMarkingNos = _listMaterialJobSchedule.Where(js => js.Block == _selectedBlock && js.Level == _selectedLevel && js.Zone == _selectedZone).OrderBy(js => js.MarkingNo).Select(js => js.MarkingNo).Distinct().ToList();
                }
                OnPropertyChanged("ListMarkingNos");
                OnPropertyChanged("SelectedZone");
            }
        }
        public List<string> ListMarkingNos { get; set; }
        //private List<string> _listMarkingNos { get; set; }
        //public List<string> ListMarkingNos
        //{
        //    get
        //    {
        //        if (_listMarkingNos != null && _listMarkingNos.Count == 1)
        //        {
        //            SelectedMarkingNo = _listMarkingNos[0];
        //        }
        //        return _listMarkingNos;
        //    }
        //    set { _listMarkingNos = value; }
        //}
        private string _selectedMarkingNo;
        public string SelectedMarkingNo
        {
            get
            {
                return _selectedMarkingNo;
            }
            set
            {
                _selectedMarkingNo = value;
                JobSchedules.Clear();

                if (!string.IsNullOrEmpty(_selectedMarkingNo))
                {
                    var listJobs = new ObservableCollection<JobScheduleDetails>(_listMaterialJobSchedule.Where(js => js.Block == _selectedBlock && js.Level == _selectedLevel && js.Zone == _selectedZone && js.MarkingNo == _selectedMarkingNo));
                    if (listJobs.Count > 0)
                    {
                        ModuleName = listJobs[0].ModuleName;
                        JobSchedules = new ObservableCollection<JobScheduleDetails>(listJobs);
                    }

                    FilterExpanded = false;
                    OnPropertyChanged("FilterExpanded");
                    OnPropertyChanged("FilterExpandIcon");
                }
                OnPropertyChanged("ModuleName");
                OnPropertyChanged("JobSchedules");
                OnPropertyChanged("SelectedMarkingNo");
                OnPropertyChanged("HeaderMargin");
                OnPropertyChanged("JobScheduleMargin");
            }
        }

        private List<JobScheduleDetails> _listMaterialJobSchedule { get; set; }
        public List<JobScheduleDetails> ListMaterialJobSchedule
        {
            get
            {
                return _listMaterialJobSchedule;
            }
            set
            {
                _listMaterialJobSchedule = value;
                OldJob = null;
                OnPropertyChanged("ListMaterialJobSchedule");

                //JobSchedules.Clear();
                if (_listMaterialJobSchedule != null)
                {
                    JobSchedules = new ObservableCollection<JobScheduleDetails>(_listMaterialJobSchedule);
                    ListBlocks = _listMaterialJobSchedule.OrderBy(js => js.Block).Select(js => js.Block).Distinct().ToList();
                    OnPropertyChanged("ListBlocks");
                    OnPropertyChanged("JobSchedules");
                }
            }
        }

        public string ModuleName { get; set; }
        public ObservableCollection<JobScheduleDetails> JobSchedules { get; set; }
        public ObservableCollection<JobScheduleDetails> ScannedJobs { get; set; }

        public Dictionary<string, ObservableCollection<JobScheduleDetails>> DictJobSchedule { get; set; }

        public void QCButtonClicked()
        {
            Navigation.PushAsync(new JobQCCase());
        }

        private JobScheduleDetails OldJob { get; set; }

        public JobScheduleVM() : base()
        {
            FilterExpandedCommand = new Command(ToggleFilterExpanded);
            ScanQRFABCommand = new Command(ScanQRFABClicked);
            JobSchedules = new ObservableCollection<JobScheduleDetails>();
            OldJob = new JobScheduleDetails();

            ListBlocks = new List<string>();
            ListZones = new List<string>();
            ListLevels = new List<string>();
            ListMarkingNos = new List<string>();
        }
    }
}