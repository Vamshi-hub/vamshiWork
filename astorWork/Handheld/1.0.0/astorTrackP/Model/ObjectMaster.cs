using System;
using Xamarin.Forms;
using SQLite;

namespace astorTrackP
{
	public class ObjectMaster
	{
		[PrimaryKey]
		public long ObjectID { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
		public string ObjectType { get; set; }
	}

}

