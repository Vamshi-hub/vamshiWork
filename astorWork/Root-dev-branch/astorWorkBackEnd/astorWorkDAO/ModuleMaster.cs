using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace astorWorkDAO
{
    public class ModuleMaster
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string UrlPrefix { get; set; }

        public List<PageMaster> Pages { get; set; }

        public int? ParentModuleID { get; set; }
        [ForeignKey("ParentModuleID")]
        public ModuleMaster ParentModule { get; set; }
    }
}
