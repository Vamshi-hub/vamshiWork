using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MaterialTrackApp.Utility
{
    public static class NavigationHelper
    {
        public static async Task PushPageAsync(this INavigation navigation, Page page, bool animated = true)
        {
            var getPage = navigation.NavigationStack.FirstOrDefault(p => p.GetType() == page.GetType());
            if (getPage == null)
                await navigation.PushAsync(page, animated);
        }

        public static async Task PushModalAsync(this INavigation navigation, Page page, bool animated = true)
        {
            var getPage = navigation.ModalStack.FirstOrDefault(p => p.GetType() == page.GetType());
            if(getPage == null)
                await navigation.PushModalAsync(page, animated);
        }
    }
}
