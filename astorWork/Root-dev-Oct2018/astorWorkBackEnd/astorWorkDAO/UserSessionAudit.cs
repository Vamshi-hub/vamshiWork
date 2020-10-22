using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace astorWorkDAO
{
    public class UserSessionAudit
    {
        public int ID { get; set; }

        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }

        [Required]
        public int ExpireIn { get; set; }

        [Required]
        public DateTimeOffset CreatedTime { get; set; }


    }
}
