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
	public class EnrollRFIDViewModel : INotifyPropertyChanged
	{
		INavigation _navigation;
        MaterialMaster _materialMaster;
        bool _enabled;
        public bool IsEnabled { get { return _enabled; } set { _enabled = value; OnPropertyChanged("IsEnabled"); } }

        public ObservableCollection<MaterialMasterMarkingNo> MaterialMasterMarkingNos { get; set; }
        public MaterialMaster MaterialMaster { get { return _materialMaster; } set { _materialMaster = value; OnPropertyChanged ("MaterialMaster"); }}

        public ICommand ScanCommand{ get { return new Command (() => OnScanCommand ()); } }
        //public ICommand SaveCommand { get { return new Command(async() => await OnSaveCommand()); } }
        public ICommand SaveCommand { get { return new Command(() => OnSaveCommand()); } }
        public ICommand QACommand { get { return new Command(() => OnQACommand()); } }

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
                    if (MaterialMaster.RFIDTagID == "" || MaterialMaster.RFIDTagID == null)
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

        public void OnQACommand()
        {
            if (MaterialMaster.QAStatus == "check")
                MaterialMaster.QAStatus = "uncheck";
            else if (MaterialMaster.QAStatus == "uncheck")
                MaterialMaster.QAStatus = "check";
        }


        public EnrollRFIDViewModel(INavigation navigation, MaterialMaster materialMaster)
		{
			_navigation = navigation;
            //if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
            //    materialMaster.CastingDate = DateTime.Now.AddDays(-2);
            //else
            //    materialMaster.CastingDate = DateTime.Now.AddDays(-1);
            //string prefix = materialMaster.MarkingNo + "-" + materialMaster.MaterialType;
            //materialMaster.LotNo = int.Parse(App.api.GetFormatKey(prefix, App.LoginDb.GetItem().UserID).Replace(prefix, ""));
            
            GetMaterialMasterMarkingNo();
            MaterialMaster = materialMaster;

            int counter = App.MaterialMasterDb.GetMaterialMasterMarkingNo().Where(w=> w.MaterialType == materialMaster.MaterialType).Max(m => m.Counter);
            counter++;
            MaterialMaster.LotNo = counter;
           var materialMasterMarkingNo = App.MaterialMasterDb.GetMaterialMasterMarkingNo().Where(w => w.MarkingNo == _materialMaster.MarkingNo && w.MaterialType == _materialMaster.MaterialType).FirstOrDefault();
            //if (materialMasterMarkingNo != null)
            //    MaterialMaster.LotNo = materialMasterMarkingNo.Counter + 1;
            //else
            //    MaterialMaster.LotNo = materialMaster.LotNo + 1;
        }

        public void GetMaterialMasterMarkingNo()
        {
            App.ShowLoading("Syncing...", Acr.UserDialogs.MaskType.Clear);

            var data = App.api.GetMaterialMasterMarkingNo(true);
            MaterialMasterMarkingNos = new ObservableCollection<MaterialMasterMarkingNo>();

            if (data != null)
            {
                foreach (var item in data)
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

        private void OnScanCommand()
		{            
		    MessagingCenter.Send<EnrollRFIDViewModel> (this, "ScanRFID");
		}

        public bool ProcessTag(string rfidTagID)
        {
            var materialMasterMarkingNo = MaterialMasterMarkingNos.Where(w => w.RFIDTagID == rfidTagID && w.MarkingNo == _materialMaster.MarkingNo && w.MaterialType == _materialMaster.MaterialType).FirstOrDefault();
            if (materialMasterMarkingNo != null)
            {
                MaterialMaster.LotNo = materialMasterMarkingNo.LotNo;
                MaterialMaster.CastingDate = materialMasterMarkingNo.CastingDate;
                MaterialMaster.RFIDTagID = rfidTagID;
                IsEnabled = false;
                return true;
            }
            else
            {
                string materialNo = App.api.ValidateRFIDTag(rfidTagID); //validation is done on the EnrollMarkingNo
                if (materialNo.Trim().Length > 2)
                {
                    App.ShowMessage(ToastNotificationType.Error, "Existing RFID Tag", string.Format("RFID Tag {0} enrolled to Marking {1}", rfidTagID, materialNo));
                    return false;
                }
                else
                {
                    if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                        MaterialMaster.CastingDate = DateTime.Now.AddDays(-2);
                    else
                        MaterialMaster.CastingDate = DateTime.Now.AddDays(-1);

                    int counter = App.MaterialMasterDb.GetMaterialMasterMarkingNo().Where(w => w.MaterialType == MaterialMaster.MaterialType).Max(m => m.Counter);
                    counter++;
                    MaterialMaster.LotNo = counter;

                    //materialMasterMarkingNo = App.MaterialMasterDb.GetMaterialMasterMarkingNo().Where(w => w.MarkingNo == _materialMaster.MarkingNo && w.MaterialType == _materialMaster.MaterialType).FirstOrDefault();
                    //if (materialMasterMarkingNo != null)
                    //    MaterialMaster.LotNo = materialMasterMarkingNo.Counter + 1;
                    MaterialMaster.RFIDTagID = rfidTagID;
                    IsEnabled = true;
                    return true;
                }
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
            if (IsEnabled)
                SaveMaterialMasterMarkingNoRecord();

            _materialMaster.Status = "Produced";            
            App.MaterialMasterDb.UpdateMaterialMaster(_materialMaster);

            _model.MaterialMaster objMaterialMaster = new _model.MaterialMaster
            {
                MaterialNo = _materialMaster.MaterialNo,
                MarkingNo = _materialMaster.MarkingNo,
                RFIDTagID = _materialMaster.RFIDTagID,
                BeaconID = "",
                DeliveryDate = _materialMaster.DeliveryDate,
                DeliveryRemarks = _materialMaster.DeliveryRemarks,
                LocationID = int.Parse(App.LoginDb.GetItem().RoleLocationID),
                Status = _materialMaster.Status,
                MRFNo = _materialMaster.MRFNo,
                Remarks = _materialMaster.Remarks,
                LotNo = _materialMaster.LotNo,
                CastingDate = _materialMaster.CastingDate,
                UpdatedBy = App.LoginDb.GetItem().UserName,
                UpdatedDate = DateTime.Now
            };

            var associationID = long.Parse(App.LoginDb.GetItem().RoleLocationID);
            _model.MaterialDetail objMaterialDetail = new _model.MaterialDetail
            {
                MaterialNo = _materialMaster.MaterialNo,
                MarkingNo = _materialMaster.MarkingNo,
                RFIDTagID = _materialMaster.RFIDTagID,
                BeaconID = "",
                LocationID = associationID,
                Location = App.LocationMasterDb.GetItem(associationID).ChildDescription,
                Stage = _materialMaster.Status,
                CreatedBy = App.LoginDb.GetItem().UserName,
                CreatedDate = DateTime.Now,
                QCStatus = _materialMaster.QAStatus == "check" ? "Pass" : "Fail",
                QCBy = App.LoginDb.GetItem().UserName,
                QCDate = DateTime.Now,
                isQC = true
            };

            List<_model.MaterialMaster> lstMaterialMaster = new List<astorTrackPAPIDataModel.MaterialMaster>();
            lstMaterialMaster.Add(objMaterialMaster);

            List<_model.MaterialDetail> lstMaterialDetail = new List<astorTrackPAPIDataModel.MaterialDetail>();
            lstMaterialDetail.Add(objMaterialDetail);

            _model.MaterialMasterModel objMaterialMasterModel = new _model.MaterialMasterModel();
            objMaterialMasterModel.MaterialMasters = lstMaterialMaster.AsEnumerable<_model.MaterialMaster>();
            objMaterialMasterModel.MaterialDetails = lstMaterialDetail.AsEnumerable<_model.MaterialDetail>();

            App.api.SaveMaterialMaster(objMaterialMasterModel);
            

            Device.BeginInvokeOnMainThread(() => RedirectPage());

           
                //save material
        }

        private void SaveMaterialMasterMarkingNoRecord()
        {
           var materialMasterMarkingNo = MaterialMasterMarkingNos.Where(w => w.MarkingNo == _materialMaster.MarkingNo && w.MaterialType == _materialMaster.MaterialType).FirstOrDefault();
            if (materialMasterMarkingNo != null)
            {
                materialMasterMarkingNo.LotNo = _materialMaster.LotNo;
                materialMasterMarkingNo.CastingDate = _materialMaster.CastingDate;
                materialMasterMarkingNo.RFIDTagID = _materialMaster.RFIDTagID;
                materialMasterMarkingNo.Counter = _materialMaster.LotNo;
                App.MaterialMasterDb.UpdateMaterialMasterMarkingNo(materialMasterMarkingNo);
            }

            //App.MaterialMasterDb.UpdateMaterialMasterMarkingNo(MaterialMasterMarkingNo);
            _model.MaterialMasterMarkingNo objMaterialMasterMarkingNo = new _model.MaterialMasterMarkingNo
            {
                MarkingNo = _materialMaster.MarkingNo,
                MaterialType = _materialMaster.MaterialType,
                //Prefix = _materialMaster.Prefix,
                RFIDTagID = _materialMaster.RFIDTagID,
                BeaconID = "",
                LotNo = _materialMaster.LotNo,
                CastingDate = _materialMaster.CastingDate,
                CreatedBy = App.LoginDb.GetItem().UserName,
                CreatedDate = DateTime.Now
            };

            List<_model.MaterialMasterMarkingNo> lstMaterialMasterMarkingNo = new List<astorTrackPAPIDataModel.MaterialMasterMarkingNo>();
            lstMaterialMasterMarkingNo.Add(objMaterialMasterMarkingNo);

            _model.MaterialMasterMarkingNoModel objMaterialMasterMarkingNoModel = new _model.MaterialMasterMarkingNoModel();
            objMaterialMasterMarkingNoModel.MaterialMasterMarkingNos = lstMaterialMasterMarkingNo.AsEnumerable<_model.MaterialMasterMarkingNo>();

            App.api.SaveMaterialMasterMarkingNo(objMaterialMasterMarkingNoModel);
        }

        private void RedirectPage()
        {
            App.HideLoading();
            _navigation.PopAsync();
        }

    }
}

