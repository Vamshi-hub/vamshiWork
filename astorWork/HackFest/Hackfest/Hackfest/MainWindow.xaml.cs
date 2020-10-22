using Hackfest.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hackfest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UIElement _controlDefault;
        private Settings _controlSettings;
        private TestCustomVision _testCustomVision;
        private TestFaceAPI _testFaceAPI;
        private SafetyCheck _safetyCheck;
        private TestCNTK _testCNTK;

        public MainWindow()
        {
            InitializeComponent();
            _controlDefault = mainContent.Children[0];
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (_controlSettings == null)
                _controlSettings = new Settings(this);

            mainContent.Children.RemoveAt(0);
            mainContent.Children.Add(_controlSettings);

            txtTitle.Text = "Settings";
        }

        public void NavigateBack()
        {
            mainContent.Children.RemoveAt(0);
            mainContent.Children.Add(_controlDefault);
            txtTitle.Text = "Hackfest 2018";
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnTestCustomVision_Click(object sender, RoutedEventArgs e)
        {
            if (_testCustomVision == null)
                _testCustomVision = new TestCustomVision(this);

            mainContent.Children.RemoveAt(0);
            mainContent.Children.Add(_testCustomVision);

            txtTitle.Text = "Test Custom Vision";
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_safetyCheck == null)
                _safetyCheck = new SafetyCheck(this);

            mainContent.Children.RemoveAt(0);
            mainContent.Children.Add(_safetyCheck);

            txtTitle.Text = "Safety Check";
        }

        private void btnTestFaceAPI_Click(object sender, RoutedEventArgs e)
        {

            if (_testFaceAPI == null)
                _testFaceAPI = new TestFaceAPI(this);

            mainContent.Children.RemoveAt(0);
            mainContent.Children.Add(_testFaceAPI);

            txtTitle.Text = "Test Face API";
        }

        private void btnTestCNTK_Click(object sender, RoutedEventArgs e)
        {

            if (_testCNTK == null)
                _testCNTK = new TestCNTK(this);

            mainContent.Children.RemoveAt(0);
            mainContent.Children.Add(_testCNTK);

            txtTitle.Text = "Test CNTK";

        }
    }
}
