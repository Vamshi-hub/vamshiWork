using astorWorkShared.GlobalModels;
using astorWorkShared.GlobalResponse;
using Microsoft.AspNetCore.Mvc;


namespace astorWorkUserManage.Controllers
{
    [Produces("application/json")]
    [Route("api/tenant")]
    public class TenantController : Controller
    {
        private TenantInfo _tenant;

        public TenantController(TenantInfo tenantInfo)
        {
            _tenant = tenantInfo;
        }
        // GET: api/Tenant
        [HttpGet("settings")]
        public TenantInfo GetSettings()
        {
            return new TenantInfo
            {
                BackgroundImageURL = _tenant.BackgroundImageURL,
                LogoImageURL = _tenant.LogoImageURL,
                EnabledModules = _tenant.EnabledModules
            };
        }
    }
}
