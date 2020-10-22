using System;
using System.Collections.Generic;
using Plugin.Toasts;
using Xamarin.Forms;

namespace astorTrackP
{
	public partial class LoginPage : ContentPage
	{
        LoginSetupViewModel _vm;
        public LoginPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            
            _vm = new LoginSetupViewModel(this);
            BindingContext = _vm;
            
        }

        protected void OnTapGestureRecognizerTapped(object sender, EventArgs e)
        {
            
            this.Navigation.PushAsync(new LoginSetup());
        }
    }
}

