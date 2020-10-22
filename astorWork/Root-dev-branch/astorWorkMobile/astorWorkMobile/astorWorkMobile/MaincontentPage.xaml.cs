using astorWorkMobile.JobTrack.Pages;
using astorWorkMobile.Shared.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainContentPage : MasterDetailPage
    {
        public MainContentPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetBackButtonTitle(this, "Back");
            ViewModelLocator.mainContentPageVM.MasterDetailPage = this;
            ViewModelLocator.mainContentPageVM.PropertyChanged += MainContentPageVM_PropertyChanged;

            //Detail = new NavigationPage(new DummyMainPage());
            switch (Application.Current.Properties["entry_point"])
            {
                case 0:
                    Detail = new NavigationPage(new DummyMainPage()); //new VendorHome());
                    break;
                case 1:
                    Detail = new NavigationPage(new DummyMainPage()); //new MaterialTrack.Pages.MainconHome());
                    break;
                case 2:
                    Detail = new NavigationPage(new DummyMainPage()); // Subcon login
                    break;
                case 3:
                    Detail = new NavigationPage(new DummyMainPage());// RTOHome()
                    break;
                case 4:
                    Detail = new NavigationPage(new DummyMainPage());// MAinCon-QC()
                    break;
                case 5:
                    Detail = new NavigationPage(new DummyMainPage());// MAinCon-QC()
                    break;
                default:
                    break;
            }
            //(Detail as NavigationPage).BarBackgroundColor = Color.FromHex("#373b99");
            var xx = Navigation.NavigationStack;
        }

        private void MainContentPageVM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Navigated")
            {
                IsPresented = false;
            }
            //var xx = Navigation.NavigationStack;
            //if (e.PropertyName == "Navigated")
            //{
            //    Detail = ViewModelLocator.mainContentPageVM.DetailPage;
            //    IsPresented = false;
            //}
        }
        //protected override bool OnBackButtonPressed()
        //{
        //    if (Device.RuntimePlatform == Device.Android)
        //    {
        //        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        //    }
        //    return base.OnBackButtonPressed();

        //}

    }
}