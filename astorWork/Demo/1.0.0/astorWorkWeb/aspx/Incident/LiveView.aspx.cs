using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Telerik.Web.UI.Map;
using astorWorkDAO;

namespace astorWork
{
    public partial class LiveView : System.Web.UI.Page
    {
        #region Declarations

        // DataTable dtLiveData;
        List<LocationEntity> lstLocations;
        const string LIVEEMPLOYEE = "LiveEmp";
        #endregion

        #region PageEvents
        protected void Page_Load(object sender, EventArgs e)
        {
            rwDetails.VisibleOnPageLoad = false;
            if ((!Page.IsPostBack) || Session[LIVEEMPLOYEE] == null)
            {
                MapLayer mapLayer = GetMapLayer(true);

                rmapLiveView.LayersCollection.Clear();
                rmapLiveView.LayersCollection.Add(mapLayer);
                hdnRefreshDuration.Value = "30";
                GetMapData(true);
            }
        }


        #endregion

        #region Events
        protected void cbkPresent_CheckedChanged(object sender, EventArgs e)
        {

            //dtLiveData = (DataTable)Session[LIVEEMPLOYEE];
            //DataView view = new DataView(dtLiveData);
            //view.RowFilter = cbkPresent.Checked ? "EmployeeKey<>0" : "";
            //DataTable distinctValues = view.ToTable(true, "LocationID", "GPSCoordinates", "LocationName");
            //rcbLocation.DataSource = distinctValues;
            //rcbLocation.DataValueField = "LocationID";
            //rcbLocation.DataTextField = "LocationName";
            //rcbLocation.DataBind();
            //ScriptManager.RegisterStartupScript(this, GetType(), "openRad", "openRadpanelBarCheck();", true);
        }
        protected void RadMap_ItemDataBound(object sender, MapItemDataBoundEventArgs e)
        {
            MapMarker marker = e.Item as MapMarker;
            if (marker != null)
            {
                DataRowView item = e.DataItem as DataRowView;
                marker.Shape = item.Row["color"].ToString();
                StringBuilder sb = new StringBuilder();
                //sb.Append("<div class='markers'><div class='title' style='text-transform:none !important'><a href=\"#\" style='color:black;font-size:10pt;"
                //    + "font-weight:bold;text-decoration: none;'" + "onclick =\"fireServerButtonEvent('" + item.Row["SNO"].ToString() + "','" + (int.Parse(item.Row["Incident_Accumulated"].ToString()) > 1 ? "red" : (int.Parse(item.Row["Incident_Accumulated"].ToString()) > 0 ? "yellow" : "green")) + "')\"" + ">"
                //    + (item.Row["Project"] + "<br />")
                //    + "<table style='width:100%'> <tr> <td> <img style = 'border:0;width:16px;height:16px;' src =\"Images/User.png\" /></td><td><span style='color:black;font-size:14pt;font-weight:bold;'> "
                //    + (item.Row["Workers_Site"].ToString() == "" ? "0" : item.Row["Workers_Site"].ToString())
                //    + "</span></td>"
                //    + "<td rowspan='3'> <span style='font-size:8pt'>Contractor</span><br />" + item.Row["Contractor"]
                //    + " </td></tr><tr><td><img style = 'border:0;width:16px;height:16px;' src =" + (item.Row["color"].ToString().Contains("green") ? "\"Images/CheckMark.png\"" : (item.Row["color"].ToString().Contains("red") ? "\"Images/danger.png\"" : "\"Images/warn.png\"")) + " /></td><td> <span style='color:black;font-size:14pt;font-weight:bold;'>"
                //    + item.Row["Incident_Accumulated"].ToString()
                //    + "</span></td></tr></table>");

                sb.Append("<div class='markers'><div class='title' style='text-transform:none !important;text-align:left;'><a href=\"#\" style='color:black;font-size:10pt;"
                   + "font-weight:bold;text-decoration: none;'" + "onclick =\"fireServerButtonEvent('" + item.Row["SNO"].ToString() + "','" + (int.Parse(item.Row["Incident_Accumulated"].ToString()) > 1 ? "red" : (int.Parse(item.Row["Incident_Accumulated"].ToString()) > 0 ? "yellow" : "green")) + "')\"" + "><span style='text-transform:uppercase;font-size:12pt;'>"
                   + (item.Row["Project"] + "</span><br />")
                   + "<table style='width:100%'> <tr><td colspan='4'>" + item.Row["Contractor"]
                   + "</td> </tr><tr><td style='width:18px'> <img style = 'border:0;width:16px;height:16px;' src =\"../../Images/User.png\" /></td><td style='text-align: left;'><span style='color:black;font-size:14pt;font-weight:bold;'> "
                   + (item.Row["Workers_Site"].ToString() == "" ? "0" : item.Row["Workers_Site"].ToString())
                   + "</span></td>"
                   + "<td style='width:18px'><img style = 'border:0;width:16px;height:16px;' src =" + (item.Row["color"].ToString().Contains("green") ? "\"../../Images/CheckMark.png\"" : (item.Row["color"].ToString().Contains("red") ? "\"../../Images/danger.png\"" : "\"../../Images/warn.png\""))
                   + " /></td><td style='text-align: left;'> <span style='color:black;font-size:14pt;font-weight:bold;'>"
                   + item.Row["Incident_Accumulated"].ToString()
                   + "</span></td></tr></table>");
                sb.Append("</div>");
                marker.TooltipSettings.Content = sb.ToString();
                marker.TooltipSettings.AutoHide = false;

            }
        }
        protected void btnReload_Click(object sender, EventArgs e)
        {
            GetMapData(true);
        }
        protected void rbtnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                GetMapData(false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected void rbtnReset_Click(object sender, EventArgs e)
        {
            hdnZoom.Value = string.Empty;
            rcbDivisions.ClearCheckedItems();
            rcbProjects.ClearCheckedItems();
            rbtnFilter_Click(this, null);
        }

        protected void lbRight_Click(object sender, EventArgs e)
        {
            List<LocationEntity> lstentity = (List<LocationEntity>)Session[LIVEEMPLOYEE];
            LocationEntity lEntity = lstentity.Where(x => x.SNO == int.Parse(hdnKey.Value)).FirstOrDefault();
            if (lEntity != null)
            {
                switch (hdnColor.Value)
                {
                    case "green":
                        li4.Visible = false;
                        break;
                    case "red":
                        // lblblSevirity.Text = "Critical";
                        // lblblSevirity.ForeColor = Color.Red;
                        li4.Visible = true;
                        hdnPDF.Value = "https://astortimeqa.blob.core.windows.net/jtc/ReportableIncident.pdf";
                        hdnVideo.Value = "https://astortimeqa.blob.core.windows.net/jtc/samplevideo.mp4";
                        break;
                    case "yellow":
                        //  lblblSevirity.Text = "Medium";
                        // lblblSevirity.ForeColor = Color.Yellow;
                        li4.Visible = true;
                        hdnPDF.Value = "https://astortimeqa.blob.core.windows.net/jtc/NonReportableIncident.pdf";
                        hdnVideo.Value = "https://astoria.blob.core.windows.net/incidentvideo/WorkingSite.mp4";
                        break;

                }

                lblProjectName.Text = lEntity.Project;
                lblProject.Text = lEntity.ProjectDesc;
                lblDivision.Text = lEntity.Division;
                lblProjType.Text = lEntity.ProjectType;
                lblContractor.Text = lEntity.Contractor;
                lblAFR.Text = lEntity.AFR.ToString();
                lblASR.Text = lEntity.ASR.ToString();
                lblFAR.Text = lEntity.FAR.ToString();
                lbIncident.Text = lEntity.Incident_Accumulated.ToString();
                lbLastInspection.Text = lEntity.LastInspectionDate.ToString();
                lbNearMiss.Text = lEntity.NearMiss;

                lblAFR.ForeColor = lEntity.AFR > 1.7 ? Color.Red : (lEntity.AFR > 1.6 ? Color.Orange : Color.Green);
                lblASR.ForeColor = lEntity.ASR > 159 ? Color.Red : (lEntity.ASR > 85 ? Color.Orange : Color.Green);
                lblFAR.ForeColor = lEntity.FAR > 4.9 ? Color.Red : (lEntity.FAR > 1.9 ? Color.Orange : Color.Green);
                ScriptManager.RegisterStartupScript(this, GetType(), "openRad", "openRadpanelBarCheck();", true);
            }
        }
        protected void btnMap_Click(object sender, EventArgs e)
        {

            Button btn = (Button)sender;
            bool IsSateillite = Convert.ToBoolean(btn.CommandArgument);
            MapLayer mapLayer = GetMapLayer(IsSateillite);
            if (IsSateillite)
            {
                btnMap.Style["font-weight"] = "normal";
                btnSattilite.Style["font-weight"] = "bold";
            }
            else
            {
                btnMap.Style["font-weight"] = "bold";
                btnSattilite.Style["font-weight"] = "normal";
            }
            rmapLiveView.LayersCollection.Clear();
            rmapLiveView.LayersCollection.Add(mapLayer);
        }
        protected void rcbDesignation_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            IEnumerable<string> allChecked = rcbDivisions.CheckedItems.Select(k => k.Value);

            List<LocationEntity> lstentity = (List<LocationEntity>)Session[LIVEEMPLOYEE];
            if (allChecked.Count() > 0)
            {
                var SelectedProject = lstentity.Where(x => allChecked.Contains(x.Division));
                rcbProjects.DataSource = SelectedProject;
            }
            else
                rcbProjects.DataSource = lstentity;
            rcbProjects.DataValueField = "Project";
            rcbProjects.DataTextField = "Project";
            rcbProjects.DataBind();
            ScriptManager.RegisterStartupScript(this, GetType(), "openRad1", "openRadpanelBar();", true);
        }

