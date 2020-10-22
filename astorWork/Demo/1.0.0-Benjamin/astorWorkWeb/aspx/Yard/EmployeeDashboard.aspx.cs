using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Charting;

namespace astorWork
{
    public partial class EmployeeDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //    CreateChart();
        }
        protected void Page_PreInit(object sender, EventArgs e)

        {
            Page.Title = "Employee Dashboard";

        }
        //private void CreateChart()
        //{
        //    JTCDemoDAO.DAO objDAO = new JTCDemoDAO.DAO();
        //    List<LocationEntity> objList = objDAO.getASR_AFR_Line();

          

        //    scatterChart.Legend.Appearance.Position = Telerik.Web.UI.HtmlChart.ChartLegendPosition.Bottom;

        //    scatterChart.PlotArea.XAxis.TitleAppearance.Text = "Division";
        //    scatterChart.PlotArea.YAxis.TitleAppearance.Text = "AFR/ASR";


        //    scatterChart.DataSource = objList;
        //    scatterChart.DataBind();

        //}
    }
}