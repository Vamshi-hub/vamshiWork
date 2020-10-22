using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace astorTrackP
{
	public partial class LoginSetup : ContentPage
	{
//		Dictionary<int> nameToColor = new Dictionary<int>
//		{
//			{"Aqua",Color.Aqua}, {"Black", Color.Black} , {"Blue", Color.Blue}
//		};
//
		LoginSetupViewModel _vm;
		public LoginSetup ()
		{
            InitializeComponent();

            NavigationPage.SetTitleIcon (this, "settings.png");
			_vm = new LoginSetupViewModel (this);
            //_vm.EndPoint = "192.168.1.89";
            //_vm.EndPoint = "192.168.0.114";
            //_vm.EndPoint = "172.20.10.10";
            //_vm.EndPoint = "192.168.0.114";
            //_vm.EndPoint = "172.20.10.9";
            //_vm.EndPoint = "192.168.1.149";
            //_vm.EndPoint = "demo";
            //_vm.EndPoint = "192.168.0.109";
            BindingContext = _vm;

		}

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}

