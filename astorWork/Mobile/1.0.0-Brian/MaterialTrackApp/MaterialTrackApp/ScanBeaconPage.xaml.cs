using FormsPlugin.Iconize;
using MaterialTrackApp.Class;
using MaterialTrackApp.DB;
using MaterialTrackApp.Entities;
using MaterialTrackApp.Utility;
using Newtonsoft.Json.Linq;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
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
            ctsScanTask = new CancellationTokenSource();

            var ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;

            if (ble.IsAvailable)
            {
                ble.StateChanged += Ble_StateChanged;
                //adapter.ScanTimeout = 30 * 1000;
                adapter.ScanMode = ScanMode.Balanced;
                adapter.DeviceDiscovered += Adapter_DeviceDiscovered;
                adapter.ScanTimeoutElapsed += Adapter_ScanTimeoutElapsed;

                ViewModelLocator.scanBeaconPageVM.BluetoothEnabled = ble.State.Equals(BluetoothState.On);
            }
        }

        private void Adapter_ScanTimeoutElapsed(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                btnToggleScan.Text = "Start";
                if (ViewModelLocator.scanBeaconPageVM.CountBeacons > 0)
                {
                    ViewModelLocator.scanBeaconPageVM.Beacons.OrderBy(b => b.Distance);
                    btnNext.IsVisible = true;
                    btnNext.IsEnabled = true;
                }
            });
        }

        private void Adapter_DeviceDiscovered(object sender, Plugin.BLE.Abstractions.EventArgs.DeviceEventArgs e)
        {
            if (e.Device.Rssi < -90)
                return;
            /*
            Debug.WriteLine("<" + e.Device.Name + ">");
            Debug.WriteLine("<" + e.Device.Rssi.ToString() + ">");
            */

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
                    {
                        proximity_uuid = uuid;
                        recordText = recordText.Substring(recordText.IndexOf(uuid));
                    }
                }

                if (!string.IsNullOrEmpty(proximity_uuid))
                {
                    byte[] uuidData = record.Data.Reverse().ToArray();
                    ushort major = BitConverter.ToUInt16(uuidData, 3);
                    ushort minor = BitConverter.ToUInt16(uuidData, 1);
                    short txPower = BitConverter.ToInt16(uuidData, 0);
                    Debug.WriteLine("--Signal power: " + txPower);

                    double distance = Math.Pow(10.0, (double)(txPower / 100 - e.Device.Rssi) / (10 * 2));

                    var name = major.ToString() + "-" + minor.ToString();
                    var beacon = ViewModelLocator.scanBeaconPageVM.Beacons
                        .Where(b => b.ProximityUUID == proximity_uuid && b.Major == major && b.Minor == minor)
                        .FirstOrDefault();
                    if (major > 0 && minor > 0 && distance > 0 && distance < 30 &&
                        beacon == null)
                    {
                        beacon = new BeaconEntity()
                        {
                            DisplayName = name,
                            ProximityUUID = proximity_uuid,
                            Major = major,
                            Minor = minor,
                            //BeaconID = string.Format("{0}-{1}-{2}", proximity_uuid, major, minor),
                            BeaconID = recordText,
                            Distance = distance
                        };
                        ViewModelLocator.scanBeaconPageVM.Beacons.Add(beacon);

                        /*
                        if (ViewModelLocator.scanBeaconPageVM.CountBeacons == 1)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                layoutControls.VerticalOptions = LayoutOptions.Start;
                                //icoScan.ScaleTo(0.5, 1000);
                            });
                        }
                        */
                    }
                    else if (beacon != null)
                    {
                        beacon.Distance = distance;
                    }
                    ViewModelLocator.scanBeaconPageVM.Beacons.OrderBy(b => b.Distance);

                    break;
                }
            }
        }

        private void Ble_StateChanged(object sender, Plugin.BLE.Abstractions.EventArgs.BluetoothStateChangedArgs e)
        {
            ViewModelLocator.scanBeaconPageVM.BluetoothEnabled = e.NewState.Equals(BluetoothState.On);
        }

        private void btnToggleScan_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (adapter.IsScanning)
                {
                    Task.Run(adapter.StopScanningForDevicesAsync);

                    ViewExtensions.CancelAnimations(icoScan);
                    btnToggleScan.Text = "Start";

                    if (ViewModelLocator.scanBeaconPageVM.CountBeacons > 0)
                    {
                        btnNext.IsVisible = true;
                        btnNext.IsEnabled = true;
                    }
                }
                else
                {
                    Task.Run(() =>
                    {
                        return adapter.StartScanningForDevicesAsync(cancellationToken: ctsScanTask.Token);
                    });

                    icoScan.RelRotateTo(360, (uint)adapter.ScanTimeout);
                    btnToggleScan.Text = "Stop";

                    btnNext.IsVisible = false;
                    btnNext.IsEnabled = false;
                }
            }
            catch (Exception exc)
            {
                btnNext.IsVisible = false;
                btnNext.IsEnabled = false;
                DisplayAlert("Alert", exc.Message, "OK");
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
            */
            if (ViewModelLocator.scanBeaconPageVM.CountBeacons > 0)
                ProceedNext();
            else
                DisplayAlert("Warning", "No beacon found", "OK");
        }

        private async Task GetMaterialWithBeacons()
        {
            try
            {
                var result = await ApiClient.Instance.MTGetMaterialMasterWithBeacons(ViewModelLocator.scanBeaconPageVM.Beacons);

                if (result.Status == 0)
                {
                    var json = result.Data as JArray;

                    ViewModelLocator.beaconInfoPageVM.ListBeacons.Clear();

                    foreach (var beacon in ViewModelLocator.scanBeaconPageVM.Beacons.OrderBy(b => b.Distance))
                    {
                        beacon.InInventory = false;

                        var task = json.Where(j => (string)j["BeaconID"] == beacon.BeaconID).FirstOrDefault();

                        if (task != null)
                        {
                            beacon.InInventory = (bool)task["InInventory"];
                            if (beacon.InInventory)
                            {
                                beacon.LotNo = (int)task["LotNo"];
                                beacon.CastingDate = (DateTime)task["CastingDate"];
                            }
                            if (!string.IsNullOrEmpty((string)task["Status"]) && (string)task["Status"] == "Installed")
                            {
                                beacon.Installed = true;
                            }
                            else if (!string.IsNullOrEmpty((string)task["Status"]) && (string)task["Status"] == "Delivered")
                            {
                                beacon.Delivered = true;
                            }
                            else if(!beacon.InInventory)
                            {
                                beacon.InTransit = true;
                            }

                            beacon.Project = (string)task["Project"];
                            beacon.MarkingNo = (string)task["MarkingNo"];
                            beacon.PassQC = (bool)task["PassQC"];
                            beacon.FailQC = !beacon.PassQC;
                        }

                        // Check whether the beacon can be used for enroll/install
                        if (Application.Current.Properties["user_role"].Equals(Constants.ROLE_SITE_SUPERVISOR))
                        {
                            if (beacon.Delivered && beacon.PassQC)
                                beacon.CanUse = true;
                            else
                                beacon.CanUse = false;
                        }
                        else if (Application.Current.Properties["user_role"].Equals(Constants.ROLE_VENDOR))
                        {
                            beacon.CanUse = true;
                            if (beacon.FailQC)
                                beacon.CanUse = false;
                            else if (beacon.InTransit && beacon.PassQC)
                                beacon.CanUse = false;
                        }

                        ViewModelLocator.beaconInfoPageVM.ListBeacons.Add(beacon);
                    }

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var beaconInfoPage = new BeaconInfoPage();
                        Navigation.PushAsync(beaconInfoPage);
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        DisplayAlert("Warning", result.Message, "OK");
                    });
                }
                //MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, result.Message);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
                //MessagingCenter.Send<Page, string>(this, App.TOPIC_LOADING_RESULT, "Network error, please try again later");
                Device.BeginInvokeOnMainThread(() =>
                {
                    DisplayAlert("Warning", "Network error, please try again later", "OK");
                });
            }
        }

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            try
            {
                ViewModelLocator.scanBeaconPageVM.Beacons.Clear();
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Need location", "astorTrack needs the location permission", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Location))
                        status = results[Permission.Location];
                }

                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Location Denied", "Can not continue, please try again later", "OK");
                    await Navigation.PopAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Unkown error", ex.Message, "OK");
                await Navigation.PopAsync();
            }
        }

        private void btnSkip_Clicked(object sender, EventArgs e)
        {
            ViewModelLocator.scanBeaconPageVM.Beacons.Clear();
        }

        private void ProceedNext()
        {
            App.InvokeLoadingView(this, layoutMain, Task.Run(GetMaterialWithBeacons), "Retrieving beacon information, please wait...");
        }
    }
}