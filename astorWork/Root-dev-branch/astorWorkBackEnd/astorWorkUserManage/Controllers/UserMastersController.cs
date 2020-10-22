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
using astorWorkShared.GlobalModels;
using EFCore.BulkExtensions;
using astorWorkShared.GlobalExceptions;

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

        [HttpGet("rtos")]
        public async Task<List<Profile>> ListRTOs()
        {
            List<Profile> result = new List<Profile>();
            try
            {
                RoleMaster role = await _context.RoleMaster.Where(r => r.Name == "RTO").FirstOrDefaultAsync();
                result = await _context.UserMaster.Where(u => u.Role == role).Select(u => new Profile
                {
                    UserID = u.ID,
                    PersonName = u.PersonName,
                    UserName = u.UserName,
                    Email = u.Email
                }).ToListAsync();

                if (result == null || result.Count == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "$ There is No RTO user Available");
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return result;
        }

        private async Task<List<Profile>> GetProfiles(string roleName) {
            RoleMaster role = await _context.RoleMaster.Where(r => r.Name == roleName).FirstOrDefaultAsync();
            return await _context.UserMaster.Where(u => u.Role == role).Select(u => new Profile
            {
                UserID = u.ID,
                PersonName = u.PersonName,
                UserName = u.UserName,
                Email = u.Email
            }).ToListAsync();
        }

        [HttpGet("subcons")]
        public async Task<List<Profile>> ListSubcons()
        {
            List<Profile> result = new List<Profile>();
            try
            {
                RoleMaster role = await _context.RoleMaster.Where(r => r.Name.ToLower().Contains("subcon") || r.Name.ToLower().Contains("vendor")).FirstOrDefaultAsync();
                result = await _context.UserMaster.Where(u => u.Role == role).Select(u => new Profile
                {
                    UserID = u.ID,
                    PersonName = u.PersonName,
                    UserName = u.UserName,
                    Email = u.Email
                }).ToListAsync();
                if (result == null || result.Count == 0)
                    throw new GenericException(ErrorMessages.DbRecordNotFound, "$ There is No Subcon or Vendor Available");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


        // GET: user/5
        [HttpGet("{id}")]
        public async Task<UserProfile> ViewUser([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            UserMaster userMaster = await _context.UserMaster.Include(u => u.Role).Include(u => u.Organisation).Include(s => s.Site).Include(p => p.Project).SingleOrDefaultAsync(m => m.ID == id);

            if (userMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound,"User not found!");

            UserProfile userProfile = CreateUserProfile(userMaster);

            return userProfile;
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        public async Task UpdateUser([FromRoute] int id, [FromBody] UserProfile userProfile)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            UserMaster userMaster = await _context.UserMaster.Include(u => u.Organisation).Include(u => u.Site).Include(u => u.Project).Where(u => u.ID == id).FirstOrDefaultAsync();

            if (UserMasterExists("", userProfile.Email) && userMaster.Email != userProfile.Email)
                throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg("User", "Email ", userProfile.Email));

            if (userMaster != null)
            {
                userMaster.PersonName = userProfile.PersonName;
                userMaster.Email = userProfile.Email;
                userMaster.Role = _context.RoleMaster.Where(r => r.ID == userProfile.RoleID).FirstOrDefault();
                userMaster.Site = _context.SiteMaster.Include(s => s.Organisation).Where(s => s.ID == userProfile.SiteID).FirstOrDefault();
                userMaster.Project = _context.ProjectMaster.Where(p => p.ID == userProfile.ProjectID).FirstOrDefault();
                if (userProfile.OrganisationID > 0)
                    userMaster.Organisation = _context.OrganisationMaster.Where(v => v.ID == userProfile.OrganisationID).FirstOrDefault();
                else if (userMaster.Site == null || userMaster.Site.Organisation == null)
                    userMaster.Organisation = null;
                else
                    userMaster.Organisation = _context.OrganisationMaster.Where(v => v.ID == userMaster.Site.Organisation.ID).FirstOrDefault();
                //if (userProfile.RoleID == 7)
                //    userMaster.Vendor = _context.VendorMaster.Where(v => v.ID == userProfile.VendorID).FirstOrDefault();
                userMaster.IsActive = userProfile.IsActive;
                
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!UserMasterExists(userProfile.UserName, userProfile.Email))
                        throw new GenericException(ErrorMessages.DbConcurrentUpdate, ex.Message);
                    else
                        throw;
                }
            }
        }

        // POST: api/user
        [HttpPost]
        [Route("create")]
        public async Task CreateUser([FromBody] UserProfile userProfile)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            string link = @"http://" + _tenant.Hostname;
            
            if (UserMasterExists(userProfile.UserName, ""))
                throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg("User", "Username ", userProfile.UserName));

            if (UserMasterExists("", userProfile.Email))
                throw new GenericException(ErrorMessages.DbDuplicateRecord, ErrorMessages.DbDuplicateRecordMsg("User", "Email ", userProfile.Email));

            List<string> emailContent = await CreateUserInDb(userProfile);
            emailContent.Add(link);
        
            SendEmail(userProfile.Email, userProfile.PersonName, emailContent.ToArray());
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public async Task<UserMaster> DeleteUserMaster([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            UserMaster userMaster = await _context.UserMaster.SingleOrDefaultAsync(m => m.ID == id);
            if (userMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found!");

            _context.UserMaster.Remove(userMaster);
            await _context.SaveChangesAsync();

            return userMaster;
        }

        // PUT: api/user/5/changePassword
        [HttpPut("{id}/change-password")]
        public async Task ChangePassword([FromRoute] int id, [FromBody] ChangePasswordRequest changePassword)
        {
            if (!ModelState.IsValid)
                throw new GenericException(ErrorMessages.BadRequest, ErrorMessages.BadRequestMsg());

            UserMaster userMaster = await _context.UserMaster.Include(u => u.UserSessionAudits).SingleOrDefaultAsync(m => m.ID == id);
            if (userMaster == null)
                throw new GenericException(ErrorMessages.DbRecordNotFound, "User not found!");

            string pwd = UserUtility.HashPassword(changePassword.CurrentPassword, userMaster.Salt);
            if (pwd == userMaster.Password)
            {
                userMaster.Salt = new Guid().ToString();
                userMaster.Password = UserUtility.HashPassword(changePassword.NewPassword, userMaster.Salt);

                List<UserSessionAudit> sessions = userMaster.UserSessionAudits.Where(us => us.ExpireIn > 0 && 
                (DateTimeOffset.Now - us.CreatedTime) <= TimeSpan.FromHours(3)).ToList();
                foreach (var session in sessions)
                {
                    await _redisCache.RemoveAsync(session.AccessToken);
                    await _redisCache.RemoveAsync(session.RefreshToken);
                    session.ExpireIn = -1;
                }

                _context.BulkUpdate(sessions);
                _context.SaveChanges();
            }
            else
            {
                throw new GenericException(ErrorMessages.PasswordNotMatch, "Wrong password!");
            }
        }
    }
}