using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class Material
    {
        public int id { get; set; }
        public string markingNo { get; set; }
        public string block { get; set; }
        public string level { get; set; }
        public string zone { get; set; }
        public string materialType { get; set; }
        public int stageId { get; set; }
        public string stageName { get; set; }

        private string _stageColor;
        public string stageColour { get
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
            }
        }
        public bool IsOpenQCCase { get; set; }
        public bool NextStageIsQCOrInstalledStage { get; set; }
        public bool qcStatus { get; set; }
        public string qcRemarks { get; set; }
        public string mrfNo { get; set; }
        public int sn { get; set; }
        public DateTime castingDate { get; set; }
        public string Location
        {
            get
            {
                return string.Format("{0}-{1}-{2}", block, level, zone);
            }
        }
        public Location SelectedLocation { get; set; }
        public int? ForgeElementId { get; set; }
        public string ForgeModelURN { get; set; }
        public bool allowUpdate { get; set; }
    }
}
