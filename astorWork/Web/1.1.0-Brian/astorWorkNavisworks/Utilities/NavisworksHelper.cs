using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using astorWork_Navisworks.Classes;
using Autodesk.Navisworks.Api.Timeliner;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Interop;

namespace astorWork_Navisworks.Utilities
{
    public class NavisworksHelper
    {
        public static string GetProjectGuid()
        {
            if (string.IsNullOrEmpty(Application.ActiveDocument.FileName))
                return string.Empty;
            else
                return Application.ActiveDocument.DocumentInfo.Value.SourceGuid.ToString();
        }

        public static string GetProjectTitle()
        {
            if (string.IsNullOrEmpty(Application.ActiveDocument.FileName))
                return string.Empty;
            else
                return Application.ActiveDocument.Title;
        }

        public static string ExportTimeLiner()
        {
            string exportFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".avi");
            int result = -1;

            try
            {
                //Autodesk.Navisworks.Api.Interop.LcTlSimulationHelper.Instance.Play();
                /*
                string pluginId = string.Empty;
                foreach (var record in Application.Plugins.PluginRecords)
                {
                    if (record.Name.Equals("NativeExportPluginAdaptor_lcodpanim_Export"))
                    {
                        var plugin = (Autodesk.Navisworks.Internal.ApiImplementation.NativeExportPluginAdaptor) record.LoadedPlugin;

                        Debug.WriteLine(plugin.ExportType.ToString());

                        pluginId = record.Id;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(pluginId))
                {
                       result = Application.Plugins.ExecuteAddInPlugin(pluginId,
                        exportFilePath
                    );
                }
                */

                var progress = Application.BeginProgress("Exporting", "Exporting current TimeLiner into a video");

                //Set the IOPlugin options
                string ioPluginName = "lcodpanim";

                ComApi.InwOpState10 state = ComApiBridge.ComApiBridge.State;

                ComApi.InwOaPropertyVec options = state.GetIOPluginOptions(ioPluginName);

                foreach (ComApi.InwOaProperty opt in options.Properties())
                {
                    Debug.WriteLine(opt.name);
                    if (opt.name == "export.image.anit-alias.level")
                        opt.value = 2;
                    else if (opt.name == "export.anim.fps")
                        opt.value = 12;
                    else if (opt.name == "export.image.height")
                        opt.value = 480;
                    else if (opt.name == "export.image.width")
                        opt.value = 640;
                    else if (opt.name == "export.anim.source")
                        opt.value = "LcTlAnimSequenceFactory";
                    else if (opt.name == "export.image.renderer")
                        opt.value = "lcodpopengl";
                }
                //Export the animation
                var status = state.DriveIOPlugin(ioPluginName, exportFilePath, options);

                if (!status.Equals(ComApi.nwEExportStatus.eExport_OK))
                    exportFilePath = string.Empty;
                else
                    result = 0;

                Application.EndProgress();

            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
            }

            if (result == 0)
                return exportFilePath;
            else
                return string.Empty;
        }

        public static bool ResetTimeLiner()
        { 
            try
            {
                var docTimeLiner = Application.ActiveDocument.GetTimeliner();

                if (docTimeLiner.SimulationAppearanceFindByDisplayName(Constants.TASK_APPEARANCE_PRODUCED) != null)
                    docTimeLiner.SimulationAppearanceRemoveByDisplayName(Constants.TASK_APPEARANCE_PRODUCED);
                if (docTimeLiner.SimulationAppearanceFindByDisplayName(Constants.TASK_APPEARANCE_DELIVERED) != null)
                    docTimeLiner.SimulationAppearanceRemoveByDisplayName(Constants.TASK_APPEARANCE_DELIVERED);
                if (docTimeLiner.SimulationAppearanceFindByDisplayName(Constants.TASK_APPEARANCE_INSTALLED) != null)
                    docTimeLiner.SimulationAppearanceRemoveByDisplayName(Constants.TASK_APPEARANCE_INSTALLED);

                if (docTimeLiner.SimulationTaskTypeFindByDisplayName(Constants.TASK_TYPE_PRODUCED) != null)
                    docTimeLiner.SimulationTaskTypeRemoveByDisplayName(Constants.TASK_TYPE_PRODUCED);
                if (docTimeLiner.SimulationTaskTypeFindByDisplayName(Constants.TASK_TYPE_DELIVERED) != null)
                    docTimeLiner.SimulationTaskTypeRemoveByDisplayName(Constants.TASK_TYPE_DELIVERED);
                if (docTimeLiner.SimulationTaskTypeFindByDisplayName(Constants.TASK_TYPE_INSTALLED) != null)
                    docTimeLiner.SimulationTaskTypeRemoveByDisplayName(Constants.TASK_TYPE_INSTALLED);

                var taskIndex = docTimeLiner.TasksRoot.Children.IndexOfDisplayName(Constants.TASK_NAME_ROOT);
                if (taskIndex >= 0)
                    docTimeLiner.TaskRemoveAt(taskIndex);

                return true;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

                return false;
            }
        }

