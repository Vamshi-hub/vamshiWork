using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkMaterialTracking.Models
{
    public class ImportMaterialMasterStats
    {
        public string Block { get; set; }
        public int CountUploaded { get; set; }
        public int RowsNotUploaded { get; set; }
        public string MaterialType { get; set; }
        public IFormFile TemplateFile { get; set; }

    }
}
