using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace astorWorkDAO
{
    public class ProjectMaster
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        public string Country { get; set; }
        public int TimeZoneOffset { get; set; }
        public string Description { get; set; }
        public DateTimeOffset? EstimatedStartDate { get; set; }
        public DateTimeOffset? EstimatedEndDate { get; set; }
    }
}
