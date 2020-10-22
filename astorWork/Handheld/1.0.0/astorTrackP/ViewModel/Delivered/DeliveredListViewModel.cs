using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using Plugin.Toasts;
using System.Collections.ObjectModel;


using _model = astorTrackPAPIDataModel;
using System.Threading.Tasks;

namespace astorTrackP
{
	public class DeliveredListViewModel: INotifyPropertyChanged
	{
        private string _searchDocument = string.Empty;
		private ObservableCollection<MaterialMaster> _materialMasters;
		private MaterialMaster _materialMaster;
		private INavigation _navigation;
		//private Command _searchCommand;
		
        public MaterialMaster MaterialMaster { get { return _materialMaster; } set { _materialMaster = value; OnSelectedCommand(); _materialMaster = null; OnPropertyChanged ("MaterialMaster"); }}
        public ObservableCollection<MaterialMaster> MaterialMasters { get { return _materialMasters; } set { _materialMasters = value;  OnPropertyChanged ("MaterialMasters");} }

        public ICommand SaveCommand { get { return new Command(async() => await OnSaveCommand()); } }

        private bool _isRefreshing;
        public bool IsRefreshing { get { return _isRefreshing; } set { _isRefreshing = value;  OnPropertyChanged ("IsRefreshing");} }

        //private Command _refreshCommand;
        //public ICommand RefreshCommand { get { return _refreshCommand = _refreshCommand ?? new Xamarin.Forms.Command(DoRefreshCommand, CanExecuteSearchCommand); } }

        public string Stage { get; set; }

        private bool _isLoading;
        public bool isLoading { get { return _isLoading; } set { _isLoading = value; OnPropertyChanged("isLoading"); } }

        private bool _isScanning;
        public bool isScanning
        {
            get { return _isScanning; }
            set
            {
                _isScanning = value; OnPropertyChanged("isScanning");
                switch (value)
                {
                    case false:
                        scanOperation = "rfidscan.png";
                        break;
                    case true:
                        scanOperation = "rfidstop.png";
                        break;
                }
            }
        }

        private string _scanOperation = "rfidscan.png";
        public string scanOperation { get { return _scanOperation; } set { _scanOperation = value; OnPropertyChanged("scanOperation"); } }

        ObservableCollection<MRFModel> _group;
        public ObservableCollection<MRFModel> MRFModel { get { return _group; } set { _group = value; OnPropertyChanged("MRFModel"); } }
        public DeliveredListViewModel(INavigation navigation)
		{
			_navigation = navigation;
            var data = App.MaterialMasterDb.GetMaterialMasters().Where(w => w.Status == "Delivered");
            MaterialMasters = new ObservableCollection<MaterialMaster>(data);
            GroupData();
        }

        private void GroupData()
        {
            if (MaterialMasters != null)
            {
                MRFModel = new ObservableCollection<MRFModel>();

                if (MaterialMasters != null)
                {

                    foreach (var item in MaterialMasters.Select(s => s.MRFNo).Distinct().ToList())
                    {
                        var model = new MRFModel() { MRFNo = item };
                        foreach (var material in MaterialMasters.Where(w => w.MRFNo == item))
                            model.Add(material);

                        MRFModel.Add(model);
                    }

                }
            }
        }
        
		private bool CanExecuteSearchCommand()
		{
			return true;
		}

        private void OnSelectedCommand()
        {
            int index = MaterialMasters.IndexOf(MaterialMaster);
            MaterialMasters.Remove(MaterialMaster);

            if (MaterialMaster.QAStatus == "check")
                MaterialMaster.QAStatus = "uncheck";
            else if (MaterialMaster.QAStatus == "uncheck")
                MaterialMaster.QAStatus = "check";

            MaterialMasters.Insert(index, MaterialMaster);
            GroupData();
        }

        async Task OnSaveCommand()
        {
            await Task.Run(() => SaveRecord());
        }

