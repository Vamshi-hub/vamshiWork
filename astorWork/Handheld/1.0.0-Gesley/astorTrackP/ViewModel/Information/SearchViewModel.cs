using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using Plugin.Toasts;
using System.Collections.ObjectModel;


using _model = astorCableAPIDataModel;

namespace astorCable
{
	public class SearchViewModel: INotifyPropertyChanged
	{
        private string _searchDocument = string.Empty;
		private ObservableCollection<CableMaster> _cableMasters;
		private CableMaster _cableMaster;
		private INavigation _navigation;
		private Command _searchCommand;
		private Command _refreshCommand;
		private bool _isRefreshing;		
		
		public CableMaster CableMaster { get { return _cableMaster; } set { _cableMaster = value; OnNavigateCommand(); _cableMaster = null; OnPropertyChanged ("CableMaster"); }}
		public ObservableCollection<CableMaster> CableMasters { get { return _cableMasters; } set { _cableMasters = value;  OnPropertyChanged ("CableMasters");} }

		public string SearchDocument { get { return _searchDocument; } set { _searchDocument = value; if (value == "") DoSearchCommand(); OnPropertyChanged ("SearchDocument"); }}
		public ICommand NavigateCommand{ get { return new Command (() => OnNavigateCommand ()); } }
		public ICommand RefreshCommand{ get { return _refreshCommand = _refreshCommand ?? new Xamarin.Forms.Command(DoSearchCommand, CanExecuteSearchCommand); } }
		public ICommand SearchCommand{ get { return _searchCommand = _searchCommand ?? new Xamarin.Forms.Command(DoSearchCommand, CanExecuteSearchCommand); } }
		public bool IsRefreshing { get { return _isRefreshing; } set { _isRefreshing = value;  OnPropertyChanged ("IsRefreshing");} }


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


        public SearchViewModel(INavigation navigation)
		{
			_navigation = navigation;
        }
        
		private bool CanExecuteSearchCommand()
		{
			return true;
		}

        public void DoSearchCommand()
        {
            IsRefreshing = true;
            if (_searchDocument != "")
            {
                var data = App.CableMasterDb.FindCableMasters(_searchDocument);
                if (data != null)
                    CableMasters = new ObservableCollection<CableMaster>(data);
            }
            else
            {
                CableMasters = null;
            }            
            IsRefreshing = false;
        }

		private void OnNavigateCommand()
		{
            //CableMasters.Remove(CableMaster);
			_navigation.PushAsync(new CableInformation(_cableMaster));
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



		
