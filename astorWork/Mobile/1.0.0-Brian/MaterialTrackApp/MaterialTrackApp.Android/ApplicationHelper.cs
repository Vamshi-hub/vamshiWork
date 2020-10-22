using Android.App;
using MaterialTrackApp.Droid;
using MaterialTrackApp.Interface;
using Xamarin.Forms;

[assembly: Dependency(typeof(ApplicationHelper))]
namespace MaterialTrackApp.Droid
{
    public class ApplicationHelper : IApplicationHelper
    {
        public void closeApplication()
        {
            var activity = (Activity) Android.App.Application.Context;
            activity.FinishAffinity();
        }
    }
}