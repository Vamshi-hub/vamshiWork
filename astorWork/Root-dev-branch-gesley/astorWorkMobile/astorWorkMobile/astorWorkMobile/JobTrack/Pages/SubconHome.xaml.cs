using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SubconHome : TabbedPage
    {
        bool IsScanBtnsVisible;
        public SubconHome()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
            Task.Run(ViewModelLocator.subconHomeVM.GetProjects);
        }

        private void TabbedPage_Appearing(object sender, EventArgs e)
        {
            //Task.Run(ViewModelLocator.subconHomeVM.GetProjects);
            //ViewModelLocator.subconHomeVM.SelectedJobSummaryProj = null;
            //ViewModelLocator.subconHomeVM.SelectedQCNotifyProj = null;
        }

        private void TabbedPage_Disappearing(object sender, EventArgs e)
        {
            //ViewModelLocator.subconHomeVM.SelectedJobSummaryProj = null;
            //ViewModelLocator.subconHomeVM.SelectedQCNotifyProj = null;
        }

        private void QCNotifications_Appearing(object sender, EventArgs e)
        {
            ViewModelLocator.pageTypeCode.PageTypeCode = (int)Enums.Pages.QC_Notifications;
            Task.Run(ViewModelLocator.subconHomeVM.GetNotifications);
            btnQRScan_QCNotify.IsVisible = false;
            //btnRFIDScan_QCNotify.IsVisible = false;
            IsScanBtnsVisible = false;
        }

        private void JobSummary_Appearing(object sender, EventArgs e)
        {
            ViewModelLocator.pageTypeCode.PageTypeCode = (int)Enums.Pages.Job_Summary;
            Task.Run(ViewModelLocator.subconHomeVM.GetNotifications);
            btnQRScan_JobSummary.IsVisible = false;
            // btnRFIDScan_JobSummary.IsVisible = false;
            IsScanBtnsVisible = false;
        }

        private async void BtnScan_JobSummary_Clicked(object sender, EventArgs e)
        {
            if (IsScanBtnsVisible)
            {
                btnQRScan_JobSummary.IsVisible = false;
                //  btnRFIDScan_JobSummary.IsVisible = false;
                IsScanBtnsVisible = false;
            }
            else
            {
                //  btnRFIDScan_JobSummary.IsVisible = true;
                //  await btnRFIDScan_JobSummary.ScaleTo(1.25, 100);
                //   await btnRFIDScan_JobSummary.ScaleTo(1, 100);

                btnQRScan_JobSummary.IsVisible = true;
                await btnQRScan_JobSummary.ScaleTo(1.25, 100);
                await btnQRScan_JobSummary.ScaleTo(1, 100);

                IsScanBtnsVisible = true;
            }
        }

        private async void BtnScan_QCNotify_Clicked(object sender, EventArgs e)
        {
            if (IsScanBtnsVisible)
            {
                btnQRScan_QCNotify.IsVisible = false;
                // btnRFIDScan_QCNotify.IsVisible = false;
                IsScanBtnsVisible = false;
            }
            else
            {
                //    btnRFIDScan_QCNotify.IsVisible = true;
                // await btnRFIDScan_QCNotify.ScaleTo(1.25, 100);
                //  await btnRFIDScan_QCNotify.ScaleTo(1, 100);

                btnQRScan_QCNotify.IsVisible = true;
                await btnQRScan_QCNotify.ScaleTo(1.25, 100);
                await btnQRScan_QCNotify.ScaleTo(1, 100);

                IsScanBtnsVisible = true;
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}