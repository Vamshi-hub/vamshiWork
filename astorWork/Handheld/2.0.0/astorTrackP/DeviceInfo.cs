using System;
using Plugin.DeviceInfo;

namespace astorTrackP
{
	public class DeviceInfo
	{
		public bool HasRFIDModule {get; set;}
		public bool HasBarcodeModule {get; set;}
		public string DeviceId { get; set; }
	}
}

