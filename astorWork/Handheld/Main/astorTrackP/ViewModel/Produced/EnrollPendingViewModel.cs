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
	public class EnrollPendingViewModel: INotifyPropertyChanged
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
        ObservableCollection<MRFModel> _group;
        public ObservableCollection<MRFModel> MRFModel { get { return _group; } set { _group = value; OnPropertyChanged("MRFModel"); } }

        public string SearchDocument { get { return _searchDocument; } set { _searchDocument = value; if (value == "") DoSearchCommand(); OnPropertyChanged ("SearchDocument"); }}
        public ICommand NavigateProducedCommand { get { return new Command(async() => await OnNavigateProducedCommand()); } }
        public ICommand RefreshCommand{ get { return _refreshCommand = _refreshCommand ?? new Xamarin.Forms.Command(DoRefreshCommand, CanExecuteSearchCommand); } }
		public ICommand SearchCommand{ get { return _searchCommand = _searchCommand ?? new Xamarin.Forms.Command(DoSearchCommand, CanExecuteSearchCommand); } }
		public bool IsRefreshing { get { return _isRefreshing; } set { _isRefreshing = value;  OnPropertyChanged ("IsRefreshing");} }
        public string Stage { get; set; }
        MaterialMasterMarkingNo MaterialMasterMarkingNo { get; set; }
        
        public EnrollPendingViewModel(INavigation navigation, MaterialMasterMarkingNo markingNo)
		{
			_navigation = navigation;
            if (markingNo != null)
                MaterialMasterMarkingNo = markingNo;
            //Task.Run(() => App.GetMaterialMasterMarkingNo(true));
            DoSearchCommand();
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
            App.GetMaterialList("Requested");
            DoSearchCommand();
        }
       
        public void DoSearchCommand()
        {
            IsRefreshing = true;
            if (_searchDocument != "")
            {
                var data = App.MaterialMasterDb.FindMaterialMasters(_searchDocument);
                if (data != null)
                    MaterialMasters = new ObservableCollection<MaterialMaster>(data);
            }
            else
            {

                var data = App.MaterialMasterDb.GetMaterialMasters("Requested");
                MaterialMasters = new ObservableCollection<MaterialMaster>(data);
                
            }

            if (MaterialMasterMarkingNo != null)
            {
                var data = MaterialMasters.Where(w => w.MarkingNo == MaterialMasterMarkingNo.MarkingNo && w.MaterialType == MaterialMasterMarkingNo.MaterialType);
                MaterialMasters = new ObservableCollection<MaterialMaster>(data);
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
            if (_materialMaster.CastingDate == null)
                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                    _materialMaster.CastingDate = DateTime.Now.AddDays(-2);
                else
                    _materialMaster.CastingDate = DateTime.Now.AddDays(-1);

            if (MaterialMasterMarkingNo != null)
            {
                _materialMaster.RFIDTagID = MaterialMasterMarkingNo.RFIDTagID;
                _materialMaster.LotNo = MaterialMasterMarkingNo.LotNo;
                _materialMaster.CastingDate = MaterialMasterMarkingNo.CastingDate;                
            }

            _navigation.PushAsync(new EnrollRFID(_materialMaster));
        }


        private async Task OnNavigateProducedCommand()
        {            
            await _navigation.PushAsync(new EnrollMarkingNo());
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



		
