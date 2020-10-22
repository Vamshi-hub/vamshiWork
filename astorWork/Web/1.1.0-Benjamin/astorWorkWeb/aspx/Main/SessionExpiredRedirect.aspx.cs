using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace astorWork.aspx.Main
{
    public partial class SessionExpiredRedirect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strRedirectPage = Request.ApplicationPath + "/aspx/Main/SessionExpired.htm";
            Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "PageRedirect", string.Format("top.location.href='{0}';", strRedirectPage), true);  
        }
    }
}