using astorWorkDAO;
using System;

namespace astorWorkUserManage.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using astorWorkDAO;
    using astorWorkShared.GlobalResponse;
    using astorWorkShared.MultiTenancy;
    using astorWorkShared.Services;
    using astorWorkShared.Utilities;
    using astorWorkUserManage.Common;
    using astorWorkUserManage.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using Models;


    [Produces("application/json")]
    [Route("api/authentication")]
    public class AuthController : CommonAuthController
    {

        public AuthController(IOptions<Audience> settings, astorWorkDbContext context,
            IDistributedCache cache, TenantInfo tenantInfo, IAstorWorkEmail emailService)
        {
            _cache = cache;
            _context = context;
            _tenant = tenantInfo;

            _settings = settings;
            _emailService = emailService;

            _jwtTokenHandler = new JwtSecurityTokenHandler();
            _repo = new TokenRepository(cache, tenantInfo);
        }


        [HttpPost]
        [Route("login")]
        public APIResponse login([FromBody] User user)
        {
            UserMaster userMaster = _context.UserMaster
                .Include(u => u.Vendor)
                .Include(u => u.Role).ThenInclude(r => r.DefaultPage)
                .Include(u => u.Role).ThenInclude(r => r.RolePageAssociations)
                .Where(u => u.IsActive)
                .FirstOrDefault(p => p.UserName == user.UserName);

            if (userMaster == null)
                return new APIResponse(ErrorMessages.DbRecordNotFound, null, "User not found");
            else
            {
                var password = UserUtility.HashPassword(user.Password, userMaster.Salt);
                if (userMaster.Password.Equals(password))
                {
                    var now = DateTime.UtcNow;

                    var tokenResponse = GenerateJwtToken(now, userMaster);
                    if (_repo.AddToken(tokenResponse))
                    {
                        var userSessionAudit = new UserSessionAudit()
                        {
                            AccessToken = tokenResponse.accessToken,
                            RefreshToken = tokenResponse.refreshToken,
                            ExpireIn = tokenResponse.expiresIn,
                            CreatedTime = now
                        };
                        _context.UserSessionAudit.Add(userSessionAudit);
                        _context.SaveChanges();

                        return new APIResponse(0, tokenResponse);
                    }
                    else
                        return new APIResponse(ErrorMessages.ExternalServiceFail, null, "Fail to add session");
                }
                else
                    return new APIResponse(ErrorMessages.PasswordNotMatch, null, "Password is wrong");
            }
        }

        [HttpPost]
        [Route("refresh")]
        public APIResponse refresh([FromBody] RefreshTokenPayload token)
        {
            if (token == null || _repo.GetToken(token.RefreshToken) == null)
                return new APIResponse(ErrorMessages.DbRecordNotFound, null, "Session not exist");
            else
            {
                _repo.RemoveToken(token.RefreshToken);
                UserMaster userMaster = _context.UserMaster
                    .Include(u => u.Vendor)
                    .Include(u => u.Role).ThenInclude(r => r.RolePageAssociations)
                    .Where(u => u.IsActive)
                    .FirstOrDefault(p => p.ID == token.UserId);

                var now = DateTime.UtcNow;

                var tokenResponse = GenerateJwtToken(now, userMaster);
                if (_repo.AddToken(tokenResponse))
                {
                    var userSessionAudit = new UserSessionAudit()
                    {
                        AccessToken = tokenResponse.accessToken,
                        RefreshToken = tokenResponse.refreshToken,
                        ExpireIn = tokenResponse.expiresIn,
                        CreatedTime = now
                    };
                    _context.UserSessionAudit.Add(userSessionAudit);
                    _context.SaveChanges();

                    return new APIResponse(0, tokenResponse);
                }
                else
                    return new APIResponse(ErrorMessages.ExternalServiceFail, null, "Fail to add session");
            }
        }

        // PUT: user/5/forgetPassword
        [HttpPost]
        [Route("forgetPassword")]
        public async Task<APIResponse> ForgetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            UserMaster userMaster = _context.UserMaster.Where(u => u.UserName == resetPasswordRequest.UserName
                                && u.Email == resetPasswordRequest.Email).ToList().FirstOrDefault();

            UserRequestAudit userRequestAudit = _context.UserRequestAudit.Where(ura => ura.User.ID == userMaster.ID
                                                && ura.RequestType == 0)
                                               .OrderByDescending(ura => ura.RequestTime)
                                               .FirstOrDefault();

            if (RequestNotAllowedWithinInterval(userRequestAudit, 15))
                return new RequestNotAllowedWithinInterval("15 mins");
            else if (userMaster == null)
                return new DbRecordNotFound("No active user with the matching username and email exist!");
            else if (!userMaster.IsActive)
                return new DbRecordNotFound("User is not active!");

            string pwd = UserUtility.GenerateRandomPassword(8);
            userMaster = UpdateUserMaster(userMaster, pwd);
            userRequestAudit = UpdateUserRequestAudit(userMaster, userRequestAudit);

            try
            {
                var senderUrl = @"http://" + _tenant.Hostname;
                List<string> content = new List<string> { userMaster.UserName, pwd, senderUrl };
                SendResetPasswordEmail(userMaster.Email, userMaster.UserName, content.ToArray());
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                userRequestAudit.RequestSuccess = false;
                _context.UserRequestAudit.Add(userRequestAudit);
                return new ExternalServiceFail("Email server");
            }

            _context.Entry(userMaster).State = EntityState.Modified;
            _context.UserRequestAudit.Add(userRequestAudit);
            await _context.SaveChangesAsync();

            return new APIResponse(0, null);
        }
    }
}
