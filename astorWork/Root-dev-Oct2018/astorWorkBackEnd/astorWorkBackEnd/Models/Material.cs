﻿using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace astorWorkBackEnd.Models
{
    public class Material
    {
        public int VendorID { get; set; }
        public List<string> MaterialTypes { get; set; }
    }
}
