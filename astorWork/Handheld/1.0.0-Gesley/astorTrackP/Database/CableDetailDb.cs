using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace astorCable
{
	public class CableDetailDb
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
		public CableDetailDb()
		{
			database = DependencyService.Get<ISQLite> ().GetConnection ();
			// create the tables
			var IsTableExisting = database.GetTableInfo("CableDetail");
			if (!IsTableExisting.Any())
			{
				database.CreateTable<CableDetail>();
			}
		}


		//public IEnumerable<CableDetail> GetItems(string cableTagNo)
		//{
		//	lock (locker)
		//	{
  //              return (from i in database.Table<CableDetail>() where i.CableTagNo == cableTagNo select i);
		//	}
		//}

        //public IEnumerable<CableDetail> GetItems()
        //{
        //    lock (locker)
        //    {
        //        return (database.Table<CableDetail>());
        //    }
        //}

        //public CableDetail GetItem(string cableTagNo, string rfidTagID)
        //{
        //    lock (locker)
        //    {
        //        return (database.Table<CableDetail>().Where(w=> w.CableTagNo == cableTagNo && w.RFIDTagID == rfidTagID).First());
        //    }
        //}


  //      public int GetItemsCount (string cableTagNo)
		//{
		//	lock (locker) {
  //              return (database.Table<CableDetail>().Where(w=>w.CableTagNo== cableTagNo).Count());
  //          }
		//}

		//public int SaveItem (CableDetail item) 
		//{
  //          lock (locker)
  //          {                
  //              if (database.Table<CableDetail>().Where(w => w.RFIDTagID == item.RFIDTagID).Count() > 0)
  //                  return database.Update(item);
  //              else
  //                  return database.Insert (item);
		//	}
		//}

		//public int UpdateItem (CableDetail item) 
		//{
		//	lock (locker) {
		//		return database.Update (item);
		//	}
		//}

		//public int DeleteItem(string RFIDTagID)
		//{
		//	lock (locker) {
		//		return database.Delete<CableDetail>(RFIDTagID);
		//	}
		//}

		//public int ClearItems()
		//{
		//	lock (locker) {
		//		return database.DeleteAll<CableDetail>();
		//	}
		//}
	}
}

