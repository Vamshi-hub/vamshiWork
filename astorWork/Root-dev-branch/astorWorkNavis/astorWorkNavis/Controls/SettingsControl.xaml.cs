using astorWorkNavis.Classes;
using System.Windows;
using System.Windows.Controls;

namespace astorWorkNavis.Controls
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        private MainWindow _wnd;
        public SettingsControl(MainWindow wnd)
        {
            InitializeComponent();

            _wnd = wnd;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            _wnd.NavigateControl(Constants.CONTROL_LOG_IN, true);
        }
    }
}
