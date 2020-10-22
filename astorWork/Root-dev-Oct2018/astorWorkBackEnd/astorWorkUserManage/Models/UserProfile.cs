using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkUserManage.Models
{
    public class UserProfile
    {
        public int UserID { get; set; }
        public string PersonName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public int RoleID { get; set; }
        public string Role { get; set; }
        public int SiteID { get; set; }
        public string Site { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public bool IsActive { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
