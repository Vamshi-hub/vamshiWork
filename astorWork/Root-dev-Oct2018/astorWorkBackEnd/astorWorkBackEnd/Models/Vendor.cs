using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class Vendor
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CycleDays { get; set; }

        [NotMapped]
        public List<ContactPerson> ContactPeople { get; set; }
    }
}
