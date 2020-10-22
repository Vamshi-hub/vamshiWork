using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;

namespace astorTrackP
{
	public class RFIDSettingsViewModel: INotifyPropertyChanged
	{
		string _rfidPower;
		string _rfidScan;

		INavigation _navigation;

		public string RFIDPower { get { return _rfidPower; } set { _rfidPower = value; SaveRecord(); OnPropertyChanged ("RFIDPower"); }}
		public string RFIDScanDetection { get { return _rfidScan; } set { _rfidScan = value; SaveRecord(); OnPropertyChanged ("RFIDScanDetection"); }}

		//public ICommand SaveCommand{ get { return new Command (() => OnSaveCommand ()); } }

		public RFIDSettingsViewModel(INavigation navigation)
		{
			//initialize
			_navigation = navigation;
			//load default value
			if (App.SettingsDb.GetItemsCount () > 0) {
				var data = App.SettingsDb.GetItem ();
				RFIDPower = data.RFIDPower.ToString();
				RFIDScanDetection = data.RFIDScanDetection;
            }
		}

        

        private void SaveRecord()
        {
            _rfidScan = "Single";

            var data = App.SettingsDb.GetItem();
            if (data == null)
                data = new Settings
                {
                    RFIDPower = int.Parse(RFIDPower),
                    RFIDScanDetection = RFIDScanDetection
                };
            else
            {
                data.RFIDPower = int.Parse(RFIDPower);
                data.RFIDScanDetection = RFIDScanDetection;
            }
            App.SettingsDb.SaveItem(data);
            App.rfidReader.SetRFIDPowerAsync(data.RFIDPower);
        }
        
		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged (this, new PropertyChangedEventArgs (propertyName));
			}
		}
	}
}