        private void SaveRecord()
        {
            App.ShowLoading("Saving...", Acr.UserDialogs.MaskType.Clear);
            List<_model.MaterialMaster> lstMaterialMaster = new List<astorTrackPAPIDataModel.MaterialMaster>();
            List<_model.MaterialDetail> lstMaterialDetail = new List<astorTrackPAPIDataModel.MaterialDetail>();

            foreach (var _materialMaster in MaterialMasters)
            {
                _model.MaterialMaster objMaterialMaster = new _model.MaterialMaster
                {
                    MaterialNo = _materialMaster.MaterialNo,
                    MarkingNo = _materialMaster.MarkingNo,
                    RFIDTagID = _materialMaster.RFIDTagID,
                    BeaconID = "",
                    DeliveryDate = _materialMaster.DeliveryDate,
                    DeliveryRemarks = _materialMaster.DeliveryRemarks,
                    LocationID = _materialMaster.LocationID,
                    Status = "Delivered",
                    MRFNo = _materialMaster.MRFNo,
                    Remarks = _materialMaster.Remarks,
                    UpdatedBy = App.LoginDb.GetItem().UserName,
                    UpdatedDate = DateTime.Now
                };

                _model.MaterialDetail objMaterialDetail = new _model.MaterialDetail
                {
                    MaterialNo = _materialMaster.MaterialNo,
                    MarkingNo = _materialMaster.MarkingNo,
                    RFIDTagID = _materialMaster.RFIDTagID,
                    BeaconID = "",
                    LocationID = _materialMaster.LocationID,
                    Location = _materialMaster.DeliveredLocation + " > " + App.LocationMasterDb.GetItem(_materialMaster.LocationID).ChildDescription,
                    Stage = "Delivered",
                    CreatedBy = App.LoginDb.GetItem().UserName,
                    CreatedDate = DateTime.Now,
                    QCStatus = _materialMaster.QAStatus == "check" ? "Pass" : "Fail",
                    QCBy = App.LoginDb.GetItem().UserName,
                    QCDate = DateTime.Now,
                    isQC = true
                };

                lstMaterialMaster.Add(objMaterialMaster);
                lstMaterialDetail.Add(objMaterialDetail);

            }

            _model.MaterialMasterModel objMaterialMasterModel = new _model.MaterialMasterModel();
            objMaterialMasterModel.MaterialMasters = lstMaterialMaster.AsEnumerable<_model.MaterialMaster>();
            objMaterialMasterModel.MaterialDetails = lstMaterialDetail.AsEnumerable<_model.MaterialDetail>();

            App.api.SaveMaterialMaster(objMaterialMasterModel);
            App.MaterialMasterDb.ClearMaterialMaster();
            App.HideLoading();
            Device.BeginInvokeOnMainThread(() => RedirectPage());
        }


        private void RedirectPage()
        {
            App.HideLoading();
            _navigation.PopAsync();
        }
        //private void OnSaveCommand()
        //{
        //    List<_model.MaterialMaster> lstMaterialMaster = new List<astorTrackPAPIDataModel.MaterialMaster>();
        //    List<_model.MaterialDetail> lstMaterialDetail = new List<astorTrackPAPIDataModel.MaterialDetail>();

        //    foreach (var data in MaterialMasters)
        //    {
        //        _model.MaterialMaster objMaterialMaster = new _model.MaterialMaster
        //        {
        //            MarkingNo = data.MarkingNo,
        //            RFIDTagID = data.RFIDTagID,
        //            Officer = App.LoginDb.GetItem().UserName,
        //            Contractor = "PrefabTech", //get login company code
        //            LocationID = 2, //prefab vendor association, can be storage in vendor site in the future
        //            Status = "Delivered",
        //            Remarks = "Testing Remarks..put where?"
        //        };

        //        _model.MaterialDetail objMaterialDetail = new _model.MaterialDetail
        //        {
        //            MarkingNo = data.MarkingNo,
        //            RFIDTagID = data.RFIDTagID,
        //            LocationID = 2,
        //            Stage = "Delivered",
        //            CreatedBy = App.LoginDb.GetItem().UserName,
        //            CreatedDate = DateTime.Now,
        //            QCStatus = data.QAStatus == "check" ?"Pass": "Fail",
        //            QCBy = App.LoginDb.GetItem().UserName,
        //            QCDate = DateTime.Now,
        //            isQC = data.QAStatus == "check" ? true : false,
        //        };


        //        lstMaterialMaster.Add(objMaterialMaster);
        //        lstMaterialDetail.Add(objMaterialDetail);

        //    }

        //    _model.MaterialMasterModel objMaterialMasterModel = new _model.MaterialMasterModel();
        //    objMaterialMasterModel.MaterialMasters = lstMaterialMaster.AsEnumerable<_model.MaterialMaster>();
        //    objMaterialMasterModel.MaterialDetails = lstMaterialDetail.AsEnumerable<_model.MaterialDetail>();
        //    App.api.SaveMaterialMaster(objMaterialMasterModel);

        //    App.MaterialMasterDb.ClearMaterialMaster();
        //    _navigation.PopAsync();
        //}

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



		
