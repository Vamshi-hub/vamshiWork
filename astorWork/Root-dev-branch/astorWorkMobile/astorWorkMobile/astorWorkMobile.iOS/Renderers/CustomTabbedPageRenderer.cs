using astorWorkMobile.iOS.Renderers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TabbedPage), typeof(CustomTabbedPageRenderer))]
namespace astorWorkMobile.iOS.Renderers
{
    [Preserve]
    public class CustomTabbedPageRenderer : TabbedRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var f = Font.SystemFontOfSize(24);
            var normalTextAttr = new UITextAttributes();
            normalTextAttr.Font = UIFont.SystemFontOfSize(17);
            normalTextAttr.TextColor = UIColor.LightTextColor;

            var selectedTextAttr = new UITextAttributes();
            selectedTextAttr.Font = UIFont.SystemFontOfSize(17);
            selectedTextAttr.TextColor = UIColor.White;

            //Normal title Color
            UITabBarItem.Appearance.SetTitleTextAttributes(normalTextAttr, UIControlState.Normal);
            //Selected title Color
            UITabBarItem.Appearance.SetTitleTextAttributes(selectedTextAttr, UIControlState.Selected);
            TabBar.BackgroundColor = UIColor.FromRGB(23, 105, 187);
        }
    }
}