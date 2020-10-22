using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorTrackPLibrary;
using Telerik.Web.UI;
using astorTrackPAPI;
using System.Net.Http.Formatting;
using astorTrackPAPIDataModel;
using Resources;

namespace astorWork
{
    public partial class MaterialMaster : System.Web.UI.Page
    {
        #region Declaration

        string strHostName = string.Empty;
        CommonGeneral objCommon = null;
        Guid _tenantID;
        astorTrackPAPIHelper api = new astorTrackPAPIHelper();                

        #endregion

        #region Page Events

        /// <summary>
        /// This is auto generated method of page class. It is executed on every postback.     
        /// </summary>
        /// <param name="sender">Parameter Of Type System.Object</param>
        /// <param name="e">Parameter Of Type System.EventArgs</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (objCommon == null)
            //    objCommon = new CommonGeneral();
                string pageID = "MaterialDetail.aspx";
                var data = api.GetPageMasterByPageID(pageID);
                uxDisplayName.Value = data.First().PageDisplayName;
            //Request.ApplicationPath + 
            uxURL.Value = data.First().PageURL.ToString();
        }

        /// <summary>
        /// This is protected overridable method of page class.      
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        /// <summary>
        /// This isto Initialize Culture
        /// </summary>
        protected override void InitializeCulture()
        {
            //objCommon = new CommonGeneral();
            base.InitializeCulture();
        }

