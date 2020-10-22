using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace astorWorkShared.GlobalModels
{
    public class TenantInfo : TableEntity
    {
        public TenantInfo()
        {
        }

        public TenantInfo(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
            TenantName = rowKey;
        }

        public string TenantName { get; set; }
        public string DBServer { get; set; }
        public string DBName { get; set; }
        public string DBUserID { get; set; }
        public string DBPassword { get; set; }
        public string ConnectionString
        {
            get
            {
                try
                {
                    SqlConnectionStringBuilder _sqlConBuilder = new SqlConnectionStringBuilder();
                    _sqlConBuilder.DataSource = DBServer;
                    _sqlConBuilder.InitialCatalog = DBName;
                    _sqlConBuilder.UserID = DBUserID;
                    _sqlConBuilder.Password = DBPassword;
                    _sqlConBuilder.ConnectTimeout = 15;
                    return _sqlConBuilder.ConnectionString.ToString();
                }
                catch (Exception exc)
                {
                    Debug.WriteLine(exc.Message);
                    return null;
                }
            }
        }
        public string BackgroundImageURL { get; set; }
        public string LogoImageURL { get; set; }
        public string PowerBIWorkSpaceGUID { get; set; }
        public int TimeZone { get; set; }
        public bool Enabled { get; set; }
        public string Hostname { get; set; }
        public string EnabledModules { get; set; }
    }
}
