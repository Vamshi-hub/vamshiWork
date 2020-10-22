using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaterialTrackApp.Class
{
    public class Constants
    {
        // Settings
        public static readonly int SETTING_API_BUFFER_SIZE = 2 * 1024 * 1024;
        public static readonly int SETTING_API_TIMEOUT_SECONDS = 8;

        //Power BI
        public static readonly string PB_API_END_POINT = "https://api.powerbi.com/v1.0/{0}/";

        public static readonly string PB_TENANT_ID = "3156e991-a773-429e-a59a-df6faa02e474";
        public static readonly string PB_USER_NAME = "powerbiadmin@astoriasolutions.com";
        public static readonly string PB_USER_PWD = "Acpl123!@#";
        public static readonly string PB_CLIENT_ID = "7ad390e8-ac68-4c32-aca5-cb00da9ff42f";
        public static readonly string PB_CLIENT_SECRET = "HA29yEdxUaFYHJgHcBzVlITPtcm9t41XaTy/3U+v8oI=";
        public static readonly string PB_NATIVE_CLIENT_ID = "902c893f-8436-4ffb-9697-368cd96d4291";

        public static readonly string OP_ENROLL = typeof(Constants).AssemblyQualifiedName + ".OP_ENROLL";
        public static readonly string OP_QC_PRODUCE = typeof(Constants).AssemblyQualifiedName + ".OP_QC_PRODUCE";

        public static readonly int ROLE_VENDOR = 0;
        public static readonly int ROLE_SITE_SUPERVISOR = 1;

    }
}
