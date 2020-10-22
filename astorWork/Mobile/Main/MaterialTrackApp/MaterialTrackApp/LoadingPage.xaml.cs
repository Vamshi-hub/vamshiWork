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
    public partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            InitializeComponent();
        }

        private Page _prevPage;
        private bool _replaceMainPage = false;

        public void InitLoadingPage(Page prevPage, Page nextPage, string message, bool replaceMainPage)
        {
            _prevPage = prevPage;
            _replaceMainPage = replaceMainPage;
            Device.BeginInvokeOnMainThread(() =>
            {
                lblLoadingMessage.Text = message;
            });

            MessagingCenter.Subscribe<Page, string>(prevPage, App.TOPIC_LOADING_RESULT, (sender, arg) =>
                {
                    MessagingCenter.Unsubscribe<Page, string>(prevPage, App.TOPIC_LOADING_RESULT);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        if (string.IsNullOrEmpty(arg))
                        {
                            if (replaceMainPage)
                                Application.Current.MainPage = nextPage;
                            else
                            {
                                //var newPage = new NavigationPage(nextPage);
                                Application.Current.MainPage.Navigation.RemovePage(this);
                                Application.Current.MainPage.Navigation.PushAsync(nextPage);
                            }
                        }
                        else
                        {
                            lblResultMessage.Text = arg;
                            layoutLoading.ScaleTo(0).ContinueWith((t) =>
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    layoutMaster.RaiseChild(layoutResult);
                                    layoutResult.ScaleTo(1);
                                });
                            });
                        }
                    });
                });
        }

        private void btnBack_Clicked(object sender, EventArgs e)
        {
            layoutResult.ScaleTo(0);
            if (_replaceMainPage)
                Application.Current.MainPage = _prevPage;
            else
                Navigation.PopAsync();
        }
    }
}