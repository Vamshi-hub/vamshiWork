using astorWorkShared.MultiTenancy;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public interface IAstorWorkTenantHandler
    {
        Task<TenantInfo> GetTenantAsync(HttpContext context);
        Task<TenantInfo> AddTenantAsync(TenantInfo tenantInfo);
    }
}
