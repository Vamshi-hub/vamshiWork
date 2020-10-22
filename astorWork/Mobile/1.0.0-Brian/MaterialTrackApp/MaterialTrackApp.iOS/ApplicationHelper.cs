using System.Threading;
using MaterialTrackApp.Interface;
using MaterialTrackApp.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(ApplicationHelper))]
namespace MaterialTrackApp.iOS
{
    public class ApplicationHelper : IApplicationHelper
    {
        public void closeApplication()
        {
            Thread.CurrentThread.Abort();
        }
    }
}