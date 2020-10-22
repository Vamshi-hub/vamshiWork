using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorTrackP
{
	public partial class RFIDSettings : ContentPage
	{
//		Dictionary<int> nameToColor = new Dictionary<int>
//		{
//			{"Aqua",Color.Aqua}, {"Black", Color.Black} , {"Blue", Color.Blue}
//		};
//
		RFIDSettingsViewModel _vm;
		public RFIDSettings ()
		{
			InitializeComponent ();

			NavigationPage.SetTitleIcon (this, "settings.png");

			_vm = new RFIDSettingsViewModel (this.Navigation);
			BindingContext = _vm;

			BindRFIDPower();
			//BindRFIDScanDetection ();            
		}

        protected override void OnAppearing()
        {
            //App.InitializeRFID();
            base.OnAppearing();
        }

        private void BindRFIDPower()
		{
			for (int i = 5; i <= 30; i++) {
				uxRFIDPower.Items.Add (i.ToString());
			} 

			if (_vm.RFIDPower != null)
				uxRFIDPower.SelectedIndex = uxRFIDPower.Items.IndexOf(_vm.RFIDPower);
		}
		//private void BindRFIDScanDetection()
		//{
		//	uxRFIDScanDetection.Items.Add ("Single");
		//	uxRFIDScanDetection.Items.Add ("Auto");

		//	if (_vm.RFIDScanDetection != null)
		//		uxRFIDScanDetection.SelectedIndex = uxRFIDScanDetection.Items.IndexOf(_vm.RFIDScanDetection);
		//}

        protected override void OnDisappearing()
        {
            //SetPower();
            base.OnDisappearing();
        }

        private async void SetPower()
        {
            Func<bool> function = new Func<bool>(() => App.rfidReader.SetRFIDPowerAsync(int.Parse(_vm.RFIDPower)));
            await Task.Run<bool>(function);            
        }        
    }
}

