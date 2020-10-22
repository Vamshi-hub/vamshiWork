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

        public async void WaitForLoadingComplete(Layout<View> layoutParent, Task loadingTask)
        {
            await loadingTask;
            Device.BeginInvokeOnMainThread(() =>
            {
                layoutParent.Children.Remove(this);
            });
        }
    }
}