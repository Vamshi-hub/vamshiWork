using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace astorTrackP
{
	public class ModuleDb
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
		public ModuleDb()
		{
			database = DependencyService.Get<ISQLite> ().GetConnection ();
			// create the tables

			var IsTableExisting = database.GetTableInfo("Modules");
			if (!IsTableExisting.Any())
			{
				database.CreateTable<Module>();
			}
		}

		public IEnumerable<Module> GetItems ()
		{
			lock (locker) {
				return (from i in database.Table<Module>() select i).ToList();
			}
		}

		public int GetItemsCount ()
		{
//			lock (locker) {
//				return database.Query<DocumentType>("SELECT * FROM [DocumentType] WHERE [Done] = 0");
//			}
			lock (locker) {
				return (database.Table<Module> ().Count ());				        
			}
		}

//		public DocumentType GetItem (int id) 
//		{
//			lock (locker) {
//				return database.Table<DocumentType>().FirstOrDefault(x => x.ID == id);
//			}
//		}

		public int InsertItem (Module item) 
		{
			lock (locker) {
				return database.Insert (item);
			}
		}

		public int DeleteItem(string name)
		{
			lock (locker) {
				return database.Delete<Module>(name);
			}
		}
	}
}

