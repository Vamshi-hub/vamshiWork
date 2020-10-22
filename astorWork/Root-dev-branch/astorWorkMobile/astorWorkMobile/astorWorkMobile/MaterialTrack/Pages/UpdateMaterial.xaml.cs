using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.MaterialTrack.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateMaterial : ContentPage
    {
        public UpdateMaterial()
        {
            InitializeComponent();
            ViewModelLocator.siteUpdateStageVM.ShowContent = true;
            NavigationPage.SetBackButtonTitle(this, "Back");
        }

        void OnPageDisappearing(Object sender, EventArgs e)
        {
            ViewModelLocator.siteUpdateStageVM.Reset();
        }

        private async Task GetNextStage()
        {
            try
            {
                var result = await ApiClient.Instance.MTGetNextStageInfo(ViewModelLocator.siteUpdateStageVM.Material.id);
                if (result.status == 0)
                {
                    ViewModelLocator.siteUpdateStageVM.NextStage = result.data as Stage;
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ViewModelLocator.siteUpdateStageVM.ShowContent = false;
                        ViewModelLocator.siteUpdateStageVM.IsLoading = false;
                        ViewModelLocator.siteUpdateStageVM.DisplaySnackBar(result.message, Shared.Classes.Enums.PageActions.PopAsync, Shared.Classes.Enums.MessageActions.Warning, null, null);
                    });

                }
            }
            catch (Exception ex)
            {
                //  Device.BeginInvokeOnMainThread(async () => { await Navigation.PopAsync(); });
                ViewModelLocator.siteUpdateStageVM.ErrorMessage = ex.Message;
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (ViewModelLocator.siteUpdateStageVM.Material != null)
            {
                ViewModelLocator.siteUpdateStageVM.IsLoading = true;

                Task.Run(async () => { await GetNextStage(); }).ContinueWith(async (t) =>
                  {
                      if (App.Current.Properties.ContainsKey("user_id"))
                      {
                          int userId = (int)App.Current.Properties["user_id"];
                          var result = await ApiClient.Instance.MTGetLocations(userId);
                          ViewModelLocator.siteUpdateStageVM.Locations = new ObservableCollection<Location>(
                              result.data as List<Location>);
                          Device.BeginInvokeOnMainThread(() => { ViewModelLocator.siteUpdateStageVM.OnPropertyChanged("Locations"); });
                          if (ViewModelLocator.siteUpdateStageVM.Material.currentStage == null)
                              ViewModelLocator.siteUpdateStageVM.CastingDate =
                                  DateTime.Now.DayOfWeek.Equals(DayOfWeek.Monday) ?
                                  DateTime.Today.AddDays(-2) : DateTime.Today.AddDays(-1);

                          if (ViewModelLocator.siteUpdateStageVM.Material.currentStage != null &&
                          ViewModelLocator.siteUpdateStageVM.Material.currentStage.MilestoneId != 3 &&
                          ViewModelLocator.siteUpdateStageVM.Locations != null)
                          {
                              ViewModelLocator.siteUpdateStageVM.SelectedLocation =
                                  ViewModelLocator.siteUpdateStageVM.Locations.Where(l => l.Id == ViewModelLocator.siteUpdateStageVM.Material.CurrentLocation.Id).FirstOrDefault();
                          }
                          ViewModelLocator.siteUpdateStageVM.IsLoading = false;
                      }
                  });
            }
            else
                ViewModelLocator.siteUpdateStageVM.ErrorMessage = "No material specified";
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new DummyMainPage());
        }
    }
}