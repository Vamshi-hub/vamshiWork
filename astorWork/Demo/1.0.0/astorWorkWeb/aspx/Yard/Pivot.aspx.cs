using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;
using System.Drawing;
using System.Xml;
using Telerik.Web.UI;
using System.IO;
using System.Web.Services;
using System.Text;

namespace astorWork.aspx.Yard
{
    public partial class Pivot : System.Web.UI.Page
    {
        ZoneDAO objZoneDAO = null;
        YardDAO objYardDAO = null;
        MaterialDAO objMaterialDAO = null;
        AstorSafeMappingDAO objAstorDAO = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (objZoneDAO == null)
                objZoneDAO = new ZoneDAO();
            if (objYardDAO == null)
                objYardDAO = new YardDAO();
            if (objAstorDAO == null)
                objAstorDAO = new AstorSafeMappingDAO();
            if (objMaterialDAO == null)
                objMaterialDAO = new MaterialDAO();

            int viewType = getViewType();

            if (!IsPostBack)
            {
                BindDropdownList();
                loadYardImage(viewType);
            }

            DrawZones(viewType);
        }

        private void loadYardImage(int viewType)
        {
            if (viewType == 0)
                imgMapYardView.ImageUrl = "~/Controls/ImageHandler.ashx?YardID=" + rddlYardMaster.SelectedValue + "&ID=1&PplTrk=1";
            else
                imgMapYardView.ImageUrl = "~/Controls/ImageHandler.ashx?YardID=" + rddlYardMaster.SelectedValue + "&ID=1";
        }

        private int getViewType()
        {
            int viewType = 0;
            if (Request.QueryString["type"] != null)
                int.TryParse(Request.QueryString["type"], out viewType);
            return viewType;
        }

        string strOnBoardWorkersPage = "ZoneEmployees.aspx";
        string strZoneMaterialPage = "ZoneMaterial.aspx";

