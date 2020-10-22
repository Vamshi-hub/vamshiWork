using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using astorWorkDAO;
using astorWorkShared.GlobalResponse;
using System.Security.Cryptography;
using System.Text;
using astorWorkUserManage.Models;
using astorWorkShared.Services;
using astorWorkShared.Utilities;
using Microsoft.Extensions.Caching.Distributed;
using astorWorkShared.MultiTenancy;
using EFCore.BulkExtensions;

namespace astorWorkUserManage.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    public class UserMastersController : CommonUserMastersController
    {
        private readonly string Module = "Material";
        private readonly string Field = "ID";

        private IDistributedCache _redisCache;
        private TenantInfo _tenant;

        public UserMastersController(astorWorkDbContext context, IAstorWorkEmail emailService, IDistributedCache redisCache, TenantInfo tenantInfo)
        {
            _context = context;
            _emailService = emailService;
            _redisCache = redisCache;
            _tenant = tenantInfo;
        }

        // GET: user
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isVendor">
        /// 0:returns non vendor users
        /// 1:returns vendor users
        /// null: all users
        /// </param>
        /// <returns></returns>
        [HttpGet]
        [Route("users")]
        public APIResponse ListUsers([FromQuery] int? isVendor)
        {
            return new APIResponse(0, CreateUserProfileList(isVendor));
        }

        // GET: user/5
        [HttpGet("{id}")]
        public async Task<APIResponse> ViewUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            UserMaster userMaster = await _context.UserMaster.Include(u => u.Role).Include(u => u.Vendor).Include(s => s.Site).Include(p => p.Project).SingleOrDefaultAsync(m => m.ID == id);

            if (userMaster == null)
                return new DbRecordNotFound("User", "Id", id.ToString());

            UserProfile userProfile = CreateUserProfile(userMaster);

            return new APIResponse(0, userProfile);
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task<APIResponse> UpdateUser([FromRoute] int id, [FromBody] UserProfile userProfile)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            var userMaster = await _context.UserMaster.Include(u => u.Vendor).Include(u => u.Site).Include(u => u.Project).Where(u => u.ID == id).FirstOrDefaultAsync();

            if (UserMasterExists("", userProfile.Email) && userMaster.Email != userProfile.Email)
                return new DbDuplicateRecord("User", "Email ", userProfile.Email);

            if (userMaster != null)
            {
                userMaster.PersonName = userProfile.PersonName;
                userMaster.Email = userProfile.Email;
                userMaster.Role = _context.RoleMaster.Where(r => r.ID == userProfile.RoleID).FirstOrDefault();
                userMaster.Site = _context.SiteMaster.Include(s => s.Vendor).Where(s => s.ID == userProfile.SiteID).FirstOrDefault();
                userMaster.Project = _context.ProjectMaster.Where(p => p.ID == userProfile.ProjectID).FirstOrDefault();
                if (userMaster.Site == null || userMaster.Site.Vendor == null)
                    userMaster.Vendor = null;
                else
                    userMaster.Vendor = _context.VendorMaster.Where(v => v.ID == userMaster.Site.Vendor.ID).FirstOrDefault();
                //if (userProfile.RoleID == 7)
                //    userMaster.Vendor = _context.VendorMaster.Where(v => v.ID == userProfile.VendorID).FirstOrDefault();
                userMaster.IsActive = userProfile.IsActive;
                
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException exc)
                {
                    if (!UserMasterExists(userProfile.UserName, userProfile.Email))
                        return new DbConcurrentUpdate(exc.Message);
                    else
                        throw;
                }
            }
            return new APIResponse(0, null);
        }

        // POST: api/user
        [HttpPost]
        [Route("create")]
        public async Task<APIResponse> CreateUser([FromBody] UserProfile userProfile)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();
            
            string link = @"http://" + _tenant.Hostname;
            
            if (UserMasterExists(userProfile.UserName, ""))
                return new DbDuplicateRecord("User", "Username ", userProfile.UserName);

            if (UserMasterExists("", userProfile.Email))
                return new DbDuplicateRecord("User", "Email ", userProfile.Email);

            List<string> emailContent = await CreateUserInDb(userProfile);
            emailContent.Add(link);
        
            SendEmail(userProfile.Email, userProfile.PersonName, emailContent.ToArray());

            return new APIResponse(0, null);
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userMaster = await _context.UserMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (userMaster == null)
            {
                return NotFound();
            }

            _context.UserMaster.Remove(userMaster);
            await _context.SaveChangesAsync();

            return Ok(userMaster);
        }

        // PUT: api/user/5/changePassword
        [HttpPut("{id}/change-password")]
        public async Task<APIResponse> ChangePassword([FromRoute] int id, [FromBody] ChangePasswordRequest changePassword)
        {
            if (!ModelState.IsValid)
                return new APIBadRequest();

            var userMaster = await _context.UserMaster.Include(u => u.UserSessionAudits).SingleOrDefaultAsync(m => m.ID == id);
            if (userMaster == null)
                return new DbRecordNotFound(Module, Field, id.ToString());

            var pwd = UserUtility.HashPassword(changePassword.CurrentPassword, userMaster.Salt);
            if (pwd == userMaster.Password)
            {
                userMaster.Salt = new Guid().ToString();
                userMaster.Password = UserUtility.HashPassword(changePassword.NewPassword, userMaster.Salt);

                var sessions = userMaster.UserSessionAudits.Where(us => us.ExpireIn > 0 && 
                (DateTimeOffset.Now - us.CreatedTime) <= TimeSpan.FromHours(3)).ToList();
                foreach (var session in sessions)
                {
                    await _redisCache.RemoveAsync(session.AccessToken);
                    await _redisCache.RemoveAsync(session.RefreshToken);
                    session.ExpireIn = -1;
                }

                _context.BulkUpdate(sessions);
                _context.SaveChanges();
                return new APIResponse { Status = 0 };
            }
            else
            {
                return new APIResponse
                {
                    Status = ErrorMessages.PasswordNotMatch,
                    Message = "Wrong password"
                };
            }
        }
    }
}