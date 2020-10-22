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
	public class DeliveredLocationViewModel : INotifyPropertyChanged
	{
		INavigation _navigation;
        ObservableCollection<LocationMaster> _locationMasters;
        LocationMaster _locationMaster;

        public ObservableCollection<LocationMaster> LocationMasters { get { return _locationMasters; } set { _locationMasters = value; OnPropertyChanged ("LocationMasters"); }}
        public LocationMaster LocationMaster { get { return _locationMaster; } set { _locationMaster = value; OnNavigateCommand(_locationMaster); _locationMaster = null; OnPropertyChanged("LocationMaster"); } }
        private Command _refreshCommand;
        public ICommand RefreshCommand { get { return _refreshCommand = _refreshCommand ?? new Xamarin.Forms.Command(DoRefreshCommand, CanExecuteSearchCommand); } }

        private bool _isRefreshing;
        public bool IsRefreshing { get { return _isRefreshing; } set { _isRefreshing = value; OnPropertyChanged("IsRefreshing"); } }

        private void OnNavigateCommand(LocationMaster locationMaster)
        {
            var item = App.LoginDb.GetItem();
            item.DeliveredAssociationID = locationMaster.AssociationID;
            App.LoginDb.UpdateItem(item);
            _navigation.PopAsync();
        }

        public DeliveredLocationViewModel(INavigation navigation)
		{
			_navigation = navigation;            
            BindLocation();
            
        }

        private void DoRefreshCommand()
        {
            BindLocation();
        }

        private bool CanExecuteSearchCommand()
        {
            return true;
        }

        public void BindLocation()
        {//move binding to start of program
            IsRefreshing = true;
            App.LocationMasterDb.ClearItems();
            var data = App.api.GetLocationHeirarchy(long.Parse(App.LoginDb.GetItem().RoleLocationID)); 
            if (data != null)
            {
                foreach (var item in data)
                {
                    if (item.ChildType == "DeliveryLocation")
                        App.LocationMasterDb.InsertItem(new LocationMaster
                        {
                            AssociationID = item.AssociationID,
                            Parents = item.Parents,
                            ChildDescription = item.ChildDescription,
                            ParentDescription = item.ParentDescription,
                        });
                }
            }
            LocationMasters = new ObservableCollection<LocationMaster>(App.LocationMasterDb.GetItems());
            IsRefreshing = false;
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

