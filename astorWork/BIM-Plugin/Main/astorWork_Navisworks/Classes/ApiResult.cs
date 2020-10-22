using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWork_Navisworks.Classes
{
    public class ApiResult
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public JToken Data { get; set; }
    }
}
