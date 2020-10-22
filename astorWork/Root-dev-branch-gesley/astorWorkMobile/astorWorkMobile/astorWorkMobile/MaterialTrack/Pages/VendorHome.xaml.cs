using astorWorkMobile.MaterialTrack.ViewModels;
using astorWorkMobile.Shared.Utilities;
using astorWorkMobile.Shared.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
//using static Android.Renderscripts.Program;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using System.Collections.Generic;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VendorHome : TabbedPage
    {
        bool IsScanBtnsVisible;
        double Sec = 0;
        bool IsReleased = false;
        Switch btnToggle = null;
        StackLayout ParentGrid = null;
        MaterialFrameVM MaterialFrame = null;
        MaterialFrameView MaterialFrameView = null;
        Task GetProjects = null;

        public VendorHome()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        private async void TabbedPage_Appearing(object sender, EventArgs e)
        {
            GetProjects = Task.Run(async () => { await ViewModelLocator.vendorHomeVM.GetProjects(); });
        }

        private async void QC_NotifyPage_Appearing(object sender, EventArgs e)
        {
            IsScanBtnsVisible = false;
            btnQRScan_QCNotify.IsVisible = false;
            //   btnRFIDScan_QCNotify.IsVisible = false;
            ViewModelLocator.vendorHomeVM.IsQCNotificationsTab = true;
            ViewModelLocator.vendorHomeVM.IsOrderedTab = false;
            ViewModelLocator.vendorHomeVM.IsInventoryTab = false;
            Task.Run(async () => { await GetProjects.ContinueWith((a) => { ViewModelLocator.vendorHomeVM.GetOrderedProduced(); }); });
            //await Task.Run(async () => { await ViewModelLocator.vendorHomeVM.GetProjects(); }).ContinueWith(t => ViewModelLocator.vendorHomeVM.GetOrderedProduced());
        }

        private void OrderedPage_Appearing(object sender, EventArgs e)
        {
            ViewModelLocator.vendorHomeVM.IsVendorScreen = true;
            IsScanBtnsVisible = false;
            btnQRScan_Ordered.IsVisible = false;
            //  btnRFIDScan_Ordered.IsVisible = false;
            ViewModelLocator.vendorHomeVM.IsPageInitializing = true;
            ViewModelLocator.vendorHomeVM.OrderedSearchText = "";
            ViewModelLocator.vendorHomeVM.IsQCNotificationsTab = false;
            ViewModelLocator.vendorHomeVM.IsOrderedTab = true;
            ViewModelLocator.vendorHomeVM.IsInventoryTab = false;
            Task.Run(() => { ViewModelLocator.vendorHomeVM.GetOrderedProduced(); ViewModelLocator.vendorHomeVM.IsPageInitializing = false; });

        }

        private void InventoryPage_Appearing(object sender, EventArgs e)
        {
            IsScanBtnsVisible = false;
            btnQRScan_Inventory.IsVisible = false;
            //   btnRFIDScan_Inventory.IsVisible = false;
            ViewModelLocator.vendorHomeVM.IsPageInitializing = true;
            ViewModelLocator.vendorHomeVM.InventorySearchText = "";
            ViewModelLocator.vendorHomeVM.IsQCNotificationsTab = false;
            ViewModelLocator.vendorHomeVM.IsOrderedTab = false;
            ViewModelLocator.vendorHomeVM.IsInventoryTab = true;
            Task.Run(() => { ViewModelLocator.vendorHomeVM.GetOrderedProduced(); ViewModelLocator.vendorHomeVM.IsPageInitializing = false; });
        }

        private void OrderedSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(e.NewTextValue.Trim()) || !string.IsNullOrWhiteSpace(e.NewTextValue.Trim()))
                {
                    if (ViewModelLocator.vendorHomeVM.IsOrderedTab)
                    {
                        ViewModelLocator.vendorHomeVM.OrderedMaterialItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage == null)
                            .OrderByDescending(m => m.Material.mrfNo).ToList();
                        if (ViewModelLocator.vendorHomeVM.OrderedMaterialItems == null || ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count == 0)
                        {
                            Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("OrderedMaterialItems"); });
                            return;
                        }
                        ViewModelLocator.vendorHomeVM.OrderedMaterialItems = ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Where(p => p.Material.QCStatus.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower()) || p.Material.markingNo.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower()) || p.Material.mrfNo.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower()) || (p.Material.DrawingNo != null && p.Material.DrawingNo.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower())) || (p.Material.Length != null && p.Material.Length.ToString().Trim().Contains(e.NewTextValue.Trim())) || (p.Material.Area != null && p.Material.Area.ToString().Trim().Contains(e.NewTextValue.Trim()))).ToList();
                        if (ViewModelLocator.vendorHomeVM.OrderedMaterialItems == null || ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count == 0)
                        {
                            Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("OrderedMaterialItems"); });
                            return;
                        }
                        else
                        {
                            foreach (var item in ViewModelLocator.vendorHomeVM.OrderedMaterialItems)
                            {
                                item.Material.IsCheckBoxVisible = true;
                            }
                            ViewModelLocator.vendorHomeVM.HasOrderedMaterial = true;
                        }
                        Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("OrderedMaterialItems"); });
                    }
                }
                else
                {
                    Task.Run(() =>
                    {
                        if (ViewModelLocator.vendorHomeVM.IsOrderedTab)
                        {
                            ViewModelLocator.vendorHomeVM.OrderedMaterialItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage == null)
    .OrderByDescending(m => m.Material.mrfNo).Take(10).ToList();

                            if (ViewModelLocator.vendorHomeVM.OrderedMaterialItems == null || ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count == 0)
                            {
                                Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
                                return;
                            }
                            else if (ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count > 0)
                            {
                                foreach (var item in ViewModelLocator.vendorHomeVM.OrderedMaterialItems)
                                {
                                    item.Material.IsCheckBoxVisible = true;
                                }
                                ViewModelLocator.vendorHomeVM.HasOrderedMaterial = true;
                            }
                            Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("OrderedMaterialItems"); });
                        }
                    });
                }
            });
        }

        private void InvSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(e.NewTextValue.Trim()) || !string.IsNullOrWhiteSpace(e.NewTextValue.Trim()))
                {
                    if (ViewModelLocator.vendorHomeVM.IsInventoryTab)
                    {
                        ViewModelLocator.vendorHomeVM.ProducedMaterialItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage != null &&
                  m.Material.currentStage.MilestoneId == 1)
                .OrderByDescending(m => m.Material.mrfNo).ToList();
                        if (ViewModelLocator.vendorHomeVM.ProducedMaterialItems == null || ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count == 0)
                        {
                            Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("ProducedMaterialItems"); });
                            return;
                        }
                        ViewModelLocator.vendorHomeVM.ProducedMaterialItems = ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Where(p => p.Material.QCStatus.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower()) || p.Material.markingNo.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower()) || p.Material.mrfNo.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower()) || (p.Material.DrawingNo != null && p.Material.DrawingNo.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower())) || (p.Material.Length != null && p.Material.Length.ToString().Trim().Contains(e.NewTextValue.Trim())) || (p.Material.Area != null && p.Material.Area.ToString().Trim().Contains(e.NewTextValue.Trim()))).ToList();
                        if (ViewModelLocator.vendorHomeVM.ProducedMaterialItems == null || ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count == 0)
                        {
                            Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("ProducedMaterialItems"); });
                            return;
                        }
                        else
                        {
                            Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("ProducedMaterialItems"); });
                        }
                    }
                }
                else
                {
                    Task.Run(() =>
                    {
                        if (ViewModelLocator.vendorHomeVM.IsInventoryTab)
                        {
                            ViewModelLocator.vendorHomeVM.ProducedMaterialItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage != null &&
                  m.Material.currentStage.MilestoneId == 1)
                .OrderByDescending(m => m.Material.mrfNo).Take(10).ToList();

                            if (ViewModelLocator.vendorHomeVM.ProducedMaterialItems == null || ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count == 0)
                            {
                                Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
                            }
                            else
                            {
                                Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("ProducedMaterialItems"); });
                            }
                        }
                    });
                }
            });
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var vm = BindingContext as VendorHomeVM;
            var material = e.Item as MaterialFrameVM;
            //  vm.ShowHideMaterial(material);
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            var vm = ViewModelLocator.vendorHomeVM;
            if (vm != null & vm.OrderedMaterialItems != null)
            {
                foreach (MaterialFrameVM item in vm.OrderedMaterialItems)
                {
                    item.Material.IsChecked = e.Value;
                }
            }
        }

        private void TabbedPage_Disappearing(object sender, EventArgs e)
        {
            //ViewModelLocator.vendorHomeVM.SelectedProjQCNotif = null;
            //ViewModelLocator.vendorHomeVM.SelectedProjOrdered = null;
            //ViewModelLocator.vendorHomeVM.SelectedProjInventory = null;
            ViewModelLocator.vendorHomeVM.IsQCNotificationsTab = false;
            ViewModelLocator.vendorHomeVM.IsOrderedTab = false;
            ViewModelLocator.vendorHomeVM.IsInventoryTab = false;
        }

        private async void BtnScan_QCNotify_Clicked(object sender, EventArgs e)
        {
            if (IsScanBtnsVisible)
            {
                btnQRScan_QCNotify.IsVisible = false;
                //   btnRFIDScan_QCNotify.IsVisible = false;
                IsScanBtnsVisible = false;
            }
            else
            {
                //    btnRFIDScan_QCNotify.IsVisible = true;
                //    await btnRFIDScan_QCNotify.ScaleTo(1.25, 100);
                //    await btnRFIDScan_QCNotify.ScaleTo(1, 100);

                btnQRScan_QCNotify.IsVisible = true;
                await btnQRScan_QCNotify.ScaleTo(1.25, 100);
                await btnQRScan_QCNotify.ScaleTo(1, 100);

                IsScanBtnsVisible = true;
            }
        }

        private async void BtnScan_Ordered_Clicked(object sender, EventArgs e)
        {
            if (IsScanBtnsVisible)
            {
                btnQRScan_Ordered.IsVisible = false;
                //  btnRFIDScan_Ordered.IsVisible = false;
                IsScanBtnsVisible = false;
            }
            else
            {
                //    btnRFIDScan_Ordered.IsVisible = true;
                //    await btnRFIDScan_Ordered.ScaleTo(1.25, 100);
                //    await btnRFIDScan_Ordered.ScaleTo(1, 100);

                btnQRScan_Ordered.IsVisible = true;
                await btnQRScan_Ordered.ScaleTo(1.25, 100);
                await btnQRScan_Ordered.ScaleTo(1, 100);

                IsScanBtnsVisible = true;
            }
        }

        private async void BtnScan_Inventory_Clicked(object sender, EventArgs e)
        {
            if (IsScanBtnsVisible)
            {
                btnQRScan_Inventory.IsVisible = false;
                //    btnRFIDScan_Inventory.IsVisible = false;
                IsScanBtnsVisible = false;
            }
            else
            {
                //    btnRFIDScan_Inventory.IsVisible = true;
                //    await btnRFIDScan_Inventory.ScaleTo(1.25, 100);
                //    await btnRFIDScan_Inventory.ScaleTo(1, 100);

                btnQRScan_Inventory.IsVisible = true;
                await btnQRScan_Inventory.ScaleTo(1.25, 100);
                await btnQRScan_Inventory.ScaleTo(1, 100);

                IsScanBtnsVisible = true;
            }
        }
        private async void Button_Pressed(object sender, EventArgs e)
        {
            IsReleased = false;
            Sec = 0;
            ParentGrid = (StackLayout)((Button)sender).CommandParameter;
            btnToggle = (Switch)ParentGrid.Children.Where(p => p.ClassId == "btnToggle").FirstOrDefault();
            var btnContext = (Button)ParentGrid.Children.Where(p => p.ClassId == "btnContext").FirstOrDefault();
            var materialParameter = btnContext.CommandParameter;
            var Grid = (Grid)ParentGrid.Children.Where(p => p.ClassId == "GridMaterialView").FirstOrDefault();
            MaterialFrameView = (MaterialFrameView)Grid.Children.Where(p => p.ClassId == "MaterialView").FirstOrDefault();
            MaterialFrame = ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Where(p => p.Material == materialParameter).FirstOrDefault();
            Device.StartTimer(TimeSpan.FromSeconds(1), GetSeconds);
            if (btnToggle != null && !btnToggle.IsVisible)
            {
                btnToggle.Toggled += BtnToggle_Toggled;
                // BtnToggle_Toggled(null, null);
            }
            else
            {
                btnToggle.Toggled -= BtnToggle_Toggled;
            }
        }

        private async void BtnToggle_Toggled(object sender, ToggledEventArgs e)
        {
            //btnToggle.IsVisible = false;
            MaterialFrame.Material.IsToggled = false;
            await MaterialFrameView.TranslateTo(btnToggle.X + btnToggle.WidthRequest, MaterialFrameView.Y, 250, Easing.Linear);
        }

        async void Animate()
        {
            await MaterialFrameView.TranslateTo(btnToggle.X + btnToggle.WidthRequest, MaterialFrameView.Y, 250, Easing.Linear);
        }
        private void Button_Released(object sender, EventArgs e)
        {
            IsReleased = true;
            Sec = 0;
        }
        bool GetSeconds()
        {
            Sec++;
            if (Sec != 1 && IsReleased)
            {
                Sec = 0;
                return false;
            }
            else if (Sec == 1 && !IsReleased)
            {
                if (btnToggle != null)
                {
                    MaterialFrame.Material.IsToggled = true;
                    // MaterialFrameView.WidthRequest = MaterialFrameView.WidthRequest - 100;
                    Animate();
                    Sec = 0;
                }
                return false;
            }
            else if (Sec >= 1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }

        private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            List<MaterialFrameVM> totalItems = null;
            if (ViewModelLocator.vendorHomeVM.IsOrderedMatSearch)
            {
                totalItems = ViewModelLocator.vendorHomeVM.OrderedMaterialItems;
            }
            else
            {
                totalItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage == null)
                        .OrderByDescending(m => m.Material.mrfNo).ToList();
            }
            if (e.ItemIndex == ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count - 1 && e.ItemIndex != totalItems.Count - 1)
            {
                if (ViewModelLocator.vendorHomeVM.IsOrderedTab)
                {
                    // ViewModelLocator.vendorHomeVM.IsListBusy = true;
                    ViewModelLocator.vendorHomeVM.OnPropertyChanged("IsListBusy");
                    var orderedMaterials = totalItems.Take(ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count + 10).ToList();
                    if (orderedMaterials.Count > 0)
                    {
                        foreach (var item in orderedMaterials)
                        {
                            item.Material.IsCheckBoxVisible = true;
                        }
                        ViewModelLocator.vendorHomeVM.HasOrderedMaterial = true;
                    }
                    ViewModelLocator.vendorHomeVM.OrderedMaterialItems = orderedMaterials;
                    //ViewModelLocator.vendorHomeVM.IsListBusy = false;
                    ViewModelLocator.vendorHomeVM.OnPropertyChanged("IsListBusy");
                    ViewModelLocator.vendorHomeVM.OnPropertyChanged("OrderedMaterialItems");
                }
            }
        }

        private void ListView_ItemAppearing_1(object sender, ItemVisibilityEventArgs e)
        {
            List<MaterialFrameVM> totalItems = null;
            if (ViewModelLocator.vendorHomeVM.IsInventoryMatSearch)
            {
                totalItems = ViewModelLocator.vendorHomeVM.ProducedMaterialItems;
            }
            else
            {
                totalItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage != null &&
                  m.Material.currentStage.MilestoneId == 1).OrderByDescending(m => m.Material.mrfNo).ToList();
            }
            if (e.ItemIndex == ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count - 1 && e.ItemIndex != totalItems.Count - 1)
            {
                if (ViewModelLocator.vendorHomeVM.IsInventoryTab)
                {
                    ViewModelLocator.vendorHomeVM.IsListBusy = true;
                    ViewModelLocator.vendorHomeVM.OnPropertyChanged("IsListBusy");
                    var producedMaterials = totalItems.Take(ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count + 10).ToList();
                    ViewModelLocator.vendorHomeVM.ProducedMaterialItems = producedMaterials;
                    ViewModelLocator.vendorHomeVM.IsListBusy = false;
                    ViewModelLocator.vendorHomeVM.OnPropertyChanged("IsListBusy");
                    ViewModelLocator.vendorHomeVM.OnPropertyChanged("ProducedMaterialItems");
                }
            }
        }

        //private void listScrollView_Scrolled(object sender, ScrolledEventArgs e)
        //{
        //    double spaceAvailableForScrolling = listScrollView.ContentSize.Height - listScrollView.Height;
        //    double buffer = 32;
        //    if (spaceAvailableForScrolling > e.ScrollY + buffer)
        //    {
        //        var totalItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage == null)
        //                 .OrderByDescending(m => m.Material.mrfNo).ToList();
        //        if (ViewModelLocator.vendorHomeVM.IsOrderedTab)
        //        {
        //            ViewModelLocator.vendorHomeVM.IsListBusy = true;
        //            ViewModelLocator.vendorHomeVM.OnPropertyChanged("IsListBusy");
        //            var orderedMaterials = totalItems.Take(ViewModelLocator.vendorHomeVM.OrderedMaterialItems.Count + 10).ToList();
        //            if (orderedMaterials.Count > 0)
        //            {
        //                foreach (var item in orderedMaterials)
        //                {
        //                    item.Material.IsCheckBoxVisible = true;
        //                }
        //                ViewModelLocator.vendorHomeVM.HasOrderedMaterial = true;
        //            }
        //            ViewModelLocator.vendorHomeVM.OrderedMaterialItems = orderedMaterials;
        //            ViewModelLocator.vendorHomeVM.IsListBusy = false;
        //            ViewModelLocator.vendorHomeVM.OnPropertyChanged("IsListBusy");
        //            ViewModelLocator.vendorHomeVM.OnPropertyChanged("OrderedMaterialItems");
        //        }
        //    }
        //}
    }
}