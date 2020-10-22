using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkShared.MultiTenancy;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace astorWorkUserManage.Controllers
{
    [Produces("application/json")]
    [Route("api/powerbi")]
    [ApiController]
    public class PowerBIController : ControllerBase
    {
        private readonly string Module = "POWER_BI";

        IAstorWorkPowerBI _powerBI;
        TenantInfo _tenantInfo;
        astorWorkDbContext _context;

        public PowerBIController(IAstorWorkPowerBI powerBI, TenantInfo tenantInfo, astorWorkDbContext context)
        {
            _powerBI = powerBI;
            _tenantInfo = tenantInfo;
            _context = context;
        }

        [HttpGet]
        [Route("authenticate")]
        public async Task<APIResponse> GetPowerBIToken()
        {
            try
            {
                var authResult = await _powerBI.AuthenticatePowerBIAsync(AppConfiguration.GetPowerBICredentials());

                if (authResult == null)
                    return new APIBadRequest();
                else
                    return new APIResponse
                    {
                        Status = 0,
                        Data = authResult
                    };
            }
            catch(Exception exc)
            {
                return new APIResponse
                {
                    Status = 400,
                    Message = exc.Message
                };
            }
        }

        [HttpGet]
        [Route("reports")]
        public async Task<APIResponse> GetPowerBIReports([FromQuery] string powerbi_token)
        {
            try
            {
                if (string.IsNullOrEmpty(powerbi_token))
                    return new APIBadRequest();
                else
                {
                    var user = _context.GetUserFromHttpContext(HttpContext);
                    if (user == null)
                        return new DbRecordNotFound(Module, "Reports", "user");

                    var reports = await _powerBI.GetReportsAsync(powerbi_token, _tenantInfo.PowerBIWorkSpaceGUID);

                    var roleReportIds = _context.RolePowerBIReport.Where(rp => rp.RoleId == user.RoleID).Select(rp => rp.PowerBIReportId).ToList();

                    return new APIResponse
                    {
                        Status = 0,
                        Data = reports.Where(r => roleReportIds.Contains(r.Id))
                    };
                }
            }
            catch (Exception exc)
            {
                return new APIResponse
                {
                    Status = 400,
                    Message = exc.Message
                };
            }
        }

        [HttpGet]
        [Route("embed-token")]
        public async Task<APIResponse> GetPowerBIEmbedToken([FromQuery] string powerbi_token, [FromQuery] string report_guid)
        {
            try
            {
                if (string.IsNullOrEmpty(powerbi_token))
                    return new APIBadRequest();
                else
                {
                    var embedToken = await _powerBI.GetEmbedTokenAsync(powerbi_token, _tenantInfo.PowerBIWorkSpaceGUID, report_guid);

                    return new APIResponse
                    {
                        Status = 0,
                        Data = embedToken
                    };
                }
            }
            catch (Exception exc)
            {
                return new APIResponse
                {
                    Status = 400,
                    Message = exc.Message
                };
            }
        }
    }
}