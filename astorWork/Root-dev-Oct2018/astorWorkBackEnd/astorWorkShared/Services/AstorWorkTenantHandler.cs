using astorWorkShared.MultiTenancy;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public class AstorWorkTenantHandler : IAstorWorkTenantHandler
    {
        private IAstorWorkTableStorage _tableStorage;
        private IDistributedCache _cache;

        public AstorWorkTenantHandler(IAstorWorkTableStorage tableStorage, IDistributedCache cache)
        {
            _tableStorage = tableStorage;
            _cache = cache;
        }

        public Task<TenantInfo> AddTenantAsync(TenantInfo tenantInfo)
        {
        }

        public async Task<TenantInfo> GetTenantAsync(HttpContext context)
        {
            var tenant = new TenantInfo();

            var hostName = context.Request.Host.Value.ToLower();
            var tenantName = string.Empty;
            var tenantInfoStr = string.Empty;

            if (context.Request.Headers.ContainsKey("TenantName"))
                tenantName = context.Request.Headers["TenantName"].FirstOrDefault();
            else if (hostName.Contains("localhost"))
                tenantName = "localhost";
            else
                tenantName = hostName.Split('.')[0];

            if (!string.IsNullOrEmpty(tenantName))
            {
                try
                {
                    tenantInfoStr = _cache.GetString(tenantName);
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"[{DateTime.UtcNow}] Redis cache failed: {exc.Message}");
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

            if (!context.Request.Headers.ContainsKey("TenantName"))
            {
                context.Request.Headers.Add("TenantName", tenant.TenantName);
            }

            if (!context.Request.Headers.ContainsKey("OutsideHost"))
            {
                context.Request.Headers.Add("OutsideHost", tenant.Hostname);
            }

            return tenant;
        }
    }
}
