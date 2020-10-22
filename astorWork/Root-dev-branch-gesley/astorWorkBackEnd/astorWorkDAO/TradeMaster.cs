using System;
using System.Collections.Generic;
using System.Text;
using static astorWorkShared.Utilities.Enums;

namespace astorWorkDAO
{
    public class TradeMaster
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string RouteTo { get; set; }

        /// <summary>
        /// 0 - archi
        /// 1 - M&E
        /// </summary>
        public int Type { get; set; }
        public InspectionStage RTOInspection { get; set; }
    }

}
