using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace astorWorkShared.Services
{
    public class AstorWorkImport: IAstorWorkImport
    {
        public AstorWorkImport() { }

        public List<string> GetRowsFromFile(IFormFile file)
        {
            string result = string.Empty;

            using (StreamReader reader = new StreamReader(file.OpenReadStream()))
                result = reader.ReadToEnd().Trim();

            return result.Split('\n').ToList();
        }
    }
}
