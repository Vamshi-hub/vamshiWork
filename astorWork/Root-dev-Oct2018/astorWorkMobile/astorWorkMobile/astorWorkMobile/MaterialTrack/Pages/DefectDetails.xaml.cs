using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefectDetails : ContentPage
    {
        public DefectDetails()
        {
            InitializeComponent();
        }
        private void OnCapturePhotoClicked(object sender, EventArgs e)
        {
            ViewModelLocator.qcDefectVM.IsLoading = true;
            Task.Run(ViewModelLocator.takePhotoVM.CapturePhoto).ContinueWith((t) =>
            {
                Navigation.PushAsync(new SiteCapturePhoto());
                ViewModelLocator.qcDefectVM.IsLoading = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnViewPhotoClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ViewQCPhoto());
        }

        private void OnRemoveClicked(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null && btn.BindingContext != null)
            {
                var qcPhoto = btn.BindingContext as QCPhoto;
                ViewModelLocator.siteUpdateStageVM.QCPhotos.Remove(qcPhoto);
                ViewModelLocator.siteUpdateStageVM.CountQCPhotos--;
            }
        }

        private void OnSubmitClickedAsync(object sender, EventArgs e)
        {
            ViewModelLocator.qcDefectVM.IsLoading = true;
            ViewModelLocator.siteListDefectsVM.StageAuditId = ViewModelLocator.qcDefectVM.StageAuditID;

            Task.Run(CreateUpdateQCDefect).ContinueWith(async (t) =>
            {
                if (t.Result)
                {
                    if (ViewModelLocator.qcDefectVM.QCOpenCase.ID == 0)
                    {
                        if(await DisplayAlert("Success", "A new QC case has been created", "View Open QC", "Done"))
                            await Navigation.PushAsync(new SiteListDefects());

                        Navigation.RemovePage(this);
                    }
                    else
                    {
                        await Navigation.PopAsync();
                    }
                }
                else
                {
                    ViewModelLocator.qcDefectVM.ErrorMessage = "Fail to update QC defect";
                }
                ViewModelLocator.qcDefectVM.IsLoading = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
            //page to navigate 
        }

        private async void OnCloseClickedAsync(object sender, EventArgs e)
        {
            try
            {
                if (ViewModelLocator.qcDefectVM.QCOpenCase.ID != 0)
                {
                    ViewModelLocator.qcDefectVM.IsLoading = true;
                    ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen = true;
                    var resDefect = await ApiClient.Instance.MTUpdateQcDefect(ViewModelLocator.qcDefectVM.QCDefectDetails.ID, ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks, ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen);
                    var defectData = JToken.Parse(resDefect.data.ToString());

                    foreach (var qcPhoto in ViewModelLocator.qcDefectVM.QCPhotos.Where(P => !string.IsNullOrEmpty(P.ImageBase64)))
                    {
                        await ApiClient.Instance.MTUpdateMaterialQCPhoto(ViewModelLocator.qcDefectVM.QCDefectDetails.ID, qcPhoto.ImageBase64, qcPhoto.Remarks, true);
                    }
                    if (resDefect.message == null)
                    {
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        //ViewModelLocator.siteUpdateStageVM.Material = ViewModelLocator.siteListDefectsVM.Material;
                        await DisplayAlert("Success", resDefect.message, "OK");
                        await Navigation.PushAsync(new SiteUpdateStage());
                        // Removing all pages except the scanning page
                        int maxPageIndex = Navigation.NavigationStack.Count - 1;
                        for (int i = 2; i < maxPageIndex; i++)
                        {
                            var page = Navigation.NavigationStack[2];
                            Navigation.RemovePage(page);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                await DisplayAlert("Error", "Fail to close QC", "OK");
                Debug.WriteLine(exc.Message);
                Debug.Write(exc.StackTrace);
            }
            ViewModelLocator.qcDefectVM.IsLoading = false;
        }

        private async Task<bool> CreateUpdateQCDefect()
        {
            bool success = false;
            try
            {
                int defectId = 0;
                var stageAuditId = ViewModelLocator.qcDefectVM.StageAuditID;
                if (ViewModelLocator.qcDefectVM.QCDefectDetails.ID == 0)
                {
                    var resDefect = await ApiClient.Instance.MTCreateQcDefect(stageAuditId, ViewModelLocator.qcDefectVM.QCOpenCase.ID, ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks);
                    if (resDefect.status == 0 && resDefect.data != null)
                    {
                        var defectData = JToken.Parse(JsonConvert.SerializeObject(resDefect.data));
                        defectId = defectData.Value<int>("id");
                    }
                }
                else
                {
                    ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen = false;
                    var resDefect = await ApiClient.Instance.MTUpdateQcDefect(ViewModelLocator.qcDefectVM.QCDefectDetails.ID, ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks, ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen);
                    defectId = ViewModelLocator.qcDefectVM.QCDefectDetails.ID;
                }

                if (defectId > 0)
                {
                    foreach (var qcPhoto in ViewModelLocator.qcDefectVM.QCPhotos.Where(P => !string.IsNullOrEmpty(P.ImageBase64)))
                    {
                        await ApiClient.Instance.MTUpdateMaterialQCPhoto(defectId, qcPhoto.ImageBase64, qcPhoto.Remarks, false);
                    }

                    success = true;
                }
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.Write(exc.StackTrace);
            }
            return success;
        }
    }
}