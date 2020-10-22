using System.ComponentModel.DataAnnotations;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
    public class OrganisationMaster
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int CycleDays { get; set; }

        //1 = Vendor

        public OrganisationType OrganisationType { get; set; }
    }
}
