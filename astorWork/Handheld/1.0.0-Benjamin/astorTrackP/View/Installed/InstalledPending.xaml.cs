using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Toasts;
using System.Linq;

namespace astorTrackP
{
	public partial class InstalledPending : ContentPage
	{
        InstalledPendingViewModel _vm;
        public InstalledPending()
        {
            InitializeComponent();
            _vm = new InstalledPendingViewModel(this.Navigation);            
            BindingContext = _vm;    
            NavigationPage.SetTitleIcon(this, "installed.png");
        }

        
    }
}

