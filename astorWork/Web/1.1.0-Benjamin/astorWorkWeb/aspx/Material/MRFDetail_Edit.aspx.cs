using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
//using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorTrackPLibrary;
using Resources;
using Telerik.Web.UI;
using astorTrackPAPI;
using System.Threading.Tasks;
using _apimodel = astorTrackPAPIDataModel;
using Newtonsoft.Json;
using System.Configuration;
using astorWorkDAO;

namespace astorWork
{
    public partial class MRFDetail_Edit : System.Web.UI.Page
    {
        #region Declarations

        string strHostName = string.Empty;
        //CommonGeneral objCommon = null;
        Guid _tenantID;
        astorTrackPAPIHelper api = new astorTrackPAPIHelper();
        _apimodel.MRFModel _model = new _apimodel.MRFModel();


        string _MRFNo;
        #endregion

        #region Page Events

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

        /// <summary>
        /// Initialise type of culture to page
        /// </summary>
        protected override void InitializeCulture()
        {
            //objCommon = new CommonGeneral();
            //Culture = objCommon.objUserProfile.UserCulture;
            base.InitializeCulture();
        }

        /// <summary>
        /// This is protected overridable method of page class.      
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }

        private void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

        /// <summary>
        /// This is auto generated method of page class. It is executed on every postback.     
        /// </summary>
        /// <param name="sender">Parameter Of Type System.Object</param>
        /// <param name="e">Parameter Of Type System.EventArgs</param>
        ///
        IEnumerable<_apimodel.MRFMaster> MRFMasters;
        protected void Page_Load(object sender, EventArgs e)
        {
            api.Endpoint = ConfigurationManager.AppSettings["astorTrackEndpoint"];
            MRFMasters = api.GetMRFMasters();

            if (!IsPostBack)
            {                
                //if (objCommon == null)
                //    objCommon = new CommonGeneral();
                string pageID = "MaterialDetail.aspx";
                var data = api.GetPageMasterByPageID(pageID);
                uxDisplayName.Value = data.First().PageDisplayName;
                uxURL.Value = data.First().PageURL.ToString();

                string MRFNo = Request.QueryString["MRFNo"];
                if (!string.IsNullOrEmpty(MRFNo))
                {
                    _MRFNo = MRFNo.ToString();
                    _model = api.GetMRFMaster(MRFNo);
                    var MRFMasters = _model.MRFMasters.FirstOrDefault();
                    uxMRFNo.Text = MRFMasters.MRFNo;
                    uxMRFDate.SelectedDate = MRFMasters.MRFDate;
                    uxStatus.Text = MRFMasters.Status;
                    uxPlannedCastingDate.SelectedDate = MRFMasters.PlannedCastingDate;
                    uxHeader.Title = "Material Request Form";
                    uxDetail.Title = "Component List";
                    uxRadWizard.DisplayProgressBar = false;
                    
                    if (_model.MRFMasters.First().Status != "New")
                    {
                        uxMRFDetail.MasterTableView.CommandItemSettings.ShowAddNewRecordButton = false;
                    }
                    uxMRFDate.Enabled = false;
                    RequiredFieldValidator3.Enabled = true;
                    BindContractor();
                    BindProject();
                    BindBlock();
                    BindLevel();
                    BindZone();
                    BindAttention();
                    BindMaterialType();

                    uxVendor.Enabled = false;
                    uxAttention.Enabled = false;
                    uxProject.Enabled = false;
                    uxMaterialType.Enabled = false;
                    uxPlannedCastingDate.Enabled = false;
                    uxBlock.Enabled = false;
                    uxLevel.Enabled = false;
                    uxZone.Enabled = false;
                }
                else
                {
                    uxHeader.StepType = RadWizardStepType.Start;
                    uxHeader.CausesValidation = true;
                    uxHeader.ValidationGroup = uxDetail.ValidationGroup = "vgMRFMaster";
                    uxDetail.StepType = RadWizardStepType.Finish;
                    _model = new _apimodel.MRFModel();
                    uxMRFDate.SelectedDate = DateTime.Now;
                    RequiredFieldValidator3.Enabled = false;
                    uxStatus.Text = "New";                    
                    BindContractor();
                    BindProject();
                    BindBlock();
                    BindLevel();
                    BindZone();
                    BindAttention();
                    BindMaterialType();
                }

            }
            //uxSearch.DataSource = api.GetMaterialMasters("MRFCables");
            //uxSearchjunctionBox.DataSource = api.GetCableLocations();

        }

