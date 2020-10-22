using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkNavis2017.Classes
{
    public class Constants
    {
        public static readonly string CONTROL_HOME = typeof(Constants).Assembly.GetName().Name + ".HOME";
        public static readonly string CONTROL_LOG_IN = typeof(Constants).Assembly.GetName().Name + ".LOG_IN";
        public static readonly string CONTROL_SYNC = typeof(Constants).Assembly.GetName().Name + ".SYNC";
        public static readonly string CONTROL_VIDEO = typeof(Constants).Assembly.GetName().Name + ".VIDEO";
        public static readonly string CONTROL_SETTING = typeof(Constants).Assembly.GetName().Name + ".SETTING";

        public static readonly string ID_TASK_ROOT = typeof(Constants).Assembly.GetName().Name + ".ID_TASK_ROOT";

        public static readonly string TASK_NAME_ROOT = "Astoria-Task";

        public static readonly string APPEARANCE_PREFIX = "Astoria-";

        public static readonly int SETTING_API_BUFFER_SIZE = 2 * 1024 * 1024;
        public static readonly int SETTING_API_TIMEOUT_SECONDS = 15;
    }
}
