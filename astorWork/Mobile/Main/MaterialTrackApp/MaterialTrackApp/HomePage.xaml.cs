using MaterialTrackApp.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            Task.Run(InitNumbers);
            /*
            var result = LocalDBHandler.Instance.ReadBeaconsAsync().Result;
            var countPendingEnroll = result.Where(x => string.IsNullOrEmpty(x.MaterialNo)).Count();
            var countPendingUpdate = result.Where(x => !string.IsNullOrEmpty(x.MaterialNo) && x.NextStatus ==
            ((int)App.Current.Properties["user_role"] == 1 ? "Installed" : "Produced")).Count();

            lblCountPendingEnroll.Text = countPendingEnroll.ToString();
            if (countPendingEnroll > 0)
                framePendingEnroll.BackgroundColor = Color.AliceBlue;

            lblCountPendingUpdate.Text = countPendingUpdate.ToString();
            if (countPendingUpdate > 0)
                framePendingUpdate.BackgroundColor = Color.AliceBlue;
                */
        }

        private async Task InitNumbers()
        {
            var result = await LocalDBHandler.Instance.ReadBeaconsAsync();

            var countPendingTask = result.Where(x => x.Material != null && x.NextStatus ==
            ((int)App.Current.Properties["role_id"] == 1 ? "Installed" : "Produced")).Count();

            Device.BeginInvokeOnMainThread(() =>
            {
                lblCountPendingTask.Text = countPendingTask.ToString();
                if (countPendingTask > 0)
                    framePendingTask.BackgroundColor = Color.LightBlue;
            });
        }

        private void btnScan_Tapped(object sender, EventArgs e)
        {
            App.mainPage.NavigateDetail(1);
        }

        private void btnPendingTask_Tapped(object sender, EventArgs e)
        {
            App.mainPage.NavigateDetail(2);
        }

        private void btnClearCache_Clicked(object sender, EventArgs e)
        {
            Task.Run(LocalDBHandler.Instance.ClearDB);
        }
    }
}