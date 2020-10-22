using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SiteUpdateStage : ContentPage
    {
        public SiteUpdateStage()
        {
            InitializeComponent();
        }

        void OnPageDisappearing(Object sender, EventArgs e)
        {
            ViewModelLocator.siteUpdateStageVM.Reset();
        }

        private async Task GetNextStage()
        {
            try
            {
                var result = await ApiClient.Instance.MTGetNextStageInfo(ViewModelLocator.siteUpdateStageVM.Material.id);
                if (result.status == 0)
                {
                    var json = result.data as JToken;
                    ViewModelLocator.siteUpdateStageVM.NextStageName = (string)json["nextStageName"];
                    ViewModelLocator.siteUpdateStageVM.NextStageColor = GetNextStageColor((string)json["nextStageColour"]);
                    ViewModelLocator.siteUpdateStageVM.NextStageQC = (bool)json["nextStageQC"];
                }
                else
                {
                    ViewModelLocator.siteUpdateStageVM.ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                ViewModelLocator.siteUpdateStageVM.ErrorMessage = ex.Message;
            }
        }

        public Color GetNextStageColor(string value)
        {
            Color NextStageColor;
            string strColour = value.ToString();
            if (!string.IsNullOrEmpty(strColour) && strColour.Length == 9)
            {
                strColour = "#" + strColour.Substring(7) + strColour.Substring(1, 6);
                NextStageColor = Color.FromHex(strColour);
            }
            else
            {
                NextStageColor = Color.FromHex(value);
            }

            return NextStageColor;
        }

        async void OnSubmitButtonClicked(Object sender, EventArgs e)
        {
            bool shouldContinue = true;
            if (ViewModelLocator.siteUpdateStageVM.NextStageQC && !ViewModelLocator.siteUpdateStageVM.TogglePass.Status)
            {
                shouldContinue = await DisplayAlert("Warning", "QC failed, are you sure?", "Yes", "No");
            }

            if (shouldContinue)
            {
                ViewModelLocator.siteUpdateStageVM.IsLoading = true;
                await Task.Run(UpdateMaterialStage).ContinueWith((task) =>
                AfterUpdateMaterialStage(task.Result), TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        async void OnCaseButtonClicked(Object sender, EventArgs e)
        {
            bool shouldContinue = true;
            if (ViewModelLocator.siteUpdateStageVM.NextStageQC && !ViewModelLocator.siteUpdateStageVM.TogglePass.Status)
            {
                shouldContinue = await DisplayAlert("Warning", "Are you sure you want to create new case?", "Yes", "No");
            }

            if (shouldContinue)
            {
                int stageAuditId = 0;
                if (ViewModelLocator.siteUpdateStageVM.Material.stageName != ViewModelLocator.siteUpdateStageVM.NextStageName)
                {
                    stageAuditId = await Task.Run(UpdateMaterialStage);
                }
                else
                {
                    stageAuditId = ViewModelLocator.siteUpdateStageVM.Material.stageId;
                    ViewModelLocator.qcDefectVM.StageAuditID = ViewModelLocator.siteUpdateStageVM.Material.stageId;
                }

                if (stageAuditId > 0)
                {
                    ViewModelLocator.qcDefectVM.Material = ViewModelLocator.siteUpdateStageVM.Material;
                    ViewModelLocator.qcDefectVM.QCOpenCase = new QCCase();
                    ViewModelLocator.qcDefectVM.QCDefectDetails = new QCDefect();
                    ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen = true;
                    ViewModelLocator.qcDefectVM.QCPhotos.Clear();

                    await Navigation.PushAsync(new DefectDetails());
                    Navigation.RemovePage(this);
                }
            }
        }

        private async Task AfterUpdateMaterialStage(int stageAuditId)
        {
            if (stageAuditId > 0)
            {
                await DisplayAlert("Success", string.Format("{0}, Level {1}, Zone {2}, {3} has been updated",
            ViewModelLocator.siteUpdateStageVM.Material.block,
            ViewModelLocator.siteUpdateStageVM.Material.level,
            ViewModelLocator.siteUpdateStageVM.Material.zone,
            ViewModelLocator.siteUpdateStageVM.Material.markingNo), "Done");

                /*
                ViewModelLocator.siteScanRFIDVM.RemoveTrackerByMaterial(ViewModelLocator.siteUpdateStageVM.Material);
                ViewModelLocator.siteScanRFIDVM.CountUpdated++;
                */

                ViewModelLocator.siteUpdateStageVM.Reset();

                await Navigation.PopAsync();
                //await Navigation.PopToRootAsync();
                //await Navigation.PushAsync(new SiteScanRFID());
            }
        }

        private async Task<int> UpdateMaterialStage()
        {
            int stageAuditId = 0;
            var result = await ApiClient.Instance.MTUpdateMaterialStage(
            ViewModelLocator.siteUpdateStageVM.Material.id,
            (ViewModelLocator.siteUpdateStageVM.TogglePass.Status || !ViewModelLocator.siteUpdateStageVM.NextStageQC),
            ViewModelLocator.siteUpdateStageVM.QCRemarks,
            ViewModelLocator.siteUpdateStageVM.SelectedLocation.Id);

            if (result.status != 0)
                ViewModelLocator.siteUpdateStageVM.ErrorMessage = result.message;
            else
            {
                var stageAudit = JToken.Parse(result.data.ToString());
                stageAuditId = stageAudit.Value<int>("id");
                ViewModelLocator.qcDefectVM.StageAuditID = stageAuditId;
            }

            ViewModelLocator.siteUpdateStageVM.IsLoading = false;

            return stageAuditId;
        }

        private async Task<int> UpdateMaterialLocation()
        {
            int stageAuditId = 0;
            var result = await ApiClient.Instance.MTUpdateMaterialLocation(
            ViewModelLocator.siteUpdateStageVM.Material.id,
            (ViewModelLocator.siteUpdateStageVM.TogglePass.Status || !ViewModelLocator.siteUpdateStageVM.NextStageQC),
            ViewModelLocator.siteUpdateStageVM.QCRemarks,
            ViewModelLocator.siteUpdateStageVM.SelectedLocation.Id);

            if (result.status != 0)
                ViewModelLocator.siteUpdateStageVM.ErrorMessage = result.message;
            else
            {
                var stageAudit = JToken.Parse(result.data.ToString());
                stageAuditId = stageAudit.Value<int>("id");
                ViewModelLocator.qcDefectVM.StageAuditID = stageAuditId;
            }

            ViewModelLocator.siteUpdateStageVM.IsLoading = false;

            return stageAuditId;
        }


        private void OnCapturePhotoClicked(object sender, EventArgs e)
        {
            Task.Run(ViewModelLocator.takePhotoVM.CapturePhoto).ContinueWith((t) =>
            {
                Navigation.PushAsync(new SiteCapturePhoto());
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnViewPhotoClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ViewQCPhoto());
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (ViewModelLocator.siteUpdateStageVM.Material != null)
            {
                ViewModelLocator.siteUpdateStageVM.IsLoading = true;

                Task.Run(GetNextStage).ContinueWith(async (t) =>
                {
                    if (App.Current.Properties.ContainsKey("user_id"))
                    {
                        int userId = (int)App.Current.Properties["user_id"];
                        var result = await ApiClient.Instance.MTGetLocations(userId);
                        ViewModelLocator.siteUpdateStageVM.Locations = new ObservableCollection<Location>(
                            result.data as List<Location>);
                        if (ViewModelLocator.siteUpdateStageVM.Material.NextStageIsQCOrInstalledStage && ViewModelLocator.siteUpdateStageVM.Locations != null)
                        {
                            ViewModelLocator.siteUpdateStageVM.SelectedLocation =
                                ViewModelLocator.siteUpdateStageVM.Locations.Where(l => l.Id == ViewModelLocator.siteUpdateStageVM.Material.SelectedLocation.Id).FirstOrDefault();
                        }
                        ViewModelLocator.siteUpdateStageVM.IsLoading = false;
                    }
                });
            }
            else
                ViewModelLocator.siteUpdateStageVM.ErrorMessage = "No material specified";
        }

        private void updateLocation_Clicked(object sender, EventArgs e)
        {
            ViewModelLocator.siteUpdateStageVM.IsLoading = true;
            Task.Run(UpdateMaterialLocation).ContinueWith((task) =>
            AfterUpdateMaterialStage(task.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}