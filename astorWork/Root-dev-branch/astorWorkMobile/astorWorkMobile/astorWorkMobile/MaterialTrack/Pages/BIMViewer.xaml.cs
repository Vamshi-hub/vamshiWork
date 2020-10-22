using astorWorkMobile.Shared.Utilities;
using astorWorkMobile.Shared.Views;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BIMViewer : ContentPage
    {
        private string targetUrl;
        private string targetJS;
        private HybridWebView theWebView;

        private string _uri;
        private string _modelUrn;
        private int _elementId;
        private DateTime _forgeTokenExpireTime;
        private string _forgeToken;

        public BIMViewer(string uri, string modelUrn, int elementId, DateTime forgeTokenExpireTime, string forgeToken)
        {
            _uri = uri;
            _modelUrn = modelUrn;
            _elementId = elementId;
            _forgeTokenExpireTime = forgeTokenExpireTime;
            _forgeToken = forgeToken;

            Task.Run(() => ApiClient.Instance.MTGetForgeModelProgress(_modelUrn))
                .ContinueWith((t) =>
                {
                    string js = string.Empty;
                    JsonSerializerSettings jsonSetting = new JsonSerializerSettings
                    {
                        StringEscapeHandling = StringEscapeHandling.EscapeHtml
                    };

                    try
                    {
                        var strBuilder = new StringBuilder();
                        var access_token = App.Current.Properties["access_token"];
                        var refresh_token = App.Current.Properties["refresh_token"];
                        var user_id = App.Current.Properties["user_id"];
                        var access_token_expires = App.Current.Properties["access_token_expires"];
                        var token_expire_time = DateTime.Parse(access_token_expires.ToString()).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fffzzz");
                        var forge_token_expire_time = _forgeTokenExpireTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss.fffzzz");

                        strBuilder.AppendLine($"localStorage.access_token = '{access_token}';");
                        strBuilder.AppendLine($"localStorage.refresh_token = '{refresh_token}';");
                        strBuilder.AppendLine($"localStorage.user_id = {user_id};");
                        strBuilder.AppendLine($"localStorage.token_expire_time = '{token_expire_time}';");

                        strBuilder.AppendLine($"sessionStorage.forge_token_expire_time = '{forge_token_expire_time}';");
                        strBuilder.AppendLine($"sessionStorage.forge_token = '{_forgeToken}';");

                        strBuilder.AppendLine($"sessionStorage.suppress_alert = true;");
                        strBuilder.AppendLine($"sessionStorage.low_quality = true;");
                        strBuilder.AppendLine($"sessionStorage.model_urn = '{_modelUrn}';");
                        strBuilder.AppendLine($"sessionStorage.element_id = {_elementId};");

                        if (t.Result.data != null)
                            strBuilder.AppendLine($"sessionStorage.setItem('{_modelUrn}_progress', JSON.stringify({JsonConvert.SerializeObject(t.Result.data, jsonSetting)}));");
                        //'{JsonConvert.SerializeObject(t.Result.data, jsonSetting)}'

                        js = strBuilder.ToString();
                        Console.Write(js);
                    }
                    catch (Exception exc)
                    {
                        Debug.WriteLine(exc.Message);
                        Debug.WriteLine(exc.StackTrace);
                    }


                    targetUrl = _uri;
                    targetJS = js;

                    if (string.IsNullOrEmpty(_uri) || string.IsNullOrEmpty(js))
                    {
                        Content = new Label
                        {
                            Text = "Invalid BIM Viewer URL",
                            TextColor = Color.Red,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center
                        };
                    }
                    else
                    {
                        theWebView = new HybridWebView
                        {
                            Uri = _uri,
                            JavaScript = js,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand
                        };
                        Content = theWebView;
                    }

                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            theWebView.Cleanup();
            theWebView = null;
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
        }

        /*
        private async void theWebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            Debug.WriteLine("Navigated to: " + e.Url);
            if (e.Url.ToLower().Contains("login"))
            {
                if (!string.IsNullOrEmpty(targetJS))
                    await theWebView.EvaluateJavaScriptAsync(targetJS);

                if (!string.IsNullOrEmpty(targetUrl))
                    theWebView.Source = targetUrl;
            }
        }
        private void theWebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            Console.WriteLine("Navigating to: " + e.Url);
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(targetUrl))
                theWebView.Source = targetUrl;

            if (!string.IsNullOrEmpty(targetJS))
                theWebView.EvaluateJavaScriptAsync(targetJS);
        }

        private void ContentPage_Disappearing(object sender, EventArgs e)
        {
            theWebView.Source = null;
        }
        */
    }
}