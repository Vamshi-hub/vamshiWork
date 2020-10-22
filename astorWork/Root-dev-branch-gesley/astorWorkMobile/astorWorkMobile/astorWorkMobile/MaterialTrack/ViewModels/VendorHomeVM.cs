using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using astorWorkMobile.Shared.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class VendorHomeVM : MasterVM
    {
        public bool IsPageInitializing { get; set; }
        public bool IsOrderedMatSearch { get { return (!string.IsNullOrEmpty(OrderedSearchText) || !string.IsNullOrWhiteSpace(OrderedSearchText)); } }
        //public string _OrderedMatSearch { get; set; }
        public string OrderedSearchText
        {
            get;
            set;
            //set
            //{
            //    _OrderedMatSearch = value;
            //    //            if (!IsPageInitializing && (string.IsNullOrEmpty(_searchedText) || string.IsNullOrWhiteSpace(_searchedText)))
            //    //            {
            //    //                Task.Run(() =>
            //    //                {
            //    //                    if (IsOrderedTab)
            //    //                    {
            //    //                        OrderedMaterialItems = ListMaterialItems?.Where(m => m.Material.currentStage == null)
            //    //.OrderByDescending(m => m.Material.mrfNo).Take(10).ToList();

            //    //                        if (OrderedMaterialItems.Count == 0)
            //    //                        {
            //    //                            Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
            //    //                        }
            //    //                        else if (OrderedMaterialItems.Count > 0)
            //    //                        {
            //    //                            foreach (var item in OrderedMaterialItems)
            //    //                            {
            //    //                                item.Material.IsCheckBoxVisible = true;
            //    //                            }
            //    //                            HasOrderedMaterial = true;
            //    //                        }
            //    //                        Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("OrderedMaterialItems"); });
            //    //                    }
            //    //                    else if (IsInventoryTab)
            //    //                    {
            //    //                        ProducedMaterialItems = ListMaterialItems?.Where(m => m.Material.currentStage != null &&
            //    //              m.Material.currentStage.MilestoneId == 1)
            //    //            .OrderByDescending(m => m.Material.mrfNo).Take(10).ToList();

            //    //                        if (ProducedMaterialItems == null || ProducedMaterialItems.Count == 0)
            //    //                        {
            //    //                            Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
            //    //                        }
            //    //                        else
            //    //                        {
            //    //                            Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("ProducedMaterialItems"); });
            //    //                        }
            //    //                    }
            //    //                    else if (IsMCStructuralInsp)
            //    //                    {
            //    //                        if (ViewModelLocator.vendorHomeVM.IsStageSelected && ViewModelLocator.vendorHomeVM.Stage != null && !ViewModelLocator.vendorHomeVM.IsSearched)
            //    //                        {

            //    //                            ProducedMaterialItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage != null)
            //    //                                .OrderByDescending(m => m.Material.mrfNo).ToList();
            //    //                            ProducedMaterialItems = ProducedMaterialItems.Where(p => p.Material.currentStage.Name == ViewModelLocator.vendorHomeVM.Stage.Name).ToList();
            //    //                        }
            //    //                        else
            //    //                        {
            //    //                            ProducedMaterialItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage != null)
            //    //                                    .OrderByDescending(m => m.Material.mrfNo).ToList();
            //    //                            ProducedMaterialItems = ListMaterialItems?.Where(m => m.Material.currentStage != null)
            //    //                           .OrderByDescending(m => m.Material.mrfNo).Take(10).ToList();
            //    //                        }
            //    //                        if (ProducedMaterialItems == null || ProducedMaterialItems.Count == 0)
            //    //                        {
            //    //                            Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
            //    //                        }
            //    //                        else
            //    //                        {
            //    //                            Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("ProducedMaterialItems"); });
            //    //                        }
            //    //                    }
            //    //                });
            //    //            }
            //    //            OnPropertyChanged("SearchedText");
            //}
        }
        public bool IsInventoryMatSearch { get { return (!string.IsNullOrEmpty(InventorySearchText) || !string.IsNullOrWhiteSpace(InventorySearchText)); } }
        public string InventorySearchText { get; set; }
        public bool IsQCInsMatSearch { get { return (!string.IsNullOrEmpty(QCInsSearchText) || !string.IsNullOrWhiteSpace(QCInsSearchText)); } }
        public string QCInsSearchText { get; set; }

        private List<string> _stageName { get; set; }
        public List<string> StageName
        {
            get { return _stageName; }
            set
            {
                _stageName = value;
                OnPropertyChanged("StageName");
            }
        }
        private List<Stage> _stages = new List<Stage>();
        public List<Stage> Stages
        {
            get
            {
                if (_stages != null && _stages.Count == 1)
                {
                    Stage = _stages[0];
                   // OnPropertyChanged("Stage");
                }
                return _stages;
            }
            set
            {
                _stages = value;
                OnPropertyChanged("Stages");
            }
        }
        public Stage _stage { get; set; }
        public Stage Stage
        {
            get
            {
                if (_stage != null)
                {
                    IsStageSelected = true;
                }
                return _stage;
            }
            set
            {
                _stage = value;
                if (_stage != null)
                {
                    Task.Run(() =>
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            if (_stage != null)
                            {
                                var listProducedMaterials = ListMaterialItems?.Where(m => m.Material.currentStage != null)
   .OrderByDescending(m => m.Material.mrfNo).ToList();
                                ProducedMaterialItems = listProducedMaterials.Where(p =>
                                p.Material.currentStage.Name == _stage.Name).Take(10).ToList();
                                OnPropertyChanged("ProducedMaterialItems");
                            }
                        });
                    });
                }
                OnPropertyChanged("Stage");
            }
        }
        public bool IsStageSelected { get; set; }
        public bool IsListBusy { get; set; }
        public bool IsVendorScreen { get; set; }
        public int ProjectID = -1;
        public bool IsQCNotificationsTab;
        public bool IsOrderedTab;
        public bool IsInventoryTab;
        public bool IsMCStructuralInsp;//MC-->MainCon
        private Project _selectedProject { get; set; }
        public Project SelectedProject
        {
            get => _selectedProject;
            set
            {
                if (value != null && _selectedProject != value)
                {
                    _selectedProject = value;
                    ProjectID = _selectedProject.id;
                    Task.Run(GetOrderedProduced);
                }
                OnPropertyChanged("SelectedProject");
            }
        }
        private List<Project> _projects;
        public List<Project> Projects
        {
            get
            {
                if (_projects != null && _projects.Count == 1)
                {
                    SelectedProject = _projects[0];
                }
                return _projects;
            }
            set
            {
                _projects = value;
                OnPropertyChanged("Projects");
            }
        }

        private Project _selectedProjQCNotif;
        public Project SelectedProjQCNotif
        {
            get => _selectedProjQCNotif;
            set
            {
                if (value != null && _selectedProjQCNotif != value)
                {
                    _selectedProjQCNotif = value;
                    ProjectID = _selectedProjQCNotif.id;
                    //    GetOrderedProduced();
                }
                OnPropertyChanged("SelectedProjQCNotif");
            }
        }

        private Project _selectedProjOrdered;
        public Project SelectedProjOrdered
        {
            get => _selectedProjOrdered;
            set
            {
                if (value != null && _selectedProjOrdered != value)
                {
                    _selectedProjOrdered = value;
                    ProjectID = _selectedProjOrdered.id;
                    //      GetOrderedProduced();
                }
                OnPropertyChanged("SelectedProjOrdered");
            }
        }

        private Project _selectedProjInventory;
        public Project SelectedProjInventory
        {
            get => _selectedProjInventory;
            set
            {
                if (value != null && _selectedProjInventory != value)
                {
                    _selectedProjInventory = value;
                    ProjectID = _selectedProjInventory.id;
                    //     GetOrderedProduced();
                }
                OnPropertyChanged("SelectedProjInventory");
            }
        }

        public MaterialFrameVM OldMaterial { get; set; }

        internal void ShowHideMaterial(MaterialFrameVM material)
        {
            if (OldMaterial == material)
            {
                //click twice in the same item will hide it
                material.Material.IsVisible = !material.Material.IsVisible;
                UpdateMaterialVisible(material);
            }
            else
            {
                if (OldMaterial != null)
                {
                    //Hide previous selected Item
                    OldMaterial.Material.IsVisible = false;
                    UpdateMaterialVisible(OldMaterial);
                }
                //expand selected item
                material.Material.IsVisible = true;
                UpdateMaterialVisible(material);
            }
            OldMaterial = material;

        }

        private void UpdateMaterialVisible(MaterialFrameVM material)
        {
            var index = OrderedMaterialItems.IndexOf(material);
            OrderedMaterialItems.Remove(material);
            OrderedMaterialItems.Insert(index, material);
        }

        private bool _isItemChecked { get; set; }

        public bool IsItemChecked
        {
            get => _isItemChecked;
            set
            {
                if (_isItemChecked != value)
                {
                    _isItemChecked = value;
                    OnPropertyChanged("IsItemChecked");
                }
            }
        }

        private bool _hasOrderedMaterial { get; set; }
        public bool HasOrderedMaterial { get => _hasOrderedMaterial; set { _hasOrderedMaterial = value; OnPropertyChanged("HasOrderedMaterial"); } }

        private List<MaterialFrameVM> _listMaterialItems;

        public List<MaterialFrameVM> ListMaterialItems
        {
            get => _listMaterialItems;
            set
            {
                _listMaterialItems = value;
                if (ListMaterialItems != null)
                {
                    if (IsQCNotificationsTab)
                    {
                        ProducedQcMaterialItems = new List<MaterialFrameVM>();
                        ProducedQcMaterialItems = ListMaterialItems?.Where(m => m.Material.currentStage != null &&  m.Material.QCStatusCode == (int)Enums.JobStatus.QC_failed_by_Maincon ||m.Material.QCStatusCode==(int)Enums.QCStatus.QC_failed_by_Maincon)
                   .OrderByDescending(m => m.Material.mrfNo).ToList();

                        if (ProducedQcMaterialItems.Count == 0)
                        {
                            Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
                        }
                    }
                    else if (IsOrderedTab)
                    {
                        OrderedMaterialItems = new List<MaterialFrameVM>();
                        OrderedMaterialItems = ListMaterialItems?.Where(m => m.Material.currentStage == null)
                    .OrderByDescending(m => m.Material.mrfNo).Take(10).ToList();

                        if (OrderedMaterialItems.Count == 0)
                        {
                            Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
                        }
                        else if (OrderedMaterialItems.Count > 0)
                        {
                            foreach (var item in OrderedMaterialItems)
                            {
                                item.Material.IsCheckBoxVisible = true;
                            }
                            HasOrderedMaterial = true;
                        }
                    }
                    else if (IsInventoryTab)
                    {
                        ProducedMaterialItems = new List<MaterialFrameVM>();
                        ProducedMaterialItems = ListMaterialItems?.Where(m => m.Material.currentStage != null &&
                  m.Material.currentStage.MilestoneId == 1)
                .OrderByDescending(m => m.Material.mrfNo).Take(10).ToList();

                        if (ProducedMaterialItems == null || ProducedMaterialItems.Count == 0)
                        {
                            Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
                        }
                    }
                    else if (IsMCStructuralInsp)
                    {
                        ProducedMaterialItems = new List<MaterialFrameVM>();
                        ProducedMaterialItems = ListMaterialItems?.Where(m => m.Material.currentStage != null)
                .OrderByDescending(m => m.Material.mrfNo).Take(10).ToList();

                        if (ProducedMaterialItems == null || ProducedMaterialItems.Count == 0)
                        {
                            Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
                        }
                    }
                }
            }
        }

        public List<MaterialFrameVM> OrderedMaterialItems { get; set; }
        public List<MaterialFrameVM> ProducedQcMaterialItems { get; set; }
        public List<MaterialFrameVM> ProducedMaterialItems { get; set; }

        public async Task<bool> GetProjects()
        {
            IsLoading = true;
            bool success = false;
            try
            {
                var result = await ApiClient.Instance.MTGetProjects();
                if (result.status == 0)
                {
                    Projects = result.data as List<Project>;
                    if (Projects != null && Projects.Count > 0)
                    {
                        success = true;
                    }
                    else
                    {
                        ErrorMessage = "Failed to get projects";
                    }
                }
                else
                {
                    ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            //  OnPropertyChanged("ListMaterialItems");
            IsLoading = false;
            return success;
        }

        public async Task GetOrderedProduced()
        {

            HasOrderedMaterial = false;
            OldMaterial = null;
            try
            {
                if (ProjectID == -1)
                {
                    return;
                }
                IsLoading = true;
                int vendorId = int.Parse(Application.Current.Properties["organisationID"].ToString());
                var result = await ApiClient.Instance.MTGetOrderedProduced(ProjectID, vendorId);
                if (result.status == 0)
                {
                    ListMaterialItems = result.data as List<MaterialFrameVM>;
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() => { DisplaySnackBar(result.message, Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
                }
            }
            catch (Exception ex)
            {
                Device.BeginInvokeOnMainThread(() => { DisplaySnackBar(ex.Message, Enums.PageActions.None, Enums.MessageActions.Error, null, null); });
            }
            OnPropertyChanged("ListMaterialItems");
            OnPropertyChanged("ProducedQcMaterialItems");
            OnPropertyChanged("OrderedMaterialItems");
            OnPropertyChanged("ProducedMaterialItems");
            //  OnPropertyChanged("HasOrderedMaterial");
            OnPropertyChanged("SelectedProjOrdered");
            IsLoading = false;
        }

        public async void GetProducedForMainCon()
        {
            try
            {
                ListMaterialItems.Clear();
                IsLoading = true;
                List<Stage> dummyStages = null;
                var result = await ApiClient.Instance.MTGetMainConProduced();
                if (result.status == 0)
                {
                    var v = result.data;
                    ListMaterialItems = result.data as List<MaterialFrameVM>;
                    var stages = ListMaterialItems.Select(p => p.Material.currentStage).ToList();
                    if (stages != null && stages.Count > 0)
                    {
                        var distStages = stages.Distinct();
                        if (distStages != null && distStages.Count() > 0)
                        {
                            dummyStages = distStages.ToList();
                        }
                        var distStage = dummyStages.Select(p => p.Name).Distinct();
                        if (distStage != null && distStage.Count() > 0)
                        {
                            StageName = distStage.Distinct().ToList();
                        }
                        if (_stageName != null && _stageName.Count > 0)
                        {
                            List<Stage> distinctStages = new List<Stage>();
                            foreach (var item in StageName)
                            {
                                var filteredStage = dummyStages.Where(p => p.Name == item).FirstOrDefault();
                                if (filteredStage != null)
                                {
                                    distinctStages.Add(filteredStage);
                                }
                            }
                            Stages = distinctStages;
                        }
                    }
                    if (ListMaterialItems.Count == 0)
                        DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                }
                else
                {
                    ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            OnPropertyChanged("ListMaterialItems");
            //     OnPropertyChanged("ProducedQcMaterialItems");
            //     OnPropertyChanged("OrderedMaterialItems");
            OnPropertyChanged("ProducedMaterialItems");
            // OnPropertyChanged("HasOrderedMaterial");
            //    OnPropertyChanged("SelectedProjOrdered");
            IsLoading = false;
        }
        public ICommand SearchCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand UpdateCommand { get; set; }
        public ICommand ChecklistButtonCommand { get; set; }
        public ICommand ScanRFIDFABCommand { get; set; }
        public ICommand ScanQRFABCommand { get; set; }

        private void UpdateButtonClicked()
        {
            int[] materialIds = OrderedMaterialItems.Where(om => om.Material.IsChecked).Select(om => om.Material.id).ToArray<int>();
            if (Navigation != null & materialIds != null)
            {
                ViewModelLocator.bulkUpdateVM = new BulkUpdateVM();
                ViewModelLocator.bulkUpdateVM.MaterialIds = materialIds;
                ViewModelLocator.bulkUpdateVM.Material = OrderedMaterialItems[0].Material;
                //ViewModelLocator.bulkUpdateVM.stag
                Navigation.PushAsync(new BulkUpdate());
            }
        }

        private void SelectAllClicked(string icon)
        {
            IsAll = !IsAll;
            if (OrderedMaterialItems != null && OrderedMaterialItems.Count > 0)
            {
                foreach (MaterialFrameVM item in OrderedMaterialItems)
                {
                    if (icon == "ic_checkbox_empty.png")
                    {
                        item.Material.IsChecked = true;
                    }
                    else
                    {
                        item.Material.IsChecked = false;
                    }
                }
            }
        }

        public void CheckListClicked(Material Material)
        {
            ViewModelLocator.jobChecklistVM.Material = Material;
            Navigation.PushAsync(new JobChecklist());
        }

        private void ScanRFIDFABClicked()
        {
            // Navigation.PushAsync(new ScanRFID());
        }

        private async void ScanQRFABClicked()
        {
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
                        ViewModelLocator.scanTrackerVM.DisplaySnackBar("No camera available", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        //void SearchClicked()
        //{
        //    if (IsOrderedTab)
        //    {
        //        OrderedMaterialItems = ListMaterialItems?.Where(m => m.Material.currentStage == null)
        //            .OrderByDescending(m => m.Material.mrfNo).ToList();

        //        if (OrderedMaterialItems.Count == 0)
        //        {
        //            Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
        //        }
        //        else if (OrderedMaterialItems.Count > 0)
        //        {
        //            foreach (var item in OrderedMaterialItems)
        //            {
        //                item.Material.IsCheckBoxVisible = true;
        //            }
        //            HasOrderedMaterial = true;
        //        }
        //        OrderedMaterialItems = OrderedMaterialItems.Where(p => p.Material.QCStatus.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.markingNo.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.mrfNo.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.DrawingNo.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.Length?.ToString().Trim() == SearchedText.Trim() || p.Material.Area?.ToString().Trim() == SearchedText.Trim()).ToList();
        //        if (OrderedMaterialItems == null || OrderedMaterialItems.Count == 0)
        //        {
        //            DisplaySnackBar("No results found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
        //        }
        //        else
        //        {
        //            Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("OrderedMaterialItems"); });
        //        }
        //    }
        //    else if (IsInventoryTab)
        //    {
        //        ProducedMaterialItems = ListMaterialItems?.Where(m => m.Material.currentStage != null &&
        //  m.Material.currentStage.MilestoneId == 1)
        //.OrderByDescending(m => m.Material.mrfNo).ToList();
        //        if (ProducedMaterialItems == null || ProducedMaterialItems.Count == 0)
        //        {
        //            Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
        //            return;
        //        }
        //        ProducedMaterialItems = ProducedMaterialItems.Where(p => p.Material.QCStatus?.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.markingNo.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.mrfNo.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.DrawingNo?.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.Length?.ToString().Trim() == SearchedText.Trim() || p.Material.Area?.ToString().Trim() == SearchedText.Trim()).ToList();
        //        if (ProducedMaterialItems == null || ProducedMaterialItems.Count == 0)
        //        {
        //            DisplaySnackBar("No results found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
        //        }
        //        else
        //        {
        //            Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("ProducedMaterialItems"); });
        //        }
        //    }
        //    else if (IsMCStructuralInsp)
        //    {
        //        if (ProducedMaterialItems == null || ProducedMaterialItems.Count == 0)
        //        {
        //            Device.BeginInvokeOnMainThread(() => { DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
        //            return;
        //        }
        //        var ProducedMaterialItems1 = ProducedMaterialItems.Where(p => p.Material.QCStatus?.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.markingNo.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.mrfNo.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.DrawingNo?.Trim().ToLower() == SearchedText.Trim().ToLower() || p.Material.Length?.ToString().Trim() == SearchedText.Trim() || p.Material.Area?.ToString().Trim() == SearchedText.Trim()).ToList();
        //        if (ProducedMaterialItems1 == null || ProducedMaterialItems1.Count == 0)
        //        {
        //            DisplaySnackBar("No results found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
        //        }
        //        else
        //        {
        //            ProducedMaterialItems = ProducedMaterialItems1;
        //            Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("ProducedMaterialItems"); });
        //        }
        //    }
        //}
        public VendorHomeVM() : base()
        {
            UpdateCommand = new Command(UpdateButtonClicked);
            ChecklistButtonCommand = new Command<Material>(CheckListClicked);
            ScanRFIDFABCommand = new Command(ScanRFIDFABClicked);
            ScanQRFABCommand = new Command(ScanQRFABClicked);
            ListMaterialItems = new List<MaterialFrameVM>();
            OrderedMaterialItems = new List<MaterialFrameVM>();
            ProducedQcMaterialItems = new List<MaterialFrameVM>();
            ProducedMaterialItems = new List<MaterialFrameVM>();
            SelectAllCommand = new Command<string>(SelectAllClicked);
         //   SearchCommand = new Command(SearchClicked);
        }

        public override void Reset()
        {
            base.Reset();
            ListMaterialItems.Clear();
            ProducedMaterialItems.Clear();
            OrderedMaterialItems.Clear();
            ProducedQcMaterialItems.Clear();
            //  HasOrderedMaterial = false;
            IsMCStructuralInsp = false;
            // ListMaterialItems = new List<MaterialFrameVM>();
        }

        public string _checkBoxIcon { get; set; } = "ic_checkbox_empty.png";
        public string CheckBoxIcon { get => _checkBoxIcon; set { _checkBoxIcon = value; OnPropertyChanged("CheckBoxIcon"); } }

        private bool _isAll { get; set; }
        public bool IsAll
        {
            get => _isAll;
            set
            {
                if (_isAll != value)
                {
                    _isAll = value;
                    OnPropertyChanged("IsAll");
                }
                if (_isAll)
                    CheckBoxIcon = "ic_checkbox_checked.png";
                else
                    CheckBoxIcon = "ic_checkbox_empty.png";
            }
        }
    }

}
