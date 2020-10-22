
using astorWorkShared.MultiTenancy;
using astorWorkShared.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public class AstorWorkTableStorage : IAstorWorkTableStorage
    {
        CloudStorageAccount _storageAccount;
        CloudTableClient _tableClient;
        bool _initSuccess = false;
        string _errorMessage = string.Empty;

        public AstorWorkTableStorage()
        {
            var connectionString = AppConfiguration.GetCloudStorageAccount();
            if (!string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    _storageAccount = CloudStorageAccount.Parse(connectionString);
                    _tableClient = _storageAccount.CreateCloudTableClient();
                    _initSuccess = true;
                }
                catch (Exception exc)
                {
                    _errorMessage = exc.Message;
                    Console.WriteLine(exc.Message);
                }
            }
        }

        public async Task<bool> AddOrUpdateEntity(string tableName, TableEntity entity)
        {
            bool success = false;
            if (_initSuccess && !string.IsNullOrEmpty(tableName) && entity != null)
            {
                try
                {
                    var targetTable = _tableClient.GetTableReference(tableName);
                    var insertOperation = TableOperation.InsertOrMerge(entity);
                    var result = await targetTable.ExecuteAsync(insertOperation);
                    // If success, it will be 204 (No content)
                    success = result.HttpStatusCode == 204;
                }
                catch (Exception exc)
                {
                    _errorMessage = exc.Message;
                    Console.WriteLine(exc.Message);
                }
            }

            return success;
        }

        public async Task<List<TenantInfo>> ListTenants()
        {
            string tableName = AppConfiguration.GetTenantTableName();
            var result = new List<TenantInfo>();
            if (_initSuccess && !string.IsNullOrEmpty(tableName))
            {
                try
                {
                    CloudTable targetTable = _tableClient.GetTableReference(tableName);
                    var query = new TableQuery<TenantInfo>();
                    // Print the fields for each customer.
                    TableContinuationToken token = null;
                    var queryResult = await targetTable.ExecuteQuerySegmentedAsync(query, token);
                    result = queryResult.Results;
                }
                catch (Exception exc)
                {
                    _errorMessage = exc.Message;
                    Console.WriteLine(exc.Message);
                }
            }

            return result;
        }

        public async Task<object> GetEntity(string tableName, string partitionKey, string rowKey)
        {
            object result = null;
            if (_initSuccess && !string.IsNullOrEmpty(tableName) &&
                !string.IsNullOrEmpty(partitionKey) && !string.IsNullOrEmpty(rowKey))
            {
                try
                {
                    CloudTable targetTable = _tableClient.GetTableReference(tableName);
                    TableOperation retrieveOperation = TableOperation.Retrieve<TableEntity>(partitionKey, rowKey);

                    TableResult retrievedResult = await targetTable.ExecuteAsync(retrieveOperation);
                    result = retrievedResult.Result;
                }
                catch (Exception exc)
                {
                    _errorMessage = exc.Message;
                    Console.WriteLine(exc.Message);
                }
            }

            return result;
        }

        public async Task<TenantInfo> GetTenantInfo(string TenantName)
        {
            string tableName = AppConfiguration.GetTenantTableName();
            string partitionKey = "TenantConfig";
            TenantInfo result = null;
            if (_initSuccess && !string.IsNullOrEmpty(tableName) &&
                !string.IsNullOrEmpty(partitionKey) && !string.IsNullOrEmpty(TenantName))
            {
                try
                {
                    CloudTable targetTable = _tableClient.GetTableReference(tableName);
                    TableOperation retrieveOperation = TableOperation.Retrieve<TenantInfo>(partitionKey, TenantName);

                    TableResult retrievedResult = await targetTable.ExecuteAsync(retrieveOperation);
                    result = retrievedResult.Result as TenantInfo;
                }
                catch (Exception exc)
                {
                    _errorMessage = exc.Message;
                    Console.WriteLine(exc.Message);
                }
            }

            return result;
        }

        public string ErrorMessage()
        {
            return _errorMessage;
        }
    }
}
