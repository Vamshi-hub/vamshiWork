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
    using astorWorkShared.GlobalModels;
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
    using astorWorkShared.GlobalExceptions;

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
        public TokenResponse login([FromBody] User user)
        {
            UserMaster userMaster = GetUser(user.UserName);

            if (userMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found!");
            else
            {
                string password = UserUtility.HashPassword(user.Password, userMaster.Salt);
                if (userMaster.Password.Equals(password))
                {
                    DateTime now = DateTime.UtcNow;
                    TokenResponse tokenResponse = GenerateJwtToken(now, userMaster);

                    if (_repo.AddToken(tokenResponse))
                    {
                        UserSessionAudit userSessionAudit = CreateUserSessionAudit(tokenResponse, now);
                        _context.UserSessionAudit.Add(userSessionAudit);
                        //_context.SaveChanges();

                        return tokenResponse;
                    }
                    else
                        throw new GenericException(ErrorMessages.ExternalServiceFail, "Failed to add session!");
                }
                else
                    throw new GenericException(ErrorMessages.PasswordNotMatch, "Password is wrong!");
            }
        }

        [HttpPost]
        [Route("refresh")]
        public TokenResponse refresh([FromBody] RefreshTokenPayload token)
        {
            if (token == null || _repo.GetToken(token.RefreshToken) == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "Session does not exist!");
            else
            {
                _repo.RemoveToken(token.RefreshToken);
                UserMaster userMaster = GetUser(token); 
                DateTime now = DateTime.UtcNow;
                TokenResponse tokenResponse = GenerateJwtToken(now, userMaster);

                if (_repo.AddToken(tokenResponse))
                {
                    UserSessionAudit userSessionAudit = CreateUserSessionAudit(tokenResponse, now);
                    _context.UserSessionAudit.Add(userSessionAudit);
                    _context.SaveChanges();

                    return tokenResponse;
                }
                else
                    throw new GenericException(ErrorMessages.ExternalServiceFail, "Failed to add session!");
            }
        }

        // PUT: user/5/forgetPassword
        [HttpPost]
        [Route("forgetPassword")]
        public async Task ForgetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            UserMaster userMaster = GetUser(resetPasswordRequest);
            UserRequestAudit userRequestAudit = GetUserRequestAudit(userMaster);

            if (RequestNotAllowedWithinInterval(userRequestAudit, 15))
                throw new GenericException(ErrorMessages.RequestNotAllowedWithinInterval, "Request not allowed within interval of 5 mins.");
            else if (userMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "No active user with the matching username and email exist!");
            else if (!userMaster.IsActive)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "User is not active.");

            string pwd = UserUtility.GenerateRandomPassword(8);
            userMaster = UpdateUserMaster(userMaster, pwd);
            userRequestAudit = UpdateUserRequestAudit(userMaster, userRequestAudit);

            try
            {
                string senderUrl = @"http://" + _tenant.Hostname;
                List<string> content = new List<string> { userMaster.UserName, pwd, senderUrl };
                SendResetPasswordEmail(userMaster.Email, userMaster.UserName, content.ToArray());
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                userRequestAudit.RequestSuccess = false;
                _context.UserRequestAudit.Add(userRequestAudit);
                throw new GenericException(ErrorMessages.ExternalServiceFail, "Email server error.");
            }

            _context.Entry(userMaster).State = EntityState.Modified;
            _context.UserRequestAudit.Add(userRequestAudit);
            await _context.SaveChangesAsync();
        }
    }
}