        protected void lbIncident_Click(object sender, EventArgs e)
        {
            try
            {
                LinkButton lbSource = (LinkButton)sender;
                List<LocationEntity> lstentity = (List<LocationEntity>)Session[LIVEEMPLOYEE];
                LocationEntity lEntity = lstentity.Where(x => x.SNO == int.Parse(hdnKey.Value)).FirstOrDefault();
                if (lEntity != null)
                {
                    if (lbSource.CommandArgument == "Incident")
                    {
                        rwDetails.Title = "Incident Detail History";
                        rdGrid.MasterTableView.GetColumn("col1").HeaderText = "Incident";
                        rdGrid.MasterTableView.GetColumn("col2").HeaderText = "Incident Date";
                        rdGrid.MasterTableView.GetColumn("col3").Visible = true;
                        string fileName = Path.Combine(Server.MapPath("~/Video"), "NVR2.mp4");
                        fileName = Page.ResolveUrl(fileName);
                        //string myurl = "http://" + Request.Url.Authority;
                        //for (int i = 0; i < Request.Url.Segments.Length - 1; i++)
                        //{
                        //    myurl = myurl + Request.Url.Segments[i];
                        //}

                        if (lEntity.Incident_Accumulated > 1)
                        {
                           // myurl = myurl + "Video/NVR1.mp4";
                             hdnVideo.Value = "https://astoria.blob.core.windows.net/incidentvideo/WorkingSite.mp4"; ;
                        }
                        else
                        {

                            // myurl = myurl + "Video/NVR2.mp4";
                            hdnVideo.Value = "https://astoria.blob.core.windows.net/incidentvideo/WorkingSite.mp4";
                        }
                    }
                    else
                    {
                        hdnVideo.Value = "https://astoria.blob.core.windows.net/incidentvideo/WorkingSite.mp4";
                        rwDetails.Title = "Inspection Detail History";
                        rdGrid.MasterTableView.GetColumn("col1").HeaderText = "Report No";
                        rdGrid.MasterTableView.GetColumn("col2").HeaderText = "Inspection Date";
                        rdGrid.MasterTableView.GetColumn("col3").Visible = false;
                    }
                    IncidentDAO objDAO = new IncidentDAO();
                    DataTable dt;
                    if (lbSource.CommandArgument == "Incident")
                        dt = objDAO.getIncidentDetails(lblProjectName.Text, lblDivision.Text, lblContractor.Text, lbSource.CommandArgument);
                    else
                        dt = objDAO.getInspectionDetails(lblProjectName.Text, lblDivision.Text, lblContractor.Text);
                    rdGrid.DataSource = dt;
                    rdGrid.DataBind();
                    rwDetails.VisibleOnPageLoad = true;


                }
            }
            catch (Exception exc)
            {

                throw;
            }

        }
        #endregion

