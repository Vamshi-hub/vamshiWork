using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
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

namespace astorWork
{
    public partial class MRFDetail : System.Web.UI.Page
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

        //List<MRFDetail> lstMRFDetail; 

        protected void Page_Load(object sender, EventArgs e)
        {
            api.Endpoint = ConfigurationManager.AppSettings["astorTrackEndpoint"];
            if (!IsPostBack)
            {
                api.Endpoint = ConfigurationManager.AppSettings["astorTrackEndpoint"];
                //if (objCommon == null)
                //    objCommon = new CommonGeneral();

                string MRFNo = Request.QueryString["MRFNo"];
                if (!string.IsNullOrEmpty(MRFNo))
                {
                    _MRFNo = MRFNo.ToString();
                    _model = api.GetMRFMaster(MRFNo);
                    var MRFMasters = _model.MRFMasters.First();
                    ulMRFNo.Text = MRFMasters.MRFNo;
                    ulMRFDate.Text = MRFMasters.MRFDate.ToString();
                    ulStatus.Text = MRFMasters.Status;
                    ulProject.Text = MRFMasters.Project;
                    ulVendor.Text = MRFMasters.Vendor;
                    ulOfficerInCharge.Text = MRFMasters.Attention;
                }
            }

        }
        #endregion

        #region Grid Events

        protected void uxMRFDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(ulMRFNo.Text))
                {
                    var _modelMRFmodel = api.GetMRFMaster(ulMRFNo.Text);
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

        #endregion
    }
}