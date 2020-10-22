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

namespace astorWorkUserManage.Controllers
{
    [Produces("application/json")]
    [Route("user")]
    public class CommonUserMastersController : Controller
    {
        protected astorWorkDbContext _context;
        protected IAstorWorkEmail _emailService;

        protected string GeneratePassword(int length)
        {
            //create constant strings for each type of characters
            string alphaCaps = "QWERTYUIOPASDFGHJKLZXCVBNM";
            string alphaLow = "qwertyuiopasdfghjklzxcvbnm";
            string numerics = "1234567890";
            string special = "@#$";

            //create another string which is a concatenation of all above
            string allChars = alphaCaps + alphaLow + numerics + special;

            Random r = new Random();
            String generatedPassword = "";
            for (int i = 0; i < length; i++)
            {
                double rand = r.NextDouble();
                if (i == 0)
                    //First character is an upper case alphabet
                    generatedPassword += alphaCaps.ToCharArray()[(int)Math.Floor(rand * alphaCaps.Length)];
                else
                    generatedPassword += allChars.ToCharArray()[(int)Math.Floor(rand * allChars.Length)];
            }

            return generatedPassword;
        }

        protected UserMaster CreateSaltAndPasswordHash(string pwd, UserMaster userMaster = null)
        {
            string pwdHashed = string.Empty;
            Guid salt = Guid.NewGuid();

            using (var algorithm = SHA256.Create())
            {
                var pwdSalted = Encoding.UTF8.GetBytes(pwd).Concat(salt.ToByteArray());
                var hash = algorithm.ComputeHash(pwdSalted.ToArray());
                pwdHashed = Convert.ToBase64String(hash);
            }

            if (userMaster == null)
                userMaster = new UserMaster();

            userMaster.Salt = salt.ToString();
            userMaster.Password = pwdHashed;

            return userMaster;
        }

        protected bool UserMasterExists(string userName, string email)
        {
            return _context.UserMaster.Any(e => e.UserName == userName || e.Email == email);
        }

        protected List<UserProfile> CreateUserProfileList(int? isVendor) {
            IEnumerable<UserMaster> users;
            if (isVendor == 0)
                users = _context.UserMaster.Include(u => u.Role).Include(u => u.Organisation).Where(U => U.Organisation == null);
            else if (isVendor == 1)
                users = _context.UserMaster.Include(U => U.Role).Include(U => U.Organisation).Where(U => U.Organisation != null);
            else
                users = _context.UserMaster.Include(U => U.Role).Include(U => U.Site).Include(U => U.Project).Include(U => U.Organisation);

            List<UserProfile> userProfileList = new List<UserProfile>();

            foreach (UserMaster userMaster in users)
                userProfileList.Add(CreateUserProfile(userMaster));

            return userProfileList;
        }

        protected UserProfile CreateUserProfile(UserMaster userMaster) {
            UserProfile userProfile = new UserProfile();

            userProfile.UserID = userMaster.ID;
            userProfile.UserName = userMaster.UserName;
            userProfile.PersonName = userMaster.PersonName;
            userProfile.Email = userMaster.Email;
            if (userMaster.Role != null)
            {
                userProfile.Role = userMaster.Role.Name;
                userProfile.RoleID = userMaster.Role.ID;
            }
            if (userMaster.Site != null)
            {
                userProfile.Site = userMaster.Site.Name;
                userProfile.SiteID = userMaster.Site.ID;
            }
            if(userMaster.Project != null)
            {
                userProfile.ProjectID = userMaster.Project.ID;
                userProfile.ProjectName = userMaster.Project.Name;
            }
            if (userMaster.Organisation != null)
            {
                userProfile.OrganisationID = userMaster.Organisation.ID;
                userProfile.OrganisationName = userMaster.Organisation.Name;
            }
            userProfile.LastLogin = userMaster.LastLogin;
            userProfile.IsActive = userMaster.IsActive;

            return userProfile;
        }

        protected async Task<List<string>> CreateUserInDb(UserProfile userProfile) {
            string pwd = GeneratePassword(8);
            UserMaster userMaster = CreateSaltAndPasswordHash(pwd);

            userMaster.UserName = userProfile.UserName;
            userMaster.PersonName = userProfile.PersonName;
            userMaster.Email = userProfile.Email;
            userMaster.Role = _context.RoleMaster.Where(r => r.ID == userProfile.RoleID).FirstOrDefault();
            if (userProfile.SiteID == 0)
                userMaster.Site = null;
            else
                userMaster.Site = _context.SiteMaster.Include(s => s.Organisation).Where(s => s.ID == userProfile.SiteID).FirstOrDefault();
            if (userProfile.OrganisationID > 0)
                userMaster.Organisation = _context.OrganisationMaster.Where(v => v.ID == userProfile.OrganisationID).FirstOrDefault();
            else if (userMaster.Site == null || userMaster.Site.Organisation == null)
                userMaster.Organisation = null;
            else
                userMaster.Organisation = _context.OrganisationMaster.Where(v => v.ID == userMaster.Site.Organisation.ID).FirstOrDefault();
            //if (userProfile.RoleID == 7)
            //    userMaster.Vendor = _context.VendorMaster.Where(v => v.ID == userProfile.VendorID).FirstOrDefault();
            userMaster.Project = _context.ProjectMaster.Where(p => p.ID == userProfile.ProjectID).FirstOrDefault();
            userMaster.IsActive = true;

            _context.UserMaster.Add(userMaster);
            await _context.SaveChangesAsync();

            return new List<string>{ userProfile.UserName, pwd };
        }

        protected UserMaster UpdateUser(UserMaster userMaster, UserProfile userProfile) {
            userMaster.PersonName = userProfile.PersonName;
            userMaster.Email = userProfile.Email;
            userMaster.Role = _context.RoleMaster.Where(r => r.ID == userProfile.RoleID).FirstOrDefault();
            userMaster.Site = _context.SiteMaster.Include(s => s.Organisation).Where(s => s.ID == userProfile.SiteID).FirstOrDefault();
            if (userMaster.Site == null || userMaster.Site.Organisation == null)
                userMaster.Organisation = null;
            else
                userMaster.Organisation = _context.OrganisationMaster.Where(v => v.ID == userMaster.Site.Organisation.ID).FirstOrDefault();
            userMaster.IsActive = userProfile.IsActive;

            return userMaster;
        }

        protected async void SendEmail(string email, string receipientName, string[] content)
        {
            string subject = "New Account Created";
            string body = string.Format(@"<html>
                                            <body>
                                                <p>
                                                    Your User account has been created with the following credentials:<br>
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
    }
}