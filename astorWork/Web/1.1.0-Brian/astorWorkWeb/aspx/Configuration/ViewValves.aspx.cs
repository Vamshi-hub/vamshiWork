using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace astorWork.aspx.Configuration
{
    public partial class ViewValves : System.Web.UI.Page
    {
        #region Declaration
        ValveDAO objValveDAO;
        #endregion

        #region PageEvents
        protected void Page_Load(object sender, EventArgs e)
        {
            if (objValveDAO == null)
                objValveDAO = new ValveDAO();
            if (!IsPostBack)
                BindZones();
        }
         #endregion

        #region DropDownEvents
        protected void rddlZone_SelectedIndexChanged(object sender, Telerik.Web.UI.DropDownListEventArgs e)
        {
            rgValve.Visible = true;
            rgValve.Rebind();
        }
        #endregion

        #region GridEvents
        protected void rgValve_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
           if(IsPostBack)
            rgValve.DataSource = objValveDAO.getZoneValves(int.Parse(rddlZone.SelectedItem.Value),0);
        }

        protected void rbtnRefreshGrid_Click(object sender, EventArgs e)
        {
            rgValve.Rebind();
            Master.DisplayAlertMessage((Enums.Operation)Enum.Parse(typeof(Enums.Operation), hdnOperation.Value), "Valve");
        }
        #endregion

        #region PrivateMethods
        private void BindZones()
        {
            ZoneDAO objZoneDAO = new ZoneDAO();
            rddlZone.DataSource = objZoneDAO.GetAllZones();
            rddlZone.DataTextField = "ZoneName";
            rddlZone.DataValueField = "ZoneID";
            rddlZone.DataBind();
            rddlZone.Items.Insert(0, new DropDownListItem("--Select--", "--Select--"));
        }

        #endregion
        
    }
}