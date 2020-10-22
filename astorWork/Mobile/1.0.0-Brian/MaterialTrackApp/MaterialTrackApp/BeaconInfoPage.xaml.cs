using MaterialTrackApp.Class;
using MaterialTrackApp.DB;
using MaterialTrackApp.Entities;
using MaterialTrackApp.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BeaconInfoPage : ContentPage
    {
        public BeaconInfoPage()
        {
            InitializeComponent();
        }

        private void btnNext_Clicked(object sender, EventArgs e)
        {
            if (ViewModelLocator.beaconInfoPageVM.ListBeacons.Where(b => b.IsChecked).Count() == 0)
            {
                DisplayAlert("Warning", "Please choose some beacons", "OK");
            }
            else if (Application.Current.Properties["user_role"].Equals(Constants.ROLE_VENDOR))
            {
                App.InvokeLoadingView(this, layoutMain, Task.Run(PrepareEnrollPage), "Loading marking no., please wait...");
            }
            else if (Application.Current.Properties["user_role"].Equals(Constants.ROLE_SITE_SUPERVISOR))
            {
                App.InvokeLoadingView(this, layoutMain, Task.Run(() => PrepareInstallPage()), "Loading beacon-attached components, please wait...");
            }
            else
            {
                DisplayAlert("Warning", "No further action is required", "OK");
            }
        }

        private async Task PrepareInstallPage()
        {
            var result = await ApiClient.Instance.MTGetMaterialForInstallation();
            if (result.Status == 0)
            {
                var json2 = result.Data as JArray;

                var listMaterials = json2.Select(j => new MaterialEntity
                {
                    MarkingNo = (string)j["MarkingNo"],
                    MaterialType = (string)j["MaterialType"],
                    MaterialID = (int)j["MaterialID"],
                    BeaconID = j["BeaconID"] == null ? null : (string)j["BeaconID"],
                    Project = (string)j["Project"],
                    Block = (string)j["Block"],
                    Level = (string)j["Level"],
                    Zone = (string)j["Zone"],
                });

                ViewModelLocator.installPageVM.PendingTasks.Clear();
                foreach (var material in listMaterials)
                {
                    var task = new InstallTaskEntity
                    {
                        Beacon = ViewModelLocator.beaconInfoPageVM.ListBeacons
                            .Where(b => b.IsChecked && b.BeaconID.Equals(material.BeaconID)).FirstOrDefault(),
                        MaterialId = material.MaterialID,
                        Project = material.Project,
                        Block = material.Block,
                        Level = material.Level,
                        Zone = material.Zone,
                        MarkingNo = material.MarkingNo
                    };

                    if (task.Beacon != null)
                        ViewModelLocator.installPageVM.PendingTasks.Add(task);
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    var installPage = new InstallPage();
                    Navigation.PushAsync(installPage);
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "Can't get marking no", "OK");
                });
            }
        }

        private async Task PrepareEnrollPage()
        {
            var result = await ApiClient.Instance.MTGetMaterialForEnrolment((string) Application.Current.Properties["user_name"]);
            if (result.Status == 0)
            {
                var json2 = result.Data as JArray;

                var listMaterials = json2.Select(j => new MaterialEntity
                {
                    MarkingNo = (string)j["MarkingNo"],
                    MRFNo = (string)j["MRFNo"],
                    LotNo = (int)j["LotNo"],
                    Project = (string)j["Project"],
                    MaterialType = (string)j["MaterialType"]
                });
                ViewModelLocator.filterMarkingNoVM.Materials = listMaterials;
                ViewModelLocator.filterMarkingNoVM.Projects = new ObservableCollection<string>(listMaterials.Select(mm => mm.Project).Distinct());

                ViewModelLocator.filterMRFNoVM.Materials = listMaterials;

                int maxLotNo = Application.Current.Properties.ContainsKey("max_lot_no") ? (int)Application.Current.Properties["max_lot_no"] : listMaterials.Max(mm => mm.LotNo);

                ViewModelLocator.enrollPageVM.PendingTasks.Clear();
                foreach (var beacon in ViewModelLocator.beaconInfoPageVM.ListBeacons)
                {
                    if (beacon.IsChecked)
                    {
                        if(!beacon.InInventory)
                            maxLotNo += 1;

                        var task = new EnrollTaskEntity
                        {
                            Beacon = beacon,
                            LotNo = beacon.InInventory ? beacon.LotNo : maxLotNo,
                            Project = beacon.Project,
                            MarkingNo = beacon.MarkingNo,
                            InTransit = !string.IsNullOrEmpty(beacon.MarkingNo),
                            CastingDate = beacon.InInventory ? beacon.CastingDate : (DateTime.Now.DayOfWeek.Equals(DayOfWeek.Monday) ?
                            DateTime.Today.AddDays(-2) : DateTime.Today.AddDays(-1)),
                        };
                        ViewModelLocator.enrollPageVM.PendingTasks.Add(task);
                    }
                }

                Device.BeginInvokeOnMainThread(() =>
                {
                    var enrollPage = new EnrollPage();
                    Navigation.PushAsync(enrollPage);
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "Can't get components pending enrolment", "OK");
                });
            }
        }
    }
}