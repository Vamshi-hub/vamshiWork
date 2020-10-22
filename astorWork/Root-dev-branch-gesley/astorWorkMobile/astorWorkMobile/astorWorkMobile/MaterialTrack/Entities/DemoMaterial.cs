using astorWorkMobile.Shared.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class DemoMaterial : MasterVM
    {
        public int id { get; set; }
        public string markingNo { get; set; }
        public string block { get; set; }
        public string level { get; set; }
        public string zone { get; set; }
        public string materialType { get; set; }
        public int stageId
        {
            get; set;
        }
        private string _stageName;
        public string stageName { get
            {
                return _stageName;
            }
            set
            {
                _stageName = value;
                OnPropertyChanged("stageName");
                OnPropertyChanged("ShowUtiliseButton");
            }
        }

        private string _stageColor;
        public string stageColour
        {
            get
            {
                return _stageColor;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && value.Length == 9)
                {
                    _stageColor = "#" + value.Substring(7) + value.Substring(1, 6);
                }
                else
                {
                    _stageColor = value;
                }
                OnPropertyChanged("stageColour");
            }
        }
        public bool qcStatus { get; set; }
        public string qcRemarks { get; set; }
        public string mrfNo { get; set; }
        public int sn { get; set; }

        private DateTime _updateTime;
        public DateTime UpdateTime { get
            {
                return _updateTime;
            }
            set
            {
                _updateTime = value;
                OnPropertyChanged("UpdateTime");
            }
        }

        public bool ShowUtiliseButton
        {
            get
            {
                return stageName == "Delivered" && id < 4;
            }
        }
    }
}
