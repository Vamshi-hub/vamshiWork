using System;
using SQLite;
using System.Collections.ObjectModel;

namespace astorTrackP
{
	public class Module
	{
		public string ParentModule { get; set; }
		public string ChildModule { get; set; }

		public Module()
		{}
	}
}

