using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class ImportMaterialMasterTemplate
    {
        public string Block { get; set; }
        public int OrganisationID { get; set; }
        public string MaterialType { get; set; }
        public IFormFile TemplateFile { get; set; }
    }
}
