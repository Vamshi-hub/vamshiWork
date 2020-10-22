﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using Telerik.Web.UI;
using System.Reflection;

namespace astorWork.aspx.Main
{
    public partial class ContentPage : System.Web.UI.Page
    {
        

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NavigationNode navNode = rnDockMenu.FindControl("rnUserProfile") as NavigationNode;
                RadButton rbtnUserProfile = navNode.FindControl("rbUserProfile") as RadButton;
                if (Session[astorWork.Helper.Constants.SESSION_USER_PROFILE] != null)
                {
                    string UserName = Session[astorWork.Helper.Constants.SESSION_USER_PROFILE].ToString();
                    rbtnUserProfile.Text = UserName.Length > 15 ? UserName.Substring(0, 14) : UserName;
                }
            }

        }

        

        #endregion
        [System.Web.Services.WebMethod]
        public static void ResetSession()
        {
            //Do not delete this method. It is for resetting session timeeout for keeping appln alive.
        }

        
    }
}