using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class MaterialOverallProgress
    {
        public DateTimeOffset? Date { get; set; }
        public double Progress { get; set; }
    }
}
