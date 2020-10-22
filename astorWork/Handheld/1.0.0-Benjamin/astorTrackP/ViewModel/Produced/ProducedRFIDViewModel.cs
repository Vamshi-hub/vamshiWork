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
	public class ProducedRFIDViewModel : INotifyPropertyChanged
	{
		INavigation _navigation;

        MaterialMasterMarkingNo _materialMasterMarkingN;
        public MaterialMasterMarkingNo MaterialMasterMarkingNo { get { return _materialMasterMarkingN; } set { _materialMasterMarkingN = value; OnPropertyChanged("MaterialMasterMarkingNo"); } }
        //string _materialNo;
        //public string MaterialNo { get { return _materialNo; } set { _materialNo = value; OnPropertyChanged("MaterialNo"); } }

        private int _count1 = 0;
        public int RFIDCount1 { get { return _count1; } set { _count1 = value; OnPropertyChanged("RFIDCount1"); } }

        public ICommand ScanCommand{ get { return new Command (() => OnScanCommand ()); } }
        public ICommand NavigatePendingCommand { get { return new Command(() => OnNavigatePendingCommand()); } }

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

		public ProducedRFIDViewModel(INavigation navigation)
		{
			_navigation = navigation;            
            //App.MaterialMasterDb.ClearMaterialMaster();
            //App.MaterialMasterDb.ClearMaterialDetail();
        }

        private void OnNavigatePendingCommand()
        {
            _navigation.PushAsync(new EnrollPending(null));
        }

        public void BindMaterial(string rfidTagID)
        {
            MaterialMasterMarkingNo = App.MaterialMasterDb.GetMaterialMasterMarkingNo().Where(w => w.RFIDTagID == rfidTagID).FirstOrDefault();                            
        }

		private void OnScanCommand()
		{
		    MessagingCenter.Send<ProducedRFIDViewModel> (this, "ScanRFID");
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
            
           BindMaterial(rfidTagID); //check in database
            if (MaterialMasterMarkingNo == null)
                return false;
            else
                return true;
        }
    }
}