        protected DateTime AddBusinessDays(DateTime startDate,
                                         int businessDays)
        {
            int direction = Math.Sign(businessDays);
            if (direction == 1)
            {
                if (startDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    startDate = startDate.AddDays(2);
                    businessDays = businessDays - 1;
                }
                else if (startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = startDate.AddDays(1);
                    businessDays = businessDays - 1;
                }
            }
            else
            {
                if (startDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    startDate = startDate.AddDays(-1);
                    businessDays = businessDays + 1;
                }
                else if (startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = startDate.AddDays(-2);
                    businessDays = businessDays + 1;
                }
            }

            int initialDayOfWeek = (int)startDate.DayOfWeek;

            int weeksBase = Math.Abs(businessDays / 5);
            int addDays = Math.Abs(businessDays % 5);

            if ((direction == 1 && addDays + initialDayOfWeek > 5) ||
                 (direction == -1 && addDays >= initialDayOfWeek))
            {
                addDays += 2;
            }

            int totalDays = (weeksBase * 7) + addDays;
            return startDate.AddDays(totalDays * direction);
        }

        protected bool IsVendor()
        {
            string userName = Session[Helper.Constants.SESSION_USER_PROFILE].ToString();
            astorWorkEntities objastorWorkEntities = new astorWorkEntities();
            astorWorkDAO.UserMaster objUser = objastorWorkEntities.UserMasters.Where(u => u.UserName.ToLower() == userName).FirstOrDefault();

            return objUser.IsVendor;
        }
        #endregion

