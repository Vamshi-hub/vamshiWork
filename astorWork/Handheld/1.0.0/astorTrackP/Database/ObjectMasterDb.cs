using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace astorTrackP
{
	public class ObjectMasterDb
	{
		static object locker = new object ();

		SQLiteConnection database;

		/// <summary>
		/// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		/// <param name='path'>
		/// Path.
		/// </param>
		public ObjectMasterDb()
		{
			database = DependencyService.Get<ISQLite> ().GetConnection ();

			var IsTableExisting = database.GetTableInfo("ObjectMaster");
			if (!IsTableExisting.Any())
			{
				database.CreateTable<ObjectMaster>();
			}
		}

		public IEnumerable<ObjectMaster> GetItems (string objectType)
		{
			lock (locker) {
				return (from i in database.Table<ObjectMaster>() where i.ObjectType == objectType select i).ToList();
			}
		}

		public IEnumerable<ObjectMaster> GetItems ()
		{
			lock (locker) {
				return (from i in database.Table<ObjectMaster>() select i).ToList();
			}
		}

		public int GetItemsCount (string locationType)
		{
//			lock (locker) {
//				return database.Query<DocumentType>("SELECT * FROM [DocumentType] WHERE [Done] = 0");
//			}
			lock (locker) {
				return (database.Table<ObjectMaster> ().Where(w=>w.ObjectType == locationType).Count ());				        
			}
		}

		public ObjectMaster GetItem (string locationType, string description) 
		{
			lock (locker) {
				return database.Table<ObjectMaster>().FirstOrDefault(x => x.Description == description && x.ObjectType == locationType);
			}
		}

		public int InsertItem (ObjectMaster item) 
		{
			lock (locker) {
				return database.Insert (item);
			}
		}

		public int DeleteItem(string locationID)
		{
			lock (locker) {
				return database.Delete<ObjectMaster>(locationID);
			}
		}

		public int ClearItems(string locationType)
		{
			lock (locker) {
				return database.Execute("Delete FROM [ObjectMaster] WHERE ObjectType = ? ", locationType);
				//return database.DeleteAll<ObjectMaster>();
			}
		}
	}
}

