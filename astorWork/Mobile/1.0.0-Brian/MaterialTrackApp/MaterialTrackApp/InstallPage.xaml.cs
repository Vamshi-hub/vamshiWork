using MaterialTrackApp.Entities;
using MaterialTrackApp.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MaterialTrackApp.Utility.ApiClient;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstallPage : ContentPage
    {
        private List<InstallTaskEntity> _pendingTasks;
        public InstallPage()
        {
            InitializeComponent();
        }

        private async void btnSubmit_Clicked(object sender, EventArgs e)
        {
            bool shouldSubmit = false;

            _pendingTasks = ViewModelLocator.installPageVM.PendingTasks
                .Where(t => t.Selected)
                .ToList();
            
            var tasksQCFail = _pendingTasks.Where(t => !t.PassQC);

            if (tasksQCFail.Count() > 0)
            {
                string listQCFail = "QC failed for the following components:\r\n";
                foreach (var task in tasksQCFail)
                {
                    listQCFail += string.Format("\r\n{0}-{1}-{2}-{3}-{4}", task.Project, task.Block,
                        task.Level, task.Zone, task.MarkingNo);
                }
                shouldSubmit = await DisplayAlert("Warning", listQCFail, "Confirm", "Cancel");
            }
            else
                shouldSubmit = true;

            if (shouldSubmit)
            {
                App.InvokeLoadingView(this, layoutMain, Task.Run(SubmitInstallUpdates), "Submitting updates, please wait...");
            }
        }

        private async Task SubmitInstallUpdates()
        {
            var siteData = new List<SiteUpdateData>();
            string userName = Application.Current.Properties["user_name"] as string;
            int locationId = int.Parse(Application.Current.Properties["user_location"].ToString());

            foreach (var task in _pendingTasks)
            {
                try
                {
                    var data = new SiteUpdateData
                    {
                        MaterialID = task.MaterialId,
                        Status = 1,
                        UserName = userName,
                        UserLocationID = locationId,
                        PassQC = task.PassQC
                    };
                    siteData.Add(data);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    Debug.WriteLine(exc.StackTrace);
                }
            }

            if (siteData.Count > 0)
            {
                var result = await ApiClient.Instance.MTUpdateMaterialMasterBySite(siteData);
                if (result.Status == 0)
                    await LocalDBHandler.Instance.ClearTable<InstallTaskEntity>();

                Device.BeginInvokeOnMainThread(() =>
                {
                    if (result.Status == 0)
                    {
                        DisplayAlert("Done", result.Message, "OK");
                    }
                    else
                    {
                        DisplayAlert("Error", result.Message, "OK");
                    }
                    Navigation.PopToRootAsync();
                    App.mainPage.NavigateDetail(0);
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Alert", "No update data", "OK");
                });
            }
        }
    }
}