using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using Plugin.Toasts;
using System.Collections.ObjectModel;


using _model = astorTrackPAPIDataModel;

namespace astorTrackP
{
	public class DeliveredPendingViewModel: INotifyPropertyChanged
	{
        private string _searchDocument = string.Empty;
		private ObservableCollection<MaterialMaster> _materialMasters;
		private MaterialMaster _materialMaster;
		private INavigation _navigation;
		private Command _searchCommand;
		private Command _refreshCommand;
		private bool _isRefreshing;		
		
		public MaterialMaster MaterialMaster { get { return _materialMaster; } set { _materialMaster = value; OnNavigateCommand(); _materialMaster = null; OnPropertyChanged ("MaterialMaster"); }}
		public ObservableCollection<MaterialMaster> MaterialMasters { get { return _materialMasters; } set { _materialMasters = value;  OnPropertyChanged ("MaterialMasters");} }

		public string SearchDocument { get { return _searchDocument; } set { _searchDocument = value; if (value == "") DoSearchCommand(); OnPropertyChanged ("SearchDocument"); }}
		public ICommand NavigateCommand{ get { return new Command (() => OnNavigateCommand ()); } }
		public ICommand RefreshCommand{ get { return _refreshCommand = _refreshCommand ?? new Xamarin.Forms.Command(DoRefreshCommand, CanExecuteSearchCommand); } }
		public ICommand SearchCommand{ get { return _searchCommand = _searchCommand ?? new Xamarin.Forms.Command(DoSearchCommand, CanExecuteSearchCommand); } }
        public ICommand IncludeCommand { get { return new Command<MaterialMaster>((e) => OnIncludeCommand(e)); } }       

        public bool IsRefreshing { get { return _isRefreshing; } set { _isRefreshing = value;  OnPropertyChanged ("IsRefreshing");} }
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


        public DeliveredPendingViewModel(INavigation navigation)
		{
			_navigation = navigation;
            DoRefreshCommand();
        }
        
		private bool CanExecuteSearchCommand()
		{
			return true;
		}
        private void DoRefreshCommand()
        {           
            DoSearchCommand();
        }

        private void OnIncludeCommand(MaterialMaster e)
        {
            var item = App.MaterialMasterDb.GetMaterialMasters().Where(w => w.MaterialNo == e.MaterialNo).FirstOrDefault();
            if (item != null)
            {
                item.Status = "Delivered";
                App.MaterialMasterDb.UpdateMaterialMaster(item);
            }
            DoSearchCommand();
        }


        ObservableCollection<MRFModel> _group;
        public ObservableCollection<MRFModel> MRFModel { get { return _group; } set { _group = value; OnPropertyChanged("MRFModel"); } }
        public void DoSearchCommand()
        {
            IsRefreshing = true;
            if (_searchDocument != "")
            {
                var data = App.MaterialMasterDb.FindMaterialMasters("Produced" , _searchDocument);
                if (data != null)
                    MaterialMasters = new ObservableCollection<MaterialMaster>(data);
            }
            else
            {
                var data = App.MaterialMasterDb.GetMaterialMasters("Produced");
                MaterialMasters = new ObservableCollection<astorTrackP.MaterialMaster>(data);
            }

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
            IsRefreshing = false;
        }

		private void OnNavigateCommand()
		{
           _navigation.PushAsync(new EnrollRFID(_materialMaster));
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



		
