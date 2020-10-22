using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static astorWorkMobile.Shared.Classes.Enums;
using static astorWorkMobile.Shared.Utilities.ApiClient;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DefectDetails : ContentPage
    {
        public DefectDetails()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
            //remarksEditor.AutoSize = EditorAutoSizeOption.TextChanges;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (ViewModelLocator.qcDefectVM.Material.countQCCase == 0)
            {
                await Task.Run(async () =>
             {
                 await ViewModelLocator.siteListDefectsVM.GetOrganisations();
                 //Device.BeginInvokeOnMainThread(() => { ViewModelLocator.qcDefectVM.OnPropertyChanged("ListSubcons"); ViewModelLocator.qcDefectVM.OnPropertyChanged("SelectedSubcon"); });
             });
            }
        }
        private async void OnCapturePhotoClicked(object sender, EventArgs e)
        {
            ViewModelLocator.qcDefectVM.IsLoading = true;
            await Task.Run(ViewModelLocator.mtTakePhotoVM.CapturePhoto).ContinueWith(async (t) =>
             {
                 await Navigation.PushAsync(new CapturePhoto());
                 ViewModelLocator.qcDefectVM.IsLoading = false;
             }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async void OnViewPhotoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewQCPhoto());
        }

        private void OnSubmitClickedAsync(object sender, EventArgs e)
        {
            if (((Button)sender).Text.ToLower() == "create defect" && pickerOrganisation.SelectedIndex == -1)
            {
                ViewModelLocator.qcDefectVM.DisplaySnackBar("Please select an organisation", PageActions.None, MessageActions.Warning, null, null);
            }
            else
            {
                ViewModelLocator.qcDefectVM.IsLoading = true;

                Task.Run(CreateUpdateQCDefect).ContinueWith(async (t) =>
                {
                    if (t.Result)
                    {
                        if (ViewModelLocator.qcDefectVM.QCOpenCase.ID == 0)
                        {
                            if (await DisplayAlert("Success", "A new QC case has been created", "View Open QC", "Done"))
                                await Navigation.PushAsync(new ListDefects());
                            else
                            {
                                await Navigation.PopAsync();
                            }
                        }
                        else
                        {
                            ViewModelLocator.qcDefectVM.DisplaySnackBar("QC case is created/updated successfully", PageActions.PopAsync, MessageActions.Success, null, null);
                            //await Navigation.PopAsync();
                        }
                    }
                    else
                        ViewModelLocator.qcDefectVM.ErrorMessage = "Failed to create/update QC defect";

                    ViewModelLocator.qcDefectVM.IsLoading = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
                //page to navigate 
            }
        }

        private async void OnCloseClickedAsync(object sender, EventArgs e)
        {
            bool success = false;
            try
            {
                ViewModelLocator.qcDefectVM.QCDefectDetails.StatusCode = (int)QCStatus.QC_passed_by_Maincon;
                if (ViewModelLocator.qcDefectVM.QCOpenCase.ID != 0)
                {
                    ViewModelLocator.qcDefectVM.IsLoading = true;
                    ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen = true;

                    ApiResult apiResponse = await ApiClient.Instance.MTUpdateQcDefect(ViewModelLocator.qcDefectVM.QCDefectDetails.ID, ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks,
                                            ViewModelLocator.qcDefectVM.QCDefectDetails.SelectedSubconID, ViewModelLocator.qcDefectVM.QCDefectDetails.StatusCode, ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen);

                    if (apiResponse.status == 0)
                    {
                        success = true;
                        foreach (QCPhoto qcPhoto in ViewModelLocator.qcDefectVM.QCPhotos
                                 .Where(P => !string.IsNullOrEmpty(P.ImageBase64)))
                        {
                            ApiResult photoResult = await ApiClient.Instance.MTUpdateMaterialQCPhoto(ViewModelLocator.qcDefectVM.QCDefectDetails.ID,
                                                                                                     qcPhoto.ImageBase64, qcPhoto.Remarks, true);
                            if (photoResult.status != 0)
                            {
                                success = false;
                                break;
                            }
                        }
                    }

                    if (success)
                    {
                        int defectId = (int)apiResponse.data;
                        if (defectId > 0)
                        {
                            ViewModelLocator.qcDefectVM.DisplaySnackBar("Defect closed successfully", PageActions.PopAsync, MessageActions.Success, null, null);
                            // await Navigation.PopAsync();
                        }

                        else
                        {
                            // await DisplayAlert("Success", "QC case is closed", "OK");
                            ViewModelLocator.qcDefectVM.DisplaySnackBar("QC case is closed", Shared.Classes.Enums.PageActions.None,
                                                                         Shared.Classes.Enums.MessageActions.Success, null, null);
                            // await Navigation.PushAsync(new UpdateMaterial());
                            // Removing all pages except the scanning page
                            int maxPageIndex = Navigation.NavigationStack.Count;
                            for (int i = 2; i < maxPageIndex; i++)
                            {
                                var page = Navigation.NavigationStack[2];
                                Navigation.RemovePage(page);
                            }
                        }
                    }
                    else
                        ViewModelLocator.qcDefectVM.ErrorMessage = "Failed to close the defect";
                }
            }
            catch (Exception exc)
            {
                ViewModelLocator.qcDefectVM.ErrorMessage = "Failed to close QC";
                Debug.WriteLine(exc.Message);
                Debug.Write(exc.StackTrace);
            }
        }

        private async Task<bool> CreateUpdateQCDefect()
        {
            bool success = false;
            try
            {
                int defectId = 0;
                ViewModelLocator.qcDefectVM.QCDefectDetails.StatusCode = (int)QCStatus.QC_failed_by_Maincon;
                int materialId = ViewModelLocator.qcDefectVM.Material.id;
                ApiClient.ApiResult result = null;

                if (ViewModelLocator.qcDefectVM.QCDefectDetails.ID == 0)
                    result = await ApiClient.Instance.MTCreateQcDefect(materialId, ViewModelLocator.qcDefectVM.QCOpenCase == null ? 0 : ViewModelLocator.qcDefectVM.QCOpenCase.ID,
                             ViewModelLocator.qcDefectVM.SelectedSubcon.ID, ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks);
                else
                    result = await ApiClient.Instance.MTUpdateQcDefect(ViewModelLocator.qcDefectVM.QCDefectDetails.ID, ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks,
                             ViewModelLocator.qcDefectVM.QCDefectDetails.SelectedSubconID, ViewModelLocator.qcDefectVM.QCDefectDetails.StatusCode, !ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen);

                if (result.status == 0 && result.data != null)
                    defectId = (int)result.data;

                if (defectId > 0)
                {
                    success = true;
                    foreach (var qcPhoto in ViewModelLocator.qcDefectVM.QCPhotos.Where(P => !string.IsNullOrEmpty(P.ImageBase64)))
                    {
                        var photoResult = await ApiClient.Instance.MTUpdateMaterialQCPhoto(defectId, qcPhoto.ImageBase64, qcPhoto.Remarks, false);
                        if (photoResult.status != 0)
                        {
                            success = false;
                            break;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                ViewModelLocator.qcDefectVM.DisplaySnackBar(exc.Message, PageActions.PopAsync, MessageActions.Error, null, null);
                Debug.WriteLine(exc.Message);
                Debug.Write(exc.StackTrace);
            }

            return success;
        }

        private async void RectifyDefect_Clicked(object sender, EventArgs e)
        {
            string strMessage = "Defect rectified successfully. ";
            bool success = false;
            MessageActions msgActions = MessageActions.Success;
            try
            {
                ViewModelLocator.qcDefectVM.IsLoading = true;
                int defectId = 0;
                ViewModelLocator.qcDefectVM.QCDefectDetails.StatusCode = (int)QCStatus.QC_rectified_by_Subcon;
                ApiClient.ApiResult result = null;
                if (ViewModelLocator.qcDefectVM.QCDefectDetails.ID != 0)
                    result = await ApiClient.Instance.MTUpdateQcDefect(ViewModelLocator.qcDefectVM.QCDefectDetails.ID, ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks,
                            ViewModelLocator.qcDefectVM.QCDefectDetails.SelectedSubconID, ViewModelLocator.qcDefectVM.QCDefectDetails.StatusCode, !ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen);

                if (result.status == 0 && result.data != null)
                    defectId = (int)result.data;

                if (defectId > 0)
                {
                    success = true;
                    foreach (var qcPhoto in ViewModelLocator.qcDefectVM.QCPhotos.Where(P => !string.IsNullOrEmpty(P.ImageBase64)))
                    {
                        var photoResult = await ApiClient.Instance.MTUpdateMaterialQCPhoto(defectId, qcPhoto.ImageBase64, qcPhoto.Remarks, false);
                        if (photoResult.status != 0)
                        {
                            strMessage += "Failed to upload some photos";
                            msgActions = MessageActions.Warning;
                            success = false;
                            // break;
                        }
                    }
                    ViewModelLocator.qcDefectVM.DisplaySnackBar(strMessage, PageActions.PopAsync, msgActions, null, null);
                }
                else
                {
                    strMessage = "Failed to rectify defect";
                    ViewModelLocator.qcDefectVM.DisplaySnackBar(strMessage, PageActions.PopAsync, MessageActions.Error, null, null);
                }

            }
            catch (Exception exc)
            {
                ViewModelLocator.qcDefectVM.DisplaySnackBar(exc.Message, PageActions.PopAsync, MessageActions.Error, null, null);
                Debug.WriteLine(exc.Message);
                Debug.Write(exc.StackTrace);
            }

        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}