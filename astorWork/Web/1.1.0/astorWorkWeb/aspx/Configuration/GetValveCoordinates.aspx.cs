using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace astorWork.aspx.Configuration
{
    public partial class GetValveCoordinates : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["IMG"] = null;
                if (Request.QueryString["ZoneID"] != null)
                {
                    this.Title = Request.QueryString["ZoneName"];
                    GetZoneImage();
                    PlaceOtherValves();
                }
            }
        }

        private void GetZoneImage()
        {

            rimgZoneLayOut.ImageUrl = "~/Controls/ZoneImage.ashx?ZoneID=" + Request.QueryString["ZoneID"] + "&ID=" + new Random().Next();

            rttYard.ForeColor = System.Drawing.Color.Blue;
            RadBinaryImage imgZone = rttYard.FindControl("rimgZone") as RadBinaryImage;
            imgZone.ImageUrl = "~/images/UndefinedValve.png";
            rttYard.Position = ToolTipPosition.TopCenter;
        }
        private void PlaceOtherValves()
        {
            ValveDAO objValveDAO = new ValveDAO();
            var AllValves = objValveDAO.getZoneValves(int.Parse(Request.QueryString["ZoneID"]), 0).Where(V => V.ValveID != int.Parse(Request.QueryString["ValveID"]) || int.Parse(Request.QueryString["ValveID"]) == 0);
            int intXCoordinate = 0;
            int intYCoordinate = 0;
            LinkButton lbnValve;

            foreach (var Valve in AllValves)
            {
                string[] strValveXY = Valve.ValveCoordinates.Split(',');
                intXCoordinate = Convert.ToInt32(strValveXY[0]);
                intYCoordinate = Convert.ToInt32(strValveXY[1]);
                lbnValve = new LinkButton();
                lbnValve.ID = "lbn" + Valve.ValveID;
                lbnValve.Style.Add("cursor", "pointer");
                lbnValve.Font.Size = new FontUnit(FontSize.Large);
                lbnValve.ForeColor = System.Drawing.Color.White;
                lbnValve.Font.Bold = true;
                lbnValve.BorderWidth = 0;
                lbnValve.Style.Add("position", "absolute");
                lbnValve.Style.Add("top", strValveXY[1] + "px");
                lbnValve.Style.Add("left", strValveXY[0] + "px");
                lbnValve.Style.Add("background-color", "transparent");
                var lbl1 = new Label();
                lbl1.Text = Valve.ValveName + "<br />";
                lbl1.Style.Add("font-size", "12px");
                lbl1.Style.Add("color", "black");
                lbnValve.Controls.Add(lbl1);
                var img1 = new System.Web.UI.WebControls.Image();
                img1.Style.Add("opacity", "0.4");
                img1.Width = new Unit(50, UnitType.Pixel);
                img1.Height = new Unit(40, UnitType.Pixel);
                img1.ImageUrl = "~/images/OtherValve.png";
                lbnValve.Controls.Add(img1);
                this.divImageMapper.Controls.Add(lbnValve);
                lbnValve.Enabled = false;
            }
        }
    }
}