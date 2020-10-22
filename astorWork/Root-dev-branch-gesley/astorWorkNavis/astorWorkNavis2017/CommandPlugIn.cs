using Autodesk.Navisworks.Api.Plugins;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace astorWorkNavis2017
{
    [Plugin("Astoria.MaterialSyncAddIn", "ASTR", DisplayName = "Astoria")]
    [RibbonLayout("CustomRibbon.xaml")]

    [RibbonTab("ID_TAB_ASTORWORK", DisplayName = "Astoria")]
    [Command("ID_BUTTON_MATERIAL_SYNC", DisplayName = "Material Sync",
        Icon = "Assets/16.ico", LargeIcon = "Assets/32.ico",
        ToolTip = "Sync material status with astorWork")]

    public class CommandPlugIn : CommandHandlerPlugin
    {
        private MainWindow _wnd = null;

        protected override void OnLoaded()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            foreach (var tab in Autodesk.Windows.ComponentManager.Ribbon.Tabs)
            {
                if (tab.UID == "astorWorkTab")
                {
                    if (Properties.Settings.Default.LAST_SYNC_TIME.Equals(DateTimeOffset.MinValue))
                        tab.Panels[0].Source.Title = "Never synced";
                    else
                        tab.Panels[0].Source.Title = "Last synced at " + Properties.Settings.Default.LAST_SYNC_TIME.LocalDateTime.ToLongDateString();
                }
            }
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(CommandPlugIn)).Location);
            string assemblyPath = Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!File.Exists(assemblyPath)) return null;
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            return assembly;
        }

        public override int ExecuteCommand(string name, params string[] parameters)
        {
            switch (name)
            {
                case "ID_BUTTON_MATERIAL_SYNC":
                    if (Autodesk.Navisworks.Api.Application.IsAutomated)
                    {
                        MessageBox.Show("Invalid when running using Automation", "Notice");
                        return 1;
                    }
                    else if (string.IsNullOrEmpty(Autodesk.Navisworks.Api.Application.ActiveDocument.FileName))
                    {
                        MessageBox.Show("Please open a model first", "Notice");
                        return 1;
                    }
                    else
                    {
                        if (_wnd == null)
                        {
                            _wnd = new MainWindow();
                            _wnd.Unloaded += _wnd_Unloaded;
                            System.Windows.Forms.Integration.ElementHost.EnableModelessKeyboardInterop(_wnd);
                            _wnd.Show();
                        }
                        else
                            _wnd.Activate();
                    }
                    break;
                default:
                    MessageBox.Show("Invalid command");
                    break;
            }
            return 0;
        }

        private void _wnd_Unloaded(object sender, RoutedEventArgs e)
        {
            _wnd = null;
        }
    }

}
