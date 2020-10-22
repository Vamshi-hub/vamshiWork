using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using astorWorkMobile.Droid;
using astorWorkMobile.Shared.Views;
using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace astorWorkMobile.Droid
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {
        Context _context;
        JavascriptResult _jsResult;
        ExtendedWebClient _webViewClient;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
            _jsResult = new JavascriptResult((r) =>
            {
                if (!string.IsNullOrEmpty(Element.Uri))
                    Control.LoadUrl(Element.Uri);
            });

            _webViewClient = new ExtendedWebClient(() =>
            {
                if(Element != null)
                    InjectJS(Element.JavaScript);
            });
        }

        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var webView = new Android.Webkit.WebView(_context);
                webView.Settings.JavaScriptEnabled = true;
                webView.Settings.AllowFileAccessFromFileURLs = true;
                webView.Settings.AllowUniversalAccessFromFileURLs = true;
                webView.Settings.AllowContentAccess = true;
                webView.Settings.AllowFileAccess = true;
                webView.Settings.DomStorageEnabled = true;
                //webView.Settings.DatabaseEnabled = true;
                webView.LayoutParameters = new Android.Widget.RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);

                webView.SetWebChromeClient(_webViewClient);
                SetNativeControl(webView);

                Console.WriteLine($"Hardware acceleration? {webView.IsHardwareAccelerated}");
            }

            if (e.OldElement != null)
            {
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }

            if (e.NewElement != null)
            {
                // Load JavaScript first, then navigate
                // if (!string.IsNullOrEmpty(e.NewElement.JavaScript))
                //   InjectJS(e.NewElement.JavaScript);
                Control.LoadUrl(Element.Uri);
            }
        }

        protected override void OnVisibilityChanged(Android.Views.View changedView, [GeneratedEnum] ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);
        }

        void InjectJS(string script)
        {
            if (!string.IsNullOrEmpty(script))
                Control.EvaluateJavascript(script, _jsResult);
        }
    }

    public class ExtendedWebClient : WebChromeClient
    {
        private Action _intializer;

        public override void OnReceivedTitle(Android.Webkit.WebView view, string title)
        {
            base.OnReceivedTitle(view, title);
            view.StopLoading();
            _intializer?.Invoke();
        }

        public ExtendedWebClient(Action intializer)
        {
            _intializer = intializer;
        }

        public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
        {
            return base.OnConsoleMessage(consoleMessage);
        }

        public override void OnShowCustomView(Android.Views.View view, ICustomViewCallback callback)
        {
            base.OnShowCustomView(view, callback);            
        }
    }

    public class JavascriptResult : Java.Lang.Object, IValueCallback
    {
        private Action<string> _callback;
        public JavascriptResult(Action<string> callback)
        {
            _callback = callback;
        }

        public void OnReceiveValue(Java.Lang.Object value)
        {
            _callback?.Invoke(Convert.ToString(value));
        }
    }
}