using System;
using System.Collections.Generic;
using System.Linq;
using astorWorkMobile.Shared.Views;
using astorWorkMobile.iOS.Renderers;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using WebKit;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(CustomWebViewRenderer))]
namespace astorWorkMobile.iOS.Renderers
{
    public class CustomWebViewRenderer : ViewRenderer<HybridWebView, WKWebView>
    {
        WKWebView _wkWebView;
        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var config = new WKWebViewConfiguration();
                _wkWebView = new WKWebView(Frame, config);
                SetNativeControl(_wkWebView);
            }
            if (e.NewElement != null)
            {
                Control.LoadRequest(new NSUrlRequest(new NSUrl(Element.Uri)));
            }
        }
    }
}