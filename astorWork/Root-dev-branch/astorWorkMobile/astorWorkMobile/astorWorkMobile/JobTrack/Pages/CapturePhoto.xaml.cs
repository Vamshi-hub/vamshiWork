using astorWorkMobile.JobTrack.ViewModels;
using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.JobTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CapturePhoto : ContentPage
    {
        public CapturePhoto()
        {
            InitializeComponent();
            NavigationPage.SetBackButtonTitle(this, "Back");
        }
    }
}