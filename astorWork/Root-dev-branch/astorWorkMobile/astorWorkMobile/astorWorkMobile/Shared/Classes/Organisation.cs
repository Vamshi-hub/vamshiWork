using static astorWorkMobile.Shared.Classes.Enums;

namespace astorWorkMobile.Shared.Classes
{
    public class Organisation
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int CycleDays { get; set; }
        public OrganisationType OrganisationType { get; set; }
        public string OrganisationTypeName { get; set; }
    }
}
