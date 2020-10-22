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
    public class ConfigRepository
    {
        readonly string _connString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public DateTime GetLastBeaconSync()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"SELECT [Setting] FROM [dbo].[ConfigurationMaster] WHERE [Configuration] = 'Last Beacon Sync'", connection))
                {
                    command.Notification = null;                    

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                        return DateTime.Parse(reader["Setting"].ToString());
                }
            }
            return DateTime.Now;
        }

        public DateTime UpdateLastBeaconSync()
        {
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"UPDATE [dbo].[ConfigurationMaster] SET [Setting] = @Setting WHERE [Configuration] = 'Last Processed Date'"
                , connection))
                {
                    command.Notification = null;

                    command.Parameters.AddWithValue("@Setting", DateTime.Now);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                        return DateTime.Parse(reader["Setting"].ToString());
                }
            }
            return DateTime.Now;
        }
    }
}