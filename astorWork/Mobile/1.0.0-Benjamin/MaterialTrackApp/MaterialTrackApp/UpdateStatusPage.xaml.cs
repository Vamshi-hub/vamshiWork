using MaterialTrackApp.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateStatusPage : ContentPage
    {
        public ObservableCollection<BeaconEntity> Items { get; set; }

        public UpdateStatusPage()
        {
            InitializeComponent();

        }

        public void InitItems(IList<BeaconEntity> listBeacons)
        {
            Items = new ObservableCollection<BeaconEntity>(listBeacons);

            PendingList.ItemsSource = Items;
        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await DisplayAlert("Item Tapped", "An item was tapped.", "OK");

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }
    }
}
