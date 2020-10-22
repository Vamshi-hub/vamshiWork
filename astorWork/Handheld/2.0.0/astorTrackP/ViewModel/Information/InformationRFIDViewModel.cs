using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Plugin.Toasts;
using _model = astorTrackPAPIDataModel;

namespace astorTrackP
{
	public class InformationRFIDViewModel : INotifyPropertyChanged
	{
		INavigation _navigation;

        //MaterialMaster _materialMaster;
        //public MaterialMaster MaterialMaster { get { return _materialMaster; } set { _materialMaster = value; OnPropertyChanged("MaterialMaster"); } }
        string _materialNo;
        public string MaterialNo { get { return _materialNo; } set { _materialNo = value; OnPropertyChanged("MaterialNo"); } }

        private int _count1 = 0;
        public int RFIDCount1 { get { return _count1; } set { _count1 = value; OnPropertyChanged("RFIDCount1"); } }

        public ICommand ScanCommand{ get { return new Command (() => OnScanCommand ()); } }

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

		public InformationRFIDViewModel(INavigation navigation)
		{
			_navigation = navigation;            
            App.MaterialMasterDb.ClearMaterialMaster();
            App.MaterialMasterDb.ClearMaterialDetail();
        }

        public void BindMaterial(string rfidTagID)
        {
            var material = App.MaterialMasterDb.GetMaterialMasters().FirstOrDefault(a => a.RFIDTagID == rfidTagID);
            if (material == null)
            {
                var data = App.api.GetMaterialMaster(rfidTagID);
                if (data != null)
                {
                    foreach (var item in data.MaterialMasters)
                    {
                        App.MaterialMasterDb.InsertMaterialMaster(new MaterialMaster
                        {
                            MaterialNo = item.MaterialNo,
                            MarkingNo = item.MarkingNo,
                            Project = item.Project,
                            Block = item.Block,
                            Level = item.Level,
                            Zone = item.Zone,
                            DrawingNo = item.DrawingNo,
                            MaterialType = item.MaterialType,
                            MaterialSize = item.MaterialSize,
                            EstimatedLength = item.EstimatedLength,
                            ActualLength = item.ActualLength,
                            Status = item.Status,
                            LocationID = item.LocationID,
                            Contractor = item.Contractor,
                            MRFNo = item.MRFNo,
                            RFIDTagID = item.RFIDTagID,
                            Location = item.Project + " > " + item.Block + " > " + item.Level + " > " + item.Zone,
                            DeliveryDate = item.DeliveryDate,
                            DeliveryRemarks = item.DeliveryRemarks,
                            Remarks = item.Remarks,
                            QAStatus = item.QCStatus,
                            LotNo = item.LotNo,
                            CastingDate = item.CastingDate
                        });

                        foreach (var detail in data.MaterialDetails)
                        {
                            App.MaterialMasterDb.InsertMaterialDetail(new MaterialDetail
                            {
                                MaterialNo = detail.MaterialNo,
                                MarkingNo = detail.MarkingNo,
                                Stage = detail.Stage,
                                Status = detail.Status,
                                CreatedBy = detail.CreatedBy,
                                CreatedDate = detail.CreatedDate,
                                LocationID = detail.LocationID,
                                Location = detail.Location,
                                SeqNo = detail.SeqNo,
                                isQC = detail.isQC,
                                QCBy = detail.QCBy,
                                QCDate = detail.QCDate,
                                QCStatus = detail.QCStatus
                            });
                        }

                        MaterialNo = App.MaterialMasterDb.GetMaterialMasters().FirstOrDefault(a => a.RFIDTagID == rfidTagID).MaterialNo;
                    }
                    
                }
            }
            else
                MaterialNo = material.MaterialNo;            
        }

		private void OnScanCommand()
		{
		    MessagingCenter.Send<InformationRFIDViewModel> (this, "ScanRFID");
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
            if (MaterialNo == null)
                return false;
            else
                return true;
        }
    }
}

