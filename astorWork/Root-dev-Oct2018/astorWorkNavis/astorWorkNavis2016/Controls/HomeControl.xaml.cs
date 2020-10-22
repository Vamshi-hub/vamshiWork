using astorWorkNavis2016.Classes;
using astorWorkNavis2016.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace astorWorkNavis2016.Controls
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

            DataContext = ViewModelLocator.homePageVM;
        }

        private void btnSync_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModelLocator.homePageVM.SelectedProject == null)
                _wnd.ShowNotice("Please select a project");
            else
            {
                Properties.Settings.Default.PROJECT_ID = ViewModelLocator.homePageVM.SelectedProject.ID;
                Properties.Settings.Default.Save();

                Keyboard.ClearFocus();
                _wnd.ToggleLoading();
                Task.Run(GetStages)
                    .ContinueWith(t =>
                {
                    NavisworksHelper.InitTimelinerConfig(t.Result);
                }, TaskScheduler.FromCurrentSynchronizationContext())
                .ContinueWith(async t =>
                {
                    await GetMaterialAsync();
                });
            }
        }

        public async Task GetMaterialAsync()
        {
            string message = string.Empty;
            int lastSyncId = Properties.Settings.Default.LAST_SYNC_ID;

            var result = await ApiClient.Instance.GetMaterialsForSync(ViewModelLocator.homePageVM.SelectedProject.ID, lastSyncId);

            if (result.Status != 0)
                message = result.Message;
            else
            {
                try
                {
                    var materials = result.Data as List<MaterialEntity>;

                    if (materials.Count() == 0)
                        message = "No material updates since last sync";
                    else
                    {
                        var listMaterialsMatched = NavisworksHelper.SearchMaterials(materials);

                        var listMaterialsUnmatched = new List<MaterialEntity>();
                        listMaterialsUnmatched.AddRange(materials.Where(m => !listMaterialsMatched.Select(mm => mm.MaterialId).Contains(m.MaterialId)));

                        Dispatcher.Invoke(() =>
                        {
                            ViewModelLocator.syncPageVM.ListMaterialsMatched = new System.Collections.ObjectModel.ObservableCollection<MaterialEntity>(listMaterialsMatched);

                            ViewModelLocator.syncPageVM.MaterialsMatched.Clear();
                            ViewModelLocator.syncPageVM.MaterialsUnmatched.Clear();

                            foreach (var material in listMaterialsMatched)
                            {
                                if (ViewModelLocator.syncPageVM.MaterialsMatched.Where(mm => mm.Block == material.Block && mm.Level == material.Level && mm.Zone == material.Zone && mm.MarkingNo == material.MarkingNo).Count() == 0)
                                {
                                    var latestStageName = listMaterialsMatched.Where(mm =>
                                    mm.Block == material.Block &&
                                    mm.Level == material.Level &&
                                    mm.Zone == material.Zone &&
                                    mm.MarkingNo == material.MarkingNo).OrderBy(mm => mm.UpdateTime).Last().StageName;

                                    ViewModelLocator.syncPageVM.MaterialsMatched.Add(new MaterialViewEntity
                                    {
                                        ID = material.MaterialId,
                                        Block = material.Block,
                                        Level = material.Level,
                                        Zone = material.Zone,
                                        MarkingNo = material.MarkingNo,
                                        Status = latestStageName
                                    });
                                }
                            }
                            ViewModelLocator.syncPageVM.ListMaterialsUnmatched = new System.Collections.ObjectModel.ObservableCollection<MaterialEntity>(listMaterialsUnmatched);

                            foreach (var material in listMaterialsUnmatched)
                            {
                                if (ViewModelLocator.syncPageVM.MaterialsUnmatched.Where(
                                    mm => mm.Block == material.Block &&
                                    mm.Level == material.Level &&
                                    mm.Zone == material.Zone &&
                                    mm.MarkingNo == material.MarkingNo
                                    ).Count() == 0)
                                {
                                    var latestStageName = listMaterialsUnmatched.Where(mm =>
                                    mm.Block == material.Block &&
                                    mm.Level == material.Level &&
                                    mm.Zone == material.Zone &&
                                    mm.MarkingNo == material.MarkingNo).OrderBy(mm => mm.UpdateTime).Last().StageName;

                                    ViewModelLocator.syncPageVM.MaterialsUnmatched.Add(new MaterialViewEntity
                                    {
                                        ID = material.MaterialId,
                                        Block = material.Block,
                                        Level = material.Level,
                                        Zone = material.Zone,
                                        MarkingNo = material.MarkingNo,
                                        Status = latestStageName
                                    });
                                }
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

        private async Task<List<Stage>> GetStages()
        {
            List<Stage> stages = new List<Stage>();
            var result = await ApiClient.Instance.MTGetStages();
            if (result.Status == 0)
            {
                stages = result.Data as List<Stage>;
                if (stages != null)
                {
                    ViewModelLocator.syncPageVM.StageNames = stages.Where(s => s.Order > 0).Select(s => s.Name).ToList();
                }
            }

            return stages;
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
                Properties.Settings.Default.LAST_SYNC_ID = 0;
                Properties.Settings.Default.LAST_SYNC_TIME = DateTimeOffset.MinValue;
                Properties.Settings.Default.Save();

                _wnd.ShowNotice("Reset done");
            }
            else
                _wnd.ShowNotice("Failed to reset");
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}