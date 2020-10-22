using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using astorWorkShared.GlobalModels;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using astorWorkShared.GlobalExceptions;
using Microsoft.PowerBI.Api.V2.Models;

namespace astorWorkUserManage.Controllers
{
    [Produces("application/json")]
    [Route("api/powerbi")]
    [ApiController]
    public class PowerBIController : ControllerBase
    {
        private readonly string module = "POWER_BI";

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
        public async Task<PowerBIAuthResult> GetPowerBIToken()
        {
            try
            {
                PowerBIAuthResult authResult = await _powerBI.AuthenticatePowerBIAsync(AppConfiguration.GetPowerBICredentials());

                if (authResult == null)
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
                else
                    return authResult;
            }
            catch(Exception ex)
            {
                throw new GenericException(ErrorMessages.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("reports")]
        public async Task<IEnumerable<Report>> GetPowerBIReports([FromQuery] string powerbi_token)
        {
            try
            {
                if (string.IsNullOrEmpty(powerbi_token))
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
                else
                {
                    UserMaster user = await _context.GetUserFromHttpContext(HttpContext);
                    if (user == null)
                        throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found!");

                    IEnumerable<Report> reports = await _powerBI.GetReportsAsync(powerbi_token, _tenantInfo.PowerBIWorkSpaceGUID);
                    List<string> roleReportIDs = _context.RolePowerBIReport.Where(rp => rp.RoleId == user.RoleID).Select(rp => rp.PowerBIReportId).ToList();

                    return reports.Where(r => roleReportIDs.Contains(r.Id));
                }
            }
            catch (Exception ex)
            {
                throw new GenericException(ErrorMessages.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("embed-token")]
        public async Task<EmbedToken> GetPowerBIEmbedToken([FromQuery] string powerbi_token, [FromQuery] string report_guid)
        {
            try
            {
                if (string.IsNullOrEmpty(powerbi_token))
                    throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());
                else
                    return await _powerBI.GetEmbedTokenAsync(powerbi_token, _tenantInfo.PowerBIWorkSpaceGUID, report_guid);
            }
            catch (Exception ex)
            {
                throw new GenericException(ErrorMessages.BadRequest, ex.Message);
            }
        }
    }
}