using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class MaterialFrameVM : MasterVM
    {
        private bool _isScanning { get; set; }
        public bool IsScanning { get { return _isScanning; } set { _isScanning = value; } }
       // public static event Action ViewCellSizeChangedEvent;
        public bool ShowUnknownTag
        {
            get
            {
                try
                {
                    return Trackers != null && Trackers.Count > 0 && string.IsNullOrEmpty(Trackers.FirstOrDefault().label);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        private List<Tracker> _trackers;
        public List<Tracker> Trackers
        {
            get => _trackers;
            set
            {
                try
                {
                    _trackers = value;
                    OnPropertyChanged("Tracker");
                    OnPropertyChanged("RFIDTracker");
                    OnPropertyChanged("QRTracker");
                    OnPropertyChanged("ShowUnknownTag");
                    OnPropertyChanged("ShowRFIDTracker");
                    OnPropertyChanged("ShowRFIDScanButton");
                    OnPropertyChanged("ShowAttachButton");
                    OnPropertyChanged("ShowQRTracker");
                    OnPropertyChanged("ShowNonAssociateAlert");
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool IsExpanded { get; set; }
        public string ExpansionIcon
        {
            get
            {
                try
                {
                    if (IsExpanded)
                    {
                        return "ic_keyboard_arrow_up.png";
                    }
                    else
                    {
                        return "ic_keyboard_arrow_down.png";
                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public Color StatusColor
        {
            get
            {
                try
                {
                    Color result = StageColor;
                    var QCStatusCode = (JobStatus)Material.QCStatusCode;
                    switch (QCStatusCode)
                    {
                        case JobStatus.Job_delayed:
                            result = Color.FromHex("#CEA051");
                            break;
                        case JobStatus.Job_not_started:
                            result = Color.FromHex("#3D3D3D");
                            break;
                        case JobStatus.Job_started:
                            result = Color.FromHex("#80C3D8");
                            break;
                        case JobStatus.Job_completed:
                            result = Color.FromHex("#00008B");
                            break;
                        case JobStatus.QC_failed_by_Maincon:
                            result = Color.FromHex("#F00028");
                            break;
                        case JobStatus.QC_passed_by_Maincon:
                            result = Color.FromHex("#286E67");
                            break;
                        case JobStatus.QC_routed_to_RTO:
                            result = Color.FromHex("#AE59AA");
                            break;
                        case JobStatus.QC_rejected_by_RTO:
                            result = Color.FromHex("#FF5722");
                            break;
                        case JobStatus.QC_accepted_by_RTO:
                            result = Color.FromHex("#286E67");
                            break;
                        case JobStatus.QC_rectified_by_Subcon:
                            result = Color.FromHex("#3F51B5");
                            break;
                        case JobStatus.All_QC_passed:
                            result = Color.FromHex("#00BFFF");
                            break;
                        default:
                            if (Material.QCStatusCode == -1)
                            {
                                result = Color.FromHex("#8E44AD");
                            }
                            break;
                    }
                    //  StatusChanged();
                    return result;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        private Tracker _QRTracker;
        public Tracker QRTracker
        {
            get
            {
                try
                {
                    return Trackers?.Where(t => t.type == "QR Code").FirstOrDefault();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool ShowQRTracker
        {
            get
            {
                try
                {
                    return QRTracker != null || Scanning;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        //private Tracker _RFIDTracker;
        public Tracker RFIDTracker
        {
            get
            {
                try
                {

                    return Trackers.Where(t => t.type == "RFID").FirstOrDefault();
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public bool ShowRFIDTracker
        {
            get
            {
                try
                {


                    return RFIDTracker != null || Scanning;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public bool ShowRFIDScanButton
        {
            get
            {
                try
                {
                    return !Scanning && RFIDTracker == null && Material != null && Material.currentStage != null && VendorId > 0 && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 4 && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 2;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public Material Material { get; set; }
        public bool ShowMaterial => Material != null;
        public string StageName
        {
            get
            {
                try
                {
                    if (Material != null)
                    {
                        if (Material.currentStage != null)
                            return Material.currentStage.Name;
                        else
                            return "Ordered";
                    }
                    else
                        return "N.A.";
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public Color StageColor
        {
            get
            {
                try
                {
                    if (Material != null)
                    {
                        if (Material.currentStage != null)
                            return Color.FromHex(Material.currentStage.Colour);
                        else
                            return Color.Green;
                    }
                    else
                        return Color.Green;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        private int VendorId
        {
            get
            {
                try
                {
                    if (App.Current.Properties.ContainsKey("organisationID"))
                        return (int)App.Current.Properties["organisationID"];
                    else
                        return 0;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public bool ShowNonAssociateAlert => Scanning && Trackers == null && Trackers.Count < 1;
        public bool ShowAttachButton
        {
            get
            {
                try
                {
                    return Scanning && RFIDTracker != null && Material != null && Material.currentStage != null && VendorId > 0 && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 4 && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 5;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool ShowUpdateButton
        {
            get
            {
                try
                {
                    var entryPoint = Convert.ToInt32(Application.Current.Properties["entry_point"]);
                    bool canUpdate = false;
                    if (Material != null && !Scanning)
                    {
                        if (DateTime.UtcNow.Date >= Material.orderDate.Date && (Material.CanIgnoreQC || Material.countQCCase == 0))
                        {
                            if (entryPoint == 0)
                            {
                                if (Material.currentStage != null)
                                {
                                    if (ViewModelLocator.materialFrameVM.IsScanning && Material.currentStage.MilestoneId == 1 && (!Material.IsChecklist || Material.QCStatusCode == (int)JobStatus.All_QC_passed))
                                    {
                                        canUpdate = true;
                                    }
                                }
                                else
                                {
                                    canUpdate = true;
                                }
                            }
                            else if (entryPoint == 1 && Material.currentStage != null && Material.currentStage.MilestoneId != 1 && Material.currentStage.MilestoneId != 3 && (!Material.IsChecklist || Material.QCStatusCode == (int)JobStatus.All_QC_passed))
                            {
                                canUpdate = true;
                            }
                            else
                                canUpdate = false;

                            #region Previous Code

                            //if (Material.currentStage != null)
                            //{
                            //if ((!Material.IsChecklist || Material.QCStatusCode == (int)Enums.JobStatus.All_QC_passed) && Material.currentStage.MilestoneId != 1 && VendorId == 0 && Material.currentStage.MilestoneId != 3 && Convert.ToInt32(Application.Current.Properties["entry_point"]) == 1)
                            // canUpdate = true;
                            //else if ((!Material.IsChecklist || Material.QCStatusCode == (int)Enums.JobStatus.All_QC_passed) && Material.currentStage.MilestoneId == 1 && VendorId == Material.organisationID && Tracker != null && Tracker.Count > 0)
                            //    canUpdate = true;
                            //else if (Material.QCStatusCode == (int)Enums.JobStatus.All_QC_passed && Material.currentStage.ID == 1 && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 1)
                            //    canUpdate = true;
                            //else if ((!Material.IsChecklist || Material.QCStatusCode == (int)Enums.JobStatus.All_QC_passed) && Material.currentStage.MilestoneId == 1 && Convert.ToInt32(Application.Current.Properties["entry_point"]) == 0 && ViewModelLocator.materialFrameVM.IsScanning && VendorId == Material.organisationID && Tracker != null && Tracker.Count > 0)
                            //    canUpdate = true;
                            //}
                            //else
                            //{
                            //    if (!string.IsNullOrEmpty(Material.mrfNo) && VendorId == Material.organisationID)
                            //        canUpdate = true;
                            //}
                            //if (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 4 || Convert.ToInt32(Application.Current.Properties["entry_point"]) == 3 || Convert.ToInt32(Application.Current.Properties["entry_point"]) == 2 || Convert.ToInt32(Application.Current.Properties["entry_point"]) == 5)
                            //{
                            //    canUpdate = false;
                            //}
                            //if (Material.currentStage == null)
                            //    if (!string.IsNullOrEmpty(Material.mrfNo) && VendorId == Material.organisationID)
                            //        canUpdate = true;
                            //    else if ((Material.QCStatusCode == (int)Enums.JobStatus.QC_Completed && Material.currentStage.ID == 1))
                            //        canUpdate = true;
                            //    else if (StageName == "Installed")
                            //        canUpdate = false;
                            //    else
                            //        // Correct vendor can update from produced to next stage if associated
                            //        if (Material.currentStage.MilestoneId == 1 && VendorId == Material.organisationID && Tracker != null && Tracker.Count > 0)
                            //        canUpdate = true;
                            //    // Site officer can update beyond produced stage
                            //    else if (Material.currentStage.MilestoneId != 1 && VendorId == 0)
                            //        canUpdate = true;

                            #endregion
                        }
                    }
                    return canUpdate;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public bool ShowQCButton
        {
            get
            {
                bool status = false;
                try
                {
                    if (Material != null && (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 4 || Convert.ToInt32(Application.Current.Properties["entry_point"]) == 1))
                    {
                        status = true;
                    }
                    else if (Material != null && Material.IsQCOpen && (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 0 || Convert.ToInt32(Application.Current.Properties["entry_point"]) == 2))
                    {
                        status = true;
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return status;
            }
        }
        public bool ShowBIMButton
        {
            get
            {
                try
                {
                    return Material != null && VendorId == 0 && Material.ForgeElementId.HasValue && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 4;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public bool ShowScanMenu { get; set; }
        public bool _showChecklistButton { get; set; }
        public bool ShowChecklistButton
        {
            get
            {
                try
                {
                    if (Material.IsChecklist)
                    {
                        if (ViewModelLocator.jobChecklistVM.IsStructural)
                        {
                            if (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 4 && Material != null && Material.QCStatusCode != -1 && Material.currentStage != null)//MC
                            {
                                _showChecklistButton = true;
                            }
                            else if (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 3 && Material != null && Material.QCStatusCode > (int)JobStatus.QC_passed_by_Maincon)//RTO   
                            {
                                _showChecklistButton = true;
                            }
                            else if (Material.IsChecklist && Convert.ToInt32(Application.Current.Properties["entry_point"]) == 0 && Material != null && Material.currentStage != null && Material.QCStatusCode == (int)JobStatus.QC_failed_by_Maincon)//Ven
                            {
                                _showChecklistButton = true;
                            }
                        }
                    }
                    return _showChecklistButton;
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
        public ICommand AssociateQRCommand { get; set; }
        public ICommand AssociateRFIDCommand { get; set; }
        public ICommand AttachButtonCommand { get; set; }
        public ICommand ChecklistButtonCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand QCCommand { get; set; }
        public ICommand BIMCommand { get; set; }
        public ICommand JOBCommand { get; set; }
        public ICommand ExpandCommand { get; set; }
        public ICommand CheckBoxCommand { get; set; }
        private async void AssociateQRClicked()
        {
            try
            {
                ShowScanMenu = false;
                Scanning = true;
                OnPropertyChanged("ShowNonAssociateAlert");
                ViewModelLocator.scanTrackerVM.Reset();
                ViewModelLocator.scanTrackerVM.MaterialItem = this;
                await Task.Run(GetCameraPermission)
                   .ContinueWith(async (t) =>
                   {
                       ViewModelLocator.scanTrackerVM.CameraReady = t.Result;
                       if (t.Result)
                       {
                           await Navigation.PushAsync(new ScanQRCode());
                       }
                       else
                       {
                           DisplaySnackBar("No camera available", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                       }
                   }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void AssociateRFIDClicked()
        {
            try
            {
                ShowScanMenu = false;
                Scanning = true;
                OnPropertyChanged("ShowNonAssociateAlert");
                ViewModelLocator.scanTrackerVM.Reset();
                ViewModelLocator.scanTrackerVM.MaterialItem = this;
                if (App.scannerRFID != null && App.scannerRFID.InitSuccess())
                    Navigation.PushAsync(new ScanRFID());
                else
                    DisplaySnackBar("Scanner not ready", Shared.Classes.Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void UpdateButtonClicked()
        {
            try
            {


                if (Navigation != null && Material != null)
                {
                    ViewModelLocator.siteUpdateStageVM = new SiteUpdateStageVM();
                    ViewModelLocator.siteUpdateStageVM.Material = Material;
                    Navigation.PushAsync(new UpdateMaterial());
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void AttchButtonClicked()
        {
            try
            {


                if (Scanning)
                {
                    if (Trackers == null || Trackers.Count == 0 || Material == null)
                    {
                        ErrorMessage = "Please select a tag first";
                    }
                    else
                    {
                        IsLoading = true;
                        var trackerAssociation = new TrackerAssociation
                        {
                            trackers = Trackers,
                            material = Material
                        };
                        Task.Run(() => ApiClient.Instance.MTUpdateAssociation(trackerAssociation))
                            .ContinueWith((t) =>
                        {
                            if (t.Result.status != 0)
                            {
                                ErrorMessage = t.Result.message;
                            }
                            else
                            {
                                IsLoading = false;
                                // Application.Current.MainPage.DisplayAlert("Success", $"{Tracker?.FirstOrDefault()?.label} has been associated with {Material.markingNo}", "OK");
                                //Navigation.PopAsync();
                                DisplaySnackBar($"{Trackers?.FirstOrDefault()?.label} has been associated with {Material.markingNo}", Enums.PageActions.PopAsync, Enums.MessageActions.Success, null, null);
                            }
                        }, TaskScheduler.FromCurrentSynchronizationContext());
                    }
                }
                else
                {
                    ShowScanMenu = !ShowScanMenu;
                    OnPropertyChanged("ShowScanMenu");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private bool _scanning { get; set; }
        public bool Scanning
        {
            get
            {
                try
                {
                    return _scanning;
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
            set
            {
                try
                {
                    _scanning = value;
                    OnPropertyChanged("Scanning");
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }
        public void ChecklistClicked(Material Material)
        {
            ViewModelLocator.jobChecklistVM.Material = Material;
            Navigation.PushAsync(new JobChecklist());
        }
        private async void QCButtonClicked()
        {
            try
            {
                if (Navigation != null && Material != null)
                {
                    ViewModelLocator.siteListDefectsVM = new ViewModels.SiteListDefectsVM();
                    ViewModelLocator.siteListDefectsVM.Material = Material;
                    ViewModelLocator.qcDefectVM.Reset();
                    ViewModelLocator.qcDefectVM.Material = Material;

                    //   await ViewModelLocator.qcDefectVM.GetOrganisations();

                    if (Material.countQCCase > 0)
                        await Navigation.PushAsync(new ListDefects());
                    else
                    {
                        await Navigation.PushAsync(new DefectDetails());
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void BIMButtonClicked()
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
        private void JOBButtonClicked()
        {
            if (Navigation != null)
            {
                Navigation.PushAsync(new JobSchedule());
            }
        }
        private void ExpandClicked()
        {
            try
            {
                IsExpanded = !IsExpanded;
                OnPropertyChanged("IsExpanded");
                OnPropertyChanged("ExpansionIcon");
              //  ViewCellSizeChangedEvent?.Invoke();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void CheckBoxClicked()
        {
            Material.IsChecked = !Material.IsChecked;
            OnPropertyChanged("IsChecked");
        }
        private async void NavigateToBrowser()
        {
            try
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
                    var bimViewerPage = new BIMViewer(url, Material.ForgeModelURN, Material.ForgeElementId.Value,
                        forgeTokenExpireTime, forgeToken);

                    await Navigation.PushAsync(bimViewerPage);
                }
                else
                {
                    ErrorMessage = forgeTokenResult.message;
                }
                IsLoading = false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public Color QCButtonBackground
        {
            get
            {
                try
                {
                    if (Material != null && Material.countQCCase > 0)
                    {
                        return Color.OrangeRed;
                    }
                    else
                    {
                        return Color.Transparent;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }
        public MaterialFrameVM() : base()
        {
            _neverLoadBefore = false;

            ExpandCommand = new Command(ExpandClicked);
            AssociateQRCommand = new Command(AssociateQRClicked);
            AssociateRFIDCommand = new Command(AssociateRFIDClicked);
            AttachButtonCommand = new Command(AttchButtonClicked);
            ChecklistButtonCommand = new Command<Material>(ChecklistClicked);
            UpdateCommand = new Command(UpdateButtonClicked);
            QCCommand = new Command(QCButtonClicked);
            BIMCommand = new Command(BIMButtonClicked);
            JOBCommand = new Command(JOBButtonClicked);
            CheckBoxCommand = new Command(CheckBoxClicked);
        }
    }
}
