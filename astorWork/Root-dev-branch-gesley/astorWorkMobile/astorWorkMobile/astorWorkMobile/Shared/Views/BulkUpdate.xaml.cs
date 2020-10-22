using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace astorWorkMobile.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BulkUpdate : ContentPage
    {
        public BulkUpdate()
        {
            InitializeComponent();
        }
        void OnPageDisappearing(Object sender, EventArgs e)
        {
            ViewModelLocator.bulkUpdateVM.Reset();
            ViewModelLocator.bulkUpdateVM.SelectedLocation = null;
            ViewModelLocator.bulkUpdateVM.ShowUpdateForm = true;
        }

        private async Task GetNextStage()
        {
            try
            {
                var result = await ApiClient.Instance.MTGetNextStageInfo(ViewModelLocator.bulkUpdateVM.Material.id);
                if (result.status == 0)
                {
                    ViewModelLocator.bulkUpdateVM.NextStage = result.data as Stage;
                }
                else
                {
                    ViewModelLocator.bulkUpdateVM.ErrorMessage = result.message;
                }
            }
            catch (Exception ex)
            {
                ViewModelLocator.bulkUpdateVM.ErrorMessage = ex.Message;
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            if (ViewModelLocator.bulkUpdateVM.Material != null)
            {
                ViewModelLocator.bulkUpdateVM.IsLoading = true;

                Task.Run(GetNextStage).ContinueWith(async (t) =>
                {
                    if (App.Current.Properties.ContainsKey("user_id"))
                    {
                        int userId = (int)App.Current.Properties["user_id"];
                        var result = await ApiClient.Instance.MTGetLocations(userId);
                        ViewModelLocator.bulkUpdateVM.Locations = new ObservableCollection<Location>(
                            result.data as List<Location>);
                        Device.BeginInvokeOnMainThread(() => { ViewModelLocator.bulkUpdateVM.OnPropertyChanged("Locations"); });
                        if (ViewModelLocator.bulkUpdateVM.Material.currentStage == null)
                            ViewModelLocator.bulkUpdateVM.CastingDate =
                                DateTime.Now.DayOfWeek.Equals(DayOfWeek.Monday) ?
                                DateTime.Today.AddDays(-2) : DateTime.Today.AddDays(-1);

                        if (ViewModelLocator.bulkUpdateVM.Material.currentStage != null &&
                        ViewModelLocator.bulkUpdateVM.Material.currentStage.MilestoneId != 3 &&
                        ViewModelLocator.bulkUpdateVM.Locations != null)
                        {
                            ViewModelLocator.bulkUpdateVM.SelectedLocation =
                                ViewModelLocator.bulkUpdateVM.Locations.Where(l => l.Id == ViewModelLocator.bulkUpdateVM.Material.CurrentLocation.Id).FirstOrDefault();
                        }
                        ViewModelLocator.bulkUpdateVM.IsLoading = false;
                    }
                });
            }
            else
                ViewModelLocator.bulkUpdateVM.ErrorMessage = "No material specified";
        }
    }
}