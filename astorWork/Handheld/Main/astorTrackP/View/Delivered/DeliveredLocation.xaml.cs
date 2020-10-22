using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Plugin.Toasts;
using System.Linq;

namespace astorTrackP
{
	public partial class DeliveredLocation : ContentPage
	{
        DeliveredLocationViewModel _viewModel;
        public DeliveredLocation()
        {
            InitializeComponent();
            _viewModel = new DeliveredLocationViewModel(this.Navigation);
            BindingContext = _viewModel;
            NavigationPage.SetTitleIcon(this, "location.png");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

    }
}

