using astorWorkDAO;
using astorWorkShared.GlobalModels;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using astorWorkUserManage.Models;
using astorWorkUserManage.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

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

        protected UserMaster GetUser(string userName)
        {
            return _context.UserMaster
                           .Include(u => u.Organisation)
                           .Include(u => u.Role).ThenInclude(r => r.DefaultPage)
                           .Include(u => u.Role).ThenInclude(r => r.RolePageAssociations)
                           .Where(u => u.IsActive)
                           .FirstOrDefault(p => p.UserName == userName);
        }

        protected UserSessionAudit CreateUserSessionAudit(TokenResponse tokenResponse, DateTime now)
        {
            return new UserSessionAudit()
            {
                AccessToken = tokenResponse.accessToken,
                RefreshToken = tokenResponse.refreshToken,
                ExpireIn = tokenResponse.expiresIn,
                CreatedTime = now
            };
        }

        protected UserMaster GetUser(RefreshTokenPayload token)
        {
            return _context.UserMaster
                     .Include(u => u.Organisation)
                     .Include(u => u.Role).ThenInclude(r => r.RolePageAssociations)
                     .Where(u => u.IsActive)
                     .FirstOrDefault(p => p.ID == token.UserId);
        }

        protected UserMaster GetUser(ResetPasswordRequest resetPasswordRequest)
        {
            return _context.UserMaster.Where(u => u.UserName == resetPasswordRequest.UserName
                                               && u.Email == resetPasswordRequest.Email)
                                      .ToList()
                                      .FirstOrDefault();
        }

        protected UserRequestAudit GetUserRequestAudit(UserMaster userMaster)
        {
            return _context.UserRequestAudit.Where(ura => ura.User.ID == userMaster.ID
                                                       && ura.RequestType == 0)
                                            .OrderByDescending(ura => ura.RequestTime)
                                            .FirstOrDefault();
        }

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
            List<PageAccessRight> result = new List<PageAccessRight>();
            if (role != null)
            {
                foreach (RolePageAssociation association in role.RolePageAssociations)
                {
                    PageMaster page = _context.PageMaster.Find(association.PageId);
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
            Claim[] claims = new Claim[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.ID.ToString()),
                    //new Claim(JwtRegisteredClaimNames.NameId, userId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64),
                    new Claim("personName", user.PersonName),
                    new Claim("organisationID", (user.Organisation == null ? 0 : user.Organisation.ID).ToString(), ClaimValueTypes.Integer),
                    new Claim("tenantName", _tenant.TenantName)
            };

            SymmetricSecurityKey signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Value.Secret));
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
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
            JwtSecurityToken jwt = new JwtSecurityToken(
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
                userId = user.ID,
                organisationId = user.Organisation?.ID
            };

            jwt.Payload["role"] = roleAccess;
            string encodedJwt = _jwtTokenHandler.WriteToken(jwt);
            var project = _context.ProjectMaster.Select(p => p.ID).ToList();
            int project_id = 0;
            if (project!= null && project.Count == 1)
                project_id = project.FirstOrDefault();
            return new TokenResponse()
            {
                tokenType = "Bearer",
                expiresIn = (int)TimeSpan.FromHours(1).TotalSeconds,
                accessToken = encodedJwt,
                refreshToken = refreshToken,
                userId = user.ID,
                projectID = user.ProjectID == null && project_id > 0 ? project_id : user.ProjectID,
                organisationType = user.Organisation == null ? (int)Enums.OrganisationType.MainCon : (int)user.Organisation.OrganisationType
            };
        }
    }
}
