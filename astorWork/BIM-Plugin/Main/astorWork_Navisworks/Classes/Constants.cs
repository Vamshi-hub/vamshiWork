using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWork_Navisworks.Classes
{
    public class Constants
    {
        public static readonly string CONTROL_HOME = typeof(Constants).Assembly.GetName().Name + ".HOME";
        public static readonly string CONTROL_LOG_IN = typeof(Constants).Assembly.GetName().Name + ".LOG_IN";
        public static readonly string CONTROL_SYNC = typeof(Constants).Assembly.GetName().Name + ".SYNC";
        public static readonly string CONTROL_VIDEO = typeof(Constants).Assembly.GetName().Name + ".VIDEO";

        public static readonly string ID_TASK_ROOT = typeof(Constants).Assembly.GetName().Name + ".ID_TASK_ROOT";

        public static readonly string TASK_NAME_ROOT = "Task-Astoria";

        public static readonly string TASK_TYPE_NA = "NA";
        public static readonly string TASK_TYPE_PRODUCED = "Produced";
        public static readonly string TASK_TYPE_DELIVERED = "Delivered";
        public static readonly string TASK_TYPE_INSTALLED = "Installed";

        public static readonly string TASK_APPEARANCE_PRODUCED = "Color-Produced";
        public static readonly string TASK_APPEARANCE_DELIVERED = "Color-Delivered";
        public static readonly string TASK_APPEARANCE_INSTALLED = "Color-Installed";
    }
}
