using System;

using Android.App;
using Android.Content.PM;
using Android.Views;
using Android.OS;
using astorWorkMobile.Shared.Classes;
using Plugin.CurrentActivity;
using FFImageLoading.Forms.Platform;
using Lottie.Forms.Droid;

namespace astorWorkMobile.Droid
{
    [Activity(Label = "astorWork", Icon = "@drawable/icon",MainLauncher =true, Theme = "@style/MainTheme.Base", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            ZXing.Net.Mobile.Forms.Android.Platform.Init();
            CrossCurrentActivity.Current.Init(this, bundle);
            CachedImageRenderer.Init(true);
            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");
            Xamarin.Forms.Forms.Init(this, bundle);
            AnimationViewRenderer.Init();
            Instance = this;

            var width = Resources.DisplayMetrics.WidthPixels;
            var height = Resources.DisplayMetrics.HeightPixels;
            var density = Resources.DisplayMetrics.Density;

            // Set this to Portrait
            this.RequestedOrientation = ScreenOrientation.Portrait;

            App.ScreenWidth = (width - 0.5f) / density;
            App.ScreenHeight = (height - 0.5f) / density;
            LoadApplication(new App());
        }

        public EventHandler scanKeyHandler;
        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            scanKeyHandler?.Invoke(this, new AndroidKeyEventArgs
            {
                Key = e.KeyCode.ToString(),
                KeyCode = e.KeyCode.GetHashCode(),
                RepeatCount = e.RepeatCount
            });

            return base.OnKeyDown(keyCode, e);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

