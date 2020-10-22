using astorWork_Navisworks.Classes;
using astorWork_Navisworks.Utilities;
using Autodesk.Navisworks.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace astorWork_Navisworks.Controls
{
    /// <summary>
    /// Interaction logic for HomeControl.xaml
    /// </summary>
    public partial class HomeControl : UserControl
    {
        private MainWindow _wnd;
        public HomeControl(MainWindow wnd)
        {
            _wnd = wnd;
            InitializeComponent();
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            if (NavisworksHelper.InitConfigurations())
            {
                Keyboard.ClearFocus();
                _wnd.ToggleLoading();
                Task.Run(GetMaterialAsync);
            }
            else
                _wnd.ShowNotice("Fail to initialize Navisworks TimeLiner");
        }

        public async Task GetMaterialAsync()
        {
            string message = string.Empty;
            var lastSyncTime = DateTime.MinValue;
            try
            {
                lastSyncTime = Properties.Settings.Default.LAST_SYNC_TIME;
            }
            catch(Exception exc)
            {
                Debug.WriteLine(exc.Message);
            }
            var result = await ApiClient.Instance.GetMaterialsForSync(Properties.Settings.Default.API_KEY, NavisworksHelper.GetProjectGuid(), lastSyncTime);

            if (result.Status != 0)
                message = result.Message;
            else
            {
                try
                {
                    var materials = result.Data.Select(j => new MaterialEntity
                    {
                        ID = (int)j["MaterialNo"],
                        MarkingNo = (string)j["MarkingNo"],
                        Block = (string)j["Block"],
                        Level = (string)j["Level"],
                        Zone = (string)j["Zone"],
                        CurrentStatus = (string)j["CurrentStatus"],
                        Status = (string)j["Status"],
                        UpdatedDate = (DateTime)j["UpdateDT"]
                    });

                    if (materials.Count() == 0)
                        message = "No material updates since last sync";
                    else
                    {
                        var listMaterialsMatched = new List<MaterialEntity>();
                        var listMaterialsUnmatched = new List<MaterialEntity>();

                        foreach (var material in materials)
                        {
                            string geId = string.Format("{0}_L{1}_ZONE-{2}_{3}", material.Block.ToUpper(), material.Level, material.Zone, material.MarkingNo);
                            Search search = new Search();
                            search.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("Element", "Mark")
                                .EqualValue(VariantData.FromDisplayString(geId)));
                            /*
                            search.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("Element", "GE-ID").EqualValue(VariantData.FromDisplayString(geId)));
                            search.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("Element", "GE-Block")
                                .EqualValue(VariantData.FromDisplayString(material.Block)));
                            search.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("Element", "GE-Zone")
                                .EqualValue(VariantData.FromDisplayString(material.Zone)));
                                */
                            search.Selection.SelectAll();
                            search.Locations = SearchLocations.DescendantsAndSelf;

                            if (search.FindFirst(Autodesk.Navisworks.Api.Application.ActiveDocument, false) != null)
                                listMaterialsMatched.Add(material);
                        }
                        listMaterialsUnmatched.AddRange(materials.Where(m => !listMaterialsMatched.Select(mm => mm.ID).Contains(m.ID)));

                        Dispatcher.Invoke(() =>
                        {
                            ViewModelLocator.syncPageVM.ListMaterialsMatched = new System.Collections.ObjectModel.ObservableCollection<MaterialEntity>(listMaterialsMatched);

                            ViewModelLocator.syncPageVM.MaterialsMatched.Clear();
                            ViewModelLocator.syncPageVM.MaterialsUnmatched.Clear();

                            foreach (var material in listMaterialsMatched)
                            {
                                if (ViewModelLocator.syncPageVM.MaterialsMatched.Where(mm => mm.Block == material.Block && mm.Level == material.Level && mm.Zone == material.Zone && mm.MarkingNo == material.MarkingNo).Count() == 0)
                                    ViewModelLocator.syncPageVM.MaterialsMatched.Add(new MaterialViewEntity
                                    {
                                        ID = material.ID,
                                        Block = material.Block,
                                        Level = material.Level,
                                        Zone = material.Zone,
                                        MarkingNo = material.MarkingNo,
                                        Status = material.CurrentStatus
                                    });
                            }
                            ViewModelLocator.syncPageVM.ListMaterialsUnmatched = new System.Collections.ObjectModel.ObservableCollection<MaterialEntity>(listMaterialsUnmatched);
                            foreach (var material in listMaterialsUnmatched)
                            {
                                if (ViewModelLocator.syncPageVM.MaterialsUnmatched.Where(mm => mm.Block == material.Block && mm.Level == material.Level && mm.Zone == material.Zone && mm.MarkingNo == material.MarkingNo).Count() == 0)
                                    ViewModelLocator.syncPageVM.MaterialsUnmatched.Add(new MaterialViewEntity
                                    {
                                        ID = material.ID,
                                        Block = material.Block,
                                        Level = material.Level,
                                        Zone = material.Zone,
                                        MarkingNo = material.MarkingNo,
                                        Status = material.CurrentStatus
                                    });
                            }
                        });
                    }
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    Debug.WriteLine(exc.StackTrace);
                    message = exc.Message;
                }
            }

            if (string.IsNullOrEmpty(message))
            {
                _wnd.NavigateControl(Constants.CONTROL_SYNC, false);
            }
            else
            {
                _wnd.ShowNotice(message);
                _wnd.ToggleLoading();
            }
        }

        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();

            _wnd.NavigateControl(Constants.CONTROL_LOG_IN, true);
        }

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            _wnd.Close();
        }

        private void btnResetConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (NavisworksHelper.ResetTimeLiner())
            {
                Properties.Settings.Default.LAST_SYNC_TIME = DateTime.MinValue;
                Properties.Settings.Default.Save();

                _wnd.ShowNotice("Reset done");
            }
            else
                _wnd.ShowNotice("Failed to reset");
        }
    }
}