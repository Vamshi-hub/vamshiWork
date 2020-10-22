using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Toasts;
using System.Linq;

namespace astorTrackP
{
	public partial class DeliveredPending : ContentPage
	{
        DeliveredPendingViewModel _controller;
        public DeliveredPending()
        {
            InitializeComponent();
            _controller = new DeliveredPendingViewModel(this.Navigation);            
            BindingContext = _controller;    
            NavigationPage.SetTitleIcon(this, "delivered.png");
        }

        
    }
}

