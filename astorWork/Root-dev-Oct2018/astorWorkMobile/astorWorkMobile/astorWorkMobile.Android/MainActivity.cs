using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using astorWorkMobile.Shared.Classes;
using Plugin.CurrentActivity;

namespace astorWorkMobile.Droid
{
    [Activity(Label = "astorWorkMobile", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
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

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Instance = this;

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
            global::ZXing.Net.Mobile.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

