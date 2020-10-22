using astorWorkMobile.MaterialTrack.ViewModels;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MaterialInspection : ContentPage
    {
        public MaterialInspection()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModelLocator.vendorHomeVM.IsPageInitializing = true;
            ViewModelLocator.vendorHomeVM.QCInsSearchText = "";
            ViewModelLocator.vendorHomeVM.IsInventoryTab = false;
            ViewModelLocator.vendorHomeVM.IsMCStructuralInsp = true;
            ViewModelLocator.vendorHomeVM.IsStageSelected = false;
            ViewModelLocator.vendorHomeVM.GetProducedForMainCon();
            ViewModelLocator.vendorHomeVM.IsPageInitializing = false;
        }
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ViewModelLocator.vendorHomeVM.IsMCStructuralInsp = false;
            ViewModelLocator.vendorHomeVM.IsStageSelected = false;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }

        private void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            List<MaterialFrameVM> producedItems = null;
            if (ViewModelLocator.vendorHomeVM.IsQCInsMatSearch)
            {
                producedItems = ViewModelLocator.vendorHomeVM.ProducedMaterialItems;
            }
            else if (ViewModelLocator.vendorHomeVM.IsStageSelected && ViewModelLocator.vendorHomeVM.Stage != null && !ViewModelLocator.vendorHomeVM.IsQCInsMatSearch)
            {

                producedItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage != null)
                    .OrderByDescending(m => m.Material.mrfNo).ToList();
                producedItems = producedItems.Where(p => p.Material.currentStage.Name == ViewModelLocator.vendorHomeVM.Stage.Name).ToList();
            }
            else
                producedItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage != null)
                    .OrderByDescending(m => m.Material.mrfNo).ToList();
            if (e.ItemIndex == ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count - 1 && e.ItemIndex != producedItems.Count - 1)
            {
                ViewModelLocator.vendorHomeVM.IsListBusy = true;
                ViewModelLocator.vendorHomeVM.OnPropertyChanged("IsListBusy");
                var producedMaterials = producedItems.Take(ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count + 10).ToList();
                ViewModelLocator.vendorHomeVM.ProducedMaterialItems = producedMaterials;
                ViewModelLocator.vendorHomeVM.IsListBusy = false;
                ViewModelLocator.vendorHomeVM.OnPropertyChanged("IsListBusy");
                ViewModelLocator.vendorHomeVM.OnPropertyChanged("ProducedMaterialItems");
            }
        }

        private void ListView_ItemAppearing_1(object sender, ItemVisibilityEventArgs e)
        {

        }
        private void QCInsSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(e.NewTextValue.Trim()) || !string.IsNullOrWhiteSpace(e.NewTextValue.Trim()))
                {
                    if (ViewModelLocator.vendorHomeVM.IsMCStructuralInsp)
                    {
                        ViewModelLocator.vendorHomeVM.ProducedMaterialItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage != null)
                .OrderByDescending(m => m.Material.mrfNo).ToList();
                        if (ViewModelLocator.vendorHomeVM.ProducedMaterialItems == null || ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count == 0)
                        {
                            return;
                        }
                        ViewModelLocator.vendorHomeVM.ProducedMaterialItems = ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Where(p => p.Material.QCStatus.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower()) || p.Material.markingNo.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower()) || p.Material.mrfNo.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower()) || (p.Material.DrawingNo != null && p.Material.DrawingNo.Trim().ToLower().Contains(e.NewTextValue.Trim().ToLower())) || (p.Material.Length != null && p.Material.Length.ToString().Trim().Contains(e.NewTextValue.Trim())) || (p.Material.Area != null && p.Material.Area.ToString().Trim().Contains(e.NewTextValue.Trim()))).ToList();
                        if (ViewModelLocator.vendorHomeVM.ProducedMaterialItems == null || ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count == 0)
                        {
                            Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("ProducedMaterialItems"); });
                            return;
                        }
                        Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("ProducedMaterialItems"); });
                    }
                }
                else
                {
                    Task.Run(() =>
                    {
                        if (ViewModelLocator.vendorHomeVM.IsMCStructuralInsp)
                        {
                            ViewModelLocator.vendorHomeVM.ProducedMaterialItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage != null)
                .OrderByDescending(m => m.Material.mrfNo).Take(10).ToList();

                            if (ViewModelLocator.vendorHomeVM.ProducedMaterialItems == null || ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count == 0)
                            {
                                Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.DisplaySnackBar("No materials found", Enums.PageActions.None, Enums.MessageActions.Warning, null, null); });
                                return;
                            }
                            Device.BeginInvokeOnMainThread(() => { ViewModelLocator.vendorHomeVM.OnPropertyChanged("ProducedMaterialItems"); });
                        }
                    });
                }
            });
        }

        //private void ScrollView_Scrolled(object sender, ScrolledEventArgs e)
        //{
        //    double spaceAvailableForScrolling = listScrollView.ContentSize.Height - listScrollView.Height;
        //    double buffer = 32;
        //    if (spaceAvailableForScrolling > e.ScrollY + buffer)
        //    {
        //        var producedItems = ViewModelLocator.vendorHomeVM.ListMaterialItems?.Where(m => m.Material.currentStage != null)
        //            .OrderByDescending(m => m.Material.mrfNo).ToList();
        //        ViewModelLocator.vendorHomeVM.IsListBusy = true;
        //        ViewModelLocator.vendorHomeVM.OnPropertyChanged("IsListBusy");
        //        var producedMaterials = producedItems.Take(ViewModelLocator.vendorHomeVM.ProducedMaterialItems.Count + 10).ToList();
        //        ViewModelLocator.vendorHomeVM.ProducedMaterialItems = producedMaterials;
        //        ViewModelLocator.vendorHomeVM.IsListBusy = false;
        //        ViewModelLocator.vendorHomeVM.OnPropertyChanged("IsListBusy");
        //        ViewModelLocator.vendorHomeVM.OnPropertyChanged("ProducedMaterialItems");
        //    }
        //}
    }
}