        #region PrivateMethods
        /// <summary>
        /// Fill DataTable based on Distinct values
        /// </summary>
        /// <param name="distintValues"></param>
        /// <returns></returns>

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
               TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }
        /// <summary>
        /// Bind RadMap
        /// </summary>
        private void GetMapData(bool IsServer)
        {
            try
            {
                lblProjectCount.Text = "0";
                IncidentDAO objDAO = new IncidentDAO();
                if (IsServer)
                {
                    lstLocations = objDAO.getLocations();
                    Session[LIVEEMPLOYEE] = lstLocations;
                }
                else
                {
                    List<LocationEntity> lstValues = (List<LocationEntity>)Session[LIVEEMPLOYEE];
                    var selectedDivisions = rcbDivisions
                                   .Items.Where(i => i.Checked)
                                   .Select(i => i.Value)
                                   .ToArray();

                    var selectedProjects = rcbProjects
                                    .Items.Where(i => i.Checked)
                                    .Select(i => i.Value)
                                    .ToArray();
                    if (selectedDivisions.Length > 0 || selectedProjects.Length > 0)
                        lstLocations = lstValues.Where(x => (selectedDivisions.Length == 0 || selectedDivisions.Contains(x.Division)) && (selectedProjects.Length == 0 || selectedProjects.Contains(x.Project))).ToList();
                    else
                        lstLocations = lstValues;
                }

                var distinctLocations = lstLocations.Select(e => new { Lattitude = e.Coordinates.Split(',')[0], Logitude = e.Coordinates.Split(',')[1], Contractor = e.Contractor, Workers_Site = e.Workers_Site, Incident_Accumulated = e.Incident_Accumulated, color = (e.Incident_Accumulated > 1 ? "redshape" : e.Incident_Accumulated > 0 ? "yellowshape" : "greenshape"), MOM = int.Parse(string.IsNullOrEmpty(e.MOM_Reportable_Accident) ? "0" : e.MOM_Reportable_Accident), NonMOM = int.Parse(string.IsNullOrEmpty(e.NonMom_Reportable_Incident) ? "0" : e.NonMom_Reportable_Incident), SNO = e.SNO, Project = e.Project }).Distinct();
                rmapLiveView.DataSource = ConvertToDataTable(distinctLocations.ToList());
                rmapLiveView.DataBind();
                if (distinctLocations.Count() > 0)
                {
                    lblProjectCount.Text = distinctLocations.Count().ToString();
                    div1.Visible = true;
                    var lstGeo = distinctLocations.Select(e => new GeocodedAddress() { Latitude = double.Parse(e.Lattitude), Longitude = double.Parse(e.Logitude) }).ToList();
                    setMap(lstGeo);
                    if (IsServer)
                    {
                        var Divisions = lstLocations.Select(e => new { Division = e.Division }).Distinct();
                        var Projects = lstLocations.Select(e => new { Project = e.Project }).Distinct();
                        rcbDivisions.DataSource = Divisions;
                        rcbDivisions.DataValueField = "Division";
                        rcbDivisions.DataTextField = "Division";
                        rcbDivisions.DataBind();

                        rcbProjects.DataSource = Projects;
                        rcbProjects.DataValueField = "Project";
                        rcbProjects.DataTextField = "Project";
                        rcbProjects.DataBind();
                    }
                }
                else if (IsServer)
                {

                    div1.Visible = false;
                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        private MapLayer GetMapLayer(bool Satellite)
        {


            MapLayer mapLayer = new MapLayer();

            mapLayer.Type = Telerik.Web.UI.Map.LayerType.Bing;
            mapLayer.Key = ConfigurationManager.AppSettings["BingMapKey"].ToString();

            if (!Satellite)
            {
                //mapLayer.Type = Telerik.Web.UI.Map.LayerType.Tile;
                // mapLayer.UrlTemplate = provider;
                mapLayer.ImagerySet = "Road";

            }
            else
            {
                mapLayer.ImagerySet = "aerialWithLabels";

                // mapLayer.Type = Telerik.Web.UI.Map.LayerType.Bing;
                //mapLayer.Key = ConfigurationManager.AppSettings["BingMapKey"].ToString();
            }

            return mapLayer;
        }

        private double Degree2Radian(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }

        private double Radian2Degree(double radians)
        {
            double degrees = radians * (180 / Math.PI);
            return (degrees);
        }
        private void setMap(List<GeocodedAddress> MappedNames)
        {
            double paddingFactor = 1.2;
            double minY = MappedNames.Min(x => x.Latitude);
            double maxY = MappedNames.Max(x => x.Latitude);
            double minX = MappedNames.Min(x => x.Longitude);
            double maxX = MappedNames.Max(x => x.Longitude);
            double centerY = (minY + maxY) / 2;
            double centerX = (minX + maxX) / 2;

            double minRadianY = Math.Log((Math.Sin(Degree2Radian(minY)) + 1) / Math.Cos(Degree2Radian(minY)));
            double maxRadianY = Math.Log((Math.Sin(Degree2Radian(maxY)) + 1) / Math.Cos(Degree2Radian(maxY)));
            double centerRadianY = (minRadianY + maxRadianY) / 2;

            centerY = Radian2Degree(Math.Atan(Math.Sinh(centerRadianY)));


            double deltaX = maxX - minX;
            double resolutionX = deltaX / rmapLiveView.Width.Value;


            double vy0 = Math.Log(Math.Tan(Math.PI * (0.25 + centerY / 360)));
            double vy1 = Math.Log(Math.Tan(Math.PI * (0.25 + maxY / 360)));
            double viewHeightHalf = rmapLiveView.Height.Value / 2;
            double zoomFactorPowered = viewHeightHalf / (40.7436654315252 * (vy1 - vy0));
            double resolutionY = 360.0 / (zoomFactorPowered * 256);


            double resolution = Math.Max(resolutionX, resolutionY) * paddingFactor;
            double zoom = Math.Log(360 / (resolution * 256), 2);

            hdnCenterLat.Value = centerY.ToString();
            hdnCenterLong.Value = centerX.ToString();
            // rmapLiveView.Zoom = Math.Floor(zoom);
            rmapLiveView.CenterSettings.Latitude = centerY;
            rmapLiveView.CenterSettings.Longitude = centerX;
        }


        #endregion



        internal class GeocodedAddress
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }


    }
}