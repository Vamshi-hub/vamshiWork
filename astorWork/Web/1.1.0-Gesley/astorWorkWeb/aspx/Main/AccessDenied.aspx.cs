using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace astorSafeWeb.aspx.Main
{
    public partial class AccessDenied : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPreInit(EventArgs e)
        {
            if (Request.QueryString["MasterPage"] != null)
                this.MasterPageFile = Request.QueryString["MasterPage"].ToString();
            base.OnPreInit(e);
        }
    }
}