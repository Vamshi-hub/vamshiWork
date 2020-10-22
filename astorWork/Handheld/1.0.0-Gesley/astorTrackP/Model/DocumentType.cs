using System;
using SQLite;

namespace astorTrackP
{
	public class DocumentType
	{
		[PrimaryKey]
		public string Name { get; set; }

		public DocumentType()
		{}
	}
}