        public static bool InitConfigurations()
        {
            try
            {
                var docTimeLiner = Application.ActiveDocument.GetTimeliner();

                //Override default color
                /*
                var settings = docTimeLiner.Settings.CreateCopy();

                settings.DefaultSimulationStatus = new SimulationStatus
                {
                    AppearanceMode = SimulationAppearanceMode.UserAppearance,
                    SimulationAppearanceName = "Grey"
                };
                docTimeLiner.SettingsReplaceWithCopy(settings);
                */

                // Init all colors
                if (docTimeLiner.SimulationAppearanceFindByDisplayName(Constants.TASK_APPEARANCE_PRODUCED) == null)
                {
                    var appearanceProduced = new SimulationAppearance(Constants.TASK_APPEARANCE_PRODUCED, new Color(1, 1, 0), 0);
                    docTimeLiner.SimulationAppearanceAddCopy(appearanceProduced);
                }

                if (docTimeLiner.SimulationAppearanceFindByDisplayName(Constants.TASK_APPEARANCE_DELIVERED) == null)
                {
                    var appearanceDelivered = new SimulationAppearance(Constants.TASK_APPEARANCE_DELIVERED, Color.Green, 0);
                    docTimeLiner.SimulationAppearanceAddCopy(appearanceDelivered);
                }


                if (docTimeLiner.SimulationAppearanceFindByDisplayName(Constants.TASK_APPEARANCE_INSTALLED) == null)
                {
                    var appearanceInstalled = new SimulationAppearance(Constants.TASK_APPEARANCE_INSTALLED, Color.Blue, 0);
                    docTimeLiner.SimulationAppearanceAddCopy(appearanceInstalled);
                }

                // Init all task type status
                var taskStatusRequested = new SimulationStatus
                {
                    AppearanceMode = SimulationAppearanceMode.Default
                };
                var taskStatusProduced = new SimulationStatus
                {
                    AppearanceMode = SimulationAppearanceMode.UserAppearance,
                    SimulationAppearanceName = Constants.TASK_APPEARANCE_PRODUCED
                };
                var taskStatusDelivered = new SimulationStatus
                {
                    AppearanceMode = SimulationAppearanceMode.UserAppearance,
                    SimulationAppearanceName = Constants.TASK_APPEARANCE_DELIVERED
                };
                var taskStatusInstalled = new SimulationStatus
                {
                    AppearanceMode = SimulationAppearanceMode.UserAppearance,
                    SimulationAppearanceName = Constants.TASK_APPEARANCE_INSTALLED
                };

                // Init all task types
                if (docTimeLiner.SimulationTaskTypeFindByDisplayName(Constants.TASK_TYPE_PRODUCED) == null)
                {
                    var taskTypeProduced = new SimulationTaskType
                    {
                        DisplayName = Constants.TASK_TYPE_PRODUCED,
                        StartStatus = taskStatusRequested,
                        EndStatus = taskStatusProduced
                    };
                    docTimeLiner.SimulationTaskTypeAddCopy(taskTypeProduced);
                }

                if (docTimeLiner.SimulationTaskTypeFindByDisplayName(Constants.TASK_TYPE_DELIVERED) == null)
                {
                    var taskTypeDelivered = new SimulationTaskType
                    {
                        DisplayName = Constants.TASK_TYPE_DELIVERED,
                        StartStatus = taskStatusProduced,
                        EndStatus = taskStatusDelivered
                    };
                    docTimeLiner.SimulationTaskTypeAddCopy(taskTypeDelivered);
                }

                if (docTimeLiner.SimulationTaskTypeFindByDisplayName(Constants.TASK_TYPE_INSTALLED) == null)
                {

                    var taskTypeInstalled = new SimulationTaskType
                    {
                        DisplayName = Constants.TASK_TYPE_INSTALLED,
                        StartStatus = taskStatusDelivered,
                        EndStatus = taskStatusInstalled
                    };
                    docTimeLiner.SimulationTaskTypeAddCopy(taskTypeInstalled);
                }

                return true;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

                return false;
            }
        }

