using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using _model = astorTrackPAPIDataModel;
using Plugin.Toasts;
using System.Threading.Tasks;

namespace astorTrackP
{
	public partial class MainMenu : ContentPage
	{
        public MainMenu()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetTitleIcon(this, "logowhite.png");

            DisplayMenu();
        }

        private void DisplayMenu()
        {
            var username = App.LoginDb.GetItem().UserName;
            var childtype = App.LocationMasterDb.GetItem(long.Parse(App.LoginDb.GetItem().RoleLocationID)).ChildType.ToLower();

            Image uxDashboard = new Image { Source = "dashboard.png", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            uxDashboard.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(o => Device.BeginInvokeOnMainThread(async () =>
                {
                    await uxDashboard.ScaleTo(0.80, 100, Easing.CubicOut);
                    await uxDashboard.ScaleTo(0.90, 100, Easing.CubicIn);

                    if (App.SettingsDb.GetItemsCount() > 0)
                    {
                        App.GetMaterialMasterDashboard();
                        //await Task.Run(() => App.GetMaterialMasterDashboard());
                        await Navigation.PushAsync(new Dashboard());
                    }
                    else
                        App.ShowMessage(ToastNotificationType.Warning, "RFIDTag Reader", "Output Power is not defined in Settings Page.");
                })
            )
            });

            uxDashboard.ScaleTo(0.90, 100, Easing.CubicIn);
            uxMenu.Children.Add(uxDashboard, 0, 0);

            Image uxInformation = new Image { Source = "information.png", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            uxInformation.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(o => Device.BeginInvokeOnMainThread(async () =>
                {
                    await uxInformation.ScaleTo(0.80, 100, Easing.CubicOut);
                    await uxInformation.ScaleTo(0.90, 100, Easing.CubicIn);
                    if (App.SettingsDb.GetItemsCount() > 0)
                        await Navigation.PushAsync(new InformationRFID());
                    else
                        App.ShowMessage(ToastNotificationType.Warning, "RFIDTag Reader", "Output Power is not defined in Settings Page.");
                })
            )
            });

            uxInformation.ScaleTo(0.90, 100, Easing.CubicIn);
            uxMenu.Children.Add(uxInformation, 1, 0);

            Image uxInventory = new Image { Source = "inventory.png", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            uxInventory.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(o => Device.BeginInvokeOnMainThread(async () =>
                {
                    await uxInventory.ScaleTo(0.80, 100, Easing.CubicOut);
                    await uxInventory.ScaleTo(0.90, 100, Easing.CubicIn);
                    if (App.SettingsDb.GetItemsCount() > 0)
                    {
                        if (childtype == "vendor" || username == "admin")
                        {
                            await Task.Run(() => App.GetMaterialMasterMarkingNo(false));
                            await Navigation.PushAsync(new EnrollMarkingNo());
                        }
                        else
                            App.ShowMessage(ToastNotificationType.Info, "Authorization", "User is not authorized to access.");
                    }
                    else
                        App.ShowMessage(ToastNotificationType.Warning, "RFIDTag Reader", "Output Power is not defined in Settings Page.");
                })
            )
            });

            uxInventory.ScaleTo(0.90, 100, Easing.CubicIn);
            uxMenu.Children.Add(uxInventory, 0, 1);                       

            Image uxProduced = new Image { Source = "produced.png", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            uxProduced.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(o => Device.BeginInvokeOnMainThread(async () =>
                {
                    await uxProduced.ScaleTo(0.80, 100, Easing.CubicOut);
                    await uxProduced.ScaleTo(0.90, 100, Easing.CubicIn);
                    if (App.SettingsDb.GetItemsCount() > 0)
                    {
                        if (childtype == "vendor" || username == "admin")
                        {
                            await Task.Run(() => App.GetMaterialMasterMarkingNo(true));
                            //await Task.Run(() => App.GetMaterialList("Requested"));
                            await Navigation.PushAsync(new ProducedRFID());
                        }
                        else
                            App.ShowMessage(ToastNotificationType.Info, "Authorization", "User is not authorized to access.");
                    }
                    else
                        App.ShowMessage(ToastNotificationType.Warning, "RFIDTag Reader", "Output Power is not defined in Settings Page.");
                })
            )
            });

            uxProduced.ScaleTo(0.90, 100, Easing.CubicIn);
            uxMenu.Children.Add(uxProduced, 1, 1);

            Image uxDeliver = new Image { Source = "delivered.png", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            uxDeliver.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(o => Device.BeginInvokeOnMainThread(async () =>
                {
                    await uxDeliver.ScaleTo(0.80, 100, Easing.CubicOut);
                    await uxDeliver.ScaleTo(0.90, 100, Easing.CubicIn);
                    if (App.SettingsDb.GetItemsCount() > 0)
                    {

                        if (childtype == "vendor")
                            App.ShowMessage(ToastNotificationType.Info, "Authorization", "User is not authorized to access.");
                        else
                        {
                            await Task.Run(() => App.GetMaterialList("Produced"));
                            await Navigation.PushAsync(new DeliveredRFID());
                        }
                    }
                    else
                        App.ShowMessage(ToastNotificationType.Warning, "RFIDTag Reader", "Output Power is not defined in Settings Page.");
                })
            )
            });

            uxDeliver.ScaleTo(0.90, 100, Easing.CubicIn);
            uxMenu.Children.Add(uxDeliver, 0, 2);

            Image uxInstall = new Image { Source = "installed.png", VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
            uxInstall.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(o => Device.BeginInvokeOnMainThread(async () =>
                {
                    await uxInstall.ScaleTo(0.80, 100, Easing.CubicOut);
                    await uxInstall.ScaleTo(0.90, 100, Easing.CubicIn);
                    if (App.SettingsDb.GetItemsCount() > 0)
                    {
                        if (childtype == "vendor")
                            App.ShowMessage(ToastNotificationType.Info, "Authorization", "User is not authorized to access.");
                        else
                        {
                            await Task.Run(() => App.GetMaterialList("Delivered"));
                            await Navigation.PushAsync(new InstalledRFID());
                        }
                    }
                    else
                        App.ShowMessage(ToastNotificationType.Warning, "RFIDTag Reader", "Output Power is not defined in Settings Page.");
                })
            )
            });

            uxInstall.ScaleTo(0.90, 100, Easing.CubicIn);
            uxMenu.Children.Add(uxInstall, 1, 2);
        }

        protected void  Logout_Clicked(Object sender, EventArgs e)
		{            
            this.Navigation.PushAsync (new LoginPage ());
		}

		protected void OpenSettings(Object sender, EventArgs e)
		{
			this.Navigation.PushAsync(new RFIDSettings());
		}

        protected override void OnAppearing()
        {
            Task.Run(() => InitializeReader());
            while (Navigation.NavigationStack.Count() > 1)
            {
                if (Navigation.NavigationStack.FirstOrDefault().Title != "Main Menu")
                    Navigation.PopAsync();
                else
                    break;
            }
            //App.rfidReader.StopInventory();
            base.OnAppearing();
        }

        public static void InitializeReader()
        {
            var isInitialized = App.rfidReader.ReaderInitialized();
            if (!isInitialized)
            {
                App.ShowLoading("Initializing Reader...", Acr.UserDialogs.MaskType.Clear);                
                var result = App.rfidReader.InitializeReaderAsync().Result;
                var rfid = App.SettingsDb.GetItem();
                if (rfid != null)
                    if (rfid.RFIDPower > 0)
                        App.rfidReader.SetRFIDPowerAsync(rfid.RFIDPower);
                App.HideLoading();
            }

        }

        
    }
}

