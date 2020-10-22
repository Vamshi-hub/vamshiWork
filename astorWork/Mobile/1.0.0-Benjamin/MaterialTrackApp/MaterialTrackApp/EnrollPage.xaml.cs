using MaterialTrackApp.DB;
using MaterialTrackApp.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MaterialTrackApp.Utility.ApiClient;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EnrollPage : CarouselPage
    {
        private List<MaterialEntity> _listMaterials;
        private Dictionary<BeaconEntity, MaterialEntity> _listCurrentSelection;

        public EnrollPage()
        {
            InitializeComponent();
            InitPageModel();
        }

        public void InitPageModel()
        {
            _listMaterials = new List<MaterialEntity>();
            _listCurrentSelection = new Dictionary<BeaconEntity, MaterialEntity>();

            if (DateTime.Today.DayOfWeek.Equals(DayOfWeek.Monday))
                pickCastingDate.Date = DateTime.Today.AddDays(-2);
            else
                pickCastingDate.Date = DateTime.Today.AddDays(-1);

            _listMaterials.AddRange(ViewModelLocator.EnrollPageVM.Materials);
            CurrentPage = Children[0];
        }

        private void pickBlock_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewModelLocator.EnrollPageVM.Zones.Clear();
            ViewModelLocator.EnrollPageVM.Levels = new ObservableCollection<string>(_listMaterials
                .Where(x => x.Block == pickBlock.SelectedItem as string)
                .Select(x => x.Level).Distinct());
        }

        private void pickLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewModelLocator.EnrollPageVM.Zones = new ObservableCollection<string>(_listMaterials
                .Where(x => x.Block == pickBlock.SelectedItem as string && x.Level == pickLevel.SelectedItem as string)
                .Select(x => x.Zone).Distinct());
        }

        private void pickZone_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewModelLocator.EnrollPageVM.Materials = new ObservableCollection<MaterialEntity>(_listMaterials.Where(m => m.Block == pickBlock.SelectedItem as string && m.Level == pickLevel.SelectedItem as string && m.Zone == pickZone.SelectedItem as string));
            ViewModelLocator.EnrollPageVM.PendingMaterials = new ObservableCollection<MaterialEntity>(ViewModelLocator.EnrollPageVM.Materials);

            CurrentPage = Children[1];
        }

        private void btnSubmit_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entryLotNo.Text))
                DisplayAlert("Alert", "Please enter a lot no.", "OK");
            else
                Task.Run(SubmitBeaconUpdates);
        }

        private async Task SubmitBeaconUpdates()
        {
            List<VendorUpdateData> vendorData = new List<VendorUpdateData>();

            foreach (var beacon in ViewModelLocator.EnrollPageVM.Beacons.Where(b => b.Material != null))
            {
                try
                {
                    var data = new VendorUpdateData
                    {
                        MaterialID = beacon.Material.MaterialId,
                        BeaconID = beacon.BeaconID,
                        CastingDate = pickCastingDate.Date,
                        LotNo = int.Parse(entryLotNo.Text),
                        Status = "Produced",
                        UserName = Application.Current.Properties["user_name"].ToString(),
                        UserLocationID = int.Parse(Application.Current.Properties["role_location_id"].ToString())
                    };
                    vendorData.Add(data);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    Debug.WriteLine(exc.StackTrace);
                }
            }

            if (vendorData.Count > 0)
            {
                var result = await ApiClient.Instance.MTUpdateMaterialMasterByVendor(vendorData);
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (result.Status == 0)
                    {
                        DisplayAlert("Done", result.Message, "OK");
                    }
                    else
                    {
                        DisplayAlert("Error", result.Message, "OK");
                    }

                    Navigation.PopToRootAsync();
                    App.mainPage.NavigateDetail(0);
                });
            }
            else
            {

                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Alert", "No update data", "OK");
                });
            }
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Debug.WriteLine(e.ToString());
        }

        private void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            MaterialEntity material = (sender as Switch).BindingContext as MaterialEntity;

            if (e.Value)
                ViewModelLocator.EnrollPageVM.PendingMaterials.Remove(material);
            else
                ViewModelLocator.EnrollPageVM.PendingMaterials.Add(material);
        }

        private void pickBindMaterial_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            BeaconEntity beacon = ((Picker)sender).BindingContext as BeaconEntity;
            if (_listCurrentSelection.ContainsKey(beacon))
                ViewModelLocator.EnrollPageVM.PendingMaterials.Add(_listCurrentSelection[beacon]);

            _listCurrentSelection[beacon] = beacon.Material;
            ViewModelLocator.EnrollPageVM.PendingMaterials.Remove(beacon.Material);    
            */
        }
    }
}