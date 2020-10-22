using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkUserManage.Models
{
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class RefreshTokenPayload
    {
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
    }
    public class Audience
    {
        public string Secret { get; set; }
        public string Iss { get; set; }
        public string Aud { get; set; }
    }
}
