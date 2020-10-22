using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkShared.Models
{
    public class ForgeBucket
    {
        public string BucketKey { get; set; }
        public long CreatedDate { get; set; }
        public DateTime CreatedDT { get
            {
                return new DateTimeOffset(new DateTime(1970, 1, 1), TimeSpan.FromHours(0))
                    .AddMilliseconds(CreatedDate).LocalDateTime;
            }
        }
        public Autodesk.Forge.Model.BucketsItems.PolicyKeyEnum PolicyKey { get; set; }
    }
}
