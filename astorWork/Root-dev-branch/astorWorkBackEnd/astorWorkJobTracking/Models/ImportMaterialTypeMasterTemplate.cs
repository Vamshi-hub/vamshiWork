using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkJobTracking.Models
{
    public class ImportMaterialTypeMasterTemplate
    {
        public int ProjectID { get; set; }
        public IFormFile TemplateFile { get; set; }
    }
}
