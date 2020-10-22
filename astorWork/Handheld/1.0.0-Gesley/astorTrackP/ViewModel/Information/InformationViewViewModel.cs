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
	public class InformationViewViewModel : INotifyPropertyChanged
	{
		INavigation _navigation;
        public string MaterialNo;

        MaterialMaster _materialMaster;
		public MaterialMaster MaterialMaster { get { return _materialMaster; } set { _materialMaster = value; OnPropertyChanged ("MaterialMaster"); }}

        ObservableCollection<MaterialDetail> _materialDetails;
        public ObservableCollection<MaterialDetail> MaterialDetails { get { return _materialDetails; } set { _materialDetails = value; OnPropertyChanged("MaterialDetails"); } }

        //private Command _refreshCommand;
        //public ICommand RefreshCommand { get { return _refreshCommand = _refreshCommand ?? new Xamarin.Forms.Command(DoRefreshCommand, CanExecuteSearchCommand); } }

        //private bool _isRefreshing;
        //public bool IsRefreshing { get { return _isRefreshing; } set { _isRefreshing = value; OnPropertyChanged("IsRefreshing"); } }

        public InformationViewViewModel(INavigation navigation, string materialNo)
		{
			_navigation = navigation;
            MaterialNo = materialNo;
            BindData();
        }

        private void BindData()
        {
            MaterialMaster = App.MaterialMasterDb.GetMaterialMasterByMaterialNo(MaterialNo);
            MaterialDetails = new ObservableCollection<MaterialDetail>(App.MaterialMasterDb.GetMaterialDetails(MaterialNo).OrderBy(o => o.SeqNo));            
        }

        //private void DoRefreshCommand()
        //{
        //    DoSearchCommand();
        //}

        private bool CanExecuteSearchCommand()
        {
            return true;
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

