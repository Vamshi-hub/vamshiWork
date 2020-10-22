using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;
using Telerik.Web.UI;


namespace astorWork.aspx.Yard
{
    public partial class ZoneEmployees : System.Web.UI.Page
    {
        #region Declarations

        RadGridFilter objRadGridFilter;
        AstorSafeMappingDAO objastorSafe = null;


        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (objastorSafe == null)
                objastorSafe = new AstorSafeMappingDAO();

            if (!IsPostBack)
            {
                rgEmployee.PageSize = 10;
                hdnZoneName.Value = Request.QueryString["ZoneName"] != null ? Request.QueryString["ZoneName"] : Request.QueryString["YardName"];
                if (Request.QueryString["Type"] == "Zone")
                    hdnType.Value = "Zone";
                else
                    hdnType.Value = string.Empty;
                if (Request.QueryString["Source"] != null && Request.QueryString["Source"].ToString() == "AllCS")
                {
                    rgEmployee.Columns[5].Visible = false;
                    rgEmployee.Columns[6].Visible = true;
                    rgEmployee.Columns[7].Visible = true;
                }
                hdnSource.Value = Request.QueryString["Source"] != null ? Request.QueryString["Source"].ToString() : "";

                hdnCompanyID.Value = Convert.ToString(Request.QueryString["CompanyIDs"]);
                hdnDepartmentID.Value = Convert.ToString(Request.QueryString["DepartmentIDs"]);
                hdnTradeID.Value = Convert.ToString(Request.QueryString["TradeIDs"]);
                hdnHasCS.Value = Convert.ToString(Request.QueryString["HasCS"]);
            }

        }

        protected override void InitializeCulture()
        {
            //objCommon = new CommonGeneral();
            //Culture = objCommon.objUserProfile.Culture;
            //UICulture = objCommon.objUserProfile.UICulture;
            base.InitializeCulture();
        }

        //protected void Page_PreInit(object sender, EventArgs e)
        //{
        //    Page.Theme = objCommon.objUserProfile.UserTheme;
        //}

        #endregion

        #region GridEvents

        protected void rgEmployee_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            try
            {
                int intZoneID = Request.QueryString["ZoneID"] != null ? Convert.ToInt32(Request.QueryString["ZoneID"]) : Convert.ToInt32(Request.QueryString["YardID"]);
                hdnZonID.Value = Request.QueryString["ZoneID"] != null ? Request.QueryString["ZoneID"] : Request.QueryString["YardID"];
                hdnCSID.Value = Request.QueryString["CSID"];
                string strCompanyIDs = Request.QueryString["CompanyIDs"];
                string strDepartmentIDs = Request.QueryString["DepartmentIDs"];
                string strTradeIDs = Request.QueryString["TradeIDs"];

                hdnReportType.Value = "Current Zone Employee Details";
                List<CurrentZoneEmployees> lstCurrentZoneEmps = new List<CurrentZoneEmployees>();
                if (Request.QueryString["ZoneID"] != null)
                {
                    if (Request.QueryString["PastDate"] == null)
                    {
                        if (Request.QueryString["CSID"] != null)
                            lstCurrentZoneEmps = objastorSafe.GetCurrentZoneEmployees(intZoneID, strCompanyIDs, strDepartmentIDs, strTradeIDs, Convert.ToInt32(Request.QueryString["CSID"]));
                        else
                            lstCurrentZoneEmps = objastorSafe.GetCurrentZoneEmployees(intZoneID, strCompanyIDs, strDepartmentIDs, strTradeIDs);
                    }
                    //else
                    //    lstCurrentZoneEmps = objZoneViewDAO.GetPastZoneEmployees(intZoneID, strCompanyIDs, strDepartmentIDs, strTradeIDs, Convert.ToInt32(Request.QueryString["YardID"]), Convert.ToDateTime(Request.QueryString["PastDate"]));


                }
                if (Request.QueryString["Source"] == null)
                {
                    rgEmployee.DataSource = lstCurrentZoneEmps.OrderByDescending(emp => emp.TimeIn);
                    rgEmployee.ClientSettings.Scrolling.AllowScroll = lstCurrentZoneEmps.Count > 8;
                    this.Title = String.Format(Request.QueryString["ZoneName"] + " Employee Details ({0})", lstCurrentZoneEmps.Count);
                }
                else if (Request.QueryString["Source"].ToString() == "ZoneView")
                {
                    rgEmployee.DataSource = lstCurrentZoneEmps.Where(emp => emp.IsCSEmployee == false).OrderByDescending(emp => emp.CSTimeIn);
                    rgEmployee.ClientSettings.Scrolling.AllowScroll = lstCurrentZoneEmps.Where(emp => emp.IsCSEmployee == false).Count() > 8;
                    this.Title = String.Format(Request.QueryString["ZoneName"] + " Employee Details ({0})", lstCurrentZoneEmps.Where(emp => emp.IsCSEmployee == false).Count());
                }
                else
                {
                    rgEmployee.DataSource = lstCurrentZoneEmps.Where(emp => emp.IsCSEmployee == true).OrderBy(emp => emp.TimeIn);
                    rgEmployee.ClientSettings.Scrolling.AllowScroll = lstCurrentZoneEmps.Where(emp => emp.IsCSEmployee == true).Count() > 8;
                    this.Title = String.Format(Request.QueryString["ZoneName"] + " Employee Details ({0})", lstCurrentZoneEmps.Where(emp => emp.IsCSEmployee == true).Count());
                }


            }
            catch (Exception ex)
            {
                // Master.DisplayAlertMessage(Messages.LoadingError, true, ex);
            }
        }

        protected void rgEmployee_ItemCommand(object source, GridCommandEventArgs e)
        {
            List<RadGridFilter> lstFilter = new List<RadGridFilter>();
            if (e.CommandName == RadGrid.FilterCommandName)
            {
                Pair filterPair = (Pair)e.CommandArgument;
                if ((e.Item as GridFilteringItem)[filterPair.Second.ToString()].Controls[0].GetType().Name == "RadComboBox")
                {
                    RadComboBox filterBox = (e.Item as GridFilteringItem)[filterPair.Second.ToString()].Controls[0] as RadComboBox;
                    objRadGridFilter = new RadGridFilter(filterPair.Second.ToString(), filterPair.First.ToString(), filterBox.Text.ToString());
                }
                else
                {
                    TextBox filterBox = (e.Item as GridFilteringItem)[filterPair.Second.ToString()].Controls[0] as TextBox;
                    objRadGridFilter = new RadGridFilter(filterPair.Second.ToString(), filterPair.First.ToString(), filterBox.Text);
                }

                if (ViewState["Filter"] != null)
                {
                    for (int i = 0; i < ((List<RadGridFilter>)ViewState["Filter"]).Count; i++)
                    {
                        if (((List<RadGridFilter>)ViewState["Filter"])[i].Column == objRadGridFilter.Column)
                            ((List<RadGridFilter>)ViewState["Filter"]).Remove(((List<RadGridFilter>)ViewState["Filter"])[i]);
                    }
                    ((List<RadGridFilter>)ViewState["Filter"]).Add(objRadGridFilter);
                }
                else
                {
                    lstFilter.Add(objRadGridFilter);
                    ViewState["Filter"] = lstFilter;
                }
            }

        }




        #endregion

        [Serializable]
        private class RadGridFilter
        {
            public RadGridFilter(string Col, string item, string Cont)
            {
                this.Column = Col;
                this.FilterItem = item;
                this.Content = Cont;
            }

            public string Column { get; set; }

            public string Content { get; set; }

            public string FilterItem { get; set; }
        }
    }
}