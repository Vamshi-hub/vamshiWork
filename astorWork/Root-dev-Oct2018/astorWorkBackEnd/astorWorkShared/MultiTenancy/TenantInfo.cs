using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Data.SqlClient;

namespace astorWorkShared.MultiTenancy
{
    public class TenantInfo : TableEntity
    {
        public string TenantName { get; set; }
        public string DBServer { get; set; }
        public string DBName { get; set; }
        public string DBUserID { get; set; }
        public string DBPassword { get; set; }
        public string ConnectionString
        {
            get
            {

                SqlConnectionStringBuilder _sqlConBuilder = new SqlConnectionStringBuilder();
                _sqlConBuilder.DataSource = DBServer;
                _sqlConBuilder.InitialCatalog = DBName;
                _sqlConBuilder.UserID = DBUserID;
                _sqlConBuilder.Password = DBPassword;
                _sqlConBuilder.ConnectTimeout = 15;
                return _sqlConBuilder.ConnectionString.ToString();
            }
        }
        public string BackgroundImageFileName { get; set; }
        public string LogoImageFileName { get; set; }
        public string PowerBIWorkSpaceGUID { get; set; }
        public int TimeZone { get; set; }
        public bool Enabled { get; set; }
        public string Hostname { get; set; }
    }
}
