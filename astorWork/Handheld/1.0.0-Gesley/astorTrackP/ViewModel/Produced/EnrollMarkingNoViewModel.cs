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
	public class EnrollMarkingNoViewModel: INotifyPropertyChanged
	{
        private string _searchDocument = string.Empty;
		private ObservableCollection<MaterialMasterMarkingNo> _materialMasterMarkingNos;
		private MaterialMasterMarkingNo _materialMasterMarkingNo;
		private INavigation _navigation;
		private Command _searchCommand;
		private Command _refreshCommand;
		private bool _isRefreshing;		
		

		public MaterialMasterMarkingNo MaterialMasterMarkingNo { get { return _materialMasterMarkingNo; } set { _materialMasterMarkingNo = value; OnNavigateCommand(); _materialMasterMarkingNo = null; OnPropertyChanged ("MaterialMasterMarkingNo"); }}
		public ObservableCollection<MaterialMasterMarkingNo> MaterialMasterMarkingNos { get { return _materialMasterMarkingNos; } set { _materialMasterMarkingNos = value;  OnPropertyChanged ("MaterialMasterMarkingNos");} }
        ObservableCollection<MaterialMasterMarkingNoModel> _group;
        public ObservableCollection<MaterialMasterMarkingNoModel> MaterialMasterMarkingNoModel { get { return _group; } set { _group = value; OnPropertyChanged("MaterialMasterMarkingNoModel"); } }

        public string SearchDocument { get { return _searchDocument; } set { _searchDocument = value; if (value == "") DoSearchCommand(); OnPropertyChanged ("SearchDocument"); }}
		public ICommand RefreshCommand{ get { return _refreshCommand = _refreshCommand ?? new Xamarin.Forms.Command(DoRefreshCommand, CanExecuteSearchCommand); } }
		public ICommand SearchCommand{ get { return _searchCommand = _searchCommand ?? new Xamarin.Forms.Command(DoSearchCommand, CanExecuteSearchCommand); } }
		public bool IsRefreshing { get { return _isRefreshing; } set { _isRefreshing = value;  OnPropertyChanged ("IsRefreshing");} }
        public string Stage { get; set; }
        
        public EnrollMarkingNoViewModel(INavigation navigation)
		{
			_navigation = navigation;
            GetData();
            //DoSearchCommand();
        }
        
		private bool CanExecuteSearchCommand()
		{
			return true;
		}

        private void DoRefreshCommand()
        {
            Task.Run(() => GetData());
        }

        private void GetData()
        {
            //App.GetMaterialMasterMarkingNo(false);
            DoSearchCommand();
        }
       
        public void DoSearchCommand()
        {
            IsRefreshing = true;
            if (_searchDocument != "")
            {
                var data = App.MaterialMasterDb.FindMaterialMasterMarkingNo(_searchDocument);
                if (data != null)
                    MaterialMasterMarkingNos = new ObservableCollection<MaterialMasterMarkingNo>(data);
            }
            else
            {

                var data = App.MaterialMasterDb.GetMaterialMasterMarkingNo();
                MaterialMasterMarkingNos = new ObservableCollection<MaterialMasterMarkingNo>(data);
                
            }

            if (MaterialMasterMarkingNos != null)
            {
                MaterialMasterMarkingNoModel = new ObservableCollection<MaterialMasterMarkingNoModel>();

                if (MaterialMasterMarkingNos != null)
                {

                    foreach (var item in MaterialMasterMarkingNos.Select(s => s.MaterialType).Distinct().ToList())
                    {
                        var model = new MaterialMasterMarkingNoModel() { MaterialType = item };
                        foreach (var material in MaterialMasterMarkingNos.Where(w => w.MaterialType == item))
                            model.Add(material);

                        MaterialMasterMarkingNoModel.Add(model);
                    }

                }
            }
            IsRefreshing = false;
        }

		private void OnNavigateCommand()
		{
            _materialMasterMarkingNo.RFIDTagID = null;
            _navigation.PushAsync(new EnrollRFIDMarkingNo(_materialMasterMarkingNo));
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



		
