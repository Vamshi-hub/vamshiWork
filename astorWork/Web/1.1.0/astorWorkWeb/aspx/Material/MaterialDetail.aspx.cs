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
using System.Configuration;

namespace astorWork
{
    public partial class MaterialDetail : System.Web.UI.Page
    {
        #region Declaration

        string strHostName = string.Empty;
        //CommonGeneral objCommon = null;
        Guid _tenantID;
        astorTrackPAPIHelper api = new astorTrackPAPIHelper();
        string _materialNo;
        MaterialMasterModel _model;
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
            api.Endpoint = ConfigurationManager.AppSettings["astorTrackEndpoint"];
            if (!IsPostBack)
            {
                string pageID = "MaterialInspectionDetail.aspx";
                var data = api.GetPageMasterByPageID(pageID);
                uxDisplayName.Value = data.First().PageDisplayName;
                uxURL.Value = Request.ApplicationPath + data.First().PageURL.ToString();

                BindData();
            }          
        }

        private void BindData()
        {
            var materialNo = Request.QueryString["MaterialNo"];
            if (materialNo != null)
            {
                _materialNo = materialNo.ToString();
                _model = api.GetMaterialMaster(_materialNo);

                BindMaterialMaster();
                SetGrouping();
            }
        }

        private void BindMaterialMaster()
        {
            uxMarkingNo.Text = _model.MaterialMasters.First().MarkingNo;
            uxProject.Text = _model.MaterialMasters.First().Project;
            //uxStatus.Text = _model.MaterialMasters.First().Status;
            uxDrawingNo.Text = _model.MaterialMasters.First().DrawingNo;
            uxDrawingIssueDate.Text = _model.MaterialMasters.First().DrawingIssueDate.ToShortDateString();
            uxMaterialType.Text = _model.MaterialMasters.First().MaterialType;
            uxMaterialSize.Text = _model.MaterialMasters.First().MaterialSize;
            uxMRFNo.Text = _model.MaterialMasters.First().MRFNo;
            uxContractor.Text = _model.MaterialMasters.First().Contractor;
            uxBlock.Text = _model.MaterialMasters.First().Block;
            uxLevel.Text = _model.MaterialMasters.First().Level;
            uxZone.Text = _model.MaterialMasters.First().Zone;
            //uxEstimatedLength.Text = ""; // _model.MaterialMasters.First().EstimatedLength.ToString() + "m";
            //uxActualLength.Text = ""; // _model.MaterialMasters.First().ActualLength.ToString() + "m";
        }

        private void SetGrouping()
        {
            var rfidTagId = _model.MaterialDetails.GroupBy(g => new { g.RFIDTagID}).Select(s => new { RFIDTagID = s.Key.RFIDTagID}).First();

            //GridColumnGroup locA = uxMaterialDetail.MasterTableView.ColumnGroups.FindGroupByName("LocationA");
            //if (locA == null)
            //{
            //    locA = new GridColumnGroup();
            //    uxMaterialDetail.MasterTableView.ColumnGroups.Add(locA);
            //    locA.Name = "LocationA";
            //}           
            //locA.HeaderText = "PRECAST PARAPET";
           
            GridColumnGroup grpA = uxMaterialDetail.MasterTableView.ColumnGroups.FindGroupByName("GroupA");
            if (grpA == null)
            {
                grpA = new GridColumnGroup();
                uxMaterialDetail.MasterTableView.ColumnGroups.Add(grpA);
                grpA.Name = "GroupA";
            }

            grpA.HeaderText = rfidTagId.RFIDTagID == null ? null : "RFID Tag " + rfidTagId.RFIDTagID;
            //grpA.ParentGroupName = "LocationA";
           
        }


        protected void uxMaterialDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            //if (objCommon.objUserProfile.UserID.ToLower() == Constants.ADMIN_USER_NAME.ToLower())
            //{
                if (_model == null)
                {
                    var MarkingNo = Request.QueryString["MarkingNo"];
                    if (MarkingNo != null)
                    {
                        _materialNo = MarkingNo.ToString();
                        _model = api.GetMaterialMaster(_materialNo);
                    }
                }
                uxMaterialDetail.DataSource = _model.MaterialDetails.ToList();
            //}
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
            //Page.Theme = objCommon.objUserProfile.UserTheme;
        }

        #endregion

        protected void uxMaterialDetail_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "RebindGrid")
            {
                uxMaterialDetail.MasterTableView.FilterExpression = "";
                BindData();
                uxMaterialDetail.Rebind();
            }
            if (e.CommandName == "")
            {
                //e.Canceled = true;
                //uxAddWindow.VisibleOnPageLoad = true;
                //string script = "function f(){$find(\"" + uxAddWindow.ClientID + "\").show(); Sys.Application.remove_load(f);}Sys.Application.add_load(f);";
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "key", script, true);
            }
        }

        protected void uxMaterialDetail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                TableCell progress = (TableCell)item["Stage"];
                TableCell qc1 = (TableCell)item["QCStatus"];

                string QCStatus = Convert.ToString(item.GetDataKeyValue("QCStatus"));

                //!(progress.Text == "Delivered" || 
                if (progress.Text == "QC")
                {
                    qc1.Text = " "; 
                }
                else
                {
                    if (!(QCStatus.ToLower() == "pass" || QCStatus.ToLower() == "fail" || QCStatus.ToLower() == "open"))
                        qc1.Text = " ";
                    else
                    {

                        Image img = (Image)item["QCStatus"].Controls[0];
                        //img.Attributes["onclick"] = string.Format("RowSelecting('{0}')", progress.Text);
                        //if (progress.Text == "Delivered" )
                        //{
                        //    img.ImageUrl = string.Format("~/Images/{0}.png", QCStatus) ;
                        //}
                    }
                    
                }
            }
        }

        protected void uxMaterialDetail_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void rbtnRefreshGrid_Click(object sender, EventArgs e)
        {
            uxMaterialDetail.DataBind();
        }
    }
}