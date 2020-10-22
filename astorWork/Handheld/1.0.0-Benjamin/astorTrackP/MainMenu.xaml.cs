using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using _model = astorTrackPAPIDataModel;
using Plugin.Toasts;

namespace astorTrackP
{
	public partial class MainMenu : ContentPage
	{
        public MainMenu()
        {
            InitializeComponent();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetTitleIcon(this, "logowhite.png");
            var isInitialized = App.rfidReader.ReaderInitialized();
            if (!isInitialized)
                isInitialized = App.InitializeRFID();

            Image uxBox = new Image { Source = "background.png", IsEnabled = false, HorizontalOptions= LayoutOptions.Center, VerticalOptions = LayoutOptions.Center};
            //if (_module.Where (i => i.ParentModule == "Enrolment").Count () > 0) {
            Image uxEnroll = new Image { Source = "enrolled.png", VerticalOptions= LayoutOptions.Center, HorizontalOptions= LayoutOptions.Center };
            uxEnroll.ScaleTo(0.60, 50, Easing.CubicIn);
            uxEnroll.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(o => Device.BeginInvokeOnMainThread(async () =>
                {
                    await uxEnroll.ScaleTo(0.65, 50, Easing.CubicOut);
                    await uxEnroll.ScaleTo(0.60, 50, Easing.CubicIn);
                    if (App.SettingsDb.GetItemsCount() > 0)
                        await Navigation.PushAsync(new EnrollPending());
                    else
                        App.ShowMessage(ToastNotificationType.Warning, "RFID Setup", "Setup not defined.");
                })
            )
            });            
            uxMenu.Children.Add(uxBox, 1, 1);
            uxMenu.Children.Add(uxEnroll, 1, 1);
            uxMenu.Children.Add(new Label { Text = "Enrolled", Margin = new Thickness(0, 70, 0, 0), TextColor = Color.White, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center}, 1, 1);

            //}

                //if (_module.Where (i => i.ParentModule == "Issue").Count () > 0) {
                Image uxDeliver = new Image { Source = "delivered.png" };
            uxDeliver.ScaleTo(0.60, 50, Easing.CubicIn);
            uxDeliver.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(o => Device.BeginInvokeOnMainThread(async () =>
                {
                    await uxDeliver.ScaleTo(0.65, 50, Easing.CubicOut);
                    await uxDeliver.ScaleTo(0.60, 50, Easing.CubicIn);
                    if (App.SettingsDb.GetItemsCount() > 0)
                        await Navigation.PushAsync(new DeliveredRFID());
                    else
                        App.ShowMessage(ToastNotificationType.Warning, "RFID Setup", "Setup not defined.");
                })
            )
            });


            uxBox = new Image { Source = "background.png", IsEnabled = false };
            uxMenu.Children.Add(uxBox, 2, 1);
            uxMenu.Children.Add(uxDeliver, 2, 1);
            uxMenu.Children.Add(new Label { Text = "Delivered", Margin= new Thickness( 0, 70, 0, 0 ), TextColor = Color.White, HorizontalOptions = LayoutOptions.Center , VerticalOptions = LayoutOptions.Center }, 2, 1);
            //}

            //if (_module.Where (i => i.ParentModule == "Lay").Count () > 0) {
            Image uxInstall = new Image { Source = "installed.png" };
            uxInstall.ScaleTo(0.60, 50, Easing.CubicIn);
            uxInstall.GestureRecognizers.Add(new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1,
                Command = new Command(o => Device.BeginInvokeOnMainThread(async () =>
                {
                    await uxInstall.ScaleTo(0.65, 50, Easing.CubicOut);
                    await uxInstall.ScaleTo(0.60, 50, Easing.CubicIn);
                    if (App.SettingsDb.GetItemsCount() > 0)
                        await Navigation.PushAsync(new InstalledRFID());
                    else
                        App.ShowMessage(ToastNotificationType.Warning, "RFID Setup", "Setup not defined.");
                })
            )
            });

            uxBox = new Image { Source = "background.png", IsEnabled = false };
            uxMenu.Children.Add(uxBox, 3, 1);
            uxMenu.Children.Add(uxInstall, 3, 1);
            uxMenu.Children.Add(new Label { Text = "Installed", Margin = new Thickness(0, 70, 0, 0), TextColor = Color.White, HorizontalOptions = LayoutOptions.Center , VerticalOptions = LayoutOptions.Center }, 3, 1);
            //}

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
            while (Navigation.NavigationStack.Count() > 1)
            {
                if (Navigation.NavigationStack.FirstOrDefault().Title != "Main Menu")
                    Navigation.PopAsync();
                else
                    break;
            }
            base.OnAppearing();
        }
    }
}

