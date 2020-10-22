using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Toasts;
using System.Linq;

namespace astorTrackP
{
	public partial class InformationView : ContentPage
	{
        InformationViewViewModel _vm;
        public InformationView(string materialNo)
        {
            InitializeComponent();
            _vm = new InformationViewViewModel(this.Navigation, materialNo);            
            BindingContext = _vm;    
            NavigationPage.SetTitleIcon(this, "installed.png");
        }

        
    }
}

