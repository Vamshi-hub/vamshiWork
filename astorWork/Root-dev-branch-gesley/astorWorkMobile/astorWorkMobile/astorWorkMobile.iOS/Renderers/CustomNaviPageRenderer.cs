using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using astorWorkMobile.iOS.Renderers;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNaviPageRenderer))]
namespace astorWorkMobile.iOS.Renderers
{
    public class CustomNaviPageRenderer : NavigationRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            var iOSVersion = UIDevice.CurrentDevice.SystemVersion.Split('.');
            if (iOSVersion != null && iOSVersion.Length > 0 && int.Parse(iOSVersion[0]) >= 13)
            {
                OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
            }
        }
    }
}