
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SiteListDefects : ContentPage
    {
        public SiteListDefects()
        {
            InitializeComponent();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var defect = e.Item as QCDefect;
            if (defect != null)
            {
                ViewModelLocator.qcDefectVM.QCOpenCase = ViewModelLocator.siteListDefectsVM.OpenCase;
                ViewModelLocator.qcDefectVM.QCDefectDetails = defect;
                ViewModelLocator.qcDefectVM.QCPhotos.Clear();
                Navigation.PushAsync(new DefectDetails());
            }
        }
        
        private void btnAddDefect_Clicked(object sender, System.EventArgs e)
        {
            ViewModelLocator.qcDefectVM.QCOpenCase = ViewModelLocator.siteListDefectsVM.OpenCase;
            ViewModelLocator.qcDefectVM.QCDefectDetails = new QCDefect();
            ViewModelLocator.qcDefectVM.QCDefectDetails.IsOpen = true;
            ViewModelLocator.qcDefectVM.QCPhotos.Clear();
            Navigation.PushAsync(new DefectDetails());
        }

        private void ContentPage_Appearing(object sender, System.EventArgs e)
        {
            ViewModelLocator.siteListDefectsVM.IsLoading = true;
            Task.Run(ViewModelLocator.siteListDefectsVM.GetQCCaseDetails).ContinueWith(t =>
            {
                ViewModelLocator.siteListDefectsVM.IsLoading = false;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ContentPage_Disappearing(object sender, System.EventArgs e)
        {
            ViewModelLocator.siteListDefectsVM.Reset();
        }
    }
}