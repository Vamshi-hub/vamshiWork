using MaterialTrackApp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Diagnostics;
using static MaterialTrackApp.Utility.ApiClient;
using MaterialTrackApp.Class;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportPage : ContentPage
    {
        private List<GroupInfo> _listGroup;
        private List<DashboardInfo> _listDashboard;
        private List<EmbedTileConfig> _listEmbed;

        private readonly string HTML_TEMPLATE =
@"<!doctype html>
<html lang='en'>
	<head>
		<meta charset='utf-8'>
        <title>The HTML5 Herald</title>
		<script type='text/javascript' src='http://greatearth.cloudapp.net/scripts/jquery-3.2.1.js'></script>
        <script type= 'text/javascript' src= 'http://greatearth.cloudapp.net/scripts/powerbi.js' ></script>
        <style>
        body, html{
            width: 100%;
            height: 100%;
            margin: 0;
        }
        iframe {    
            width: 100%;
            min-height: 600px
            max-height: 1000px;
            border: 0;
        }
        div#pbContainer{
            width: 100%; 
            height: 100%; 
            margin: 0 auto;
        }
		</style>
	</head>
	<body>  
		<div id = 'pbContainer'>Loading...</div>
<script>
	var models = window['powerbi-client'].models;
    var config = {
        type: 'tile',
        tokenType: models.TokenType.Embed,
        accessToken: '{0}',
        embedUrl: '{1}',
        id: '{2}',
        dashboardId: '{3}'
    };
    var dashboardContainer = $('#pbContainer')[0];
    var dashboard = powerbi.embed(dashboardContainer, config);
</script>
	</body>
</html>";
        public ReportPage()
        {
            InitializeComponent();
            var task = Task.Run(GetListDashboard);
            App.InvokeLoadingView(this, (Grid)layoutNoData.Parent, task, "Retrieving information, please wait...");
        }

        private async Task GetListDashboard()
        {
            _listEmbed = new List<EmbedTileConfig>();
            _listDashboard = new List<DashboardInfo>();
            if (await ApiClient.Instance.GetAccessToken(App.PB_NATIVE_CLIENT_ID, App.PBClientSecret, App.PB_USER_NAME, App.PB_USER_PWD, App.PB_TENANT_ID))
            {
                ViewModelLocator.ReportPageVM.Groups.Clear();

                string accessToken = Application.Current.Properties["pb_access_token"].ToString();
                var groups = await ApiClient.Instance.PBGetAllGroups(accessToken);
                if (groups.Status == 0)
                {
                    _listGroup = groups.Data as List<GroupInfo>;

                    foreach (var gi in _listGroup)
                    {
                        var dashboards = await ApiClient.Instance.PBGetAllDashboardByGroup(gi, accessToken);

                        if (dashboards.Status == 0)
                        {
                            if (dashboards.Data.Count() > 0)
                            {
                                List<DashboardInfo> tmpList = dashboards.Data as List<DashboardInfo>;
                                ViewModelLocator.ReportPageVM.Groups.Add(new ListViewGroup<string, DashboardInfo>(gi.Name, tmpList));
                                _listDashboard.AddRange(tmpList);
                            }
                        }
                    }
                }
            }

            Device.BeginInvokeOnMainThread(() =>
            {
                if (_listDashboard.Count > 0)
                {
                    layoutNoData.IsVisible = false;
                }
                listDashboards.IsRefreshing = false;
            });
        }

        private void listDashboards_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var dashboard = e.SelectedItem as DashboardInfo;
                var task = Task.Run(() => GetEmbedConfig(dashboard));
                App.InvokeLoadingView(this, (Grid)layoutNoData.Parent, task, "Loading dashboards, please wait...");
            }
        }

        private void listDashboards_Refreshing(object sender, EventArgs e)
        {
            layoutNoData.IsVisible = true;
            LoadDashboards();
        }

        private void LoadDashboards()
        {
            var task = Task.Run(GetListDashboard);
            App.InvokeLoadingView(this, (Grid)layoutNoData.Parent, task, "Loading dashboards, please wait...");
        }

        private async Task GetEmbedConfig(DashboardInfo di)
        {
            string accessToken = Application.Current.Properties["pb_access_token"].ToString();
            var embedConfigResult = await ApiClient.Instance.PBGetTileEmbedConfig(di, accessToken);

            if (embedConfigResult.Status == 0)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    var reportViewPage = new ReportTilesPage();
                    reportViewPage.Title = di.DisplayName;
                    foreach (EmbedTileConfig embedConfig in embedConfigResult.Data)
                    {
                        string html = HTML_TEMPLATE.Replace("{0}", embedConfig.EmbedToken);
                        html = html.Replace("{1}", embedConfig.EmbedUrl);
                        html = html.Replace("{2}", embedConfig.ID);
                        html = html.Replace("{3}", embedConfig.DashboardId);
                        var viewSource = new HtmlWebViewSource()
                        {
                            Html = html
                        };

                        var contentPage = new ContentPage
                        {
                            Padding = 0,
                            Content = new Grid
                            {
                                Children = {
                                        new WebView
                                        {
                                            Source = viewSource
                                        }
                                    }
                            }
                        };
                        reportViewPage.Children.Add(contentPage);
                    }
                    Navigation.PushAsync(reportViewPage);
                });
            }
        }
    }
}
