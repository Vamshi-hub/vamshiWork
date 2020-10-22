using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;


namespace astorTrackP
{
    public class MaterialMasterDb
    {
        static object locker = new object();

        SQLiteConnection database;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tasky.DL.TaskDatabase"/> TaskDatabase. 
        /// if the database doesn't exist, it will create the database and all the tables.
        /// </summary>
        /// <param name='path'>
        /// Path.
        /// </param>
        public MaterialMasterDb()
        {
            database = DependencyService.Get<ISQLite>().GetConnection();
            // create the tables
            var IsTableExisting = database.GetTableInfo("MaterialMaster");
            if (!IsTableExisting.Any())
            {
                database.CreateTable<MaterialMaster>();
            }

            IsTableExisting = database.GetTableInfo("MaterialDetail");
            if (!IsTableExisting.Any())
            {
                database.CreateTable<MaterialDetail>();
            }

            IsTableExisting = database.GetTableInfo("MaterialMasterMarkingNo");
            if (!IsTableExisting.Any())
            {
                database.CreateTable<MaterialMasterMarkingNo>();
            }

            IsTableExisting = database.GetTableInfo("MaterialMasterDashboard");
            if (!IsTableExisting.Any())
            {
                database.CreateTable<MaterialMasterDashboard>();
            }
        }

        #region MaterialMaster

        public MaterialMaster GetMaterialMaster(string markingNo)
        {
            lock (locker)
            {
                return (from i in database.Table<MaterialMaster>() where i.MarkingNo == markingNo select i).First();
            }
        }

        public MaterialMaster GetMaterialMasterByMaterialNo(string materialNo)
        {
            lock (locker)
            {
                return (from i in database.Table<MaterialMaster>() where i.MaterialNo == materialNo select i).First();
            }
        }

        public MaterialMaster GetMaterialMaster(string status, string searchString)
        {
            lock (locker)
            {
                var MaterialDetail = App.MaterialMasterDb.GetMaterialDetails().Where(w => w.Status == status && w.RFIDTagID == searchString).Select(i => i.MarkingNo).ToList();
                if (MaterialDetail.Count() > 0)
                {
                    var MaterialMaster = database.Table<MaterialMaster>().Where(w => MaterialDetail.Contains(w.MarkingNo)); //w.Status == status &&
                    if (MaterialMaster.Count() > 0)
                        return MaterialMaster.First();
                    else
                        return null;
                }
                return null;
                //return (from master in MaterialMaster
                //        join detail in MaterialDetail on master.MarkingNo equals detail.MarkingNo
                //        where master.MarkingNo.Contains(searchString) || detail.RFIDTagID.Contains(searchString) select master );
            }
        }

        public MaterialMaster GetMaterialMaster(long associationID, string status, string searchString)
        {
            lock (locker)
            {
                var MaterialDetail = App.MaterialMasterDb.GetMaterialDetails().Where(w => w.Status == status && w.RFIDTagID == searchString).Select(i => i.MarkingNo).ToList();
                if (MaterialDetail.Count() > 0)
                {
                    var MaterialMaster = database.Table<MaterialMaster>().Where(w => MaterialDetail.Contains(w.MarkingNo) && (w.LocationID == associationID)); // w.Status == status &&
                    if (MaterialMaster.Count() > 0)
                        return MaterialMaster.First();
                    else
                        return null;
                }
                return null;
                //return (from master in MaterialMaster
                //        join detail in MaterialDetail on master.MarkingNo equals detail.MarkingNo
                //        where master.MarkingNo.Contains(searchString) || detail.RFIDTagID.Contains(searchString) select master );
            }
        }

