using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace astorWorkDAO
{
    public class PageMaster
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string UrlPath { get; set; }

        public int ModuleMasterID { get; set; }
        [ForeignKey("ModuleMasterID")]
        public ModuleMaster Module { get; set; }

    }
}
