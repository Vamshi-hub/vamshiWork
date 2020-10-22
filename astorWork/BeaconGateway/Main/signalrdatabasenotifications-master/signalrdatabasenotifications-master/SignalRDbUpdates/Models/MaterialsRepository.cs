using SignalRDbUpdates.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SignalRDbUpdates.Models
{
    public class MaterialsRepository
    {
        //readonly string _connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private astorTrack_GEEntities db = new astorTrack_GEEntities();

        public List<MaterialMaster> GetProducedList(DateTime lastSyncDate)
        {
            return db.MaterialMaster.Where(m => m.Status == "Produced" && m.UpdatedDate > lastSyncDate).ToList();

            //var materials = new List<MaterialMaster>();
            //using (var connection = new SqlConnection(_connString))
            //{
            //    connection.Open();
            //    using (var command = new SqlCommand(@"SELECT [MaterialID], [MaterialNo], [MarkingNo], [BeaconID] 
            //        FROM [dbo].[MaterialMaster] WHERE [Status] = 'Produced' AND [UpdatedDate] > @lastSyncDate"
            //        , connection))
            //    {
            //        command.Notification = null;

            //        // to handle 1st time when Produced List is synced and year is 0001
            //        if (lastSyncDate.Year < 2)
            //            lastSyncDate = lastSyncDate.AddYears(1800);

            //        command.Parameters.AddWithValue("@LastSyncDate", lastSyncDate);

            //        if (connection.State == ConnectionState.Closed)
            //            connection.Open();

            //        var reader = command.ExecuteReader();

            //        while (reader.Read())
            //        {
            //            materials.Add(
            //                item: new MaterialMaster {
            //                    MaterialID = int.Parse(reader["MaterialID"].ToString()),
            //                    MaterialNo = reader["MaterialNo"].ToString(),
            //                    MarkingNo = reader["MarkingNo"].ToString(),
            //                    BeaconID = reader["BeaconID"].ToString()
            //                }
            //            );
            //        }
            //    }
              
            //}

            //return materials;
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                BeaconsHub.SendBeacons();
            }
        }

        public void InsertMaterialDetail(MaterialDetail materialDetail)
        {
            // Insert to MaterialDetal
            db.MaterialDetails.Add(materialDetail);

            // Update MaterialMaster
            MaterialMaster mm = db.MaterialMaster.Where(m => m.MaterialNo == materialDetail.MaterialNo && m.Status == "Produced").FirstOrDefault<MaterialMaster>();            
            mm.Status = "Delivered";
            mm.UpdatedDate = DateTime.Now;
            mm.UpdatedBy = "admin";
            db.Entry(mm).State = System.Data.Entity.EntityState.Modified;

            db.SaveChanges();

            //var materials = new List<MaterialMaster>();
            //using (var connection = new SqlConnection(_connString))
            //{
            //    connection.Open();
            //    using (var command = new SqlCommand(
            //        "bc_processDeliveredComponent"
            //        , connection)
            //    )
            //    {
            //        command.CommandType = CommandType.StoredProcedure;
            //        command.Parameters.AddWithValue("@MaterialNo", materialDetail.MaterialNo);
            //        command.Parameters.AddWithValue("@MarkingNo", materialDetail.MarkingNo);
            //        command.Parameters.AddWithValue("@Stage", materialDetail.Stage);
            //        command.Parameters.AddWithValue("@IsQC", materialDetail.IsQC);
            //        command.Parameters.AddWithValue("@QCStatus", materialDetail.QCStatus);
            //        command.Parameters.AddWithValue("@QCBy", materialDetail.QCBy);
            //        command.Parameters.AddWithValue("@QCDate", materialDetail.QCDate);
            //        command.Parameters.AddWithValue("@LocationID", materialDetail.LocationID);
            //        command.Parameters.AddWithValue("@BeaconID", materialDetail.BeaconID);
            //        command.Parameters.AddWithValue("@CreatedBy", materialDetail.CreatedBy);
            //        command.Parameters.AddWithValue("@CreatedDate", materialDetail.CreatedDate);
                    
            //        command.Notification = null;

            //        if (connection.State == ConnectionState.Closed)
            //            connection.Open();

            //        command.ExecuteNonQuery();              
            //    }
            //}
        }

        //public void UpdateMaterialMaster(string materialNo) {
        //    using (var connection = new SqlConnection(_connString))
        //    {
        //        connection.Open();
        //        using (var command = new SqlCommand(
        //            "UPDATE MaterialMaster SET Status = 'Delivered', UpdatedDate = GETDATE(), UpdatedBy = 'admin' WHERE " +
        //            "MaterialNo = @MaterialNo AND Status = 'Produced'; "
        //            , connection)
        //        )
        //        {
        //            command.Parameters.AddWithValue("@MaterialNo", materialNo);
        //            command.Notification = null;

        //            if (connection.State == ConnectionState.Closed)
        //                connection.Open();

        //            command.ExecuteNonQuery();
        //        }
        //    }
        //}
    }
}