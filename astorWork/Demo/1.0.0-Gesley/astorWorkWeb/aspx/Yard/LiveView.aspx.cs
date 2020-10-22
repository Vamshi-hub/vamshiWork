using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;
using Telerik.Web.UI;
using Telerik.Web.UI.Map;

namespace astorWork.aspx.Yard
{
    public partial class LiveView : System.Web.UI.Page
    {

        #region  Declarations
        ZoneDAO objZoneDAO = null;
        YardDAO objYardDAO = null;
        MaterialDAO objMaterialDAO = null;
        AstorSafeMappingDAO objAstorSafe = null;
        #endregion

        #region PageEvents
        protected void Page_Load(object sender, EventArgs e)
        {
            if (objYardDAO == null)
                objYardDAO = new YardDAO();
            if (objZoneDAO == null)
                objZoneDAO = new ZoneDAO();
            if (objAstorSafe == null)
                objAstorSafe = new AstorSafeMappingDAO();
            if (objMaterialDAO == null)
                objMaterialDAO = new MaterialDAO();

            if (!IsPostBack)
            {
                hdnRefreshDuration.Value = "60";
                rmapLiveView.LayersCollection[0].Key = System.Configuration.ConfigurationManager.AppSettings["BingMapKey"];
            }
            GetMapData();
        }
        #endregion

        #region Methods
        private void GetMapData()
        {
            try
            {
                List<YardMaster> dtLiveData = objYardDAO.GetAllYards();
                DataView view = new DataView(ToDataTable(dtLiveData));
                DataTable distinctValues = view.ToTable(true, "Location", "YardID", "GPSCoordinates");

                rmapLiveView.DataSource = GetData(distinctValues);
                rmapLiveView.DataBind();
            }

            catch (Exception ex)
            {

            }
        }

        private DataTable GetData(DataTable distintValues)
        {
            DataSet ds = new DataSet("TelerikOffices");
            DataTable dt = new DataTable("TelerikOfficesTable");
            dt.Columns.Add("YardID", typeof(string));
            dt.Columns.Add("LocationName", typeof(string));
            dt.Columns.Add("Coordinates", typeof(string));
            dt.Columns.Add("LocationID", typeof(string));
            dt.Columns.Add("Latitude", typeof(decimal));
            dt.Columns.Add("Longitude", typeof(decimal));
            dt.Columns.Add("EmployeeCount", typeof(int));
            dt.Columns.Add("MaterialCount", typeof(int));

            // Hard-code for material tracking
            /*
            var listMaterials = objMaterialDAO.GetActiveMaterials();
            int countProduced = 0;
            int countDelivered = 0;
            int countInstalled = 0;

            foreach(var material in listMaterials)
            {
                
                switch (material.Status)
                {
                    case "Produced":
                        countProduced++;
                        break;
                    case "Delivered":
                        countDelivered++;
                        break;
                    case "Installed":
                        countInstalled++;
                        break;
                    default:
                        break;
                }
            }
            */
            List<MaterialCountModel> listMaterialCount = objMaterialDAO.GetActiveMaterialCount();

            foreach (DataRow row in distintValues.Rows)
            {
                int countMaterials = 0;
                List<CurrentZoneEmployees> lstCurrentYardEmployees = new List<CurrentZoneEmployees>();
                var varAllZones = objZoneDAO.GetAstorYardDetails(Convert.ToInt32(row["YardID"].ToString()));
                foreach (var Zone in varAllZones)
                {

                    List<CurrentZoneEmployees> lstCurrentZoneEmployees = objAstorSafe.GetCurrentZoneEmployees(Zone.ZoneID, string.Empty, string.Empty, string.Empty);
                    lstCurrentYardEmployees.AddRange(lstCurrentZoneEmployees);

                    ZoneMapping objZoneMapping = objZoneDAO.GetMappingZoneID(Convert.ToInt16(Zone.ZoneID), 1);
                    if (objZoneMapping != null)
                    {
                        MaterialCountModel objMap = listMaterialCount.Where(x => x.ZoneID == objZoneMapping.AstorTrackZoneID).FirstOrDefault();
                        if (objMap != null)
                        {
                            countMaterials += objMap.MaterialCount;
                        }
                    }
                }

                dt.Rows.Add(row["YardID"].ToString(), row["Location"], row["GPSCoordinates"], row["YardID"].ToString(), decimal.Parse(row["GPSCoordinates"].ToString().Split(',')[0]), decimal.Parse(row["GPSCoordinates"].ToString().Split(',')[1]), lstCurrentYardEmployees.Count(), countMaterials);
            }
            Session["LIVEEMPLOYEE"] = dt;
            return dt;
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        #endregion

        #region Grid&MapEvents
        protected void RadMap_ItemDataBound(object sender, MapItemDataBoundEventArgs e)
        {
            MapMarker marker = e.Item as MapMarker;
            if (marker != null)
            {
                DataTable dtLiveData = (DataTable)(Session["LIVEEMPLOYEE"]);
                DataRowView item = e.DataItem as DataRowView;
                string YardID = item.Row["LocationID"] as string;
                string Coordinates = item.Row["Coordinates"] as string;
                DataRow[] drows = dtLiveData.Select("YardID='" + YardID + "'");
                StringBuilder sb = new StringBuilder();
                sb.Append("<div class='rightCol'><div class='country'><span style='color:#2ccef5;font-size:12px;font-weight:bold;'>" + item.Row["LocationName"] + "</span></div><div class='city'>Employee Count <img style='border:0;width:13px;height:13px;' src=\"../../Images/Page_Icon.png\" /><a onclick=\"addTab('Site View (Employee)','../Yard/Pivot.aspx?type=0&YardID=" + YardID + "');\"><span style='color:LightBlue;font-size:15px;font-weight:bold;text-decoration: underline;cursor:pointer;'>" + item.Row["EmployeeCount"] + "</span></a></div><div class='city'>Material Count <img style='border:0;width:13px;height:13px;' src=\"../../Images/Page_Icon.png\" /><a onclick=\"addTab('Site View (Material)','../Yard/Pivot.aspx?type=1&YardID=" + YardID + "');\"><span style='color:LightBlue;font-size:15px;font-weight:bold;text-decoration: underline;cursor:pointer;'>" + item.Row["MaterialCount"] + "</span></a></div></div>");
                marker.TooltipSettings.Content = sb.ToString();
                marker.TooltipSettings.AutoHide = false;
            }
        }

        #endregion

    }
}