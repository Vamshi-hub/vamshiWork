using FormsPlugin.Iconize;
using MaterialTrackApp.DB;
using MaterialTrackApp.Utility;
using Newtonsoft.Json.Linq;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTrackApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScanBeaconPage : ContentPage
    {
        private IAdapter adapter;
        private CancellationTokenSource ctsScanTask;
        private List<BeaconEntity> listBeacons;

        private EnrollPage enrollPage;

        private static readonly string[] LIST_PROXIMITY_UUID = {
            "61687109905F443691F8E602F514C96D",
            "B9407F30F5F8466EAFF925556B57FE6D"
        };

        public ScanBeaconPage()
        {
            InitializeComponent();
            InitPage();
        }

        private void InitPage()
        {
            listBeacons = new List<BeaconEntity>();
            ctsScanTask = new CancellationTokenSource();

            var ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;

            if (ble.IsAvailable)
            {
                ble.StateChanged += Ble_StateChanged;
                //adapter.ScanTimeout = 30 * 1000;
                adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
                adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;

                if (ble.State.Equals(BluetoothState.On))
                    btnToggleScan.IsVisible = true;
                else
                    promptBluetooth.IsVisible = true;
            }
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                btnToggleScan.Text = "Start";
                btnToggleScan.StyleClass = new string[] { "Primary" };
                if (listBeacons.Count > 0)
                {
                    btnNext.IsVisible = true;
                    btnNext.IsEnabled = true;
                }
            });
        }

        private void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            if (e.Device.Rssi < -90)
                return;

            Debug.WriteLine("<" + e.Device.Name + ">");
            Debug.WriteLine("<" + e.Device.Rssi.ToString() + ">");
            foreach (var record in e.Device.AdvertisementRecords)
            {
                var recordText = BitConverter.ToString(record.Data).Replace("-", "");
                /*
                Debug.WriteLine("--Length: " + record.Data.Length.ToString());
                Debug.WriteLine("--Data: " + recordText);
                Debug.WriteLine("--Text: " + Encoding.UTF8.GetString(record.Data, 0, record.Data.Length));
                */

                string proximity_uuid = null;
                foreach (var uuid in LIST_PROXIMITY_UUID)
                {
                    if (recordText.Contains(uuid))
                        proximity_uuid = uuid;
                }

                if (!string.IsNullOrEmpty(proximity_uuid))
                {
                    byte[] uuidData = record.Data.Reverse().ToArray();
                    ushort major = BitConverter.ToUInt16(uuidData, 3);
                    ushort minor = BitConverter.ToUInt16(uuidData, 1);
                    var name = major.ToString() + "-" + minor.ToString();
                    if (!listBeacons.Exists(b => b.ProximityUUID == proximity_uuid && b.Major == major && b.Minor == minor))
                    {
                        var beacon = new BeaconEntity()
                        {
                            DisplayName = name,
                            ProximityUUID = proximity_uuid,
                            Major = major,
                            Minor = minor,
                            BeaconID = string.Format("{0}-{1}-{2}", proximity_uuid, major, minor)
                        };
                        listBeacons.Add(beacon);

                        Device.BeginInvokeOnMainThread(() =>
                        {
                            lblCountBeacon.Text = listBeacons.Count.ToString();
                        });
                    }
                    break;
                }
            }
        }

        private void Ble_StateChanged(object sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                switch (e.NewState)
                {
                    case BluetoothState.On:
                        btnToggleScan.IsVisible = true;
                        promptBluetooth.IsVisible = false;
                        break;
                    default:
                        btnToggleScan.IsVisible = false;
                        promptBluetooth.IsVisible = true;
                        break;
                }
            });
        }

        private void btnToggleScan_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (adapter.IsScanning)
                {
                    adapter.StopScanningForDevicesAsync();

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ViewExtensions.CancelAnimations(icoScan);
                        btnToggleScan.Text = "Start";
                        btnToggleScan.StyleClass = new string[] { "Primary" };

                        if (listBeacons.Count > 0)
                        {
                            btnNext.IsVisible = true;
                            btnNext.IsEnabled = true;
                        }
                    });
                }
                else
                {
                    adapter.StartScanningForDevicesAsync(cancellationToken: ctsScanTask.Token);
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        icoScan.RelRotateTo(360, (uint)adapter.ScanTimeout);
                        btnToggleScan.Text = "Stop";
                        btnToggleScan.StyleClass = new string[] { "Warning" };

                        btnNext.IsVisible = false;
                        btnNext.IsEnabled = false;
                    });
                }
            }
            catch (Exception exc)
            {

            }
        }

        private void btnNext_Clicked(object sender, EventArgs e)
        {
            // Only for testing purpose
            /*
            if(listBeacons.Count == 0)
            {
                for(int i = 0; i < 6; i++)
                {
                    var beacon = new BeaconEntity
                    {
                        ProximityUUID = PROXIMITY_UUID,
                        Major = 4,
                        Minor = (ushort) (10086 + i),
                        DisplayName = "4-" + (10086 + i).ToString()
                    };
                    listBeacons.Add(beacon);
                }
            }
            */

            if (Application.Current.Properties["role_name"].Equals("Vendor"))
            {
                if (enrollPage != null)
                {
                    if (Navigation.NavigationStack.Contains(enrollPage))
                        Navigation.RemovePage(enrollPage);
                }

                var task = Task.Run(GetVendorMaterial);
                App.InvokeLoadingView(this, layoutMain, task, "Retrieving material information, please wait...");

                //App.InvokeLoadingPage(this, enrollPage, "Loading material information...", false);

            }
            else
            {
                var updateStatusPage = new UpdateStatusPage();
                updateStatusPage.InitItems(listBeacons);
                Navigation.PushAsync(updateStatusPage);
            }
        }

        private async Task GetVendorMaterial()
        {
            IEnumerable<MaterialEntity> listMaterials = null;
            try
            {
                var result = await ApiClient.Instance.MTGetMaterialMasterByStatus("Produced");

                if (result.Status == 0)
                {
                    var json = result.Data as JArray;

                    listMaterials = json.Select(j => new MaterialEntity
                    {
                        MaterialId = (int)j["MaterialNo"],
                        MarkingNo = (string)j["MarkingNo"],
                        Block = (string)j["Block"],
                        Level = (string)j["Level"],
                        Zone = (string)j["Zone"],
                        QCStatus = (string)j["QCStatus"],
                        Status = (string)j["Status"],
                        BeaconID = (string)j["BeaconID"],
                        IsInUse = false
                    });

                    string alertTemplate = "The following beacons are already in use: \n";

                    bool isBeaconInUse = false;
                    foreach (var beacon in listBeacons)
                    {
                        if (listMaterials.Where(m => m.BeaconID == beacon.BeaconID).Count() > 0)
                        {
                            alertTemplate += beacon.DisplayName + "\n";
                            isBeaconInUse = true;
                        }
                    }

                    ViewModelLocator.BeaconInfoPageVM.BeaconsInUse = new ObservableCollection<BeaconEntity>(listBeacons.Where (b => listMaterials.Select(m => m.BeaconID).Contains(b.BeaconID)));

                    listBeacons = listBeacons.Where(b => !listMaterials.Select(m => m.BeaconID).Contains(b.BeaconID)).ToList();

                    ViewModelLocator.BeaconInfoPageVM.BeaconsNotInUse = new ObservableCollection<BeaconEntity>(listBeacons.Where(b => listMaterials.Select(m => m.BeaconID).Contains(b.BeaconID)));

                    var result2 = await ApiClient.Instance.MTGetMaterialMasterByStatus("Requested");
                    if (result2.Status == 0)
                    {
                        var json2 = result2.Data as JArray;

                        listMaterials = json2.Select(j => new MaterialEntity
                        {
                            MaterialId = (int)j["MaterialNo"],
                            MarkingNo = (string)j["MarkingNo"],
                            Block = (string)j["Block"],
                            Level = (string)j["Level"],
                            Zone = (string)j["Zone"],
                            QCStatus = (string)j["QCStatus"],
                            Status = (string)j["Status"],
                            BeaconID = (string)j["BeaconID"],
                            IsInUse = false
                        });
                        listMaterials = listMaterials.Where(m => string.IsNullOrEmpty(m.BeaconID));

                        /*
                        if (listMaterials == null || listMaterials.Count() == 0)
                        {
                            // MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, "No materials requested");

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                DisplayAlert("Alert", "No materials requested", "OK");
                            });
                        }
                        else
                        {
                        */
                            ViewModelLocator.EnrollPageVM.Beacons = new ObservableCollection<BeaconEntity>(listBeacons);
                            ViewModelLocator.EnrollPageVM.Materials = new ObservableCollection<MaterialEntity>(listMaterials);
                            ViewModelLocator.EnrollPageVM.PendingMaterials = new ObservableCollection<MaterialEntity>(listMaterials);
                            ViewModelLocator.EnrollPageVM.Blocks = new ObservableCollection<string>(listMaterials.Select(m => m.Block).Distinct());

                            ViewModelLocator.EnrollPageVM.Levels.Clear();
                            ViewModelLocator.EnrollPageVM.Zones.Clear();

                            var beaconInfoPage = new BeaconInfoPage();
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                Navigation.PushAsync(beaconInfoPage);
                            });
                            /*
                            enrollPage = new EnrollPage();
                            if (isBeaconInUse)
                            {
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    DisplayAlert("Alert", alertTemplate, "OK");

                                    Navigation.PushAsync(enrollPage);
                                });
                                //MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, "");
                            }
                            else
                                Device.BeginInvokeOnMainThread(() =>
                                {
                                    Navigation.PushAsync(enrollPage);
                                });
                                */
                            //MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, "");
                        //}
                    }
                    else
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            DisplayAlert("Error", result.Message, "OK");
                        });
                }
                else
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Error", result.Message, "OK");
                    });
                //MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, result.Message);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Error", "Network error, please try again later", "OK");
                });
                //MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, "Network error, please try again later");
            }
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
        }
    }
}