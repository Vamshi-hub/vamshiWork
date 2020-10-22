using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VendorInventory : ContentPage
    {
        public VendorInventory()
        {
            InitializeComponent();
            if (App.Current.Properties.ContainsKey("user_id"))
            {
                int userId = (int)App.Current.Properties["user_id"];
                Task.Run(() => ApiClient.Instance.MTGetLocations(userId)).ContinueWith((t) =>
                {
                    ViewModelLocator.vendorStartDeliveryVM.Locations = t.Result.data as List<Location>;
                });
            }
            Task.Run(ViewModelLocator.vendorInventoryVM.GetProjects);
        }

        void OnRowTapped(Object sender, EventArgs e)
        {
            var grid = sender as Grid;
            var inventory = grid.BindingContext as Inventory;
            inventory.Expanded = !inventory.Expanded;
        }

        void OnScanButtonClicked(Object sender, EventArgs e)
        {
            ViewModelLocator.vendorInventoryVM.ScanMenuVisible = !ViewModelLocator.vendorInventoryVM.ScanMenuVisible;
            /*
            if (ViewModelLocator.vendorInventoryVM.SelectedProject != null)
            {
                Xamarin.Forms.Application.Current.Properties["project_id"] = ViewModelLocator.vendorInventoryVM.SelectedProject.id;
                ViewModelLocator.vendorEnrolmentVM.Project = ViewModelLocator.vendorInventoryVM.SelectedProject;
                ViewModelLocator.vendorStartDeliveryVM.Project = ViewModelLocator.vendorInventoryVM.SelectedProject;
                Navigation.PushAsync(new VendorScanRFID());
            }
            */
        }
        
        private void OnQRCodeScanButtonClicked(object sender, EventArgs e)
        {
            Application.Current.Properties["project_id"] = ViewModelLocator.vendorInventoryVM.SelectedProject.id;
            ViewModelLocator.vendorEnrolmentVM.Project = ViewModelLocator.vendorInventoryVM.SelectedProject;
            ViewModelLocator.vendorStartDeliveryVM.Project = ViewModelLocator.vendorInventoryVM.SelectedProject;
            Navigation.PushAsync(new VendorScanQRCode());
            ViewModelLocator.vendorInventoryVM.SelectedProject = null;
        }

        private void OnRFIDScanButtonClicked(object sender, EventArgs e)
        {
            Application.Current.Properties["project_id"] = ViewModelLocator.vendorInventoryVM.SelectedProject.id;
            ViewModelLocator.vendorEnrolmentVM.Project = ViewModelLocator.vendorInventoryVM.SelectedProject;
            ViewModelLocator.vendorStartDeliveryVM.Project = ViewModelLocator.vendorInventoryVM.SelectedProject;
            Navigation.PushAsync(new VendorScanRFID());
            ViewModelLocator.vendorInventoryVM.SelectedProject = null;
        }
    }
}