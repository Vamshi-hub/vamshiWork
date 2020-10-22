using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MTRTOHome : TabbedPage
    {
        bool IsScanBtnsVisible;
        public MTRTOHome()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
            ViewModelLocator.mtRTOHomeVM.IsQCCompleted = false;
            Task.Run(async () =>
            {
                await ViewModelLocator.mtRTOHomeVM.GetProjects();
            });
        }

        //private void QCCompleted_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ViewModelLocator.mtRTOHomeVM.IsQCCompleted = true;
        //}

        //private void QCPending_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ViewModelLocator.mtRTOHomeVM.IsQCCompleted = false;
        //}

        private void TabbedPage_Disappearing(object sender, EventArgs e)
        {
            ViewModelLocator.mtRTOHomeVM.IsQCCompleted = false;
            ViewModelLocator.mtRTOHomeVM.SelectedProjQCPending = null;
            ViewModelLocator.mtRTOHomeVM.SelectedProjQCCompltd = null;
        }

        private async void BtnScan_QCComp_Clicked(object sender, EventArgs e)
        {
            if (IsScanBtnsVisible)
            {
                btnQRScan_QCComp.IsVisible = false;
                //  btnRFIDScan_QCComp.IsVisible = false;
                IsScanBtnsVisible = false;
            }
            else
            {
                //   btnRFIDScan_QCComp.IsVisible = true;
                //   await btnRFIDScan_QCComp.ScaleTo(1.25, 100);
                //  await btnRFIDScan_QCComp.ScaleTo(1, 100);

                btnQRScan_QCComp.IsVisible = true;
                await btnQRScan_QCComp.ScaleTo(1.25, 100);
                await btnQRScan_QCComp.ScaleTo(1, 100);

                IsScanBtnsVisible = true;
            }
        }

        private async void BtnScan_QCPend_Clicked(object sender, EventArgs e)
        {
            if (IsScanBtnsVisible)
            {
                btnQRScan_QCPend.IsVisible = false;
                //  btnRFIDScan_QCPend.IsVisible = false;
                IsScanBtnsVisible = false;
            }
            else
            {
                //     btnRFIDScan_QCPend.IsVisible = true;
                //     await btnRFIDScan_QCPend.ScaleTo(1.25, 100);
                //     await btnRFIDScan_QCPend.ScaleTo(1, 100);

                btnQRScan_QCPend.IsVisible = true;
                await btnQRScan_QCPend.ScaleTo(1.25, 100);
                await btnQRScan_QCPend.ScaleTo(1, 100);

                IsScanBtnsVisible = true;
            }
        }

        private void QC_CompledtedPage_Appearing(object sender, EventArgs e)
        {
            ViewModelLocator.mtRTOHomeVM.IsQCCompleted = true;
            Task.Run(ViewModelLocator.mtRTOHomeVM.GetRTOMaterials);
            IsScanBtnsVisible = false;
            btnQRScan_QCComp.IsVisible = false;
            //   btnRFIDScan_QCComp.IsVisible = false;
        }
        private void QC_PendingPage_Appearing(object sender, EventArgs e)
        {
            ViewModelLocator.mtRTOHomeVM.IsQCCompleted = false;
            Task.Run(ViewModelLocator.mtRTOHomeVM.GetRTOMaterials);
            IsScanBtnsVisible = false;
            btnQRScan_QCPend.IsVisible = false;
            //    btnRFIDScan_QCPend.IsVisible = false;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}