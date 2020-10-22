using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace astorWorkMobile.MaterialTrack.ViewModels
{
    public class SiteScanRFIDDemoVM : MasterVM
    {
        public static string BoxTag = "E2005032510B01520200EFFB";
        public static string[] MaterialTags = {
            "E20000175709008818105CF3",
            "ABCD0080D0C57F0034393031",
            "300833B2DDD9014000000000",
            "E20000175709008914308460"
        };

        public List<DemoMaterial> AllMaterials { get; set; }

        private bool _showBox;
        public bool ShowBox
        {
            get
            {
                return _showBox;
            }
            set
            {
                _showBox = value;
                OnPropertyChanged("ShowBox");
            }
        }

        private bool _showBoxConfirm;
        public bool ShowBoxConfirm
        {
            get
            {
                return _showBoxConfirm;
            }
            set
            {
                _showBoxConfirm = value;
                OnPropertyChanged("ShowBoxConfirm");
            }
        }
        private bool _showListMaterial;
        public bool ShowListMaterial
        {
            get
            {
                return _showListMaterial;
            }
            set
            {
                _showListMaterial = value;
                OnPropertyChanged("ShowListMaterial");
            }
        }

        private ObservableCollection<DemoMaterial> _materials;
        public ObservableCollection<DemoMaterial> Materials
        {
            get
            {
                return _materials;
            }
            set
            {
                _materials = value;
                OnPropertyChanged("Materials");
            }
        }


        private double _scanIconRotation;
        public Double ScanIconRotation
        {
            get
            {
                return _scanIconRotation;
            }
            set
            {
                _scanIconRotation = value;
                OnPropertyChanged("ScanIconRotation");
            }
        }

        public string ToggleScanButtonLabel
        {
            get
            {
                if (_allowScan)
                    return "Scan";
                else
                    return "Stop";
            }
        }

        private bool _allowScan;
        public bool AllowScan
        {
            get
            {
                return _allowScan;
            }
            set
            {
                _allowScan = value;
                OnPropertyChanged("AllowScan");
            }
        }

        public string BoxName { get; set; }

        public SiteScanRFIDDemoVM()
        {
            AllowScan = true;
            ShowBoxConfirm = true;
            BoxName = "Half Height Container 001";
            AllMaterials = new List<DemoMaterial>()
            {
                new DemoMaterial
                {
                    materialType = "M",
                    stageName = "Pending",
                    stageColour = "#ffff66",
                    id = 1,
                    markingNo = "Fire Extinguisher",
                    UpdateTime = DateTime.Now.AddDays(-1).AddHours(-10)
                },
                new DemoMaterial
                {
                    materialType = "M",
                    stageName = "Pending",
                    stageColour = "#ffff66",
                    id = 2,
                    markingNo = "Dry Pipe Valve",
                    UpdateTime = DateTime.Now.AddDays(-1).AddHours(-8)
                },
                new DemoMaterial
                {
                    materialType = "M",
                    stageName = "Pending",
                    stageColour = "#ffff66",
                    id = 3,
                    markingNo = "Equipment Box",
                    UpdateTime = DateTime.Now.AddDays(-1).AddHours(-6)
                },
                new DemoMaterial
                {
                    materialType = "M",
                    stageName = "Pending",
                    stageColour = "#ffff66",
                    id = 4,
                    markingNo = "Pipe Section",
                    UpdateTime = DateTime.Now.AddDays(-1).AddHours(-3)
                },
                new DemoMaterial
                {
                    materialType = "M",
                    stageName = "Pending",
                    stageColour = "#ffff66",
                    id = 5,
                    markingNo = "Pneumatic torque wrench",
                    UpdateTime = DateTime.Now.AddDays(-1)
                }
            };
            Materials = new ObservableCollection<DemoMaterial>();
        }
    }
}
