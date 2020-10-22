
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class JobQCCase : ContentPage
    {
        public JobQCCase()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var defect = e.Item as QCDefect;
            if (defect != null)
            {
                ViewModelLocator.qcDefectVM.QCOpenCase = ViewModelLocator.siteListDefectsVM.OpenCase;
                ViewModelLocator.qcDefectVM.QCDefectDetails = defect;
                if (ViewModelLocator.qcDefectVM.QCPhotos != null)
                    ViewModelLocator.qcDefectVM.QCPhotos.Clear();
                ViewModelLocator.qcDefectVM.QCPhotos = new System.Collections.ObjectModel.ObservableCollection<QCPhoto>();
                ViewModelLocator.qcDefectVM.QCPhotos.Add(new QCPhoto
                {
                    IsOpen = true,
                    Url = @"https://astorwork.blob.core.windows.net/dev-tenant-repo/sink-damage-1.JPG",
                    CreatedBy = "Susan",
                    CreatedDate = DateTime.Now.AddDays(-5),
                    Remarks = "Sink installed wrongly"
                });
                ViewModelLocator.qcDefectVM.QCPhotos.Add(new QCPhoto
                {
                    IsOpen = false,
                    Url = @"https://astorwork.blob.core.windows.net/dev-tenant-repo/sink-fixed-2.jpg",
                    CreatedBy = "Oscar",
                    CreatedDate = DateTime.Now.AddDays(-3),
                    Remarks = "Sink fixed"
                });
                Navigation.PushAsync(new JobQCDefect());
            }
        }

        private void btnAddDefect_Clicked(object sender, System.EventArgs e)
        {
            //ViewModelLocator..QCOpenCase = ViewModelLocator.siteListDefectsVM.OpenCase;
            //ViewModelLocator.qcDefectVM.QCDefectDetails = new QCDefect
            //{
            //    IsOpen = true,
            //    ID = 0
            //};
            //ViewModelLocator.qcDefectVM.QCPhotos.Clear();
            ViewModelLocator.qcDefectVM.QCOpenCase = ViewModelLocator.siteListDefectsVM.OpenCase;
            ViewModelLocator.qcDefectVM.QCDefectDetails = new QCDefect();
            ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen = true;
            if (ViewModelLocator.qcDefectVM.QCPhotos != null)
                ViewModelLocator.qcDefectVM.QCPhotos.Clear();
            ViewModelLocator.qcDefectVM.QCPhotos = new System.Collections.ObjectModel.ObservableCollection<QCPhoto>();

            Navigation.PushAsync(new JobQCDefect());
        }

        private void ContentPage_Appearing(object sender, System.EventArgs e)
        {
        }

        private void ContentPage_Disappearing(object sender, System.EventArgs e)
        {
            //ViewModelLocator.siteListDefectsVM.Reset();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}