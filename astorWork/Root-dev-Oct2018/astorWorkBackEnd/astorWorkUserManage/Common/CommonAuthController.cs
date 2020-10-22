using astorWorkDAO;
using astorWorkShared.MultiTenancy;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using astorWorkUserManage.Models;
using astorWorkUserManage.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkUserManage.Common
{
    public class CommonAuthController : Controller
    {
        protected IAstorWorkEmail _emailService;
        protected IOptions<Audience> _settings;
        protected astorWorkDbContext _context;
        protected IDistributedCache _cache;
        protected TenantInfo _tenant;
        protected JwtSecurityTokenHandler _jwtTokenHandler;
        protected TokenRepository _repo;

        protected async void SendResetPasswordEmail(string email, string receipientName, string[] content)
        {
            string subject = "Password Resetted";
            string body = string.Format(@"<html>
                                            <body>
                                                <p>
                                                    Your User account has been reset with the following credentials:<br>
                                                    Username:<b>{0}</b><br> 
                                                    Password:<b>{1}</b><br>
                                                </p>                
                                                <p>
                                                    You can login through the following <a href={2}>link</a>.<br>
                                                    Please change your password upon login.
                                                </p>
                                            </body>
                                          </html>",
                                          content);


            await _emailService.SendSingle(email, receipientName, subject, body);
        }

        protected UserMaster UpdateUserMaster(UserMaster userMaster, string pwd)
        {
            userMaster.Salt = new Guid().ToString();
            userMaster.Password = UserUtility.HashPassword(pwd, userMaster.Salt);

            return userMaster;
        }

        protected UserRequestAudit UpdateUserRequestAudit(UserMaster userMaster, UserRequestAudit userRequestAudit)
        {
            userRequestAudit = new UserRequestAudit();
            userRequestAudit.User = userMaster;
            userRequestAudit.RequestType = 0;
            userRequestAudit.RequestTime = DateTime.Now;
            userRequestAudit.RequestSuccess = true;

            return userRequestAudit;
        }

        protected bool RequestNotAllowedWithinInterval(UserRequestAudit userRequestAudit, int interval)
        {
            if (userRequestAudit != null)
            {
                TimeSpan span = DateTimeOffset.Now.Subtract(userRequestAudit.RequestTime);
                if (span.TotalMinutes < interval)
                    return true;
            }

            return false;
        }

        protected string GetPageFullUrl(PageMaster page)
        {
            var result = string.Empty;
            if (page != null)
            {
                var urlBuilder = new StringBuilder(page.UrlPath);
                var module = _context.ModuleMaster.Find(page.ModuleMasterID);
                while (module != null)
                {
                    urlBuilder.Insert(0, "/");
                    urlBuilder.Insert(0, module.UrlPrefix);
                    urlBuilder.Insert(0, "/");
                    module = _context.ModuleMaster.Find(module.ParentModuleID);
                }
                result = urlBuilder.ToString();
            }
            return result;
        }

        protected PageAccessRight[] GetPageAccessRights(RoleMaster role)
        {
            var result = new List<PageAccessRight>();
            if (role != null)
            {
                foreach (var association in role.RolePageAssociations)
                {
                    var page = _context.PageMaster.Find(association.PageId);
                    result.Add(new PageAccessRight
                    {
                        url = GetPageFullUrl(page),
                        right = association.AccessLevel
                    });
                }
            }

            return result.ToArray();
        }

        protected TokenResponse GenerateJwtToken(DateTime now, UserMaster user)
        {
            string refreshToken = Guid.NewGuid().ToString().Replace("-", "");
            var claims = new Claim[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
                    //new Claim(JwtRegisteredClaimNames.NameId, userId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64),
                    new Claim("personName", user.PersonName),
                    new Claim("vendorId", (user.Vendor == null ? 0 : user.Vendor.ID).ToString(), ClaimValueTypes.Integer),
                    new Claim("tenantName", _tenant.TenantName)
            };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Value.Secret));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = _settings.Value.Iss,
                ValidateAudience = true,
                ValidAudience = _settings.Value.Aud,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,

            };
            var jwt = new JwtSecurityToken(
                issuer: _settings.Value.Iss,
                audience: _settings.Value.Aud,
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromHours(1)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            var roleAccess = new
            {
                roleId = user.RoleID,
                defaultPage = GetPageFullUrl(user.Role.DefaultPage),
                mobileEntryPoint = user.Role.MobileEntryPoint,
                expiryTime = now.Add(TimeSpan.FromHours(1)),
                pageAccessRights = GetPageAccessRights(user.Role),
                userId = user.ID
            };

            jwt.Payload["role"] = roleAccess;
            var encodedJwt = _jwtTokenHandler.WriteToken(jwt);

            return new TokenResponse()
            {
                tokenType = "Bearer",
                expiresIn = (int) TimeSpan.FromHours(1).TotalSeconds,
                accessToken = encodedJwt,
                refreshToken = refreshToken,
                userId = user.ID
            };
        }
    }
}
