using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace astorWorkDAO
{
    public class VendorMaster
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int CycleDays { get; set; }
    }
}
