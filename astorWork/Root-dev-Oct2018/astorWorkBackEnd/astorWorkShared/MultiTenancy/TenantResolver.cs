using astorWorkShared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SaasKit.Multitenancy;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkShared.MultiTenancy
{
    public class TenantResolver : ITenantResolver<TenantInfo>
    {
        private IAstorWorkTableStorage _tableStorage;
        private IDistributedCache _cache;

        public TenantResolver(IAstorWorkTableStorage tableStorage, IDistributedCache cache)
        {
            _tableStorage = tableStorage;
            _cache = cache;
        }

        public async Task<TenantContext<TenantInfo>> ResolveAsync(HttpContext context)
        {
            var tenant = new TenantInfo();
            TenantContext<TenantInfo> tenantContext = null;
            var hostName = context.Request.Host.Value.ToLower();
            var tenantName = string.Empty;
            var tenantInfoStr = string.Empty;

            if (context.Request.Headers.ContainsKey("TenantName"))
            {
                tenantName = context.Request.Headers["TenantName"].FirstOrDefault();
            }
            else if (hostName.Contains("localhost"))
            {
                tenantName = "localhost";
            }
            else
            {
                tenantName = hostName.Split('.')[0];
            }

            // Hard coded for testing
            //tenantName = "tenant2";

            if (!string.IsNullOrEmpty(tenantName))
            {
                try
                {
                    tenantInfoStr = _cache.GetString(tenantName);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Redis cache failed: " + exc.Message);
                }

                if (string.IsNullOrEmpty(tenantInfoStr))
                {
                    tenant = await _tableStorage.GetTenantInfo(tenantName);
                }
                else
                {
                    tenant = JsonConvert.DeserializeObject<TenantInfo>(tenantInfoStr);
                }
            }

            if (tenant == null)
            {
                throw new UnauthorizedAccessException("Invalid tenant");
            }
            else
            {
                tenant.TenantName = tenantName;

                if (context.Request.Headers.ContainsKey("OutsideHost"))
                {
                    tenant.Hostname = context.Request.Headers["OutsideHost"].FirstOrDefault();
                }
                else
                {
                    tenant.Hostname = hostName;
                }

                try
                {
                    tenantInfoStr = JsonConvert.SerializeObject(tenant);
                    var tenantInfoExpiry = TimeSpan.FromHours(1);
                    var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(tenantInfoExpiry);
                    _cache.SetString(tenantName, tenantInfoStr, cacheEntryOptions);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Redis cache failed: " + exc.Message);
                }
            }

            tenantContext = new TenantContext<TenantInfo>(tenant);

            if (!context.Request.Headers.ContainsKey("TenantName"))
            {
                context.Request.Headers.Add("TenantName", tenant.TenantName);
            }

            if (!context.Request.Headers.ContainsKey("OutsideHost"))
            {
                context.Request.Headers.Add("OutsideHost", tenant.Hostname);
            }

            return tenantContext;
        }


        public string GetConnection(TenantInfo _appTenant)
        {
            SqlConnectionStringBuilder _sqlConBuilder = new SqlConnectionStringBuilder();
            _sqlConBuilder.DataSource = _appTenant.DBServer;
            _sqlConBuilder.InitialCatalog = _appTenant.DBName;
            _sqlConBuilder.UserID = _appTenant.DBUserID;
            _sqlConBuilder.Password = _appTenant.DBPassword;
            _sqlConBuilder.ConnectTimeout = 15;
            return _sqlConBuilder.ConnectionString.ToString();
        }

    }
}