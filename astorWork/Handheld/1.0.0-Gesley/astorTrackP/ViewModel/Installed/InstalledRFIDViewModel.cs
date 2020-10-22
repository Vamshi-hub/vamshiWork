using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Plugin.Toasts;
using _model = astorTrackPAPIDataModel;
using System.Threading.Tasks;

namespace astorTrackP
{
	public class InstalledRFIDViewModel : INotifyPropertyChanged
	{
		INavigation _navigation;
        InstalledRFID _page;

        ObservableCollection<MaterialMaster> _materialMaster;
		public ObservableCollection<MaterialMaster> MaterialMaster { get { return _materialMaster; } set { _materialMaster = value; OnPropertyChanged ("MaterialMaster"); }}

        public ICommand OpenPendingCommand { get { return new Command(() => OnOpenPendingCommand()); } }        
        public ICommand ScanCommand{ get { return new Command (() => OnScanCommand ()); } }
        private int _count;
        public int RFIDCount { get { return _count; } set { _count = value; OnPropertyChanged("RFIDCount"); } }
        private int _count1=0;
        public int RFIDCount1 { get { return _count1; } set { _count1 = value; OnPropertyChanged("RFIDCount1"); } }


        private bool _isLoading;
		public bool isLoading { get { return _isLoading; } set { _isLoading = value; OnPropertyChanged ("isLoading");}}

        private string _scanText = "Scan";
        public string ScanText { get { return _scanText; } set { _scanText = value; OnPropertyChanged("ScanText"); } }

        private bool _isScanning;
        public bool isScanning
        {
            get { return _isScanning; }
            set
            {
                _isScanning = value; OnPropertyChanged("isScanning");
                scanOperation = (value == true) ? "rfidscan.png" : "rfidstop.png";
                ScanText = (value == true) ? "Scanning..." : "Scan";
            }
        }

        private string _scanOperation = "rfidstop.png";
		public string scanOperation {get { return _scanOperation; } set {_scanOperation = value; OnPropertyChanged ("scanOperation");}}

		private void OnOpenPendingCommand()
		{
			_navigation.PushAsync (new InstalledPending());
		}

		public InstalledRFIDViewModel(INavigation navigation, InstalledRFID page)
		{
			_navigation = navigation;
            _page = page;
            Task.Run(() => BindMaterial());
        }

        public void BindMaterial()
        {
            App.ShowLoading("Loading...", Acr.UserDialogs.MaskType.Clear);
            MaterialMaster = new ObservableCollection<MaterialMaster>(App.MaterialMasterDb.GetMaterialMasters());
            if (MaterialMaster.Count() == 0)
            {
                var item = _page.ToolbarItems.Where(w => w.Icon == "pending.png").FirstOrDefault();
                _page.ToolbarItems.Remove(item);
            }

            App.HideLoading();
        }
        
        private void OnScanCommand()
		{
            Device.BeginInvokeOnMainThread(() => MessagingCenter.Send<InstalledRFIDViewModel>(this, "ScanRFID"));
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
        internal bool ProcessTag(string rfidTagID)
        {
            var item = MaterialMaster.FirstOrDefault(a => a.RFIDTagID == rfidTagID);
            if (item != null)
            {
                var associationID = long.Parse(App.LoginDb.GetItem().RoleLocationID);

                item.Status = "Installed";
                item.LocationID = associationID;
                item.Location = item.Location;
                App.MaterialMasterDb.UpdateMaterialMaster(item);
                return true;
            }
            else
                return false;            
        }
    }
}

