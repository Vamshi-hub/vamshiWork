using MaterialTrackApp.Entities;
using MaterialTrackApp.PartialView;
using MaterialTrackApp.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MaterialTrackApp.Utility.ApiClient;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollPage : ContentPage
    {
        public EnrollPage()
        {
            InitializeComponent();
            InitPageModel();
        }

        public void InitPageModel()
        {
            
            foreach(var task in ViewModelLocator.enrollPageVM.PendingTasks)
            {
                var view = new EnrollTaskView();
                view.MinimumHeightRequest = 300;
                view.BindingContext = task;
                layoutTasks.Children.Add(view);
            }            
        }


        private async void btnSubmit_Clicked(object sender, EventArgs e)
        {
            bool shouldSubmit = false;

            var taskNoUpdate = ViewModelLocator.enrollPageVM.PendingTasks
                .Where(t => t.NoUpdate)
                .FirstOrDefault();

            var tasksFailQC = ViewModelLocator.enrollPageVM.PendingTasks
                .Where(t => !t.PassQC && !string.IsNullOrEmpty(t.MRFNo));

            if (taskNoUpdate != null)
            {
                await DisplayAlert("Error", string.Format("Beacon {0} doesn't have any update", taskNoUpdate.Beacon.DisplayName), "OK");
            }
            else if (tasksFailQC.Count() > 0)
            {
                string listFailQC = "QC failed for the following components (beacon id):\r\n";
                foreach (var task in tasksFailQC)
                    listFailQC += string.Format("\r\n{0} ({1})", task.MarkingNo, task.Beacon.DisplayName);

                shouldSubmit = await DisplayAlert("Warning", listFailQC, "Confirm", "Cancel");
            }
            else
                shouldSubmit = true;

            if (shouldSubmit)
            {
                App.InvokeLoadingView(this, layoutMain, Task.Run(SubmitBeaconUpdates), "Submitting updates, please wait...");
            }

        }

        private async Task SubmitBeaconUpdates()
        {
            List<VendorUpdateData> vendorData = new List<VendorUpdateData>();
            string userName = Application.Current.Properties["user_name"] as string;
            int locationId = int.Parse(Application.Current.Properties["user_location"].ToString());

            foreach (var task in ViewModelLocator.enrollPageVM.PendingTasks)
            {
                try
                {
                    var data = new VendorUpdateData
                    {
                        BeaconID = task.Beacon == null ? null : task.Beacon.BeaconID,
                        CastingDate = task.CastingDate,
                        LotNo = task.LotNo,
                        Status = "Produced",
                        UserName = userName,
                        UserLocationID = locationId,
                        MarkingNo = task.MarkingNo,
                        MRFNo = task.MRFNo,
                        PassQC = task.PassQC,
                        Project = task.Project
                    };
                    vendorData.Add(data);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    Debug.WriteLine(exc.StackTrace);
                }
            }

            if (vendorData.Count > 0)
            {
                var result = await ApiClient.Instance.MTUpdateMaterialMasterByVendor(vendorData);
                if (result.Status == 0)
                    await LocalDBHandler.Instance.ClearTable<EnrollTaskEntity>();

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

        private async void btnSave_Clicked(object sender, EventArgs e)
        {
            int result = await LocalDBHandler.Instance.SaveEntitiesAsync(ViewModelLocator.enrollPageVM.PendingTasks);
            if (result > 0)
            {
                await Navigation.PopToRootAsync();
                App.mainPage.NavigateDetail(0);
                await DisplayAlert("Done", result + " tasks are saved", "OK");
            }
            else
                await DisplayAlert("Warning", "An error happened", "OK");
        }
    }
}