using System;
using System.Collections.Generic;
using System.Text;

namespace astorWorkDAO
{
    public class ConfigurationMaster
    {
        public int ID { get; set; }
        public string Cofiguration { get; set; }
        public string Setting { get; set; }
        public UserMaster LastUpdatedBy { get; set; }
        public int LastUpdatedByID { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
