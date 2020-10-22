using System;
using Xamarin.Forms;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using Plugin.Toasts;
using System.Threading.Tasks;
using Acr.UserDialogs;

namespace astorTrackP
{
	public class LoginSetupViewModel : INotifyPropertyChanged
	{
		string _endPoint;
		bool _rememberMe;
		string _userName;
		string _password;
        bool _isEnabled;


        INavigation _navigation;

		public string EndPoint { get { return _endPoint; } set { _endPoint = value; SaveEndPoint(); OnPropertyChanged("EndPoint"); } }
		public bool RememberMe { get { return _rememberMe; } set { _rememberMe = value; OnPropertyChanged("RememberMe"); } }
		public string UserName { get { return _userName; } set { _userName = value; OnPropertyChanged("UserName"); } }
		public string Password { get { return _password; } set { _password = value; OnPropertyChanged("Password"); } }
        public bool IsEnabledProperty { get { return _isEnabled; } set { _isEnabled = value; OnPropertyChanged("IsEnabledProperty"); } }
       
        private Xamarin.Forms.Page _page { get; set; }
        public ICommand LoginCommand { get { return new Command(async () => await OnLoginCommand(),()=>true); } }

		public LoginSetupViewModel(Xamarin.Forms.Page page)
		{
            IsEnabledProperty = true;
            _page = page;            
			_navigation = page.Navigation;
			if (App.LoginDb.GetItemsCount() > 0)
			{
				var data = App.LoginDb.GetItem();
				EndPoint = data.EndPoint;
				RememberMe = data.RememberMe;
				if (RememberMe)
				{
					UserName = data.UserName;
					Password = data.Password;
				}
			}
		}

		async Task OnLoginCommand()
		{
            IsEnabledProperty = false;

            if (App.LoginDb.GetItemsCount() == 0)
                App.ShowMessage(ToastNotificationType.Error, "Endpoint", "Please setup EndPoint.");
            else
			{
                App.ShowLoading("Authenticating...", MaskType.Gradient);
                if(App.LoginDb.GetItem().EndPoint == "demo")
                    App.api.Endpoint = "http://astortrack.cloudapp.net/astorWorkAPI_Demo"; //demo
                else if (App.LoginDb.GetItem().EndPoint == "greatearth")
                    App.api.Endpoint = "http://astortrack.cloudapp.net/astorWorkAPI"; //demo
                else
                    App.api.Endpoint = "http://astortrack.cloudapp.net/astorTrackPAPI"; //demo

                var valid = await Task.Run<bool>(() => Validate());
               
                if (valid)
                {
                    App.HideLoading();
                    await Task.Run(() => GetLocation());
                    Device.BeginInvokeOnMainThread(() => RedirectPage());                    
                }
            }
        }
        

        private void GetLocation()
        {
            App.LocationMasterDb.ClearItems();
            var data = App.api.GetLocationHeirarchy(long.Parse(App.LoginDb.GetItem().RoleLocationID));
            if (data != null)
            {
                foreach (var item in data)
                {
                    App.LocationMasterDb.InsertItem(new LocationMaster
                    {
                        AssociationID = item.AssociationID,
                        Parents = item.Parents,
                        ChildDescription = item.ChildDescription,
                        ParentDescription = item.ParentDescription,
                        ChildType =item.ChildType,
                        ParentType = item.ParentType
                    });
                }
            }
        }

        private bool Validate()
		{
			bool valid = false;
			if (UserName == "" || UserName ==null)
				App.ShowMessage(ToastNotificationType.Error,"Login failed","Please enter user name.");
			else if (Password == "" || Password == null)
                App.ShowMessage(ToastNotificationType.Error, "Login failed", "Please enter password.");
            else
			{
				valid = App.api.ValidateUserExistForMobile(UserName, Password);
				var item = App.LoginDb.GetItem();

				if (!valid)
				{
					item.LoginAttempt++;
                    App.ShowMessage(ToastNotificationType.Error, "Login failed", "Please enter correct credential.Login attempt: " + item.LoginAttempt.ToString());                    
				}
				else
				{
					var details = App.api.GetUserDetailsAsyncForMobile(UserName);
					if (details != null)
					{
						item.RoleID = details.UserRoleID;
						item.UserID = details.UserID;						
						item.IsContractor = details.IsContractorRole;
						item.RoleLocationID = details.RoleLocationID;
					}

					item.UserName = UserName;
					item.Password = Password;
					item.RememberMe = RememberMe;
					item.DeviceID = App.deviceInfo.DeviceId;
					item.LoginAttempt = 0;
				}
				
				App.LoginDb.UpdateItem(item);

			}

            return valid;
            
		}

        private void RedirectPage()
        {
            _navigation.InsertPageBefore(new MainMenu(), _page);
            _navigation.PopAsync();            
        }

		public void SaveEndPoint()
		{
            if (App.LoginDb.GetItemsCount() == 0)
                App.LoginDb.InsertItem(new Login { EndPoint = _endPoint, DeviceID = App.deviceInfo.DeviceId });
            else
            {
                var item = App.LoginDb.GetItem();
                item.EndPoint = _endPoint;
                App.LoginDb.UpdateItem(item);
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

