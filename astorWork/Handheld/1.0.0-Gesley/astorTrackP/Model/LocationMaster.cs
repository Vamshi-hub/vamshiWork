using System;
using Xamarin.Forms;
using SQLite;
using System.Collections.Generic;

namespace astorTrackP
{
	public class LocationMaster
	{        
        [PrimaryKey]
        public long AssociationID { get; set; }
        public string ParentType { get; set; }
        public string ParentId { get; set; }
        public string AssociationType { get; set; }
        public string ChildType { get; set; }
        public string ChildId { get; set; }
        public string ParentCode { get; set; }
        public string ParentDescription { get; set; }
        public string ChildCode { get; set; }
        public string ChildDescription { get; set; }
        public string Parents { get; set; }
    }

    
}

