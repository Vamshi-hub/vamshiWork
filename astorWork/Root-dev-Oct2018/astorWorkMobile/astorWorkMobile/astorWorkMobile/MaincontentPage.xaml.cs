using astorWorkMobile.MaterialTrack.Pages;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainContentPage : MasterDetailPage
    {
        public MainContentPage(int MobileEntryPoint)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            if (MobileEntryPoint == 0)
            {
                Application.Current.Properties["c72_power"] = 10;
                Detail = new NavigationPage(new VendorInventory());
            }
            else
            {
                Application.Current.Properties["c72_power"] = 20;
                Application.Current.Properties["scan_timeout_seconds"] = 10;
                Detail = new NavigationPage(new SiteHome());
            }
            (Detail as NavigationPage).BarBackgroundColor = Color.FromHex("#373b99");
        }
    }
}