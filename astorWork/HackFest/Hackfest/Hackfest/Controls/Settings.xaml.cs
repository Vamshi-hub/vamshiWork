using System;
using System.Windows;
using System.Windows.Controls;
using Camera_NET;
using DirectShowLib;

namespace Hackfest.Controls
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        private MainWindow _wnd;
        private CameraChoice _cameraChoice;
        private DsDevice _camera;

        public Settings(MainWindow wnd)
        {
            InitializeComponent();

            _wnd = wnd;

            _cameraChoice = new CameraChoice();
            _cameraChoice.UpdateDeviceList();

            for (int i = 0; i < _cameraChoice.Devices.Count; i++)
            {
                selectCamera.Items.Add(_cameraChoice.Devices[i].Name);
            }

            if(!string.IsNullOrEmpty(Properties.Settings.Default.CAMERA_UUID))
            {
                selectCamera.SelectedIndex = _cameraChoice.Devices.FindLastIndex(d => d.DevicePath.Equals(Properties.Settings.Default.CAMERA_UUID));
            }
        }

        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.SAFETY_CHECK_INTERVAL < 3)
                MessageBox.Show("Safety check interval must be greater or equal to 3 seconds", 
                    "Cannot save");
            else if (Properties.Settings.Default.CUSTOM_VISION_SAFE_THRESHOLD >= 1 || Properties.Settings.Default.CUSTOM_VISION_SAFE_THRESHOLD <= 0)
                MessageBox.Show("Safety threshold out of range");
            else if (Properties.Settings.Default.CUSTOM_VISION_UNSAFE_THRESHOLD >= 1 || Properties.Settings.Default.CUSTOM_VISION_UNSAFE_THRESHOLD <= 0)
                MessageBox.Show("Unsafe threshold out of range");
            else if (Properties.Settings.Default.FACE_API_THRESHHOLD >= 1 || Properties.Settings.Default.FACE_API_THRESHHOLD <= 0)
                MessageBox.Show("Face API threshold out of range");
            else
            {
                Properties.Settings.Default.Save();
                _wnd.NavigateBack();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reload();
            _wnd.NavigateBack();
        }

        private void selectCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectResolution.Items.Clear();

            if (selectCamera.SelectedIndex >= 0)
            {
                _camera = _cameraChoice.Devices[selectCamera.SelectedIndex];
                Properties.Settings.Default.CAMERA_UUID = _camera.DevicePath;

                var resolutions = Camera.GetResolutionList(_camera.Mon);
                for (int i = 0; i < resolutions.Count; i++)
                {
                    string res_string = string.Format("{0} x {1}", resolutions[i].Width, resolutions[i].Height);
                    selectResolution.Items.Add(res_string);
                }

                if (Properties.Settings.Default.CAMERA_RESOLUTION_WIDTH > 0 && Properties.Settings.Default.CAMERA_RESOLUTION_HEIGHT > 0)
                {
                    selectResolution.SelectedIndex = resolutions.FindLastIndex(r => r.Width == Properties.Settings.Default.CAMERA_RESOLUTION_WIDTH && r.Height == Properties.Settings.Default.CAMERA_RESOLUTION_HEIGHT);
                }
            }

        }

        private void selectResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_camera != null && selectResolution.SelectedIndex >= 0)
            {
                var resolutions = Camera.GetResolutionList(_camera.Mon);
                Properties.Settings.Default.CAMERA_RESOLUTION_WIDTH = resolutions[selectResolution.SelectedIndex].Width;
                Properties.Settings.Default.CAMERA_RESOLUTION_HEIGHT = resolutions[selectResolution.SelectedIndex].Height;
            }
        }
    }
}
