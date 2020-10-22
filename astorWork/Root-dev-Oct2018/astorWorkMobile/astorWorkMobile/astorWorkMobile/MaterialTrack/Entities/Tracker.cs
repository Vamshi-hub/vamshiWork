using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace astorWorkMobile.MaterialTrack.Entities
{
    public class Tracker
    {
        public int id { get; set; }
        public string tag { get; set; }
        public string trackerLabel { get; set; }
        public Material material { get; set; }
        public Inventory inventory { get; set; }

        public bool displayMaterial { get { return material != null; } }
        public bool displayInventory { get { return inventory != null; } }
        public bool displayBIM
        {
            get
            {
                return material != null && material.ForgeElementId.HasValue;
            }
        }
        public bool displayQC { get { return material != null && !material.qcStatus; } }

        public string qcRemarks
        {
            get
            {
                if (displayQC)
                    return material.qcRemarks;
                else
                    return string.Empty;
            }
        }

        public string markingNo
        {
            get
            {
                if (displayMaterial)
                    return material.markingNo;
                else if (displayInventory)
                    return inventory.markingNo;
                else
                    return "N.A.";
            }
        }

        public string stageName
        {
            get
            {
                if (displayMaterial)
                    return material.stageName;
                else if (displayInventory)
                    return "Inventory";
                else
                    return "Unassociated";
            }
        }

        public Color stageColor
        {
            get
            {
                if (displayMaterial) { 
                    return Color.FromHex(material.stageColour);
                }
                else if (displayInventory)
                    return Color.Yellow;
                else
                    return Color.Green;
            }
        }

        public string block
        {
            get
            {
                if (displayMaterial)
                    return material.block;
                else
                    return string.Empty;
            }
        }

        public string level
        {
            get
            {
                if (displayMaterial)
                    return material.level;
                else
                    return string.Empty;
            }
        }

        public string zone
        {
            get
            {
                if (displayMaterial)
                    return material.zone;
                else
                    return string.Empty;
            }
        }

        public string Location
        {
            get
            {
                if (material != null)
                    return material.Location;
                else
                    return string.Empty;
            }
        }

        public int sn
        {
            get
            {
                if (displayInventory)
                    return inventory.sn;
                else
                    return 0;
            }
        }

        public DateTime castingDate
        {
            get
            {
                if (displayInventory)
                    return inventory.castingDate;
                else
                    return DateTime.MinValue;
            }
        }

        public DateTime UpdatedTime { get; set; }

        public bool allowUpdate {
            get {
                return material.allowUpdate;
            }
        }
    }
}
