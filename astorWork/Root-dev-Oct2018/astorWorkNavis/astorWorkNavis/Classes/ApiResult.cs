using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkNavis.Classes
{
    public class ApiResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public JToken Data { get; set; }
    }
}
