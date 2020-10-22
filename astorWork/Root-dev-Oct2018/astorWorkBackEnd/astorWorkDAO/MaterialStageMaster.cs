using System.ComponentModel.DataAnnotations;

namespace astorWorkDAO
{
    public class MaterialStageMaster
    {
        public int ID { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [Required]
        public string Colour { get; set; }
        public int Order { get; set; }
        public bool IsQCStage { get; set; }
        public bool IsEditable { get; set; }

        public string MaterialTypes { get; set; }
    }
}
