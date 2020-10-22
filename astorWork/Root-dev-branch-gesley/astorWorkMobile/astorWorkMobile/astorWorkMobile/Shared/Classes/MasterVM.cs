using Newtonsoft.Json;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.Shared.Classes
{
    public class MasterVM : INotifyPropertyChanged
    {
        private double pageWidth = 0;
        private Color bgColor = Color.Transparent;
        private string strMessageIcon = string.Empty;
        private Page page = null;
        private Page CurrentPage = null;
        private PageActions action = 0;
        private Button btnOK = null;
        private Grid ParentGrid = null;
        private StackLayout snak = null;
        [JsonIgnore]
        public Page MainPage
        {
            get
            {
                if (Application.Current.MainPage.GetType().IsSubclassOf(typeof(MasterDetailPage)))
                {
                    return (Application.Current.MainPage as MasterDetailPage).Detail;
                }
                else
                {
                    return Application.Current.MainPage;
                }
            }
        }

        [JsonIgnore]
        public INavigation Navigation => MainPage.Navigation;

        private bool _loading;

        [JsonIgnore]
        public bool IsLoading
        {
            get => _loading;
            set
            {
                _loading = value;

                if (_loading)
                {
                    _neverLoadBefore = false;
                    ErrorMessage = string.Empty;
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    OnPropertyChanged("IsLoading");
                    OnPropertyChanged("ShowLoadingView");
                    OnPropertyChanged("ShowContent");
                });
            }
        }

        [JsonIgnore]
        public bool ShowLoadingView => _loading || !string.IsNullOrEmpty(_errorMessage);

        [JsonIgnore]
        public bool ShowContent => !_neverLoadBefore && !ShowLoadingView;

        private string _errorMessage;
        private string _warningMessage;
        private string _successMessage;

        [JsonIgnore]
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                if (!string.IsNullOrEmpty(_errorMessage))
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        // await Application.Current.MainPage.DisplayAlert("Error", _errorMessage, "OK");
                        DisplaySnackBar(_errorMessage, PageActions.None, MessageActions.Error, null, null);
                        // ErrorMessage = string.Empty;
                        IsLoading = false;
                    });
                }
            }
        }

        [JsonIgnore]
        public string WarningMessage
        {
            get => _warningMessage;
            set
            {
                _warningMessage = value;
                if (!string.IsNullOrEmpty(_warningMessage))
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        // await Application.Current.MainPage.DisplayAlert("Error", _errorMessage, "OK");
                        DisplaySnackBar(_warningMessage, PageActions.None, MessageActions.Warning, null, null);
                        ErrorMessage = string.Empty;
                        IsLoading = false;
                    });
                }
            }
        }

        [JsonIgnore]
        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                if (!string.IsNullOrEmpty(_successMessage))
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        // await Application.Current.MainPage.DisplayAlert("Error", _errorMessage, "OK");
                        DisplaySnackBar(_warningMessage, PageActions.None, MessageActions.Success, null, null);
                        ErrorMessage = string.Empty;
                        IsLoading = false;
                    });
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public static event Action ViewCellSizeChangedEvent;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName.Equals("IsExpanded"))
            {
                ViewCellSizeChangedEvent?.Invoke();
            }
        }

        protected bool _neverLoadBefore = true;

        public ICommand NotificationButtonCommand { get; set; }

        public void NotificationButtonClicked()
        {
            Navigation.PushAsync(new DummyMainPage());
        }
        public virtual void Reset()
        {
            IsLoading = false;
            ErrorMessage = string.Empty;
            _neverLoadBefore = true;
        }
        public async void DisplaySnackBar(string message, PageActions action, MessageActions messageAction, Page page, object pageParam)
        {
            try
            {
                this.action = action;
                this.page = page;
                CurrentPage = Navigation.NavigationStack.LastOrDefault();
                if (CurrentPage == null)
                {
                    return;
                }
                var pageType = CurrentPage.GetType().BaseType.FullName.Split('.').LastOrDefault();
                if (pageType == "TabbedPage")
                {
                    pageWidth = ((TabbedPage)CurrentPage).CurrentPage.Width;
                    CurrentPage = ((TabbedPage)CurrentPage).CurrentPage;
                    ParentGrid = (Grid)((ContentPage)CurrentPage).Content;
                }
                else if (pageType == "MasterDetailPage")
                {
                    pageWidth = ((ContentPage)((NavigationPage)((MasterDetailPage)CurrentPage).Detail).CurrentPage).Width;
                    CurrentPage = (ContentPage)((NavigationPage)((MasterDetailPage)CurrentPage).Detail).CurrentPage;
                    ParentGrid = (Grid)((ContentPage)CurrentPage).Content;
                }
                else
                {
                    pageWidth = ((ContentPage)CurrentPage).Width;
                    CurrentPage = (ContentPage)CurrentPage;
                    ParentGrid = (Grid)((ContentPage)CurrentPage).Content;
                }
                if (ParentGrid != null)
                {
                    switch (messageAction)
                    {
                        case MessageActions.Success:
                            bgColor = Color.FromHex("#43a047");
                            strMessageIcon = "ic_ok.png";
                            break;
                        case MessageActions.Warning:
                            bgColor = Color.FromHex("#ffa000");
                            strMessageIcon = "ic_warn.png";
                            break;
                        case MessageActions.Error:
                            bgColor = Color.FromHex("#d32f2f");
                            strMessageIcon = "ic_warn.png";
                            break;

                    }
                    var snackBarView = ParentGrid.Children.Where(p => p.ClassId == "snackBar").FirstOrDefault();
                    if (snackBarView != null)
                    {
                        snak = (StackLayout)snackBarView;
                        snak.BackgroundColor = bgColor;
                        btnOK = (Button)((StackLayout)snak.Children[0]).Children.Where(p => p.ClassId == "btnOK").FirstOrDefault();
                        ((Label)((StackLayout)snak.Children[0]).Children.Where(p => p.ClassId == "lblMessage").FirstOrDefault()).Text = message;
                        ((Image)((StackLayout)snak.Children[0]).Children.Where(p => p.ClassId == "messageIcon").FirstOrDefault()).Source = strMessageIcon;
                        btnOK.Clicked -= BtnOK_Clicked;
                    }
                    else
                    {
                        snak = new StackLayout() { ClassId = "snackBar", HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.EndAndExpand, BackgroundColor = bgColor, HeightRequest = 0 };
                        StackLayout stkMessage = new StackLayout() { Padding = new Thickness(3, 0, 3, 0), Orientation = StackOrientation.Horizontal };
                        Image messageIcon = new Image() { Source = strMessageIcon, ClassId = "messageIcon", VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.Start, HeightRequest = 20, WidthRequest = 20 };
                        Label lblMessage = new Label() { Text = message, TextColor = Color.White, ClassId = "lblMessage", VerticalOptions = LayoutOptions.CenterAndExpand, HorizontalOptions = LayoutOptions.StartAndExpand, LineBreakMode = LineBreakMode.WordWrap };
                        btnOK = new Button() { HeightRequest = 50, WidthRequest = 60, Text = "OK", ClassId = "btnOK", HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.CenterAndExpand, BorderColor = Color.Transparent, TextColor = Color.White, BackgroundColor = Color.Transparent };
                        stkMessage.Children.Add(messageIcon);
                        stkMessage.Children.Add(lblMessage);
                        stkMessage.Children.Add(btnOK);
                        snak.Children.Add(stkMessage);
                        if (SynchronizationContext.Current == null)
                        {
                            Device.BeginInvokeOnMainThread(() => { ParentGrid.Children.Add(snak); });
                        }
                        else
                        {
                            ParentGrid.Children.Add(snak);
                        }
                    }
                    btnOK.Clicked += BtnOK_Clicked;
                    await snak.LayoutTo(new Rectangle(0, CurrentPage.Height - 55, pageWidth, CurrentPage.Height), 200, Easing.Linear);
                }
            }
            catch (System.Exception ex)
            {
                var exec = ex;
                return;
            }
        }

        private async void BtnOK_Clicked(object sender, System.EventArgs e)
        {
            View snackBarView = ParentGrid.Children.Where(p => p.ClassId == "snackBar").FirstOrDefault();
            if (snackBarView != null)
            {
                snak = (StackLayout)snackBarView;
            }
            await snak.LayoutTo(new Rectangle(0, CurrentPage.Height, pageWidth, CurrentPage.Height), 200, Easing.Linear);
            switch (action)
            {
                case PageActions.PushAsync:
                    await Navigation.PushAsync(page);
                    break;
                case PageActions.PopAsync:
                    await Navigation.PopAsync();
                    break;
                case PageActions.None:
                    break;
                default:
                    break;
            }
        }

        private async Task BtnOK_Click()
        {
            View snackBarView = ParentGrid.Children.Where(p => p.ClassId == "snackBar").FirstOrDefault();
            if (snackBarView != null)
            {
                snak = (StackLayout)snackBarView;
            }
            await snak.LayoutTo(new Rectangle(0, CurrentPage.Height, pageWidth, CurrentPage.Height), 200, Easing.Linear);
            switch (action)
            {
                case PageActions.PushAsync:
                    await Navigation.PushAsync(page);
                    break;
                case PageActions.PopAsync:
                    await Navigation.PopAsync();
                    break;
                case PageActions.None:
                    break;
                default:
                    break;
            }

            // return snackBarView;
        }

        public async Task<bool> GetCameraPermission()
        {
            var granted = false;
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
                if (status != PermissionStatus.Granted)
                {
                    await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera);

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Camera))
                    {
                        status = results[Permission.Camera];
                    }
                }

                if (status == PermissionStatus.Granted)
                {
                    granted = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                granted = false;
            }

            return granted;
        }
        public MasterVM()
        {
            IsLoading = false;
            ErrorMessage = string.Empty;
        }
    }
}
