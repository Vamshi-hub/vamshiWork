using System;

namespace astorWorkShared.Models
{
    public class ForgeToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public DateTime ExpireTime;
    }
}
