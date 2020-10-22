using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.Class
{
    public class AccessToken
    {
        public string token_type;
        public string scope { get; set; }
        public int expires_in { get; set; }
        public string expires_on { get; set; }
        public string not_before { get; set; }
        public string resource { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
    }

    public class GroupInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public bool IsReadOnly { get; set; }
    }

    public class DashboardInfo
    {
        public string ID { get; set; }
        public string DisplayName { get; set; }
        public string EmbedUrl { get; set; }

        public GroupInfo Group { get; set; }
    }

    public class TileInfo
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string EmbedUrl { get; set; }

        public DashboardInfo Dashboard { get; set; }
    }

    public class EmbedConfig
    {
        public string EmbedToken { get; set; }
        public string EmbedUrl { get; set; }
        public string ID { get; set; }
    }

    public class EmbedTileConfig
    {
        public string EmbedToken { get; set; }
        public string EmbedUrl { get; set; }
        public string ID { get; set; }
        public string DashboardId { get; set; }
    }
}
