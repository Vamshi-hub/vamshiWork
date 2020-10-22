using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using RoundedBoxView.Forms.Plugin.Droid;
using Xamarin.Forms;
using Plugin.Toasts;
using Plugin.DeviceInfo;
using Android.Net;
using Acr.UserDialogs;

namespace astorTrackP.Droid
{
	[Activity (Label = "astorWork", Icon = "@drawable/logowhite", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
        ChainwayRFIDReader chainwayRFIDReader = new ChainwayRFIDReader();
        ConnectivityManager connectivityManager;

        protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
           

			global::Xamarin.Forms.Forms.Init (this, bundle);
			RoundedBoxViewRenderer.Init ();
			ActionBar.SetIcon (Android.Resource.Color.Transparent);
            UserDialogs.Init(() => (Activity)Forms.Context);
            connectivityManager = (ConnectivityManager)Forms.Context.ApplicationContext.GetSystemService(Context.ConnectivityService);            
            try
			{
				DependencyService.Register<ToastNotificatorImplementation>();
				ToastNotificatorImplementation.Init(this);

				App.rfidReader = chainwayRFIDReader;
                App.deviceInfo.DeviceId = CrossDeviceInfo.Current.Id;    
            }
			catch 
			{
				App.deviceInfo.HasRFIDModule = false;
			}
            LoadApplication (new App ());
        }

        public override bool OnKeyDown (Keycode keyCode, KeyEvent e)
		{
			if (keyCode.ToString() == "139" || keyCode.ToString() == "F9") {
				App app = (App)Xamarin.Forms.Application.Current;
				MessagingCenter.Send<App>(app, "OnKeyDown");
			}
			return base.OnKeyDown (keyCode, e);
		}

        protected override void OnResume()
        {
            base.OnResume();
            //App.InitialiazeReader();
            //if (App.rfidReader == null)
            //{
            //    chainwayRFIDReader = new ChainwayRFIDReader();
            //    App.rfidReader = chainwayRFIDReader;
            //}
            //else
            //    App.rfidReader.CloseReader();
        }

    }
}

