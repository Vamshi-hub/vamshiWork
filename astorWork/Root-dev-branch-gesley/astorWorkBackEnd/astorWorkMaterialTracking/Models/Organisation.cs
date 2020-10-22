using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class Organisation
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CycleDays { get; set; }
        public int OrganisationType { get; set; }
        public string OrganisationTypeName { get; set; }

        [NotMapped]
        public List<ContactPerson> ContactPeople { get; set; }
        [NotMapped]
        public List<Location> Locations { get; set; }
    }
}
