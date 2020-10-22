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
    public partial class ActiveYardEmployees : System.Web.UI.Page
    {
        #region  Declarations
        ZoneDAO objZoneDAO = null;
        YardDAO objYardDAO = null;
        AstorSafeMappingDAO objAstorSafe = null;
        #endregion

        #region Page
        protected void Page_Load(object sender, EventArgs e)
        {
            if (objZoneDAO == null)
                objZoneDAO = new ZoneDAO();
            if (objYardDAO == null)
                objYardDAO = new YardDAO();
            if (objAstorSafe == null)
                objAstorSafe = new AstorSafeMappingDAO();

            if (!IsPostBack)
            {
                rgActiveEmployees.PageSize = 10;
                rgActiveEmployees.Visible = false;
                rvYard.InitialValue = "-Select-";
                LoadDropdowns();

            }

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
                    rgActiveEmployees.Visible = true;
                    rgActiveEmployees.Rebind();
                }
                else
                {
                    int intYardID = lstYards.Where(YM => YM.IsDefault == true).FirstOrDefault().YardID;
                    rddlYard.SelectedValue = intYardID.ToString();
                    rddlYard.Enabled = true;
                    rvYard.Enabled = true;
                    rgActiveEmployees.Visible = true;
                    rgActiveEmployees.Rebind();
                }
            }
            catch (Exception ex)
            {
                //  Master.DisplayAlertMessage(Messages.LoadingError, true, ex);
            }
        }

        #endregion
        #region Grid Methods
        protected void rgActiveEmployees_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            List<CurrentZoneEmployees> lstCurrentYardEmployees =new List<CurrentZoneEmployees>();
            try
            {
                var varAllZones = objZoneDAO.GetAstorYardDetails(Convert.ToInt32(rddlYard.SelectedValue));
                foreach (var Zone in varAllZones)
                {
                    List<CurrentZoneEmployees> lstCurrentZoneEmployees = objAstorSafe.GetCurrentZoneEmployees(Zone.ZoneID, string.Empty, string.Empty, string.Empty);
                    lstCurrentYardEmployees.AddRange(lstCurrentZoneEmployees);
                }
                rgActiveEmployees.DataSource = lstCurrentYardEmployees;
            }
            catch (Exception ex)
            {

                //    Master.DisplayAlertMessage("Error in rgActiveEmployees_NeedDataSource", true, ex, true);
            }

        }
        #endregion

        #region Events

        protected void rddlYard_SelectedIndexChanged(object sender, DropDownListEventArgs e)
        {
            rgActiveEmployees.Visible = false;
            if (rddlYard.SelectedValue != "-1")
            {
                rgActiveEmployees.Visible = true;
                rgActiveEmployees.CurrentPageIndex = 0;
                rgActiveEmployees.Rebind();

            }
        }

        #endregion
    }
}