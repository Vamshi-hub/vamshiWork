using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using astorWorkDAO;


namespace astorWork
{
    public partial class Login : System.Web.UI.Page
    {
        #region Declarations

        string strHostName = string.Empty;
        Guid _tenantID;
        LoginDAO objLogin = null;

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            objLogin = new LoginDAO();
            //
            if (!Page.IsPostBack)
            {
                FormsAuthentication.SignOut();
                Session.Abandon();
                Response.Cache.SetExpires(DateTime.Now);
                Response.Expires = -1;
                Response.Flush();
            }

        }

        protected void loginCtrl_Authenticate(object sender, AuthenticateEventArgs e)
        {

            ValidationResult enumResult;
            try
            {

                enumResult = (ValidationResult)objLogin.LoginValidation(loginCtrl.UserName.Trim(), loginCtrl.Password.Trim(), _tenantID);
                switch (enumResult)
                {
                    case ValidationResult.InvalidPassword:
                        e.Authenticated = false;
                        loginCtrl.FailureText = "Invalid Password";
                        break;
                    case ValidationResult.InvalidUser:
                        e.Authenticated = false;
                        loginCtrl.FailureText = "Invalid Username";
                        break;
                    case ValidationResult.InactiveUser:
                        e.Authenticated = false;
                        loginCtrl.FailureText = "User Inactive";
                        break;
                    case ValidationResult.Error:
                        e.Authenticated = false;
                        loginCtrl.FailureText = "Error validating user";
                        break;
                    case ValidationResult.Success:
                        e.Authenticated = true;

                        Session.Add(astorWork.Helper.Constants.SESSION_USER_PROFILE, loginCtrl.UserName);

                        FormsAuthentication.SetAuthCookie(loginCtrl.UserName.Trim(), false);
                        Server.Transfer("MainFrame.htm");
                        break;
                }
            }
            catch (Exception)
            {
                loginCtrl.FailureText = "Error validating user";
            }

        }

        #endregion
        #region Others

        enum ValidationResult
        {
            InvalidUser = 1,
            InactiveUser = 2,
            InvalidPassword = 3,
            Success = 4,
            Error = -1
        }

        private void ExtractHost()
        {
            string strURL = Request.Url.ToString();

            if (!strURL.Contains("://"))
                strURL = "http://" + strURL;

            strHostName = new Uri(strURL).Host;
        }

        #endregion

    }
}