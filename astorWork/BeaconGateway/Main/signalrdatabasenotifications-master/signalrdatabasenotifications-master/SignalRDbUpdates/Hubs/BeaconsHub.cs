using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Configuration;

namespace SignalRDbUpdates.Hubs
{
    public class BeaconsHub : Hub
    {
        private static string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public void Hello()
        {
            Clients.All.hello();
        }

        [HubMethodName("sendBeacons")]
        public static void SendBeacons()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<BeaconsHub>();
            context.Clients.All.updateBeacons();
        }

        
    }
}