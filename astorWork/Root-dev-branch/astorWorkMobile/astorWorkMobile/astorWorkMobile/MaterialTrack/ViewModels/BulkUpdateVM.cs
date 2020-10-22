using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class BulkUpdateVM : MasterVM
    {
        private ObservableCollection<Location> _locations;
        public ObservableCollection<Location> Locations
        {
            get
            {
                if (_locations != null && _locations.Count == 1)
                {
                    SelectedLocation = _locations[0];
                }
                return _locations;
            }
            set
            {
                _locations = value;
                OnPropertyChanged("Locations");
            }
        }

        private Location _selectedLocation;
        public Location SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                _selectedLocation = value;
                OnPropertyChanged("ShowUpdateForm");
                OnPropertyChanged("SelectedLocation");
            }
        }
        public bool _showUpdateForm { get; set; } = true;
        public bool ShowUpdateForm
        {
            get { return SelectedLocation != null && _showUpdateForm && Convert.ToInt32(Application.Current.Properties["entry_point"]) != 5; }
            set { _showUpdateForm = value; }
        }

        public Material Material { get; set; }

        public Stage CurrStage
        {
            get
            {
                if (Material.currentStage == null)
                {
                    return new Stage { Name = "Ordered", Colour = "#008000" };
                }
                else
                {
                    return Material.currentStage;
                }
            }
        }

        private Stage _nextStage;
        public Stage NextStage
        {
            get => _nextStage;
            set
            {
                _nextStage = value;
                OnPropertyChanged("NextStage");
            }
        }

        private DateTime _castingDate;
        public DateTime CastingDate
        {
            get => _castingDate;
            set
            {
                _castingDate = value;
                OnPropertyChanged("CastingDate");
            }
        }

        public int[] MaterialIds { get; set; }

        public ICommand UpdateStageCommand { get; set; }

        private void UpdateStageClicked()
        {
            IsLoading = true;

            Task.Run(UpdateMaterialStage).ContinueWith((task) =>
            AfterUpdateMaterial(task.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }
        public async Task<int> UpdateMaterialStage()
        {
            int countUpdates = 0;
            ApiClient.ApiResult result = null;
            if (CastingDate > DateTime.MinValue)
            {
                result = await ApiClient.Instance.MTUpdateMaterialsStage(MaterialIds, NextStage.ID, SelectedLocation.Id, CastingDate);
            }
            else
            {
                result = await ApiClient.Instance.MTUpdateMaterialsStage(MaterialIds, NextStage.ID, SelectedLocation.Id, null);
            }

            if (result.status != 0)
            {
                ErrorMessage = result.message;
            }
            else
            {
                countUpdates = Convert.ToInt32(result.data);
            }

            return countUpdates;
        }

        public async Task AfterUpdateMaterial(int countUpdates)
        {
            if (countUpdates > 0)
            {
                IsLoading = false;
                // await App.Current.MainPage.DisplayAlert("Success", $"{countUpdates} Materials have been updated successfully", "Done");
                ShowUpdateForm = false;
                Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("ShowUpdateForm"); DisplaySnackBar($"{countUpdates} Materials have been updated successfully", Enums.PageActions.PopAsync, Enums.MessageActions.Success, null, null); });
            }
        }

        public BulkUpdateVM() : base()
        {
            _neverLoadBefore = false;
            UpdateStageCommand = new Command(UpdateStageClicked);
            CastingDate = DateTime.MinValue;
        }
    }
}
