using astorWorkDAO;
using astorWorkDAO.Entities;
using System;
using System.Data;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using Telerik.Web.UI;

namespace astorWork.aspx.Reports.JobAllocation
{
    public partial class JobDeploymentDetailRpt : System.Web.UI.Page
    {
        #region Declaration

        JobAllocationRpts dsJobDepliymentDetail;
        UsersDAO objUsersDAO;
        bool blnIsLocationMandatory = false;
        ConfigSetting objConfig;
        #endregion

        #region Page Events

        /// <summary>
        /// This is auto generated method of page class. It is executed on every postback.
        /// The filter controls which are required to be visible to UI are set here.
        /// </summary>
        /// <param name="sender">Parameter Of Type System.Object</param>
        /// <param name="e">Parameter Of Type System.EventArgs</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (hdnDB.Value == "")
            {
                if (objUsersDAO == null)
                    objUsersDAO = new UsersDAO();
                ConfigurationMaster objUserMaster = objUsersDAO.GetastorTimeDB();
                hdnDB.Value = objUserMaster.Setting;
            }

            if (!IsPostBack)
            {
                clrFromDate.SelectedDate = DateTime.Now;
                clrToDate.SelectedDate = DateTime.Now;
                blnIsLocationMandatory = false;
            }
        }
        protected void Page_PreRender(object sender, EventArgs e)
        {

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), this.UniqueID, string.Format("objFrmDate = document.getElementById('{0}'); objToDate = document.getElementById('{1}');",
               clrFromDate.ClientID, clrToDate.ClientID), true);
        }

        #endregion

        #region Button Events

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rpbGenerate_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
            {
                reportData.Attributes.Add("style", "display:none");
                return;
            }
            DataView dvInnerTime;

            try
            {
                objConfig = new ConfigSetting();
                astorTimeDAO objastorTimeDAO = new astorTimeDAO(hdnDB.Value);
                objConfig = objastorTimeDAO.GetConfiguration();
                bool EnableSubCode = false;
                if (objConfig.EnableSubCodeInJobAllocation || objConfig.EnableSubCodeInTimeCleaner)
                    EnableSubCode = true;
                reportData.Attributes.Add("style", "display:none");
                dsJobDepliymentDetail = objastorTimeDAO.DisplayJobDeploymentDetailRpt(clrFromDate.SelectedDate.Value, clrToDate.SelectedDate.Value);
                if (dsJobDepliymentDetail != null)
                {
                    if (dsJobDepliymentDetail.Tables["JobDeploymentDetail"].Rows.Count > 0)
                    {
                        reportData.Attributes.Add("style", "display:block");
                        dvInnerTime = dsJobDepliymentDetail.Tables["JobDeploymentDetail"].DefaultView;
                        ReportDataSource rptInnerTime = new ReportDataSource("JobDeploymentDetail", dvInnerTime);
                        ReportParameter[] rpParameter = new ReportParameter[3];
                        rpParameter[0] = new ReportParameter("Culture", "aa");
                        rpParameter[1] = new ReportParameter("DateRange", clrFromDate.SelectedDate.Value.ToString("dd MMM yyyy") + " to " + clrToDate.SelectedDate.Value.ToString("dd MMM yyyy"));
                        rpParameter[2] = new ReportParameter("EnableSubCode", EnableSubCode.ToString());
                        if (rcbReportType.SelectedValue == "1")
                        {
                            rptvwrJobDeployment.LocalReport.ReportEmbeddedResource = "Reports.JobAllocation.JobDeploymentDetailReport.rdlc";
                            rptvwrJobDeployment.LocalReport.ReportPath = @"Reports\JobAllocation\JobDeploymentDetailReport.rdlc";
                        }
                        else
                        {
                            rptvwrJobDeployment.LocalReport.ReportEmbeddedResource = "Reports.JobAllocation.JobDeploymentDetailSummaryReport.rdlc";
                            rptvwrJobDeployment.LocalReport.ReportPath = @"Reports\JobAllocation\JobDeploymentDetailSummaryReport.rdlc";

                        }
                        rptvwrJobDeployment.LocalReport.DataSources.Clear();
                        rptvwrJobDeployment.LocalReport.SetParameters(rpParameter);
                        rptvwrJobDeployment.LocalReport.DataSources.Add(rptInnerTime);
                        rptvwrJobDeployment.LocalReport.Refresh();
                    }
                    else
                    {
                        reportData.Attributes.Add("style", "display:none");
                        Master.DisplayNotificationMessage("No Records", false, null);
                    }
                }

            }

            catch (Exception ex)
            {
                Master.DisplayNotificationMessage("Loading Report", true, ex);
            }
        }

        #endregion


    }
}