using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;
using Telerik.Web.UI;

namespace astorWork.aspx.Configuration
{
    public partial class ViewZoneMaster : System.Web.UI.Page
    {
        #region  Declarations
        ZoneDAO objZoneDAO = null;
        YardDAO objYardDAO = null;
        #endregion

        #region Page
        protected void Page_Load(object sender, EventArgs e)
        {
            if (objZoneDAO == null)
                objZoneDAO = new ZoneDAO();
            if (objYardDAO == null)
                objYardDAO = new YardDAO();

            if (!IsPostBack)
            {
                rgZone.PageSize = 10;//objCommon.objUserProfile.UserPreference.PageSize;
                rgZone.Visible = false;
                rvYard.InitialValue = "-Select-";
                LoadDropdowns();
                if (Request.QueryString["Operation"] != null)
                {
                    hdnOperation.Value = Request.QueryString["Operation"];
                    rbtnRefreshGrid_Click(this, null);
                }
            }
            RadWin_ZonePreview.VisibleOnPageLoad = false;

        }
        #endregion
        #region Private Methods

        private void LoadDropdowns()
        {
            try
            {
                rddlYard.Items.Clear();
                rddlYard.AppendDataBoundItems = true;
                rddlYard.Items.Add(new DropDownListItem("-Select-", "-1"));
                rddlYard.DataTextField = "YardName";
                rddlYard.DataValueField = "YardID";
                var lstYards = (from yard in objYardDAO.GetAllYards()
                                select new
                                {
                                    YardName = string.Format("{0} ({1}{2})", yard.YardName, yard.UTCOffset < 0 ? "-" : "+", TimeSpan.FromMinutes(yard.UTCOffset).ToString(@"hh\:mm")),
                                    yard.YardID,
                                    yard.UTCOffset,
                                    yard.IsDefault
                                }).ToList();
                rddlYard.DataSource = lstYards;
                rddlYard.DataBind();

                if (lstYards == null || lstYards.Count == 0)
                    return;

                if (lstYards.Count == 1)
                {
                    rddlYard.SelectedValue = lstYards.FirstOrDefault().YardID.ToString();

                    rddlYard.Enabled = false;
                    rvYard.Enabled = false;
                    rgZone.Visible = true;
                    rgZone.Rebind();
                }
                else
                {
                    int intYardID = lstYards.Where(YM => YM.IsDefault == true).FirstOrDefault().YardID;
                    rddlYard.SelectedValue = intYardID.ToString();
                    rddlYard.Enabled = true;
                    rvYard.Enabled = true;
                    rgZone.Visible = true;
                    rgZone.Rebind();
                }
            }
            catch (Exception ex)
            {
                //  Master.DisplayAlertMessage(Messages.LoadingError, true, ex);
            }
        }

        #endregion
        #region Grid Methods
        protected void rgZone_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            try
            {

                rgZone.DataSource = objZoneDAO.GetAllZonesByYardID(Convert.ToInt32(rddlYard.SelectedValue));

            }
            catch (Exception ex)
            {

                Master.DisplayAlertMessage("Error in rgZone_NeedDataSource", true, ex, true);
            }

        }
        #endregion

        #region Events

        protected void rbAddZone_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddZoneMaster.aspx?ZoneID=&YardID=" + rddlYard.SelectedValue + "&Edit=0&ID=0");
        }

        protected void rddlYard_SelectedIndexChanged(object sender, DropDownListEventArgs e)
        {
            rgZone.Visible = false;
            if (rddlYard.SelectedValue != "-1")
            {
                rgZone.Visible = true;
                rgZone.CurrentPageIndex = 0;
                rgZone.Rebind();

            }
        }
        protected void rbtnRefreshGrid_Click(object sender, EventArgs e)
        {
            rgZone.Rebind();
            Master.DisplayAlertMessage((Enums.Operation)Enum.Parse(typeof(Enums.Operation), hdnOperation.Value), "Zone");
        }
        #endregion

     
    }
}