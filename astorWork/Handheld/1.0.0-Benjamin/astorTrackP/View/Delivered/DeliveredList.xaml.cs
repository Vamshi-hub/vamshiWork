using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Toasts;
using System.Linq;

namespace astorTrackP
{
	public partial class DeliveredList : ContentPage
	{
        DeliveredListViewModel _controller;
        public DeliveredList()
        {
            InitializeComponent();
            _controller = new DeliveredListViewModel(this.Navigation);            
            BindingContext = _controller;    
            NavigationPage.SetTitleIcon(this, "delivered.png");
        }        

    }
}

