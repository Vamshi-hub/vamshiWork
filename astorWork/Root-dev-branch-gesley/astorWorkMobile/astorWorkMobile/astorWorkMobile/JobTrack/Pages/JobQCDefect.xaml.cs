using astorWorkMobile.MaterialTrack.Pages;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobQCDefect : ContentPage
    {
        public JobQCDefect()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        private void OnCapturePhotoClicked(object sender, EventArgs e)
        {
            ViewModelLocator.qcDefectVM.IsLoading = true;
            Task.Run(ViewModelLocator.mtTakePhotoVM.CapturePhoto).ContinueWith((t) =>
            {
                Navigation.PushAsync(new CapturePhoto());
                ViewModelLocator.qcDefectVM.IsLoading = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void OnViewPhotoClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ViewQCPhoto());
        }

        private void OnSubmitClickedAsync(object sender, EventArgs e)
        {
            ViewModelLocator.qcDefectVM.IsLoading = true;

            Task.Run(CreateUpdateQCDefect).ContinueWith(async (t) =>
            {
                if (t.Result)
                {
                    if (ViewModelLocator.qcDefectVM.QCOpenCase.ID == 0)
                    {
                        if (await DisplayAlert("Success", "A new QC case has been created", "View Open QC", "Done"))
                        {
                            await Navigation.PushAsync(new ListDefects());
                        }
                        //ViewModelLocator.qcDefectVM.DisplaySnackBar("A new QC case has been created", Enums.Actions.PushAsync, new ListDefects(), null, Color.Green);
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
            bool success = false;
            try
            {
                if (ViewModelLocator.qcDefectVM.QCOpenCase.ID != 0)
                {
                    ViewModelLocator.qcDefectVM.IsLoading = true;
                    ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen = false;
                    ViewModelLocator.qcDefectVM.QCDefectDetails.StatusCode = 2;

                    //var apiResponse = await ApiClient.Instance.MTUpdateQcDefect(ViewModelLocator.qcDefectVM.QCDefectDetails.ID, ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks, ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen);

                    //if ()
                    //{
                    success = true;
                    if (ViewModelLocator.qcDefectVM.QCPhotos != null && ViewModelLocator.qcDefectVM.QCPhotos.Count > 0)
                    {
                        foreach (var qcPhoto in ViewModelLocator.qcDefectVM.QCPhotos
                            .Where(P => !string.IsNullOrEmpty(P.ImageBase64)))
                        {
                            qcPhoto.IsOpen = false;
                            success = true;
                            //var photoResult = await ApiClient.Instance.MTUpdateMaterialQCPhoto(ViewModelLocator.qcDefectVM.QCDefectDetails.ID, qcPhoto.ImageBase64, qcPhoto.Remarks, true);
                            //if (photoResult.status != 0)
                            //{
                            //    success = false;
                            //    break;
                            //}
                        }
                    }

                    if (success)
                    {
                        int defectId = ViewModelLocator.qcDefectVM.QCDefectDetails.ID;
                        if (defectId > 0)
                        {
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            // await DisplayAlert("Success", "QC case is closed", "OK");
                            ViewModelLocator.qcDefectVM.DisplaySnackBar("QC case is closed", Enums.PageActions.None, Enums.MessageActions.Success, new ListDefects(), null);
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
                    {
                        ViewModelLocator.qcDefectVM.ErrorMessage = "Fail to close the defect";
                    }
                }
            }
            catch (Exception exc)
            {
                ViewModelLocator.qcDefectVM.ErrorMessage = "Fail to close QC";
                Debug.WriteLine(exc.Message);
                Debug.Write(exc.StackTrace);
            }
        }

        private async Task<bool> CreateUpdateQCDefect()
        {
            bool success = true;
            try
            {
                int defectId = 0;
                //int materialId = ViewModelLocator.qcDefectVM.Job.id;
                //ApiClient.ApiResult result = null;

                if (ViewModelLocator.qcDefectVM.QCDefectDetails.ID == 0)
                {
                    Random rnd = new Random();
                    defectId = 5 * rnd.Next(52);
                    ViewModelLocator.siteListDefectsVM.OpenCase.ListQCDefect.Add(new MaterialTrack.Entities.QCDefect
                    {
                        ID = defectId,
                        IsOpen = true,
                        CreatedBy = "Susan",
                        CreatedDate = DateTime.Now,
                        CountPhotos = 1,
                        StatusCode = 0,
                        Remarks = ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks,
                        IsDummyDefects = true
                    });
                    //result = await ApiClient.Instance.MTCreateQcDefect(materialId, ViewModelLocator.qcDefectVM.QCOpenCase == null? 0: ViewModelLocator.qcDefectVM.QCOpenCase.ID, ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks);
                }
                else
                {
                    defectId = ViewModelLocator.qcDefectVM.QCDefectDetails.ID;
                    ViewModelLocator.qcDefectVM.QCDefectDetails.UpdatedBy = ViewModelLocator.qcDefectVM.QCDefectDetails.CreatedBy;
                    ViewModelLocator.qcDefectVM.QCDefectDetails.UpdatedDate = ViewModelLocator.qcDefectVM.QCDefectDetails.CreatedDate;
                    //result = await ApiClient.Instance.MTUpdateQcDefect(ViewModelLocator.qcDefectVM.QCDefectDetails.ID, ViewModelLocator.qcDefectVM.QCDefectDetails.Remarks, !ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen);
                }

                //if (result.status == 0 && result.data != null)
                //{
                //    defectId = (int)result.data;
                //}

                //if (defectId > 0)
                //{
                //    success = true;
                //    foreach (var qcPhoto in ViewModelLocator.qcDefectVM.QCPhotos.Where(P => !string.IsNullOrEmpty(P.ImageBase64)))
                //    {
                //        var photoResult = await ApiClient.Instance.MTUpdateMaterialQCPhoto(defectId, qcPhoto.ImageBase64, qcPhoto.Remarks, false);
                //        if (photoResult.status != 0)
                //        {
                //            success = false;
                //            break;
                //        }
                //    }
                //}
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.Write(exc.StackTrace);
            }
            return success;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}