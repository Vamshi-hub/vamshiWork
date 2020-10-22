using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;

namespace astorWork.aspx.Configuration
{
    public partial class AddUser : System.Web.UI.Page
    {
        #region Declarations
        UsersDAO objUsersDAO;
        Enums.Operation eOperation;
        bool pwdChanged = false;
        #endregion

        #region Page Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (objUsersDAO == null)
                objUsersDAO = new UsersDAO();
            if (string.IsNullOrEmpty(Request.QueryString["UserId"]))
            {
                rbtnSubmit.Text = "Save";
                this.Title = "Add User";
            }
            else
            {
                this.Title = "Edit User";
                rbtnSubmit.Text = "Update";
                if (!IsPostBack)
                    PopulateUserData();
            }
        }
        #endregion

        #region Button Events
        protected void rbtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(Request.QueryString["UserId"]))
                    if (objUsersDAO.CheckUserIDExists(txtUserId.Text.Trim()))
                    {
                        Master.DisplayAlertMessage("User already existed.", true);
                        return;
                    }
                UserMaster objUser = new UserMaster();

                objUser.UserID = txtUserId.Text.Trim();
                objUser.UserName = txtUserName.Text.Trim();

                if (!string.IsNullOrEmpty(txtPwd.Text.Trim()))
                {

                    objUser.UserPwd = txtPwd.Text.Trim();
                    pwdChanged = true;
                }
                else
                    objUser.UserPwd = lblPwd.Text;

                bool isSaved = false;
                if (Request.QueryString["UserId"] != null)
                    isSaved = objUsersDAO.updateUser(objUser);
                else
                    isSaved = objUsersDAO.insertUser(objUser);
                if(isSaved)
                {
                    eOperation = string.IsNullOrEmpty(Request.QueryString["UserId"]) ? Enums.Operation.Save : Enums.Operation.Update;
                    ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "onClose", "<script>CloseAndReload('" + eOperation + "');</script>", false);
                }
                else
                    Master.DisplayAlertMessage("Valve Master failed to save", true, null, false);
                
            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(ex.Message, true, ex, false);
                return;
            }
        }
        #endregion

        #region Private Methods
        private void PopulateUserData()
        {
            try
            {
                UserMaster objUserMaster = objUsersDAO.GetUserByUserID(Request.QueryString["UserId"]);
                if(objUserMaster!=null)
                {
                    txtUserId.Text = objUserMaster.UserID;
                    txtUserName.Text = objUserMaster.UserName;
                    lblPwd.Text = objUserMaster.UserPwd;
                    txtUserId.Enabled = false;
                    rfvPwd.EnableClientScript = false;
                    rfvCPwd.EnableClientScript = false;
                }
            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(ex.Message, true, ex, false);
                throw;
            }
        }
        #endregion
    }
}