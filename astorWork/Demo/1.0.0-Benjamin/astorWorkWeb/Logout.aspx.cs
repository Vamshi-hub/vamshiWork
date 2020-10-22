using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;
using System.Web.Security;

namespace astorSafeWeb
{
    public partial class Logout : System.Web.UI.Page
    {
        string strHostName = string.Empty;
        Guid _tenantID;

        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            Response.Cache.SetExpires(DateTime.Now);
            Response.Expires = -1;
            Response.Flush();
        }
        private void ExtractHost()
        {
            string strURL = Request.Url.ToString();

            if (!strURL.Contains("://"))
                strURL = "http://" + strURL;

            strHostName = new Uri(strURL).Host;
        }
    }
}