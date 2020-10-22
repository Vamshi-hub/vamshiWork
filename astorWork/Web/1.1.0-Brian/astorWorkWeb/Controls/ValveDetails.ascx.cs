using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;

namespace astorWork.Controls
{
    public partial class ValveDetails : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["ValveID"] != null)
            {
                GetValveDetails(int.Parse(Session["ValveID"].ToString()));
                rgTransactions.Rebind();
            }
        }
        private void GetValveDetails(int ValveID)
        {
            ValveDAO objValveDao = new ValveDAO();
            ValveMaster objValveMaster = objValveDao.getValveDetails(ValveID);
            if (objValveMaster != null)
            {
                rbImgValve.DataValue = objValveMaster.ValveImage;
                lblDesc.Text = objValveMaster.ValveDescription;
                lblStatus.Text = ((astorWork.Enums.ValveStatus)Enum.Parse(typeof(astorWork.Enums.ValveStatus), objValveMaster.Status.ToString())).ToString();
                lblType.Text = ((astorWork.Enums.ValveTypes)Enum.Parse(typeof(astorWork.Enums.ValveTypes), objValveMaster.ValveTypeID.ToString())).ToString();
                lblVName.Text = objValveMaster.ValveName;
            }
        }

        protected void rgTransactions_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            ValveDAO objValveDao = new ValveDAO();
            List<ValveEventMaster> objEventList = objValveDao.getTopEvents(int.Parse(Session["ValveID"].ToString()));
            rgTransactions.DataSource = objEventList;
            rgTransactions.ClientSettings.Scrolling.AllowScroll = objEventList.Count() > 5;
        }
    }
}