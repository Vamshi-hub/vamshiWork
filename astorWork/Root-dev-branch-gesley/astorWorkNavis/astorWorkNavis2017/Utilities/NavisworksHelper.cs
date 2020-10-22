using astorWorkNavis2017.Classes;
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Timeliner;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComApiBridge = Autodesk.Navisworks.Api.ComApi;

namespace astorWorkNavis2017.Utilities
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

        public static bool InitTimelinerConfig(List<Stage> stages)
        {
            bool result = false;
            try
            {
                var docTimeLiner = Application.ActiveDocument.GetTimeliner();
                // Init all colors
                var listTaskTypes = new List<SimulationStatus>() {
                    new SimulationStatus { AppearanceMode = SimulationAppearanceMode.Default }
                };

                foreach (var stage in stages.Where(s => s.Order > 0).OrderBy(s => s.Order))
                {
                    var taskName = Constants.APPEARANCE_PREFIX + stage.Name;
                    if (docTimeLiner.SimulationAppearanceFindByDisplayName(taskName) == null)
                    {
                        var red = ((double)Convert.ToInt32(stage.Colour.Substring(1, 2), 16) / 255.0);
                        var green = ((double)Convert.ToInt32(stage.Colour.Substring(3, 2), 16) / 255.0);
                        var blue = ((double)Convert.ToInt32(stage.Colour.Substring(5, 2), 16) / 255.0);
                        /*
                        var transparency = 1 - ((double)Convert.ToInt32(stage.Colour.Substring(7), 16) / 255.0);
                        var appearance = new SimulationAppearance(taskName, new Color(red, green, blue), transparency);
                        */
                        var appearance = new SimulationAppearance(taskName, new Color(red, green, blue), 0);
                        docTimeLiner.SimulationAppearanceAddCopy(appearance);

                        var taskStatus = new SimulationStatus
                        {
                            AppearanceMode = SimulationAppearanceMode.UserAppearance,
                            SimulationAppearanceName = taskName
                        };

                        var taskType = new SimulationTaskType
                        {
                            DisplayName = taskName,
                            StartStatus = listTaskTypes.LastOrDefault(),
                            EndStatus = taskStatus
                        };

                        listTaskTypes.Add(taskStatus);

                        docTimeLiner.SimulationTaskTypeAddCopy(taskType);
                    }
                }

                result = true;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc);
            }
            return result;
        }

        public static bool SyncTimeliner(List<string> stageNames, IEnumerable<MaterialEntity> listMaterials)
        {
            bool success = false;
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
                    foreach (var material in listMaterials.Where(m => stageNames.Contains(m.StageName)))
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

                        var taskTypeName = Constants.APPEARANCE_PREFIX + material.StageName;
                        int taskTypeIndex = stageNames.IndexOf(material.StageName);
                        string prevStageName = taskTypeIndex > 0 ? stageNames[taskTypeIndex - 1] : string.Empty;

                        var itemTask = (TimelinerTask)zoneTask.Children.Where(t => t.DisplayName == material.MarkingNo && (t as TimelinerTask).SimulationTaskTypeName == taskTypeName).FirstOrDefault();
                        var prevMaterial = listMaterials.Where(m => m.Block == material.Block && m.Level == material.Level && m.Zone == material.Zone && m.MarkingNo == material.MarkingNo && m.StageName == prevStageName).FirstOrDefault();

                        DateTimeOffset prevDate = material.UpdateTime.AddDays(-1);
                        if (prevMaterial != null)
                            prevDate = prevMaterial.UpdateTime;

                        if (itemTask == null)
                        {
                            itemTask = new TimelinerTask
                            {
                                DisplayName = material.MarkingNo,
                                PlannedStartDate = null,
                                PlannedEndDate = null,
                                ActualStartDate = prevDate.LocalDateTime,
                                ActualEndDate = material.UpdateTime.LocalDateTime,
                                SimulationTaskTypeName = taskTypeName
                            };

                            Search search = new Search();
                            search.Selection.SelectAll();
                            search.Locations = SearchLocations.DescendantsAndSelf;

                            AddSearchConditions(search, material);


                            itemTask.Selection.CopyFrom(search);
                            docTimeLiner.TaskAddCopy(zoneTask, itemTask);
                        }
                    }
                    trans.Commit();
                    success = true;
                }
            }
            catch (Exception exc)
            {
                Console.Write(exc);
            }

            return success;
        }

        public static IEnumerable<MaterialEntity> SearchMaterials(IEnumerable<MaterialEntity> materials)
        {
            var listMaterialsMatched = new List<MaterialEntity>();
            try
            {
                Search search = new Search();
                search.Selection.SelectAll();
                search.Locations = SearchLocations.DescendantsAndSelf;

                foreach (var material in materials.Select(m => new { m.Block, m.Zone, m.Level, m.MarkingNo }).Distinct())
                {
                    if (string.IsNullOrEmpty(material.Block) || string.IsNullOrEmpty(material.Level) || string.IsNullOrEmpty(material.Zone) || string.IsNullOrEmpty(material.MarkingNo))
                        continue;

                    AddSearchConditions(search, material);


                    if (search.FindFirst(Application.ActiveDocument, false) != null)
                    {
                        var materialEntities = materials.Where(m => m.Block == material.Block && m.Level == material.Level && m.Zone == material.Zone && m.MarkingNo == material.MarkingNo);

                        if (materialEntities != null)
                            listMaterialsMatched.AddRange(materialEntities);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.Write(exc);
            }
            return listMaterialsMatched;
        }

        private static void AddSearchConditions(Search search, dynamic material)
        {
            search.SearchConditions.Clear();
            
            search.SearchConditions.Add(
                SearchCondition.HasPropertyByDisplayName("Astoria", "Block")
                .EqualValue(VariantData.FromDisplayString(material.Block)));
            
            search.SearchConditions.Add(
                SearchCondition.HasPropertyByDisplayName("Astoria", "Level")
                .EqualValue(VariantData.FromDisplayString(material.Level)));

            search.SearchConditions.Add(
                SearchCondition.HasPropertyByDisplayName("Astoria", "Zone")
                .EqualValue(VariantData.FromDisplayString(material.Zone)));

            search.SearchConditions.Add(
                SearchCondition.HasPropertyByDisplayName("Astoria", "MarkingNo").EqualValue(VariantData.FromDisplayString(material.MarkingNo)));                
        }
    }
}
