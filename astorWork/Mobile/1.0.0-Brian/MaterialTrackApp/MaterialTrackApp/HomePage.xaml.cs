using FormsPlugin.Iconize;
using MaterialTrackApp.Class;
using MaterialTrackApp.DB;
using MaterialTrackApp.Entities;
using MaterialTrackApp.Utility;
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
    public partial class HomePage : ContentPage
    {
        private List<Task> allTasks;
        private DateTime lastRefreshTime;
        private IconToolbarItem btnRefresh;

        public HomePage()
        {
            InitializeComponent();

            allTasks = new List<Task>();
            lastRefreshTime = DateTime.Now;
            btnRefresh = new IconToolbarItem
            {
                Icon = "md-refresh",
                IconColor = Color.White,
                Command = new Command(btnRefresh_Clicked),
                Order = ToolbarItemOrder.Primary
            };
            //ToolbarItems.Add(btnRefresh);

            RunInitTasks();
        }

        private async Task InitNumbers()
        {
            var result = await LocalDBHandler.Instance.ReadEnrollTaskAsync();
            ViewModelLocator.pendingTaskPageVM.ListEnrollTasks = new ObservableCollection<EnrollTaskEntity>(result);
            ViewModelLocator.homePageVM.CountPendingTask = result.Count;
        }

        private async void btnPendingTask_Tapped(object sender, EventArgs e)
        {
            if (ViewModelLocator.homePageVM.CountPendingTask == 0)
                await DisplayAlert("Warning", "No action is required for pending tasks", "OK");
            else
            {
                App.mainPage.NavigateDetail(2);
            }
        }

        private void btnPendingEnroll_Tapped(object sender, EventArgs e)
        {
            Application.Current.Properties["operation"] = Constants.OP_ENROLL;
            Application.Current.SavePropertiesAsync();
            App.mainPage.NavigateDetail(1);
        }

        private void btnPendingQC_Tapped(object sender, EventArgs e)
        {
            Application.Current.Properties["operation"] = Constants.OP_QC_PRODUCE;
            Application.Current.SavePropertiesAsync();
            App.mainPage.NavigateDetail(1);
        }

        private void btnRefresh_Clicked()
        {
            var timeDiff = DateTime.Now - lastRefreshTime;
            if (timeDiff > TimeSpan.FromSeconds(10))
            {
                lastRefreshTime = DateTime.Now;
                RunInitTasks();
            }
            else
            {
                DisplayAlert("Warning", "Refresh is limited to once every 10 seconds", "OK");
            }
        }

        private Task RunInitTasks()
        {
            allTasks.Clear();
            Task.Run(InitNumbers);

            /*
            App.InvokeLoadingView(this, ((layoutMain.Children[0] as Frame).Content as Grid), Task.Run(InitNumbers), "Loading, please wait...");
            App.InvokeLoadingView(this, ((layoutMain.Children[1] as Frame).Content as Grid), Task.Run(InitPendingEnroll), "Loading, please wait...");
            App.InvokeLoadingView(this, ((layoutMain.Children[2] as Frame).Content as Grid), Task.Run(InitPendingQC), "Loading, please wait...");
            */
            return Task.WhenAll(allTasks);
        }

        private void btnClearCache_Clicked(object sender, EventArgs e)
        {
            Task.Run(LocalDBHandler.Instance.ClearDB);
        }

        private void btnStartScan_Clicked(object sender, EventArgs e)
        {
            //Application.Current.Properties["operation"] = Constants.OP_ENROLL;
            //Application.Current.SavePropertiesAsync();
            App.mainPage.NavigateDetail(1);
        }

        private void btnPendingTask_Clicked(object sender, EventArgs e)
        {

        }

        private void btnManualUpdate_Clicked(object sender, EventArgs e)
        {
            App.InvokeLoadingView(this, layoutMain, Task.Run(() => PrepareManualInstallPage()), "Loading delivered components, please wait...");
        }

        private async Task PrepareManualInstallPage()
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
                        Beacon = null,
                        MaterialId = material.MaterialID,
                        Project = material.Project,
                        Block = material.Block,
                        Level = material.Level,
                        Zone = material.Zone,
                        MarkingNo = material.MarkingNo
                    };

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
    }
}