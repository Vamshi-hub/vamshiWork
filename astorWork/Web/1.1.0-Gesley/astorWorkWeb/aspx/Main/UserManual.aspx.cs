using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace astorWork.aspx.Main
{
    public partial class UserManual : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                setUrl();
        }


        public void setUrl()
        {
            try
            {
                string url = Request.QueryString["UserManualURL"].ToString();
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                iframeUserManual.Src = url;
                divPagenotFound.Visible = false;
                divUserManual.Visible = true;
            }
            catch
            {
                divUserManual.Visible = false;
                divPagenotFound.Visible = true;

            }
        }
    }
}