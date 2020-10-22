using System;
using astorWorkDAO;
using Telerik.Web.UI;
using System.Collections.Generic;
using System.Linq;
namespace astorWork.aspx.Configuration
{
    public partial class ViewUsers : System.Web.UI.Page
    {
        #region Declaration

        UsersDAO objUsersDAO;

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (objUsersDAO == null)
                objUsersDAO = new UsersDAO();

          
        }


        #endregion

        #region Gridview Events
      
        protected void rgUsers_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            try
            {
                
                rgUsers.DataSource =objUsersDAO.getAllUsers().Where(p=>p.UserID!="admin");
            }
            catch (Exception ex)
            {
               
            }
        }

       

        #endregion

        #region Button Events

       protected void rbtnRefreshGrid_Click(object sender, EventArgs e)
        {
            rgUsers.Rebind();
            Master.DisplayAlertMessage((Enums.Operation)Enum.Parse(typeof(Enums.Operation), hdnOperation.Value), "User");
        }

        #endregion

        #region Private Methods
      
        #endregion
    }
}