using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkUserManage.Models
{
    public class TokenResponse
    {
        public string tokenType { get; set; }
        public int expiresIn { get; set; }
        public string accessToken { get; set; }
        public string refreshToken { get; set; }
        public int userId { get; set; }
    }
}
