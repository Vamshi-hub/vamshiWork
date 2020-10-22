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
	public class EnrollRFIDMarkingNoViewModel : INotifyPropertyChanged
	{
		INavigation _navigation;

        ObservableCollection<MaterialMasterMarkingNo> _materialMasterMarkingNos;
        public ObservableCollection<MaterialMasterMarkingNo> MaterialMasterMarkingNos { get { return _materialMasterMarkingNos; } set { _materialMasterMarkingNos = value; OnPropertyChanged("MaterialMasterMarkingNos"); } }

        MaterialMasterMarkingNo _materialMasterMarkingNo;
		public MaterialMasterMarkingNo MaterialMasterMarkingNo { get { return _materialMasterMarkingNo; } set { _materialMasterMarkingNo = value; OnPropertyChanged ("MaterialMasterMarkingNo"); }}

		public ICommand ScanCommand{ get { return new Command (() => OnScanCommand ()); } }
        //public ICommand SaveCommand { get { return new Command(async() => await OnSaveCommand()); } }
        public ICommand SaveCommand { get { return new Command(() => OnSaveCommand()); } }
        //public ICommand QACommand { get { return new Command(() => OnQACommand()); } }

        private string _scanSaveText = "Scan";
        public string ScanSaveText { get { return _scanSaveText; } set { _scanSaveText = value; OnPropertyChanged("ScanSaveText"); } }
        
        private bool _isLoading;
		public bool isLoading { get { return _isLoading; } set { _isLoading = value; OnPropertyChanged ("isLoading");}}

		private bool _isScanning;
        public bool isScanning
        {
            get { return _isScanning; }
            set
            {
                _isScanning = value; OnPropertyChanged("isScanning");
                scanOperation = (value == true) ? "rfidscan.png" : "rfidstop.png";
                if (value == true)
                    ScanSaveText = "Scanning...";
                else
                {
                    if (MaterialMasterMarkingNo.RFIDTagID == "" || MaterialMasterMarkingNo.RFIDTagID == null)
                        ScanSaveText = "Scan";
                    else
                        ScanSaveText = "Save";
                }
            }
        }

        private string _scanOperation = "rfidstop.png";
		public string scanOperation {get { return _scanOperation; } set {_scanOperation = value; OnPropertyChanged ("scanOperation");}}

        private void OnNavigateCommand()
		{
			//_navigation.PushAsync (new ReceivingItemScanDetail (_cableItem.Description, _cableItem.Quantity, _documentSerialNo));
		}

        public void GetMaterialMasterMarkingNo()
        {
            App.ShowLoading("Syncing...", Acr.UserDialogs.MaskType.Clear);

            var data = App.api.GetMaterialMasterMarkingNo(true);
            MaterialMasterMarkingNos = new ObservableCollection<MaterialMasterMarkingNo>();

            if (data != null)
            {
                //
                foreach (var item in data.Where(w=> w.MarkingNo == _materialMasterMarkingNo.MarkingNo && w.MaterialType == _materialMasterMarkingNo.MaterialType))
                {
                    MaterialMasterMarkingNos.Add(new MaterialMasterMarkingNo
                    {
                        ID = item.ID,
                        MarkingNo = item.MarkingNo,
                        MaterialType = item.MaterialType,
                        Prefix = item.Prefix,
                        Counter = item.Counter,
                        RFIDTagID = item.RFIDTagID,
                        BeaconID = item.BeaconID,
                        LotNo = item.LotNo,
                        CastingDate = item.CastingDate
                    });
                }
            }

            App.HideLoading();
        }

        public EnrollRFIDMarkingNoViewModel(INavigation navigation, MaterialMasterMarkingNo materialMasterMarkingNo)
		{
            _materialMasterMarkingNo = materialMasterMarkingNo;
            GetMaterialMasterMarkingNo();

            _navigation = navigation;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                materialMasterMarkingNo.CastingDate = DateTime.Now.AddDays(-2);
            else
                materialMasterMarkingNo.CastingDate = DateTime.Now.AddDays(-1);
            string prefix = materialMasterMarkingNo.MarkingNo + "-" + materialMasterMarkingNo.MaterialType;
            //var counter = int.Parse(App.api.GetFormatKey(prefix, App.LoginDb.GetItem().UserID).Replace(prefix, ""));
            //int counter = MaterialMasterMarkingNos.Max(m => m.LotNo);
            int counter = App.MaterialMasterDb.GetMaterialMasterMarkingNo().Where(w => w.MaterialType == materialMasterMarkingNo.MaterialType).Max(m => m.Counter);
            counter++;
            //int counter = materialMasterMarkingNo.Counter + 1;
            materialMasterMarkingNo.LotNo = counter;
            materialMasterMarkingNo.Counter = counter;
            MaterialMasterMarkingNo = materialMasterMarkingNo;
        }

		private void OnScanCommand()
		{            
		    MessagingCenter.Send<EnrollRFIDMarkingNoViewModel> (this, "ScanRFID");
		}

        public bool ProcessTag(string rfidTagID)
        {            
            string materialNo = App.api.ValidateRFIDTag(rfidTagID);
            if (materialNo.Trim().Length > 2)
            {
                App.ShowMessage(ToastNotificationType.Error, "Existing RFID Tag", string.Format("RFID Tag {0} enrolled to Marking {1}", rfidTagID, materialNo));
                return false;
            }
            else
            {
                MaterialMasterMarkingNo.RFIDTagID = rfidTagID;
                return true;
            }            
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

        void OnSaveCommand() //async Task 
        {
            if (_scanSaveText == "Save")
                //await Task.Run(() => SaveRecord());
                Task.Run(() => SaveRecord());
            else
            {
                OnScanCommand();
            }
        }

        private void SaveRecord()
        {
            App.ShowLoading("Saving...", Acr.UserDialogs.MaskType.Clear);
            MaterialMasterMarkingNo.Counter = _materialMasterMarkingNo.LotNo;
            App.MaterialMasterDb.UpdateMaterialMasterMarkingNo(MaterialMasterMarkingNo);

            _model.MaterialMasterMarkingNo objMaterialMasterMarkingNo = new _model.MaterialMasterMarkingNo
            {
                MarkingNo = _materialMasterMarkingNo.MarkingNo,
                MaterialType= _materialMasterMarkingNo.MaterialType,
                Prefix = _materialMasterMarkingNo.Prefix,                
                RFIDTagID = _materialMasterMarkingNo.RFIDTagID,
                BeaconID = "",
                LotNo = _materialMasterMarkingNo.LotNo,
                CastingDate = _materialMasterMarkingNo.CastingDate,
                CreatedBy = App.LoginDb.GetItem().UserName,
                CreatedDate = DateTime.Now
            };

            List<_model.MaterialMasterMarkingNo> lstMaterialMasterMarkingNo = new List<astorTrackPAPIDataModel.MaterialMasterMarkingNo>();
            lstMaterialMasterMarkingNo.Add(objMaterialMasterMarkingNo);

            _model.MaterialMasterMarkingNoModel objMaterialMasterMarkingNoModel = new _model.MaterialMasterMarkingNoModel();
            objMaterialMasterMarkingNoModel.MaterialMasterMarkingNos = lstMaterialMasterMarkingNo.AsEnumerable<_model.MaterialMasterMarkingNo>();

            App.api.SaveMaterialMasterMarkingNo(objMaterialMasterMarkingNoModel);
            Device.BeginInvokeOnMainThread(() => RedirectPage());
        }

        private void RedirectPage()
        {
            App.HideLoading();
            _navigation.PopAsync();
        }

    }
}

