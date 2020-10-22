using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainContentPageMaster : ContentPage
    {
        public ListView ListView;

        public MainContentPageMaster()
        {
            InitializeComponent();
        }

        private void LogoutButton_Clicked(object sender, EventArgs e)
        {
            ViewModelLocator.loginVM.Reset();
            ViewModelLocator.vendorInventoryVM.Reset();
            Application.Current.Properties.Clear();
            Application.Current.SavePropertiesAsync();
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }
    }
}