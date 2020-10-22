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
	public class DashboardViewModel : INotifyPropertyChanged
	{
		INavigation _navigation;
        private MaterialMasterDashboard _dashboard;
        public MaterialMasterDashboard Dashboard { get { return _dashboard; } set { _dashboard = value; OnPropertyChanged("Dashboard"); } }

        public DashboardViewModel(INavigation navigation)
		{
			_navigation = navigation;
            Dashboard = App.MaterialMasterDb.GetMaterialMasterDashboard().FirstOrDefault();
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

