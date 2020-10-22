using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Plugin.Toasts;
using System.Linq;

namespace astorTrackP
{
	public partial class InstalledList : ContentPage
	{
        InstalledListViewModel _vm;
        public InstalledList()
        {
            InitializeComponent();
            _vm = new InstalledListViewModel(this.Navigation);            
            BindingContext = _vm;    
            NavigationPage.SetTitleIcon(this, "installed.png");
        }        

    }
}

