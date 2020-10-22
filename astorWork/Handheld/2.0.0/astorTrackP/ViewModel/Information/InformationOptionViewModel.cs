using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using Plugin.Toasts;

namespace astorCable
{
	public class InformationOptionViewModel: INotifyPropertyChanged
	{
		DocumentType _docType;
		INavigation _navigation;

		public DocumentType DocumentType { get { return _docType; } set { _docType = value; OnNavigateCommand (); _docType = null; OnPropertyChanged ("DocumentType"); }}
        		
		public InformationOptionViewModel(INavigation navigation)
		{
			_navigation = navigation;
		}

        private async void OnNavigateCommand()
        {
           
            switch (_docType.Name)
            {
                case "Cable Tags":
                    await _navigation.PushAsync(new SearchCableTagNo("Information"));
                    break;
                case "Junction Box":
                    await _navigation.PushAsync(new SearchJunctionBox("Information"));
                    break;
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
	}
}

