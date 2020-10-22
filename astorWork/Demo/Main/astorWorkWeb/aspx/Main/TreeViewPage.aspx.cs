using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;

using Telerik.Web.UI;

using System.Web.UI.HtmlControls;

namespace astorWork.aspx.Main
{
    public partial class TreeViewPage : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            // GenerateMenuTree();
            string userName = string.Empty;
            try
            {
                userName = Session[Helper.Constants.SESSION_USER_PROFILE].ToString();
            }
            catch (Exception exc)
            {

            }

            if(userName.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
            {
                menuConfig.Visible = true;
            }
        }



    }
}