using Microsoft.AspNetCore.Builder;

namespace astorWorkShared.Middlewares
{
    public static class RequestTenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestTenant(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestTenantMiddleware>();
        }
    }
}