        //public IEnumerable<MaterialMaster> GetPendingTerminateMaterialMasters(long associationID)
        //{
        //    lock (locker)
        //    {                
        //        var MaterialDetail = App.MaterialMasterDb.GetMaterialDetails().Where(w => w.Status == "Terminate" && w.LocationID == associationID).Select(i => i.MarkingNo).ToList();
        //        if (MaterialDetail.Count() > 0)
        //        {
        //            var MaterialMaster = database.Table<MaterialMaster>().Where(w => !MaterialDetail.Contains(w.MarkingNo) && (w.LocationID == associationID)); //w.Status == status &&
        //            if (MaterialMaster.Count() > 0)
        //                return MaterialMaster;
        //            else
        //                return null;
        //        }
        //        else
        //        {
        //            var MaterialMaster = database.Table<MaterialMaster>().Where(w => (w.LocationID == associationID)); //w.Status == status &&
        //            if (MaterialMaster.Count() > 0)
        //                return MaterialMaster;
        //            else
        //                return null;
        //        }
        //    }
        //}

        //public IEnumerable<MaterialMaster> GetIncompleteItems(string status)
        //{
        //    lock (locker)
        //    {
        //        var MaterialDetail = App.MaterialMasterDb.GetMaterialDetails().Where(w => w.Status == status).Select(i => i.MarkingNo).ToList();
        //        if (MaterialDetail.Count() > 0)
        //        {
        //            var MaterialMaster = database.Table<MaterialMaster>().Where(w => MaterialDetail.Contains(w.MarkingNo)); //w.Status == status && 
        //            return MaterialMaster;
        //        }
        //        return null;
        //     }
        //}

        public IEnumerable<MaterialMaster> GetMaterialMasters()
        {
            lock (locker)
            {
                return (database.Table<MaterialMaster>());
            }
        }

        public IEnumerable<MaterialMaster> GetMaterialMasters(string status)
        {
            lock (locker)
            {
                var MaterialMaster = database.Table<MaterialMaster>().Where(w => w.Status == status);
                return MaterialMaster;
            }
        }

        public IEnumerable<MaterialMasterMarkingNo> GetMaterialMasterMarkingNo()
        {
            lock (locker)
            {
                var MaterialMasterMarkingNo = database.Table<MaterialMasterMarkingNo>();
                return MaterialMasterMarkingNo;
            }
        }

        //public IEnumerable<MaterialMaster> GetMaterialMaster(long associationID, string status)
        //{
        //    lock (locker)
        //    {
        //        var MaterialDetail = App.MaterialMasterDb.GetMaterialDetails().Where(w => w.Status == status).Select(i => i.MarkingNo).ToList();
        //        if (MaterialDetail.Count() > 0)
        //        {
        //            var MaterialMaster = database.Table<MaterialMaster>().Where(w => w.Status == status && (w.LocationID == associationID) && MaterialDetail.Contains(w.MarkingNo));
        //            return MaterialMaster;
        //        }
        //        return null;
        //    }
        //}

        public IEnumerable<MaterialMaster> FindMaterialMasters(string status, string searchString)
        {
            lock (locker)
            {
                return (from i in database.Table<MaterialMaster>() where i.MarkingNo.Contains(searchString) && i.Status == status select i);
            }
        }

        public IEnumerable<MaterialMaster> FindMaterialMasters(string searchString)
        {
            lock (locker)
            {
                var MaterialMaster = database.Table<MaterialMaster>().Where(w => w.MarkingNo.Contains(searchString));
                if (MaterialMaster.Count() > 0)
                    return MaterialMaster;
                else
                    return null;
            }
        }

        public IEnumerable<MaterialMasterMarkingNo> FindMaterialMasterMarkingNo(string searchString)
        {
            lock (locker)
            {
                var MaterialMaster = database.Table<MaterialMasterMarkingNo>().Where(w => w.MarkingNo.Contains(searchString));
                if (MaterialMaster.Count() > 0)
                    return MaterialMaster;
                else
                    return null;
            }
        }

        //public IEnumerable<MaterialMaster> FindMaterialMasters(string status, string searchString)
        //{
        //    lock (locker)
        //    {                
        //        var MaterialDetail = App.MaterialDetailDb.GetItems().Where(w=>w.RFIDTagID.Contains(searchString)).Select(i=> i.MarkingNo).ToList();
        //        var MaterialMaster = database.Table<MaterialMaster>().Where(w => w.Status == status && (w.MarkingNo.Contains(searchString) || MaterialDetail.Contains(w.MarkingNo)));
        //        return MaterialMaster;
        //        //return (from master in MaterialMaster
        //        //        join detail in MaterialDetail on master.MarkingNo equals detail.MarkingNo
        //        //        where master.MarkingNo.Contains(searchString) || detail.RFIDTagID.Contains(searchString) select master );
        //    }
        //}

