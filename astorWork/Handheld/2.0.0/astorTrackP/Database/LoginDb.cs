using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace astorTrackP
{
	public class LoginDb
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
		public LoginDb()
		{
			database = DependencyService.Get<ISQLite> ().GetConnection ();
			// create the tables

			var IsTableExisting = database.GetTableInfo("Login");
			if (!IsTableExisting.Any())
			{
				database.CreateTable<Login>();
			}
		}

		public Login GetItem ()
		{
			lock (locker) {
				return (from i in database.Table<Login> ()
					select i).FirstOrDefault ();
			}
		}

		public int GetItemsCount ()
		{
			lock (locker) {
				return database.Table<Login> ().Count ();
			}
		}

		public int InsertItem (Login item) 
		{
			lock (locker) {
				if (GetItemsCount () > 0)
					ClearItems ();

				return database.Insert (item);					
			}
		}

		public int UpdateItem(Login item)
		{
			lock (locker)
			{
				return database.Update(item);
			}
		}

		public int DeleteItem(string name)
		{
			lock (locker) {
				return database.Delete<Login>(name);
			}
		}

		public int ClearItems()
		{
			lock (locker) {
				return database.DeleteAll<Login>();
			}
		}

	}
}

