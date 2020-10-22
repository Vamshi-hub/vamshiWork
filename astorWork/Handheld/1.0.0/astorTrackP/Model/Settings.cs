using System;
using SQLite;

namespace astorTrackP
{
	public class Settings
	{
		[PrimaryKey, AutoIncrement]
        public int SettingsID { get; set; }
        public int RFIDPower { get; set; }
        public string RFIDScanDetection { get; set; }
    }
}

