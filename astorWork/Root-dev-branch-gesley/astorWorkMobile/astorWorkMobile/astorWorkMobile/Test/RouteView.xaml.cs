using astorWorkMobile.MaterialTrack.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.Test
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RouteView : ContentView
	{
		public RouteView ()
		{
			InitializeComponent ();
        }

        void OnVendorButtonClicked(object sender, EventArgs e)
        {
            Xamarin.Forms.Application.Current.Properties["c72_power"] = 10;
            Xamarin.Forms.Application.Current.Properties["user_id"] = 2;
            Xamarin.Forms.Application.Current.Properties["role_id"] = 4;
            Navigation.PushAsync(new VendorHome());
        }

        void OnSiteButtonClicked(object sender, EventArgs e)
        {
            Xamarin.Forms.Application.Current.Properties["c72_power"] = 20;
            Xamarin.Forms.Application.Current.Properties["user_id"] = 5;
            Xamarin.Forms.Application.Current.Properties["role_id"] = 2;
            Xamarin.Forms.Application.Current.Properties["scan_timeout_seconds"] = 10;
            Navigation.PushAsync(new ScanRFID());// add the scanner not found condition in case scanner is not attached to the device before navigating to ScanRFID page.
        }
    }
}