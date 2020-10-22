using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace astorWork.aspx.Main
{
    public partial class Banner : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if(Session[astorWork.Helper.Constants.SESSION_USER_PROFILE]!=null)
            {
                rbUserProfile.Text = Session[astorWork.Helper.Constants.SESSION_USER_PROFILE].ToString();
            }
        }





    }
}