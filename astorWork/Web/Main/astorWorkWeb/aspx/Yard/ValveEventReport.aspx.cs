using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.HtmlChart;

namespace astorWork.aspx.Yard
{
    public partial class ValveEventReport : System.Web.UI.Page
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
            BindValves();
        }
        #endregion

        #region PrivateMethods
        private void BindZones()
        {
            try
            {
                ZoneDAO objZoneDAO = new ZoneDAO();
                rddlZone.DataSource = objZoneDAO.GetAllZones();
                rddlZone.DataTextField = "ZoneName";
                rddlZone.DataValueField = "ZoneID";
                rddlZone.DataBind();
                
            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(ex.Message, true, ex, true);
                throw;
            }

        }
        private void BindValves()
        {
            try
            {
                if (rddlZone.SelectedItem.Value != "0")
                {
                    rddlValve.ClearSelection();
                    rddlValve.DataSource = objValveDAO.getZoneValves(int.Parse(rddlZone.SelectedItem.Value), 0);
                    rddlValve.DataTextField = "ValveName";
                    rddlValve.DataValueField = "ValveID";
                    rddlValve.DataBind();
                    rddlValve.DefaultMessage = "--Select valve--";
                }
            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(ex.Message, true, ex, true);
                throw;
            }

        }
        #endregion

        #region Button Events

        protected void rbnClear_Click(object sender, EventArgs e)
        {
            rddlZone.ClearSelection();
            rddlValve.ClearSelection();
            rddlValve.Items.Clear();
            rmyPicker.Clear();
            rhcEvents.Visible = false;
        }

        protected void rbnReport_Click(object sender, EventArgs e)
        {
            try
            {
                //   rccValveEvent.Visible = true;
                List<usp_GET_ValveEventReport_Result> objList = objValveDAO.getValveEvents(int.Parse(rddlValve.SelectedItem.Value), rmyPicker.SelectedDate.Value.Date, rmyPicker.SelectedDate.Value.AddMonths(1).AddDays(-1).Date);
                if (objList.Count > 0)
                {
                    var result = from _r in objList
                                 select new
                                 {
                                     CloseStatus = (_r.Status == 0 ? 0 :1),
                                     StatusText = (_r.Status == 0 ? "Open" : "Close"),
                                     Timestamp = _r.Timestamp.ToString("d MMM HH:mm:ss")
                                     ,
                                     StatusColor = (_r.Status == 0 ? "Red" : "Green")
                                     ,
                                     OpenStatus = (_r.Status == 0 ? 1.5 :0)
                                 };
                    rhcEvents.Visible = true;
                    rhcEvents.DataSource = result;
                   
                    rhcEvents.DataBind();
                    rhcEvents.ChartTitle.Text = "Valve Event Report " + rmyPicker.SelectedDate.Value.ToString("- MMMM yyyy");
                }
                else
                {
                    rhcEvents.Visible = false;
                    Master.DisplayAlertMessage("No Records found.",false,null,false);
                    
                }
                   


            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(ex.Message, true, ex, true);
                throw;
            }
        }
        #endregion

    }
}