        protected void uxMRFDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(uxMRFNo.Text))
                {
                    var _modelMRFmodel = api.GetMRFMaster(uxMRFNo.Text);
                    if (_modelMRFmodel.MaterialMasters != null)
                    { uxMRFDetail.DataSource = _modelMRFmodel.MaterialMasters; }
                    else { uxMRFDetail.DataSource = new object[0]; }
                }
                else
                {
                    uxMRFDetail.DataSource = new object[0];
                }

            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(Messages.LoadingError, true, ex);
            }
        }

        private void BindContractor()
        {
            var dataChild = api.GetLocationAssocByType("HAS Vendor");
            uxVendor.DataSource = dataChild.ToList();
            uxVendor.DataTextField = "ChildDescription";
            uxVendor.DataValueField = "AssociationID";
            uxVendor.DataBind();

            if (!string.IsNullOrEmpty(_MRFNo))
            {
                var item = uxVendor.FindItemByValue(_model.MRFMasters.First().LocationID.ToString());
                if (item != null)
                    item.Selected = true;
            }
        }

        private void BindProject()
        {
            var dataChild = api.GetObjectMasterByType("Project", "");
            uxProject.DataSource = dataChild.ToList();
            uxProject.DataTextField = "Description";
            uxProject.DataValueField = "Code";
            uxProject.DataBind();

            if (!string.IsNullOrEmpty(_MRFNo))
            {
                var item = uxProject.FindItemByValue(_model.MRFMasters.First().Project);
                if (item != null)
                    item.Selected = true;
            }
        }

        private void BindBlock()
        {
            var dataChild = api.GetObjectMasterByType("Block", "");
            uxBlock.DataSource = dataChild.ToList();
            uxBlock.DataTextField = "Description";
            uxBlock.DataValueField = "Code";
            uxBlock.DataBind();

            if (!string.IsNullOrEmpty(_MRFNo))
            {
                var item = uxBlock.FindItemByValue(_model.MRFMasters.First().Block);
                if (item != null)
                    item.Selected = true;
            }
        }

        private void BindLevel()
        {            

            var dataChild = api.GetObjectMasterByType("Level", "");
            uxLevel.DataSource = dataChild.ToList();
            uxLevel.DataTextField = "Description";
            uxLevel.DataValueField = "Code";
            uxLevel.DataBind();

            if (!string.IsNullOrEmpty(_MRFNo))
            {
                var item = uxLevel.FindItemByValue(_model.MRFMasters.First().Level);
                if (item != null)
                    item.Selected = true;
            }
        }

        private void BindZone()
        {
            var dataChild = api.GetObjectMasterByType("Zone", "");
            uxZone.DataSource = dataChild.ToList();
            uxZone.DataTextField = "Description";
            uxZone.DataValueField = "Code";
            uxZone.DataBind();

            if (!string.IsNullOrEmpty(_MRFNo))
            {
                var item = uxZone.FindItemByValue(_model.MRFMasters.First().Zone);
                if (item != null)
                    item.Selected = true;
            }
        }

        private void BindAttention()
        {
            var dataChild = api.GetObjectMasterByType("Attention", "");
            uxAttention.DataSource = dataChild.ToList();
            uxAttention.DataTextField = "Description";
            uxAttention.DataValueField = "Code";
            uxAttention.DataBind();

            if (!string.IsNullOrEmpty(_MRFNo))
            {
                var attentions = _model.MRFMasters.First().Attention.Split(new char[] { ',' });
                foreach (var attention in attentions)
                {
                    var item = uxAttention.FindItemByText(attention);
                    if (item != null)
                        item.Checked = true;
                }
            }
        }

        private void BindMaterialType()
        {
            var dataChild = api.GetObjectMasterByType("MaterialType", "");
            uxMaterialType.DataSource = dataChild.ToList();
            uxMaterialType.DataTextField = "Description";
            uxMaterialType.DataValueField = "Code";
            uxMaterialType.DataBind();

            if (!string.IsNullOrEmpty(_MRFNo))
            {
                var materialTypes = _model.MRFMasters.First().MaterialType.Split(new char[] { ',' });
                foreach (var materialType in materialTypes)
                {
                    var item = uxMaterialType.FindItemByText(materialType);
                    if (item != null)
                        item.Checked = true;
                }
            }
        }


        protected void uxProject_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void uxSearch_Search(object sender, SearchBoxEventArgs e)
        {
            //BindMaterialMaster(e.Text);
        }


        protected void uxMRFDetail_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                if (item.GetDataKeyValue("Status").ToString() != "Requested" || IsVendor())
                {
                    ImageButton img = (ImageButton)item["EditCommandColumn"].Controls[0];
                    img.Visible = false;
                }
            }
        }

        protected void uxSearch_DataSourceSelect(object sender, SearchBoxDataSourceSelectEventArgs e)
        {
            //SqlDataSource source = (SqlDataSource)e.DataSource;
            //RadSearchBox searchBox = (RadSearchBox)sender;

            //string likeCondition = string.Format("'{0}' + @filterString + '%'", searchBox.Filter == SearchBoxFilter.Contains ? "%" : "");
            //string countCondition = e.ShowAllResults ? " " : " TOP 100 ";

            //source.SelectCommand = string.Format("SELECT {0}* FROM [Products] WHERE [" + searchBox.DataTextField + "] LIKE {1}", countCondition, likeCondition);

            //source.SelectParameters.Add("filterString", e.FilterString.Replace("%", "[%]").Replace("_", "[_]"));

            //var uxSearch = sender as RadSearchBox;
            //uxSearch.DataSource = api.GetMaterialMasters(e.FilterString.ToString());
        }


        //_apimodel.MaterialMasterModel MaterialMasterModel;
        //private void BindMaterialMaster(string _MarkingNo)
        //{
        //    MaterialMasterModel = api.GetMaterialMaster(_MarkingNo);
        //    var MaterialMaster = MaterialMasterModel.MaterialMasters.FirstOrDefault();

        //    uxMarkingNo.Text = MaterialMaster.MarkingNo;
        //    ulProject.Text = MaterialMaster.Project;
        //    uxStatus.Text = MaterialMaster.Status;
        //    uxDrawingNo.Text = MaterialMaster.DrawingNo;
        //    uxMaterialType.Text = MaterialMaster.MaterialType;
        //    uxMaterialSize.Text = MaterialMaster.MaterialSize;
        //    //uxCableOD.Text = MaterialMaster.CableOD;
        //    ulContractor.Text = MaterialMaster.Contractor;
        //    uxAttention.Text = MaterialMaster.Attention;
        //    uxFromLocation.Text = MaterialMaster.Location != null ? MaterialMaster.Location.Replace("|", "<br/>") : MaterialMaster.Location;
        //    uxEstimatedLength.Text = MaterialMaster.EstimatedLength.ToString();
        //    uxActualLength.Text = MaterialMaster.EstimatedLength.ToString();
        //    uxActualLength.ReadOnly = false;
        //}

        //private void ClearMaterialMaster()
        //{
        //    uxMarkingNo.Text = "";
        //    uxProject.Text = "";
        //    //uxStatus.Text = _model.MaterialMasters.First().Status;
        //    uxDrawingNo.Text = "";
        //    uxMaterialType.Text = "";
        //    uxMaterialSize.Text = "";
        //    uxCableOD.Text = "";
        //    uxVendor.Text = "";
        //    uxOfficer.Text = "";
        //    uxFromLocation.Text = "";
        //    uxToLocation.Text = "";
        //    uxEstimatedLength.Text = "";
        //    uxActualLength.Text = "";
        //    uxSearch.Text = "";
        //    ulProject.Text = "";
        //    ulContractor.Text = "";
        //    uxJunctionBox.Text = "";
        //    uxCompartment.Text = "";
        //    uxDeck.Text = "";
        //    uxSearchjunctionBox.Text = "";
        //    uxAssociationID.Value = "";
        //}

        protected void uxSearch_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            //var uxSearch = sender as RadComboBox;
            //uxSearch.DataSource = api.GetMaterialMasters(e.Text);
        }

        //protected void btnInsert_Click(object sender, EventArgs e)
        //{
        //    RadButton btnInsert = (RadButton)sender;
        //    List<_apimodel.MRFDetail> lstMRFDetail = new List<_apimodel.MRFDetail>();
        //    _apimodel.MRFDetail objMRFDetail = new _apimodel.MRFDetail();

        //    objMRFDetail.MRFNo = uxMRFNo.Text;
        //    objMRFDetail.Component = null;
        //    objMRFDetail.DeliveryDate = null;
        //    objMRFDetail.Remarks = null;

        //    lstMRFDetail.Add(objMRFDetail);
        //    _apimodel.MRFModel objMRFModel = new _apimodel.MRFModel();
        //    //objMRFModel.MRFDetails = lstMRFDetail;
        //    if (lstMRFDetail.Count > 0)
        //    {
        //        api.SaveMRFMaster(objMRFModel);
        //        uxMRFDetail.Rebind();
        //        //ClearMaterialMaster();
        //    }
        //}

        protected void uxMRFDetail_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == RadGrid.UpdateCommandName)
            {
                if (IsValid)
                {

                    _apimodel.MaterialMaster objMaterialMaster = new _apimodel.MaterialMaster();
                    GridEditableItem item = e.Item as GridEditableItem;

                    RadDatePicker uxDeliveryDate = item.FindControl("uxDeliveryDate") as RadDatePicker;
                    RadTextBox uxDeliveryRemarks = item.FindControl("uxDeliveryRemarks") as RadTextBox;

                    objMaterialMaster.DeliveryDate = uxDeliveryDate.SelectedDate.Value;
                    objMaterialMaster.DeliveryRemarks = uxDeliveryRemarks.Text;
                    objMaterialMaster.MaterialNo = item.GetDataKeyValue("MaterialNo").ToString();
                    objMaterialMaster.Status = item.GetDataKeyValue("Status").ToString();
                    objMaterialMaster.MRFNo = item.GetDataKeyValue("MRFNo").ToString();
                    objMaterialMaster.UpdatedBy = "admin";
                    objMaterialMaster.UpdatedDate = DateTime.Now;

                    List<_apimodel.MaterialMaster> lstMaterialMaster = new List<astorTrackPAPIDataModel.MaterialMaster>();
                    lstMaterialMaster.Add(objMaterialMaster);

                    _apimodel.MaterialMasterModel objMaterialMasterModel = new _apimodel.MaterialMasterModel();
                    objMaterialMasterModel.MaterialMasters = lstMaterialMaster;
                    
                    bool Result = api.SaveMaterialMaster(objMaterialMasterModel);

                    uxMRFDetail.MasterTableView.ClearEditItems();
                    uxMRFDetail.MasterTableView.IsItemInserted = false;
                    uxMRFDetail.Rebind();
                }
            }
        }

        #region RadWizard events

        protected void uxRadWizard_NextButtonClick(object sender, WizardEventArgs e)
        {
            RadWizard uxRadWizard = (RadWizard)sender;
            string strsavupd = string.Empty;
            Page.Validate("vgMRFMaster");
            if (Page.IsValid )
            {
                try
                {
                    if (uxMRFNo.Text == "")
                    {
                        List<_apimodel.MRFMaster> lstMRFMaster = new List<_apimodel.MRFMaster>();
                        _apimodel.MRFMaster objMRFMaster = new _apimodel.MRFMaster();
                        if (string.IsNullOrEmpty(uxMRFNo.Text))
                        {
                            objMRFMaster.MRFNo = api.GetFormatKey("MRF", "admin");
                            uxMRFNo.Text = objMRFMaster.MRFNo;
                        }
                        else
                        {
                            objMRFMaster.MRFNo = uxMRFNo.Text;
                        }
                        objMRFMaster.MRFDate = uxMRFDate.SelectedDate.Value;
                        objMRFMaster.Project = uxProject.SelectedValue;
                        objMRFMaster.Vendor = uxVendor.SelectedValue;
                        objMRFMaster.LocationID = int.Parse(uxVendor.SelectedValue);

                        objMRFMaster.Attention = string.Join(",", uxAttention.CheckedItems.Select(s => s.Text));
                        objMRFMaster.MaterialType = string.Join(",", uxMaterialType.CheckedItems.Select(s => s.Text));

                        objMRFMaster.Project = uxProject.SelectedValue;
                        objMRFMaster.Block = uxBlock.SelectedValue;
                        objMRFMaster.Level = uxLevel.SelectedValue;
                        objMRFMaster.Zone = uxZone.SelectedValue;
                        objMRFMaster.PlannedCastingDate = uxPlannedCastingDate.SelectedDate.Value;
                        
                        if (uxStatus.Text == "New")
                        {
                            objMRFMaster.Status = "Pending";
                        }
                        else
                        {
                            objMRFMaster.Status = uxStatus.Text;

                        }
                        objMRFMaster.CreatedBy = "admin";
                        objMRFMaster.CreatedDate = DateTime.Now;

                        lstMRFMaster.Add(objMRFMaster);
                        _apimodel.MRFModel objMRFModel = new _apimodel.MRFModel();
                        objMRFModel.MRFMasters = lstMRFMaster;

                        if (lstMRFMaster.Count > 0)
                        {
                            api.SaveMRFMaster(objMRFModel);
                            Master.DisplayAlertMessage(string.Format("{0} saved successfully.", objMRFMaster.MRFNo));
                            uxRadWizard.ActiveStepIndex = e.NextStepIndex;
                            uxMRFDetail.Rebind();
                        }
                    }
                    else
                    {
                        uxRadWizard.ActiveStepIndex = e.NextStepIndex;
                    }
                }
                catch (Exception ex)
                {
                    uxRadWizard.ActiveStepIndex = e.CurrentStepIndex;
                    Master.DisplayAlertMessage(Messages.LoadingError, true, ex);
                }
            }
            else
            {
                uxRadWizard.ActiveStepIndex = e.CurrentStepIndex;
            }
        }

        protected void uxRadWizard_FinishButtonClick(object sender, WizardEventArgs e)
        {

            Button btnuxAddNewItem = (Button)(uxMRFDetail.MasterTableView.GetItems(GridItemType.CommandItem)[0].FindControl("uxAddNewItem"));
            LinkButton lnkbtnuxInsertAddNewItem = (LinkButton)(uxMRFDetail.MasterTableView.GetItems(GridItemType.CommandItem)[0].FindControl("uxInsertAddNewItem"));
            //btnuxAddNewItem.Visible = false;
            //lnkbtnuxInsertAddNewItem.Visible = false;
            string closeurl = HttpContext.Current.Server.UrlDecode(Request.RawUrl);
            string refreshpage = Request.ApplicationPath;
            refreshpage = refreshpage + "/Pages/MRFMaster.aspx";
            closeurl = closeurl.Replace("/aspx", "*");
            closeurl = ".." + closeurl.Split('*')[1];// ("/astorWorkWeb/aspx", "..");
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "function", "urltadclose('" + closeurl + "','" + refreshpage + "')", true);
            //Master.DisplayAlertMessage("MRF Generated successfully..... ");
        }
        protected void uxRadWizard_NavigationBarButtonClick(object sender, WizardEventArgs e)
        {
            RadWizard uxRadWizard = (RadWizard)sender;
            string strsavupd = string.Empty;
            Page.Validate("vgMRFMaster");
            if (Page.IsValid)
            {
                uxRadWizard.ActiveStepIndex = e.NextStepIndex;
            }
            else
            {
                uxRadWizard.ActiveStepIndex = e.CurrentStepIndex;
            }
        }


        #endregion
        
        protected void Location_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var MRFNo = MRFMasters.Where(w => w.Block == uxBlock.SelectedValue && w.Level == uxLevel.SelectedValue && w.Zone==uxZone.SelectedValue && w.MRFNo != uxMRFNo.Text && uxMaterialType.CheckedItems.Select(s => s.Text).Contains(w.MaterialType)).FirstOrDefault();
            if (MRFNo != null)
            {
                Master.DisplayAlertMessage(string.Format("Location already exists in MRF No: {0}.", MRFNo.MRFNo), true);
                ((RadComboBox)sender).SelectedValue = "";
                ((RadComboBox)sender).Text = "";
            }
        }
    }
}