using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Utilities.ApiClient;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class ScanTrackerVM : MasterVM
    {
        public bool ShowSingleMaterial
        {
            get
            {
                return _materialItem != null;
            }
        }

        private MaterialFrameVM _materialItem;
        public MaterialFrameVM MaterialItem
        {
            get
            {
                return _materialItem;
            }
            set
            {
                _materialItem = value;
                OnPropertyChanged("MaterialItem");
                OnPropertyChanged("ShowSingleMaterial");
            }
        }

        private ObservableCollection<MaterialFrameVM> _listMaterialItems;
        public ObservableCollection<MaterialFrameVM> ListMaterialItems
        {
            get
            {
                try
                {


                    return _listMaterialItems;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
            set
            {
                _listMaterialItems = value;
                OnPropertyChanged("ListMaterialItems");
            }
        }

        public void AddTrackerAssociations(List<TrackerAssociation> trackerAssociations)
        {
            try
            {
                if (trackerAssociations != null)
                {
                    var currentTags = _listMaterialItems.Select(mi => mi.Tracker?.FirstOrDefault()?.tag).Distinct();
                    foreach (MaterialFrameVM vm in trackerAssociations.Select(t => new MaterialFrameVM
                    {
                        Tracker = t.tracker,
                        Material = t.material
                    }).Where(t => !currentTags.Contains(t.Tracker?.FirstOrDefault()?.tag)))
                    {
                        //Device.BeginInvokeOnMainThread(() =>
                        //{
                        if (vm.Material != null && vm.Tracker != null)
                            _listMaterialItems.Add(vm);
                        //else
                        //    Application.Current.MainPage.DisplayAlert("", "No material found", "OK");
                        //});

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private int _countTags;
        public int CountTags
        {
            get
            {
                return _countTags;
            }
            set
            {
                _countTags = value;
                OnPropertyChanged("CountTags");
            }
        }

        private double _scanIconRotation;
        public double ScanIconRotation
        {
            get
            {
                return _scanIconRotation;
            }
            set
            {
                _scanIconRotation = value;
                OnPropertyChanged("ScanIconRotation");
            }
        }

        public string ToggleScanButtonLabel
        {
            get
            {
                if (_allowScan)
                {
                    return "Scan";
                }
                else
                {
                    return "Stop";
                }
            }
        }

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

        private bool _cameraReady;
        public bool CameraReady
        {
            get
            {
                return _cameraReady;
            }
            set
            {
                _cameraReady = value;
                OnPropertyChanged("CameraReady");
            }
        }

        public async Task<int> GetTrackerAssociations(string[] tags)
        {
            ApiResult result = null;
            int countTrackers = 0;
            ViewModelLocator.materialFrameVM.IsScanning = true;
            ListMaterialItems = new ObservableCollection<MaterialFrameVM>();
            List<TrackerAssociation> trackerAssociations = new List<TrackerAssociation>();

            if (tags != null && tags.Count() > 0)
            {
                try
                {
                    IsLoading = true;
                    List<string> materialTags = new List<string>();
                    foreach (string tag in tags)
                        materialTags.Add(tag);

                    if (materialTags.Count > 0)
                    {
                        if (ViewModelLocator.jobChecklistVM.IsStructural && Convert.ToInt32(Application.Current.Properties["entry_point"]) == 2)
                        {
                            result = await ApiClient.Instance.MTGetQCDefectsListTrackerInfo(materialTags.ToArray());
                        }
                        else if (ViewModelLocator.jobChecklistVM.IsStructural)
                        {
                            result = await ApiClient.Instance.MTGetListTrackerInfo(materialTags.ToArray());
                        }
                        if (result.status == 0)
                            trackerAssociations = result.data as List<TrackerAssociation>;
                        else
                            ErrorMessage = result.message;
                    }
                    var listRegisteredTags = trackerAssociations.Select(t => t.tracker.Where(tr => tags.Contains(tr.tag)).FirstOrDefault().tag);

                    if (ShowSingleMaterial)
                    {
                        if (trackerAssociations.Count() == 0)
                        {
                            ErrorMessage = "No registered tags found";
                        }
                        else if (trackerAssociations.Any(t => t.material == null))
                        {
                            Device.BeginInvokeOnMainThread(async () =>
                            {
                                var selectedLabel = await Application.Current.MainPage.DisplayActionSheet(
                                    "Select a tracker", "Cancel", null,
                                    trackerAssociations.Where(t => t.material == null)
                                    .Select(t => t.tracker.FirstOrDefault().label).ToArray());
                                if (!string.IsNullOrEmpty(selectedLabel) && selectedLabel.ToLower() != "cancel")
                                {
                                    MaterialItem.Tracker = trackerAssociations
                                    .Where(t => t.tracker.Select(tr => tr.label).Contains(selectedLabel)).FirstOrDefault()?.tracker;
                                }
                            });
                        }
                        else
                        {
                            ErrorMessage = "All trackers found are already in use";
                        }
                    }
                    else
                    {
                        var listUnregisteredTrackers = tags
                            .Where(tag => !listRegisteredTags.Contains(tag))
                            .Select(tag => new TrackerAssociation
                            {
                                tracker = new List<Tracker>
                                {
                                    new Tracker {tag = tag }
                                }
                            }).ToList();

                        AddTrackerAssociations(trackerAssociations);
                        AddTrackerAssociations(listUnregisteredTrackers);
                    }
                    countTrackers = trackerAssociations.Count;


                }
                catch (Exception ex)
                {
                    ErrorMessage = ex.Message;
                }
                IsLoading = false;
            }
            OnPropertyChanged("ListMaterialItems");
            AllowScan = true;
            return countTrackers;
        }

        public ScanTrackerVM() : base()
        {
            _cameraReady = false;
            AllowScan = true;
            CountTags = 0;
            ScanIconRotation = 0;
            ListMaterialItems = new ObservableCollection<MaterialFrameVM>();
            //GenerateListJobAssignment();
        }

        public override void Reset()
        {
            base.Reset();
            _neverLoadBefore = false;
            AllowScan = true;
            CountTags = 0;
            ScanIconRotation = 0;
            //  ListMaterialItems = new ObservableCollection<MaterialFrameVM>();
            MaterialItem = null;
        }
    }
}