        public static bool SyncTimeliner(IEnumerable<MaterialEntity> listMaterials)
        {
            List<string> taskStages = new List<string> { "Produced", "Delivered", "Installed" };

            try
            {
                using (var trans = Application.ActiveDocument.BeginTransaction("Sync Timeliner"))
                {
                    var docTimeLiner = trans.Document.Timeliner as DocumentTimeliner;

                    var rootCopy = docTimeLiner.TasksRoot.CreateCopyWithoutChildren();
                    var rootTask = (TimelinerTask)docTimeLiner.Tasks.Where(t => ((TimelinerTask)t).DisplayId == Constants.ID_TASK_ROOT).FirstOrDefault();
                    if (rootTask == null)
                    {
                        rootTask = new TimelinerTask
                        {
                            DisplayId = Constants.ID_TASK_ROOT,
                            DisplayName = Constants.TASK_NAME_ROOT
                        };
                        docTimeLiner.TaskAddCopy(docTimeLiner.TasksRoot, rootTask);

                        rootTask = (TimelinerTask)docTimeLiner.Tasks.Where(t => ((TimelinerTask)t).DisplayId == Constants.ID_TASK_ROOT).FirstOrDefault();
                    }

                    //GroupItem rootTaskCopy = rootTask.CreateCopy() as GroupItem;
                    foreach (var material in listMaterials.Where(m => taskStages.Contains(m.Status)))
                    {

                        var blockTask = (TimelinerTask)rootTask.Children.Where(t => t.DisplayName == material.Block).FirstOrDefault();
                        if (blockTask == null)
                        {
                            blockTask = new TimelinerTask
                            {
                                DisplayName = material.Block
                            };
                            docTimeLiner.TaskAddCopy(rootTask, blockTask);

                            blockTask = (TimelinerTask)rootTask.Children.Where(t => t.DisplayName == material.Block).FirstOrDefault();
                        }

                        var levelTask = (TimelinerTask)blockTask.Children.Where(t => t.DisplayName == material.Level).FirstOrDefault();
                        if (levelTask == null)
                        {
                            levelTask = new TimelinerTask
                            {
                                DisplayName = material.Level
                            };
                            docTimeLiner.TaskAddCopy(blockTask, levelTask);

                            levelTask = (TimelinerTask)blockTask.Children.Where(t => t.DisplayName == material.Level).FirstOrDefault();
                        }

                        var zoneTask = (TimelinerTask)levelTask.Children.Where(t => t.DisplayName == material.Zone).FirstOrDefault();

                        if (zoneTask == null)
                        {
                            zoneTask = new TimelinerTask
                            {
                                DisplayName = material.Zone
                            };
                            docTimeLiner.TaskAddCopy(levelTask, zoneTask);
                            zoneTask = (TimelinerTask)levelTask.Children.Where(t => t.DisplayName == material.Zone).FirstOrDefault();
                        }

                        string taskTypeName = string.Empty;
                        string prevStatus = string.Empty;
                        switch (material.Status)
                        {
                            case "Produced":
                                taskTypeName = Constants.TASK_TYPE_PRODUCED;
                                prevStatus = "Requested";
                                break;
                            case "Delivered":
                                taskTypeName = Constants.TASK_TYPE_DELIVERED;
                                prevStatus = "Produced";
                                break;
                            case "Installed":
                                taskTypeName = Constants.TASK_TYPE_INSTALLED;
                                prevStatus = "Delivered";
                                break;
                            default:
                                break;
                        }
                        var itemTask = (TimelinerTask)zoneTask.Children.Where(t => t.DisplayName == material.MarkingNo && (t as TimelinerTask).SimulationTaskTypeName == taskTypeName).FirstOrDefault();
                        var prevMaterial = listMaterials.Where(m => m.Block == material.Block && m.Level == material.Level && m.Zone == material.Zone && m.MarkingNo == material.MarkingNo && m.Status == prevStatus).FirstOrDefault();

                        DateTime prevDate = material.UpdatedDate.AddDays(-1);
                        if (prevMaterial != null)
                            prevDate = prevMaterial.UpdatedDate;

                        if (itemTask == null)
                        {
                            itemTask = new TimelinerTask
                            {
                                DisplayName = material.MarkingNo,
                                PlannedStartDate = null,
                                PlannedEndDate = null,
                                ActualStartDate = prevDate,
                                ActualEndDate = material.UpdatedDate,
                                SimulationTaskTypeName = taskTypeName
                            };

                            //string geId = string.Format("{0}-{1}", material.Level, material.MarkingNo);
                            string geId = string.Format("{0}_L{1}_ZONE-{2}_{3}", material.Block.ToUpper(), material.Level, material.Zone, material.MarkingNo);

                            Search search = new Search();
                            /*
                            search.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("Element", "GE-ID").EqualValue(VariantData.FromDisplayString(geId)));
                            search.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("Element", "GE-Block")
                                .EqualValue(VariantData.FromDisplayString(material.Block)));
                            search.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("Element", "GE-Zone")
                                .EqualValue(VariantData.FromDisplayString(material.Zone)));
                                */
                            search.SearchConditions.Add(SearchCondition.HasPropertyByDisplayName("Element", "Mark")
                                .EqualValue(VariantData.FromDisplayString(geId)));

                            search.Selection.SelectAll();
                            search.Locations = SearchLocations.DescendantsAndSelf;

                            itemTask.Selection.CopyFrom(search);
                            docTimeLiner.TaskAddCopy(zoneTask, itemTask);
                        }
                    }
                    trans.Commit();
                }

                return true;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

                return false;
            }
        }
    }
}