        /// <summary>
        /// This is auto generated method of page class.
        /// Here we are configuring the Page theme.
        /// </summary>
        /// <param name="sender">Parameter Of Type System.Object</param>
        /// <param name="e">Parameter Of Type System.EventArgs</param>
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Page.Theme = "MetroTouch";
        }

        #endregion

        #region Gridview Events
        
        protected void uxMaterialMaster_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            //if (objCommon.objUserProfile.UserID.ToLower() == Constants.ADMIN_USER_NAME.ToLower())
            //{
                var result = api.GetMaterialMasters("");
                uxMaterialMaster.DataSource = result.ToList();
            //}
        }

        protected void uxMaterialMaster_ItemDataBound(object sender, GridItemEventArgs e)
        {
            //if (e.Item is GridDataItem)
            //{
            //    GridDataItem dataItem = (GridDataItem)e.Item;
            //    RadButton uxDetails = (RadButton)dataItem.FindControl("uxDetails");

            //    uxDetails.CommandName = url + "?JobID=" + dataItem.GetDataKeyValue("JobID").ToString();
            //    uxDetails.CommandArgument = displayName + " - " + dataItem.GetDataKeyValue("JobNo").ToString();
            //}          
            //if (e.Item is GridEditFormInsertItem && e.Item.OwnerTableView.IsItemInserted)
            //{
            //    GridEditableItem editItem = (GridEditableItem)e.Item;
            //    RadLabel uxLegendName = (RadLabel)e.Item.FindControl("uxLegendName");
            //    uxLegendName.Text = "New Job Order";

            //    BindCustomer(editItem, false);
            //    BindCompany(editItem, false);
            //}
           
        }

        protected void uxMaterialMaster_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "RebindGrid")
            {
                uxMaterialMaster.MasterTableView.FilterExpression = "";
            }
                //if (e.CommandName == "WorkOrder")
                //{ 

                //}
                //if (e.CommandName == RadGrid.InitInsertCommandName)
                //{

                //    e.Item.OwnerTableView.IsItemInserted = true;
                //    uxMaterialMaster.MasterTableView.ClearEditItems();
                //    e.Canceled = true;
                //    int pageIndex = e.Item.OwnerTableView.CurrentPageIndex;
                //    e.Item.OwnerTableView.InsertItem();
                //    e.Item.OwnerTableView.CurrentPageIndex = pageIndex;
                //    e.Item.OwnerTableView.Rebind();
                //}
                //if (e.CommandName == RadGrid.EditCommandName)
                //{
                //    GridEditableItem editItem = e.Item as GridEditableItem;
                //    e.Item.OwnerTableView.IsItemInserted = false;
                //    e.Item.OwnerTableView.Rebind();
                //}
                //if (e.CommandName == RadGrid.PerformInsertCommandName || e.CommandName == RadGrid.UpdateCommandName)
                //{
                //    if (IsValid)
                //    {
                //        GridEditableItem item = e.Item as GridEditableItem;
                //        RadTextBox uxJobNo = item.FindControl("uxJobNo") as RadTextBox;
                //        RadTextBox uxTitle = item.FindControl("uxTitle") as RadTextBox;
                //        RadComboBox uxCompanyID = item.FindControl("uxCompanyID") as RadComboBox;
                //        RadDatePicker uxStartDate = item.FindControl("uxStartDate") as RadDatePicker;
                //        RadDatePicker uxTargetEndDate = item.FindControl("uxTargetEndDate") as RadDatePicker;
                //        RadComboBox uxCustomerID = item.FindControl("uxCustomerID") as RadComboBox;
                //        RadTextBox uxVesselName = item.FindControl("uxVesselName") as RadTextBox;
                //        if (e.CommandName == "Update")
                //        {
                //            objJobMaster.JobID = Convert.ToInt32(item.GetDataKeyValue("JobID"));
                //        }
                //        JobMaster obJobMaster = new JobMaster();
                //        objJobMaster.JobNo = uxJobNo.Text.Trim();
                //        obJobMaster = api.GetJobMasterByJobNo(objJobMaster.JobNo, objJobMaster.JobID);

                //        /// validate JobNo based on JobNO
                //        if (!string.IsNullOrEmpty(obJobMaster.JobNo))
                //        {
                //            Master.DisplayAlertMessage(Resources.Messages.JobNumberExists, true);
                //            return;
                //        }
                //        objJobMaster.Title = uxTitle.Text.Trim();
                //        objJobMaster.CompanyID = Convert.ToInt32(uxCompanyID.SelectedValue);
                //        objJobMaster.TargetEndDate = uxTargetEndDate.SelectedDate;
                //        objJobMaster.StartDate = uxStartDate.SelectedDate;
                //        objJobMaster.CustomerID = Convert.ToInt32(uxCustomerID.SelectedValue);
                //        objJobMaster.VesselName = uxVesselName.Text.Trim();
                //        objJobMaster.CreatedBy = objCommon.objUserProfile.UserID;
                //        objJobMaster.CreatedDate = DateTime.UtcNow;

                //        // Save/Update JobMaster
                //        bool Result = api.SaveJobMaster(objJobMaster);
                //        if (Result && objJobMaster.JobID == 0)
                //            Master.DisplayAlertMessage(Messages.JobNoCreated);
                //        else if (Result && objJobMaster.JobID != 0)
                //            Master.DisplayAlertMessage(Messages.JobNoUpdated);

                //        uxMaterialMaster.MasterTableView.ClearEditItems();
                //        uxMaterialMaster.MasterTableView.IsItemInserted = false;
                //        uxMaterialMaster.Rebind();
                //    }
                //}
            }

        #endregion

        #region Method

        //private void BindCompany(GridEditableItem editItem, bool isEdit)
        //{
        //    RadComboBox uxCompanyID = editItem.FindControl("uxCompanyID") as RadComboBox;
        //    var data = api.GetLocationMasterByType("Company"); //check here requirements
        //    if (data != null)
        //    {
        //        uxCompanyID.DataSource = data.ToList();
        //        uxCompanyID.DataTextField = "Description";
        //        uxCompanyID.DataValueField = "LocationID";
        //        uxCompanyID.DataBind();

        //        if (isEdit)
        //        {
        //            if (editItem.GetDataKeyValue("CompanyID") != null)
        //            {
        //                string companyID = editItem.GetDataKeyValue("CompanyID").ToString();
        //                uxCompanyID.SelectedValue = companyID;
        //            }
        //        }
        //    }
        //}

        //private void BindCustomer(GridEditableItem editItem, bool isEdit)
        //{
        //    RadComboBox uxCustomerID = editItem.FindControl("uxCustomerID") as RadComboBox;
        //    var objCustomer = api.GetLocationMasterByType("Customer"); //check here requirements
        //    uxCustomerID.DataSource = objCustomer.ToList();
        //    uxCustomerID.DataTextField = "Description";
        //    uxCustomerID.DataValueField = "LocationID";
        //    uxCustomerID.DataBind();

        //    if (isEdit)
        //    {
        //        if (editItem.GetDataKeyValue("CustomerID") != null)
        //        {
        //            string customerID = editItem.GetDataKeyValue("CustomerID").ToString();
        //            uxCustomerID.SelectedValue = customerID;
        //        }
        //    }
        //}

        //private void BindFilterCompany(GridFilteringItem item)
        //{
        //    RadComboBox uxCompanyFilter = item.FindControl("uxCompanyFilter") as RadComboBox;
        //    var data = api.GetLocationMaster("Company");
        //    uxCompanyFilter.DataSource = data.ToList();
        //    uxCompanyFilter.DataTextField = "Description";
        //    uxCompanyFilter.DataValueField = "LocationID";
        //    uxCompanyFilter.DataBind();

        //    uxCompanyFilter.Items.Insert(0, new RadComboBoxItem("No Filter", ""));
        //}

        #endregion

        #region control events

        protected void rbtnRefreshGrid_Click(object sender, EventArgs e)
        {
            uxMaterialMaster.Rebind();
            //Master.DisplayAlertMessage((Enums.Operation)Enum.Parse(typeof(Enums.Operation), hdnOperation.Value), "MaterialMaster");
        }

        //protected void uxCompanyFilter_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        //{
        //    string filterExpression;
        //    ViewState["CompanyName"] = e.Value;
        //    if (e.Value != "")
        //        filterExpression = string.Format("(iif(CompanyName == null, \"\", CompanyName).ToString().ToUpper() = \"{0}\")", e.Value.ToUpper());
        //    else
        //        filterExpression = "";

        //    uxMaterialMaster.MasterTableView.FilterExpression = filterExpression;
        //    uxMaterialMaster.MasterTableView.Rebind();

        //}

        //protected void uxCompanyFilter_PreRender(object sender, EventArgs e)
        //{
        //    if (ViewState["CompanyName"] != null)
        //    {
        //        ((RadComboBox)sender).SelectedValue = ViewState["CompanyName"].ToString();
        //    }
        //}

        #endregion

    }
}