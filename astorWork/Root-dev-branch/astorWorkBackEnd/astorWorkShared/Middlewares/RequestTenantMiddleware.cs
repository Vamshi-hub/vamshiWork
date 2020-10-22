using astorWorkShared.GlobalModels;
using astorWorkShared.GlobalResponse;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkShared.Middlewares
{
    public class RequestTenantMiddleware
    {
        private readonly RequestDelegate _next;
        private IAstorWorkTableStorage _tableStorage;
        private IDistributedCache _cache;

        public RequestTenantMiddleware(RequestDelegate next, IAstorWorkTableStorage tableStorage, IDistributedCache cache)
        {
            _next = next;
            _tableStorage = tableStorage;
            _cache = cache;
        }

        private string GetTenantName(HttpContext context, string hostName)
        {
            string tenantName = string.Empty;

            if (context.Request.Headers.ContainsKey("TenantName"))
                return context.Request.Headers["TenantName"].FirstOrDefault();
            else if (hostName.Contains("localhost"))
                return "localhost";
            else
                return hostName.Split('.')[0];
        }

        private async Task<TenantInfo> GetTenant(string tenantName)
        {
            TenantInfo tenant = new TenantInfo();

            if (!string.IsNullOrEmpty(tenantName))
            {
                string tenantInfoStr = string.Empty;

                try
                {
                    tenantInfoStr = _cache.GetString(tenantName);
                }
                catch (Exception exc)
                {
                    Console.WriteLine($"[{DateTime.UtcNow}] Redis cache failed: {exc.Message}");
                }

                if (string.IsNullOrEmpty(tenantInfoStr))
                    tenant = await _tableStorage.GetTenantInfo(tenantName);
                else
                    tenant = JsonConvert.DeserializeObject<TenantInfo>(tenantInfoStr);
            }

            return tenant;
        }

        private TenantInfo CopyTenantInfo(HttpContext context, TenantInfo tenant, TenantInfo tenantInfo, string tenantName, string hostName)
        {
            tenant.TenantName = tenantName;
            CommonUtility.MemberwiseCopy(tenant, tenantInfo);

            if (context.Request.Headers.ContainsKey("OutsideHost"))
                tenantInfo.Hostname = context.Request.Headers["OutsideHost"].FirstOrDefault();
            else
                tenantInfo.Hostname = hostName;

            SetCacheString(tenantInfo, tenantName);
            return tenantInfo;
        }

        private void SetCacheString(TenantInfo tenantInfo, string tenantName)
        {
            try
            {
                string tenantInfoStr = JsonConvert.SerializeObject(tenantInfo);
                var tenantInfoExpiry = TimeSpan.FromHours(1);
                var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(tenantInfoExpiry);
                _cache.SetString(tenantName, tenantInfoStr, cacheEntryOptions);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Redis cache failed: " + exc.Message);
            }
        }

        // IMyScopedService is injected into Invoke
        public async Task Invoke(HttpContext context, TenantInfo tenantInfo)
        {
            string hostName = context.Request.Host.Value.ToLower();
            string tenantName = GetTenantName(context, hostName);
            TenantInfo tenant = await GetTenant(tenantName);

            if (tenant == null)
                throw new UnauthorizedAccessException("Invalid tenant");
            else
                tenantInfo = CopyTenantInfo(context, tenant, tenantInfo, tenantName, hostName);

            if (!context.Request.Headers.ContainsKey("TenantName"))
                context.Request.Headers.Add("TenantName", tenantInfo.TenantName);

            if (!context.Request.Headers.ContainsKey("OutsideHost"))
                context.Request.Headers.Add("OutsideHost", tenantInfo.Hostname);

            await _next(context);
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            //We need to read the response stream from the beginning...
            response.Body.Seek(0, SeekOrigin.Begin);

            //...and copy it into a string
            string text = await new StreamReader(response.Body).ReadToEndAsync();

            //We need to reset the reader for the response so that the client can read it.
            response.Body.Seek(0, SeekOrigin.Begin);

            //Return the string for the response, including the status code (e.g. 200, 404, 401, etc.)
            return JsonConvert.SerializeObject(text);
        }

        // IMyScopedService is injected into Invoke
        /*
        public async Task Invoke2(HttpContext context, TenantInfo tenantInfo)
        {
            TenantInfo tenant = new TenantInfo();

            var hostName = context.Request.Host.Value.ToLower();
            string tenantName = string.Empty;
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
                    tenant = await _tableStorage.GetTenantInfo(tenantName);
                else
                    tenant = JsonConvert.DeserializeObject<TenantInfo>(tenantInfoStr);
            }

            if (tenant == null)
                throw new UnauthorizedAccessException("Invalid tenant");
            else
            {
                // Copy all properties
                tenant.TenantName = tenantName;
                CommonUtility.MemberwiseCopy(tenant, tenantInfo);

                if (context.Request.Headers.ContainsKey("OutsideHost"))
                    tenantInfo.Hostname = context.Request.Headers["OutsideHost"].FirstOrDefault();
                else
                    tenantInfo.Hostname = hostName;

                try
                {
                    tenantInfoStr = JsonConvert.SerializeObject(tenantInfo);
                    var tenantInfoExpiry = TimeSpan.FromHours(1);
                    var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(tenantInfoExpiry);
                    _cache.SetString(tenantName, tenantInfoStr, cacheEntryOptions);
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Redis cache failed: " + exc.Message);
                }
                tenantInfo = CopyTenantInfo(context, tenant, tenantInfo, tenantName, hostName);
            }

            if (!context.Request.Headers.ContainsKey("TenantName"))
                context.Request.Headers.Add("TenantName", tenantInfo.TenantName);

            if (!context.Request.Headers.ContainsKey("OutsideHost"))
                context.Request.Headers.Add("OutsideHost", tenantInfo.Hostname);

            await _next(context);
        }
        */
    }
}
