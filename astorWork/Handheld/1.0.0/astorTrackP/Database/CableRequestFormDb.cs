using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace astorTrackP
{
	public class CableRequestFormDb
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
		public CableRequestFormDb()
		{
			database = DependencyService.Get<ISQLite> ().GetConnection ();
			// create the tables
			var IsTableExisting = database.GetTableInfo("CRFMaster");
			if (!IsTableExisting.Any())
			{
				database.CreateTable<CRFMaster>();
            }

            IsTableExisting = database.GetTableInfo("CRFDetail");
            if (!IsTableExisting.Any())
            {
                database.CreateTable<CRFDetail>();
            }
        }

        #region CRFMaster
        public CRFMaster GetCRFMaster(string CRFNo)
		{
			lock (locker)
			{
                return (from i in database.Table<CRFMaster>() where i.CRFNo == CRFNo select i).FirstOrDefault();
			}
		}

        public IEnumerable<CRFMaster> GetCRFMasters()
        {
            lock (locker)
            {
                return (database.Table<CRFMaster>());
            }
        }

        public IEnumerable<CRFMaster> FindCRFMaster(string searchDocument)
        {
            lock (locker)
            {
                return (database.Table<CRFMaster>().Where(w => w.CRFNo.Contains(searchDocument) || w.Contractor.Contains(searchDocument) || w.Officer.Contains(searchDocument)));
            }
        }

        public int GetCRFMasterCount()
        {
            lock (locker)
            {
                return (database.Table<CRFMaster>().Count());
            }
        }

        public int GetCRFDetailCount()
        {
            lock (locker)
            {
                return (database.Table<CRFDetail>().Count());
            }
        }

        public int UpdateCRFMaster(CRFMaster item)
        {
            lock (locker)
            {
                return database.Update(item);
            }
        }

        public int InsertCRFMaster(CRFMaster item)
        {
            lock (locker)
            {
                return database.Insert(item);
            }
        }

        public int ClearCRFMaster()
        {
            lock (locker)
            {
                return database.DeleteAll<CRFMaster>();
            }
        }

        public int DeleteCRFMaster(string crfNo)
        {
            lock (locker)
            {
                return database.Delete<CRFMaster>(crfNo);
            }
        }

        #endregion

        #region CRFDetail

        public IEnumerable<CRFDetail> GetCRFDetail(string CRFNo)
        {
            lock (locker)
            {
                return (from i in database.Table<CRFDetail>() where i.CRFNo == CRFNo select i);
            }
        }

        public CRFDetail GetCRFDetail(string CRFNo, string MarkingNo)
        {
            lock (locker)
            {
                return (from i in database.Table<CRFDetail>() where i.CRFNo == CRFNo && i.MarkingNo == MarkingNo select i).First();
            }
        }

        public CRFDetail GetCRFDetail(string CRFNo, long associationID)
        {
            lock (locker)
            {
                return (from i in database.Table<CRFDetail>() where i.CRFNo == CRFNo && i.AssociationID == associationID select i).First();
            }
        }

        public IEnumerable<CRFDetail> GetCRFDetails()
        {
            lock (locker)
            {
                return (database.Table<CRFDetail>());
            }
        }

        public int UpdateCRFDetail(CRFDetail item)
        {
            lock (locker)
            {
                return database.Update(item);
            }
        }

        public int InsertCRFDetail(CRFDetail item)
        {
            lock (locker)
            {
                return database.Insert(item);
            }
        }

        //      public int DeleteItem(string RFIDTagID)
        //{
        //	lock (locker) {
        //		return database.Delete<CRFMaster>(RFIDTagID);
        //	}
        //}

        public int ClearCRFDetail()
        {
            lock (locker)
            {
                return database.DeleteAll<CRFDetail>();
            }
        }

        public int DeleteCRFDetail(string MarkingNo)
        {
            lock (locker)
            {
                return database.Delete<CRFDetail>(MarkingNo);
            }
        }
        
        #endregion

    }
}

