using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using astorWorkMobile.iOS.Renderers;

[assembly: ResolutionGroupName("astorWork")]
[assembly: ExportEffect(typeof(SafeAreaPaddingEffect), nameof(SafeAreaPaddingEffect))]
namespace astorWorkMobile.iOS.Renderers
{
    public class SafeAreaPaddingEffect : PlatformEffect
    {
        Thickness _padding;
        protected override void OnAttached()
        {
            if (Element is Layout element)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
                {
                    _padding = element.Padding;
                    var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
                    if (insets.Top >= 44) // We have a notch (iPhone)
                    {
                        element.Padding = new Thickness(_padding.Left + insets.Left, _padding.Top + insets.Top - 11, _padding.Right + insets.Right, _padding.Bottom);
                    }
                    else if (insets.Top >= 20) // We have a notch (iPad)
                    {
                        element.Padding = new Thickness(_padding.Left + insets.Left, _padding.Top + insets.Top - 5, _padding.Right + insets.Right, _padding.Bottom);
                    }
                    else if (insets.Top <= 0)
                    {
                        element.Padding = new Thickness(_padding.Left, _padding.Top + 9, _padding.Right, _padding.Bottom);
                    }
                }
                else
                {
                    element.Padding = new Thickness(_padding.Left, _padding.Top + 19, _padding.Right, _padding.Bottom);
                }
                
            }
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Padding = _padding;
            }
        }
    }
}