        private void BindDropdownList()
        {

            var lstYardMaster = (from yard in objYardDAO.GetAllYards()
                                 select new
                                 {
                                     yard.YardName,
                                     yard.YardID,
                                     yard.IsDefault,
                                     yard.UTCOffset
                                 }).OrderBy(ym => ym.YardName).ToList();

            var lstYardList = (from y in lstYardMaster
                               select new
                               {
                                   YardName = string.Format("{0} ({1}{2})", y.YardName, y.UTCOffset < 0 ? "-" : "+", TimeSpan.FromMinutes(y.UTCOffset).ToString(@"hh\:mm")),
                                   y.YardID,
                                   y.IsDefault
                               }).ToList();
            rddlYardMaster.DataSource = lstYardList;
            rddlYardMaster.DataTextField = "YardName";
            rddlYardMaster.DataValueField = "YardID";
            rddlYardMaster.DataBind();

            if (lstYardMaster == null || lstYardMaster.Count == 0)
                return;
            if (Request.QueryString["YardID"] == null)
                rddlYardMaster.SelectedValue = lstYardMaster.Count == 1 ? lstYardList.FirstOrDefault().YardID.ToString() : lstYardList.Where(YM => YM.IsDefault == true).FirstOrDefault().YardID.ToString();
            else
                rddlYardMaster.SelectedValue = Request.QueryString["YardID"].ToString();
            if (lstYardMaster.Count <= 1)
                rddlYardMaster.Enabled = false;
        }
        protected void rddlYardMaster_SelectedIndexChanged(object sender, Telerik.Web.UI.DropDownListEventArgs e)
        {
            imgMapYardView.ImageUrl = "~/Controls/ImageHandler.ashx?YardID=" + rddlYardMaster.SelectedValue + "&ID=1";

        }
        public void DrawZones(int viewType)
        {
            if (rddlYardMaster.SelectedValue != "-1")
            {
                YardMaster objYardMaster = objYardDAO.GetYardByYardID(Convert.ToInt32(rddlYardMaster.SelectedValue));
                List<usp_GET_AstorSafeYardSummary_Result> AllZones = objZoneDAO.GetAstorYardDetails(objYardMaster.YardID);

                int labelCount;

                List<EmployeeCountModel> ObjYardEmployeesCount = new List<EmployeeCountModel>();

                ObjYardEmployeesCount = objAstorDAO.GetEmpCountDetailsByYardID(objAstorDAO.GetMappingYardID(objYardMaster.YardID).AstorSafeYardID);

                List<MaterialCountModel> listMaterialCount = objMaterialDAO.GetActiveMaterialCount();

                List<Point> ptl = null;
                int intXCoordinate = 0;
                int intYCoordinate = 0;
                LinkButton btnWorkerCount;
                System.Drawing.Image imgZone;
                //  byte[] imgData = null;
                Bitmap bitMapYardImage = null;

                // YardMaster objYardMaster = objYardDAO.GetYardByDeafault();
                //   imgData = System.IO.File.ReadAllBytes(Server.MapPath("../../Images/yardlayout.png"));
                MemoryStream memoryStream = new MemoryStream(objYardMaster.YardLayout, false);
                imgZone = System.Drawing.Image.FromStream(memoryStream);
                bitMapYardImage = new System.Drawing.Bitmap(imgZone);

                if (viewType == 0)
                    AllZones = AllZones.Where(x => x.ZoneColor == "#EEEEEE" || x.ZoneColor == "#FFFF33").ToList();

                foreach (var Zone in AllZones)
                { 
                    //ZoneMapping objZoneMapping = objZoneDAO.GetMappingZoneID(Convert.ToInt16(Zone.ZoneID));
                    //if (objZoneMapping != null)
                    //{
                    labelCount = 0;
                    string detailPage = strOnBoardWorkersPage;
                    int zoneID = Zone.ZoneID;
                    if (viewType == 1)
                    {
                        ZoneMapping objZoneMapping = objZoneDAO.GetMappingZoneID(Convert.ToInt16(Zone.ZoneID), 1);
                        if (objZoneMapping != null)
                        {
                            MaterialCountModel objMap = listMaterialCount.Where(x => x.ZoneID == objZoneMapping.AstorTrackZoneID).FirstOrDefault();
                            if (objMap != null)
                            {
                                labelCount = objMap.MaterialCount;

                                detailPage = strZoneMaterialPage;
                                zoneID = (int)objMap.ZoneID;
                            }
                        }
                    }
                    else
                    {
                        EmployeeCountModel objemp = ObjYardEmployeesCount.Where(x => x.ZoneID == Zone.ZoneID).FirstOrDefault();
                        if (objemp != null)
                            labelCount = objemp.EmployeeCount;
                    }
                    ptl = new List<Point>();
                    var xml = new XmlDocument();
                    xml.LoadXml(Zone.ZoneCoordinates);
                    List<Coordinates> LstCoordinates = new List<Coordinates>();
                    Coordinates objCoordinates;

                    string polyCoordinates = string.Empty;

                    foreach (XmlNode node in xml.GetElementsByTagName("Coordinates"))
                    {
                        objCoordinates = new Coordinates();
                        objCoordinates.x = node.Attributes["x"].Value;
                        objCoordinates.y = node.Attributes["y"].Value;
                        LstCoordinates.Add(objCoordinates);
                    }
                    foreach (var b in LstCoordinates)
                    {
                        int X = Decimal.ToInt32(Convert.ToDecimal(b.x));
                        int Y = Decimal.ToInt32(Convert.ToDecimal(b.y));
                        ptl.Add(new Point(X, Y));
                        polyCoordinates += polyCoordinates == "" ? b.x + "," + b.y : "," + b.x + "," + b.y;
                    }
                    PolygonHotSpot poly = new PolygonHotSpot();
                    poly.Coordinates = polyCoordinates;
                    // poly.PostBackValue = Zone.ZoneName;
                    //   poly.NavigateUrl = "ViewZoneValves.aspx?ZoneID=" + Zone.ZoneID;

                    imgMapYardView.HotSpots.Add(poly);


                    intXCoordinate = (from x in ptl orderby x.Y + x.X ascending select x.X).Distinct().First();
                    intYCoordinate = (from y in ptl orderby y.Y + y.X ascending select y.Y).Distinct().First();
                    btnWorkerCount = new LinkButton();
                    btnWorkerCount.ID = "btn" + Zone.ZoneID;
                    btnWorkerCount.Style.Add("cursor", "pointer");
                    btnWorkerCount.Font.Size = new FontUnit(FontSize.Large);
                    btnWorkerCount.ForeColor = System.Drawing.Color.White;
                    btnWorkerCount.Font.Bold = true;
                    btnWorkerCount.BorderWidth = 0;
                    btnWorkerCount.Style.Add("position", "absolute");
                    decimal Xpadding = (Convert.ToDecimal(intXCoordinate) / bitMapYardImage.Width) * 100;
                    decimal Ypadding = (Convert.ToDecimal(intYCoordinate) / bitMapYardImage.Height) * 100;
                    btnWorkerCount.Style.Add("top", Ypadding + "%");
                    btnWorkerCount.Style.Add("left", Xpadding + "%");
                    btnWorkerCount.Style.Add("background-color", "transparent");
                    btnWorkerCount.Style.Add("text-shadow", "3px 3px 4px #000000");



                    var lbl1 = new Label();
                    lbl1.Text = labelCount.ToString();
                    lbl1.Attributes.Add("style", "color:white;font-size:24px; float:left;");
                    btnWorkerCount.Controls.Add(lbl1);
                    btnWorkerCount.OnClientClick = string.Format("ShowWorkerPopup('{0}','{1}','{2}','{3}','{4}','{5}','{6}');return false;", zoneID, detailPage, Zone.ZoneName, "1", string.Empty, string.Empty, string.Empty);
                    //var lbl2 = new Label();
                    //lbl2.Text = "/";
                    //btnWorkerCount.Controls.Add(lbl2);
                    //lbl2.Attributes.Add("style", "float:left;");
                    //var lbl3 = new Label();
                    //lbl3.Text = "3";
                    //lbl3.Attributes.Add("style", "color:white; float:left;");
                    //btnWorkerCount.Controls.Add(lbl3);

                    this.divImageMapper.Controls.Add(btnWorkerCount);
                    //   this.RadToolTipManager1.TargetControls.Add(btnWorkerCount.ID, Zone.ZoneID.ToString() + "," + Zone.ZoneName, false);
                    btnWorkerCount.Enabled = false;
                }
                //     }

            }
        }

        protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
        {
            this.UpdateToolTip(args.Value, args.UpdatePanel);
        }
        private void UpdateToolTip(string elementID, UpdatePanel panel)
        {
            //Session["TTMZoneID"] = elementID;
            //Control ctrl = Page.LoadControl("~/Controls/EvenDetails.ascx");
            //panel.ContentTemplateContainer.Controls.Add(ctrl);

        }

        protected void btnReload_Click(object sender, EventArgs e)
        {

        }
    }
}