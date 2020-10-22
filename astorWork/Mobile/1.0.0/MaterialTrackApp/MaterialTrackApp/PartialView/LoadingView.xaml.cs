using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTrackApp.PartialView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingView : ContentView
    {

        public LoadingView(string loadingText = "Loading data, please wait...")
        {
            InitializeComponent();

            lblMessage.Text = loadingText;
            
        }

        public async void WaitForLoadingComplete(Layout layoutParent, Task loadingTask)
        {
            await loadingTask;
            Device.BeginInvokeOnMainThread(() =>
            {
                layoutLoading.Children.Clear();
                layoutParent.LowerChild(this);
            });
        }
    }
}