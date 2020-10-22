using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkShared.Services
{
    public interface IAstorWorkImport
    {
        List<string> GetRowsFromFile(IFormFile file);
    }
}
