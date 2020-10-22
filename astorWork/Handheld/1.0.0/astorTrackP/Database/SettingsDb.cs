using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace astorTrackP
{
	public class SettingsDb
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
		public SettingsDb()
		{
			database = DependencyService.Get<ISQLite> ().GetConnection ();

			var IsTableExisting = database.GetTableInfo("Settings");
			if (!IsTableExisting.Any())
			{
				database.CreateTable<Settings>();
			}
		}

		public Settings GetItem ()
		{
			lock (locker) {
				return (from i in database.Table<Settings> ()
				        select i).FirstOrDefault ();
			}
		}

		public int GetItemsCount ()
		{
			lock (locker) {
				return (database.Table<Settings> ().Count ());				        
			}
		}

		public int SaveItem (Settings item) 
		{
			lock (locker) {
                if (GetItemsCount() > 0)
                    return database.Update(item);
                else
                    return database.Insert(item);
			}
		}

		public int DeleteItem(string name)
		{
			lock (locker) {
				return database.Delete<Settings>(name);
			}
		}

		public int ClearItems()
		{
			lock (locker) {
				return database.DeleteAll<Settings>();
			}
		}
	}
}

