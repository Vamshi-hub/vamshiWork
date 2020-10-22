using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;
using Telerik.Web.UI;
using System.Drawing;
namespace astorWork.aspx.Configuration
{
    public partial class EvenDetails : System.Web.UI.UserControl
    {
        ValveDAO objValveDAO = null;
        int intZoneID;
        string strZoneName;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (objValveDAO==null)
                objValveDAO = new ValveDAO();
            string strZoneDetails = Session["TTMZoneID"].ToString();
            intZoneID = Convert.ToInt32(strZoneDetails.Split(',')[0]);
            strZoneName = strZoneDetails.Split(',')[1];
           rgEmployee.Rebind();

        }

        protected void rgEmployee_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
           
        }

        protected void rgEmployee_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            var ValMaster = objValveDAO.getValveMaster().Where(m => m.ZoneID == intZoneID).OrderBy(m => m.Status).ThenByDescending(m => m.ModifiedDate);
            lbZoneName.Text = strZoneName;
            rgEmployee.DataSource = (from m in ValMaster
                            select new
                            {
                                m.ValveImage,
                                m.ValveDescription,
                                m.Status,
                                ValveType = m.ValveTypeID == 0 ? "QA" : "Production",
                                Capturedate= m.ModifiedDate
                            });
            rgEmployee.ClientSettings.Scrolling.AllowScroll = ValMaster.Count() > 6;
           ;
        }

        protected void rgEmployee_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {

                GridDataItem dataBoundItem = e.Item as GridDataItem;

                int intSatus = Convert.ToInt32(dataBoundItem.GetDataKeyValue("Status").ToString());
                if (intSatus == 0)
                {
                    dataBoundItem["Image"].BackColor = Color.Red;
                    dataBoundItem["ValveDescription"].BackColor = Color.Red;
                    dataBoundItem["ValveType"].BackColor = Color.Red;
                    dataBoundItem["Capturedate"].BackColor = Color.Red;
                }
            }
        }
    }
}