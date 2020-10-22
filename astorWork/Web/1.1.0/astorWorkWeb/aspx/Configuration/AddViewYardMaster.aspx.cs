using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;
using Telerik.Web.UI;

namespace astorWork.aspx.Configuration
{
    public partial class AddViewYardMaster : System.Web.UI.Page
    {
        #region Declaration

        YardDAO objYardDAO = null;
        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (objYardDAO == null)
                objYardDAO = new YardDAO();
            RadWin_YardPreview.VisibleOnPageLoad = false;
            if (!IsPostBack)
            {
                //string UserID = objCommon.objUserProfile.UserID;
                //this.Master.InsertUserPageAccessDetail(UserID, this.Context);
            }
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
           // Page.Theme = objCommon.objUserProfile.UserTheme;
        }

        protected override void InitializeCulture()
        {
            base.InitializeCulture();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        #endregion

        #region Button Events

        protected void rbtnRefreshGrid_Click(object sender, EventArgs e)
        {
            rgYard.Rebind();
            rgYard.Visible = true;
          //  Master.DisplayAlertMessage((Enums.Operation)Enum.Parse(typeof(Enums.Operation), hdnOperation.Value), "Yard");
        }

        protected void rbtnPreview_Click(object sender, EventArgs e)
        {
            RadButton rdbtn = sender as RadButton;
            int intYardID = Convert.ToInt32(rdbtn.CommandArgument.ToString());
            var objYardTbl = objYardDAO.GetYardByYardID(intYardID);
            MemoryStream memoryStream = new MemoryStream(objYardTbl.YardLayout, false);
            rimgMapYardView.DataValue = memoryStream.ToArray();
            RadWin_YardPreview.Title = objYardTbl.YardName;
            RadWin_YardPreview.VisibleOnPageLoad = true;
        }

        #endregion

        #region Gridview Events

        protected void rgYard_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            try
            {
                rgYard.DataSource = from ym in objYardDAO.GetAllYards()
                                    select new
                                    {
                                        ym.YardID,
                                        ym.YardName,
                                        ym.UTCOffset,
                                        IsDefault = ym.IsDefault == true ? "Yes" : "No",
                                        EnableYardTimeCapture = ym.EnableYardTimeCapture == true ? "Yes" : "No"
                                    };
            }
            catch (Exception ex)
            {
              //  Master.DisplayAlertMessage(Messages.LoadingError, true, ex);
            }
        }

        #endregion

    }
}