using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace astorTrackP
{
	public class LocationMasterDb
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
		public LocationMasterDb()
		{
			database = DependencyService.Get<ISQLite> ().GetConnection ();

			var IsTableExisting = database.GetTableInfo("LocationMaster");
			if (!IsTableExisting.Any())
			{
				database.CreateTable<LocationMaster>();
			}
		}

        public IEnumerable<LocationMaster> GetItems()
        {
            lock (locker)
            {
                return (database.Table<LocationMaster>());
            }
        }

        public LocationMaster GetItem(long associationID)
        {
            lock (locker)
            {
                return (database.Table<LocationMaster>().Where(w=> w.AssociationID == associationID).First());
            }
        }

        //public LocationMaster CheckRFIDExists(long associationID, string rfid)
        //{
        //    lock (locker)
        //    {
        //        var location = database.Table<LocationMaster>().Where(w => w.AssociationID != associationID && w.RFIDTagID == rfid);
        //        if (location.Count()>0)
        //            return (location.First());
        //        else
        //            return null;
        //    }
        //}

        //public IEnumerable<LocationMaster> FindItems(string location)
        //{
        //    lock (locker)
        //    {
        //        return (database.Table<LocationMaster>().Where(w => w.JunctionBox.Contains(location) || w.RFIDTagID.Contains(location)));
        //    }
        //}

        //public LocationMaster FindItem(string RFIDTagID)
        //{
        //    lock (locker) {
        //        return (database.Table<LocationMaster>().Where(w => w.RFIDTagID == RFIDTagID)).First();
        //    }
        //}

        public int GetItemsCount()
        {
            lock (locker) {
                return (database.Table<LocationMaster>().Count());
            }
        }

        public int UpdateItem(LocationMaster item)
        {
            lock (locker)
            {
                return database.Update(item);
            }
        }

        public int InsertItem (LocationMaster item) 
		{
			lock (locker) {
               return database.Insert(item);               
			}
		}

        public int ClearItems()
        {
            lock (locker)
            {
                return database.DeleteAll<LocationMaster>();
            }
        }
    }
}

