using MaterialTrackApp.Interface;
using MaterialTrackApp.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using MaterialTrackApp.Entities;

namespace MaterialTrackApp.Utility
{
    public sealed class LocalDBHandler
    {
        private static volatile LocalDBHandler instance;
        private static volatile SQLiteAsyncConnection database;


        private static object syncRoot = new Object();
        private bool initDone = false;

        private LocalDBHandler()
        {
        }

        public static LocalDBHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new LocalDBHandler();

                        }
                    }
                }

                return instance;
            }
        }

        public async Task<bool> InitDB(string databasePath)
        {
            initDone = false;
            if (database == null)
            {
                try
                {
                    database = new SQLiteAsyncConnection(DependencyService.Get<IFileHelper>().GetLocalFilePath(databasePath));

                    await database.CreateTableAsync<EnrollTaskEntity>();

                    initDone = true;
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    Debug.WriteLine(exc.StackTrace);
                }
            }
            return initDone;
        }

        public async Task<bool> ClearTable<T>() where T : new()
        {
            try
            {
                await database.DropTableAsync<T>();
                await database.CreateTableAsync<T>();

                return true;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);

                return false;
            }

        }

            public async Task<bool> ClearDB()
        {
            bool result = false;
            if (database != null && initDone)
            {
                try
                {
                    await database.DropTableAsync<EnrollTaskEntity>();

                    await database.CreateTableAsync<EnrollTaskEntity>();

                    result = true;
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    Debug.WriteLine(exc.StackTrace);
                }
            }
            return result;
        }

        public async Task<int> SaveEntitiesAsync(IEnumerable<MasterEntity> items)
        {
            int count = 0;
            foreach (var item in items)
            {
                try
                {
                    count += await database.InsertOrReplaceAsync(item);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }
            return count;
        }

        public Task<int> SaveBeaconAsync(BeaconEntity item)
        {
            if (item.ID != 0)
            {
                return database.UpdateAsync(item);
            }
            else
            {
                return database.InsertAsync(item);
            }
        }

        public async Task<int> SaveBeaconsAsync(IEnumerable<BeaconEntity> items)
        {
            int count = 0;
            foreach (var item in items)
            {
                try
                {
                    count += await database.InsertOrReplaceAsync(item);
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                }
            }
            return count;
        }

        public Task<List<MaterialEntity>> ReadMaterialsAsync()
        {
            return database.Table<MaterialEntity>().ToListAsync();
        }

        public Task<List<BuildingEntity>> ReadBuildingsAsync()
        {
            return database.Table<BuildingEntity>().ToListAsync();
        }

        public Task<List<BeaconEntity>> ReadBeaconsAsync()
        {
            return database.Table<BeaconEntity>().ToListAsync();
        }

        public Task<List<EnrollTaskEntity>> ReadEnrollTaskAsync()
        {
            return database.Table<EnrollTaskEntity>().ToListAsync();
        }
    }
}
