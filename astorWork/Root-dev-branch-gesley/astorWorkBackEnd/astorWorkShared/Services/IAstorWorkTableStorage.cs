using astorWorkShared.GlobalModels;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public interface IAstorWorkTableStorage
    {
        Task<List<TenantInfo>> ListTenants();
        Task<TenantInfo> GetTenantInfo(string TenantName);
        Task<object> GetEntity(string tableName, string partitionKey, string rowKey);
        Task<bool> AddOrUpdateEntity(string tableName, TableEntity entity);

        string ErrorMessage();
    }
}
