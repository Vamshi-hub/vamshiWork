using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class SingleScanTrackerVM : MasterVM
    {
        public Color FrameBGColor { get; } = Color.FromRgba(255, 255, 255, 127);

        private Tracker _tracker;
        public Tracker Tracker
        {
            get
            {
                return _tracker;
            }
            set
            {
                _tracker = value;
                ShowAttachButton = false;
                ShowDeliverButton = false;
                ShowQCStatus = false;

                if (_tracker == null)
                {
                    TrackerLabel = "N.A.";
                    MarkingNo = "N.A.";
                    StageName = "Unknown";
                    StageColor = Color.Gray;
                }
                else
                {
                    TrackerLabel = _tracker.trackerLabel;
                    MarkingNo = _tracker.markingNo;
                    StageName = _tracker.stageName;
                    StageColor = _tracker.stageColor;
                    if (_tracker.displayQC)
                    {
                        QCRemarks = _tracker.qcRemarks;
                        ShowQCStatus = true;
                        ShowDeliverButton = true;
                    }
                    else if (_tracker.displayInventory)
                        ShowDeliverButton = true;
                    else if (!_tracker.displayMaterial)
                        ShowAttachButton = true;
                }

                OnPropertyChanged("TrackerLabel");
                OnPropertyChanged("MarkingNo");
                OnPropertyChanged("StageName");
                OnPropertyChanged("StageColor");
                OnPropertyChanged("QCRemarks");
                OnPropertyChanged("ShowAttachButton");
                OnPropertyChanged("ShowDeliverButton");
                OnPropertyChanged("ShowQCStatus");
                OnPropertyChanged("Tracker");
                OnPropertyChanged("DisplayInventory");
                OnPropertyChanged("DisplayMaterial");
                OnPropertyChanged("DisplayBIM");
                OnPropertyChanged("ShowDetails");
            }
        }

        public string TrackerLabel { get; set; }
        public string MarkingNo { get; set; }
        public string StageName { get; set; }
        public Color StageColor { get; set; }
        public string QCRemarks { get; set; }
        public bool ShowAttachButton { get; set; }
        public bool ShowDeliverButton { get; set; }
        public bool ShowQCStatus { get; set; }
        public bool DisplayInventory { get {
                return _tracker == null ? false : _tracker.displayInventory;
            } }
        public bool DisplayMaterial { get {
                return _tracker == null ? false : _tracker.displayMaterial;
            } }
        public bool DisplayBIM { get { return _tracker == null ? false : _tracker.displayBIM; } }

        public List<Tracker> ListTrackers { get; set; }

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

        public async void SetTag(string value)
        {
            string _tag = value;
            if (!string.IsNullOrEmpty(_tag))
            {
                Console.WriteLine($"One tag scanned: {_tag}");
                IsLoading = true;
                AllowScan = false;

                var tracker = ListTrackers.Where(t => t.tag == _tag).FirstOrDefault();
                if (tracker == null || (DateTime.Now - tracker.UpdatedTime) >= TimeSpan.FromMinutes(5))
                {
                    ListTrackers.Remove(tracker);

                    try
                    {
                        var result = await ApiClient.Instance.MTGetListTrackerInfo(new string[] { _tag });
                        if (result.data != null)
                        {
                            var trackers = result.data as List<Tracker>;
                            if (trackers.Count > 0)
                            {
                                Tracker = trackers.ElementAt(0);
                                Tracker.UpdatedTime = DateTime.Now;
                                ListTrackers.Add(ViewModelLocator.singleScanTrackerVM.Tracker);
                            }
                            else
                            {
                                Tracker = null;
                                ErrorMessage = "This tracker is not in our system";
                            }
                        }
                        else if (!string.IsNullOrEmpty(result.message))
                        {
                            ErrorMessage = result.message;
                        }
                        else
                            ErrorMessage = "Fail to get tracker information";
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = ex.Message;
                    }
                }
                else
                {
                    Tracker = tracker;
                }
            }
            else
            {
                ErrorMessage = "No RFID tag found";
            }
            IsLoading = false;
            AllowScan = true;
        }

        public bool ShowDetails
        {
            get
            {
                return !IsLoading && _tracker != null;
            }
        }

        public ICommand AttachCommand { get; set; }
        public ICommand DeliverCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand BIMCommand { get; set; }

        public INavigation Navigation { get; set; }

        void SelectButtonClicked()
        {
            if (Navigation != null && _tracker != null)
            {
                ViewModelLocator.siteUpdateStageVM.Material = _tracker.material;
                if (_tracker.material.IsOpenQCCase)
                {
                    ViewModelLocator.siteListDefectsVM.StageAuditId = _tracker.material.stageId;
                    ViewModelLocator.qcDefectVM.Material = _tracker.material;
                    Navigation.PushAsync(new SiteListDefects());
                }
                else
                {
                    Navigation.PushAsync(new SiteUpdateStage());
                }
            }
        }

        void AttachButtonClicked()
        {
            if (Navigation != null && _tracker != null)
            {
                ViewModelLocator.vendorEnrolmentVM.Reset();
                Navigation.PushAsync(new VendorEnrolment());
            }
        }

        void DeliverButtonClicked()
        {
            if (Navigation != null && _tracker != null)
            {
                ViewModelLocator.vendorStartDeliveryVM.Reset();
                ViewModelLocator.vendorStartDeliveryVM.TrackerInfo = _tracker;

                Navigation.PushAsync(new VendorStartDelivery());
            }
        }

        void BIMButtonClicked()
        {
            if (Navigation != null)
            {
                IsLoading = true;
                OnPropertyChanged("ShowDetails");
                var tokenSource = new CancellationTokenSource();
                var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
                Task.Factory.StartNew(NavigateToBrowser, tokenSource.Token, TaskCreationOptions.LongRunning, scheduler);
            }
        }

        private async void NavigateToBrowser()
        {
            var host = string.Format(App.ASTORWORK_WEB_HOST, App.Current.Properties["tenant_name"]);
            var url = $"{host}/forge-viewer";
            // url = "https://www.google.com";
            var forgeTokenResult = await ApiClient.Instance.MTGetForgeToken();
            if (forgeTokenResult.status == 0)
            {
                var forgeTokenJson = forgeTokenResult.data as JObject;
                var forgeToken = forgeTokenJson.Value<string>("access_token");
                var forgeTokenExpireSeconds = forgeTokenJson.Value<int>("expires_in");
                var forgeTokenExpireTime = DateTime.Now.AddSeconds(forgeTokenExpireSeconds);
                var bimViewerPage = new BIMViewer(url, _tracker.material.ForgeModelURN, _tracker.material.ForgeElementId.Value, forgeTokenExpireTime, forgeToken);

                await Navigation.PushAsync(bimViewerPage);
            }
            else
            {
                ErrorMessage = forgeTokenResult.message;
            }
            IsLoading = false;
            // Navigation.RemovePage(App.bimViewerPage);
        }

        public SingleScanTrackerVM() : base()
        {
            AllowScan = true;
            ListTrackers = new List<Tracker>();
            AttachCommand = new Command(AttachButtonClicked);
            DeliverCommand = new Command(DeliverButtonClicked);
            SelectCommand = new Command(SelectButtonClicked);
            BIMCommand = new Command(BIMButtonClicked);
        }

        public override void Reset()
        {
            base.Reset();
            AllowScan = true;
            Tracker = null;
        }
    }
}