        public int GetMaterialMasterCount ()
		{
			lock (locker) {
                return (database.Table<MaterialMaster>().Count());
            }
		}

        public int InsertMaterialMaster(MaterialMaster item)
        {
            lock (locker)
            {
                return database.Insert(item);
            }
        }

        public int InsertMaterialMasterMarkingNo(MaterialMasterMarkingNo item)
        {
            lock (locker)
            {
                return database.Insert(item);
            }
        }

        public int UpdateMaterialMaster (MaterialMaster item) 
		{
			lock (locker) {
				return database.Update (item);
			}
		}

        public int UpdateMaterialMasterMarkingNo(MaterialMasterMarkingNo item)
        {
            lock (locker)
            {
                return database.Update(item);
            }
        }


        public int DeleteMaterialMaster(string name)
		{
			lock (locker) {
				return database.Delete<MaterialMaster>(name);
			}
		}

		public int ClearMaterialMaster()
		{
			lock (locker) {
				return database.DeleteAll<MaterialMaster>();
			}
		}

        #endregion

        #region MaterialDetail

        public IEnumerable<MaterialDetail> GetMaterialDetails(string materialNo)
        {
            lock (locker)
            {
                return (from i in database.Table<MaterialDetail>() where i.MaterialNo == materialNo select i);
            }
        }

        public IEnumerable<MaterialDetail> GetMaterialDetails()
        {
            lock (locker)
            {
                return (database.Table<MaterialDetail>());
            }
        }

        //public MaterialDetail GetMaterialDetail(string MarkingNo, string rfidTagID)
        //{
        //    lock (locker)
        //    {
        //        return (database.Table<MaterialDetail>().Where(w => w.MarkingNo == MarkingNo && w.RFIDTagID == rfidTagID).First());
        //    }
        //}

        //public int GetMaterialDetailCount(string MarkingNo)
        //{
        //    lock (locker)
        //    {
        //        return (database.Table<MaterialDetail>().Where(w => w.MarkingNo == MarkingNo).Count());
        //    }
        //}

        //public int SaveMaterialDetail(MaterialDetail item)
        //{
        //    lock (locker)
        //    {
        //        if (database.Table<MaterialDetail>().Where(w => w.RFIDTagID == item.RFIDTagID).Count() > 0)
        //            return database.Update(item);
        //        else
        //            return database.Insert(item);
        //    }
        //}

        public int InsertMaterialDetail(MaterialDetail item)
        {
            lock (locker)
            {
                return database.Insert(item);
            }
        }

        //public int DeleteMaterialDetail(string RFIDTagID)
        //{
        //    lock (locker)
        //    {
        //        return database.Delete<MaterialDetail>(RFIDTagID);
        //    }
        //}

        public int ClearMaterialDetail()
        {
            lock (locker)
            {
                return database.DeleteAll<MaterialDetail>();
            }
        }

        public int ClearMaterialMasterMarkingNo()
        {
            lock (locker)
            {
                return database.DeleteAll<MaterialMasterMarkingNo>();
            }
        }
        #endregion

        #region MaterialMasterDashboard
        public int ClearMaterialMasterDashboard()
        {
            lock (locker)
            {
                return database.DeleteAll<MaterialMasterDashboard>();
            }
        }

        public int InsertMaterialMasterDashboard(MaterialMasterDashboard item)
        {
            lock (locker)
            {
                return database.Insert(item);
            }
        }

        public IEnumerable<MaterialMasterDashboard> GetMaterialMasterDashboard()
        {
            lock (locker)
            {
                return (database.Table<MaterialMasterDashboard>());
            }
        }
        #endregion
    }
}

