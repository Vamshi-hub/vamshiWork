using System;
using SQLite;

namespace astorTrackP
{
	public class Login
	{
		[PrimaryKey]
        public string DeviceID { get; set; }

        public string EndPoint { get; set; }
		public string UserID {get;set;}
		public string UserName {get;set;}
		public string Password {get;set;}
		public int RoleID {get;set;}
		public string RoleLocationID {get;set;}        
        public bool IsContractor { get; set; }		
		public bool RememberMe {get;set;}		
		public int LoginAttempt { get; set; }
        

        public long DeliveredAssociationID { get; set; }
        public long InstalledAssociationID { get; set; }

        public Login ()
		{
		}
	}
}

