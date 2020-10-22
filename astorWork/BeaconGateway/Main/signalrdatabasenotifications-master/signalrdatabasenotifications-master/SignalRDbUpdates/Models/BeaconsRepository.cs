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
    public class BeaconsRepository
    {
        readonly string _connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        private astorTrack_GEEntities db = new astorTrack_GEEntities();


        public IEnumerable<Beacon> GetAllBeacons()
        {

            var beacons = new List<Beacon>();
            //return db.Beacons.ToList<Beacon>();

            using (var connection = new SqlConnection(_connString))
            {

                connection.Open();
                using (var command = new SqlCommand(@"SELECT [BeaconID], [LocationID], [CreatedDate], [CreatedBy] FROM [dbo].[Beacons]", connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        beacons.Add(item: new Beacon
                        {
                            BeaconID = (string)reader["BeaconID"],
                            LocationID = (int)reader["LocationID"],
                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                            CreatedBy = (string)reader["CreatedBy"]
                        });
                    }
                }
            }

            return beacons;
        }
            
        //public void DeleteBeacon(string beaconID) {
        //    using (SqlConnection conn =
        //    new SqlConnection(_connString))
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd =
        //            new SqlCommand("DELETE FROM Beacons " +
        //                "WHERE BeaconID=@BeaconID", conn))
        //        {
        //            cmd.Parameters.AddWithValue("@BeaconID", beaconID);

        //            int rows = cmd.ExecuteNonQuery();
        //        }
        //    }
        //}

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                BeaconsHub.SendBeacons();
            }
        }
    }
}