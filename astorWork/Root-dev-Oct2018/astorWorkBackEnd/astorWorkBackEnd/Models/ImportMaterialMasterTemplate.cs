using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class ImportMaterialMasterTemplate
    {
        public string Block { get; set; }
        public int VendorId { get; set; }
        public string MaterialType { get; set; }
        public IFormFile TemplateFile { get; set; }

    }
}
