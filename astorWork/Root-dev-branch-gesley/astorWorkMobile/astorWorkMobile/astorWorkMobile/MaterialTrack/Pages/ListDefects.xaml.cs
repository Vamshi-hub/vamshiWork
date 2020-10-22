
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListDefects : ContentPage
    {
        public ListDefects()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            QCDefect defect = e.Item as QCDefect;

            if (defect != null)
                ViewModelLocator.siteListDefectsVM.InitialiseDefectPage(defect);
        }

        private void btnAddDefect_Clicked(object sender, System.EventArgs e)
        {
            ViewModelLocator.siteListDefectsVM.InitialiseDefectPage(null);
        }

        private async void ContentPage_Appearing(object sender, System.EventArgs e)
        {

            ViewModelLocator.siteListDefectsVM.IsLoading = true;
            await Task.Run(ViewModelLocator.siteListDefectsVM.GetQCCaseDetails).ContinueWith(t =>
            {
                ViewModelLocator.siteListDefectsVM.IsLoading = false;
            });
        }

        private void ContentPage_Disappearing(object sender, System.EventArgs e)
        {
            //ViewModelLocator.siteListDefectsVM.Reset();
        }

        private async void ToolbarItem_Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}