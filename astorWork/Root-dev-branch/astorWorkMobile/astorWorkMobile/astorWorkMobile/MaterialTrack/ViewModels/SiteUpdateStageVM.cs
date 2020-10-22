using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using astorWorkMobile.Shared.Utilities;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class SiteUpdateStageVM : MasterVM
    {
        public bool DisplayButton
        {
            get
            {
                if (Convert.ToInt32(Application.Current.Properties["entry_point"]) == 5)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        private bool _showContent { get; set; }
        public bool ShowContent { get { return _showContent; } set { _showContent = value; OnPropertyChanged("ShowContent"); } }
        private ObservableCollection<Location> _locations;
        public ObservableCollection<Location> Locations
        {
            get
            {
                if (_locations != null && _locations.Count == 1)
                {
                    SelectedLocation = _locations[0];
                }
                else
                {
                    SelectedLocation = null;
                }
                return _locations;
            }
            set
            {
                _locations = value;
                OnPropertyChanged("Locations");
            }
        }

        public bool ShowVendorOptions
        {
            get
            {
                int vendorId = 0;
                if (Application.Current.Properties.ContainsKey("organisationID"))
                {
                    vendorId = (int)Application.Current.Properties["organisationID"];
                }

                return vendorId > 0;
            }
        }

        public bool ShowCastingDate => Material != null && Material.currentStage == null;

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

        private bool _updateLocationOnly;
        public bool UpdateLocationOnly
        {
            get => _updateLocationOnly;
            set
            {
                _updateLocationOnly = value;
                OnPropertyChanged("UpdateLocationOnly");
                OnPropertyChanged("UpdateModeLabel");
            }
        }

        public string UpdateModeLabel
        {
            get
            {
                if (_updateLocationOnly)
                {
                    return "Update Location";
                }
                else
                {
                    return "Update Stage";
                }
            }
        }
        public bool _showUpdateForm { get; set; } = true;
        public bool ShowUpdateForm
        {
            get
            {
                return (SelectedLocation != null && _showUpdateForm);
            }
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

        public Location CurrLocation
        {
            get
            {
                if (Material.CurrentLocation == null)
                {
                    return new Location { Name = "N.A." };
                }
                else
                {
                    return Material.CurrentLocation;
                }
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

        public async Task<int> UpdateMaterialStage()
        {
            int stageAuditId = 0;
            ApiClient.ApiResult result = null;

            if (CastingDate > DateTime.MinValue)
            {
                result = await ApiClient.Instance.MTUpdateMaterialStage(Material.id, NextStage.ID, SelectedLocation.Id, CastingDate);
            }
            else
            {
                result = await ApiClient.Instance.MTUpdateMaterialStage(Material.id, NextStage.ID, SelectedLocation.Id, null);
            }

            if (result.status != 0)
            {
                ErrorMessage = result.message;
            }
            else
            {
                stageAuditId = Convert.ToInt32(result.data);
            }

            return stageAuditId;
        }

        public async Task<int> UpdateMaterialLocation()
        {
            int stageAuditId = 0;
            var result = await ApiClient.Instance.MTUpdateMaterialLocation(Material.id, SelectedLocation.Id);

            if (result.status != 0)
            {
                ErrorMessage = result.message;
            }
            else
            {
                stageAuditId = Convert.ToInt32(result.data);
            }

            return stageAuditId;
        }

        public async Task AfterUpdateMaterial(int stageAuditId)
        {
            IsLoading = false;
            if (stageAuditId > 0)
            {
                //await App.Current.MainPage.DisplayAlert("Success", string.Format("{0} with destination of {1} has been updated", Material.markingNo, Material.destination), "Done");
                //  await Navigation.PopAsync();
                ShowUpdateForm = false;
                Device.BeginInvokeOnMainThread(() => { OnPropertyChanged("ShowUpdateForm"); DisplaySnackBar(string.Format("{0} with destination of {1} has been updated", Material.markingNo, Material.destination), Enums.PageActions.PopAsync, Enums.MessageActions.Success, null, null); });
            }
            Reset();
        }

        public ICommand UpdateStageCommand { get; set; }
        public ICommand UpdateLocationCommand { get; set; }

        private void UpdateStageClicked()
        {
            if (SelectedLocation == null)
            {
                DisplaySnackBar("Please select a location", Enums.PageActions.None, Enums.MessageActions.Warning, null, null);
                return;
            }
            IsLoading = true;
            Task.Run(UpdateMaterialStage).ContinueWith((task) =>
            AfterUpdateMaterial(task.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void UpdateLocationClicked()
        {
            IsLoading = true;

            Task.Run(UpdateMaterialLocation).ContinueWith((task) =>
            AfterUpdateMaterial(task.Result), TaskScheduler.FromCurrentSynchronizationContext());
        }

        public SiteUpdateStageVM() : base()
        {
            _neverLoadBefore = false;
            UpdateStageCommand = new Command(UpdateStageClicked);
            UpdateLocationCommand = new Command(UpdateLocationClicked);
            CastingDate = DateTime.MinValue;
        }

        public override void Reset()
        {
            _neverLoadBefore = true;
            SelectedLocation = null;
            UpdateLocationOnly = false;

            CastingDate = DateTime.MinValue;
        }
    }
}
