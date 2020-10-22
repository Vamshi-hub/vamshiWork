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
using System.Drawing.Imaging;

namespace astorWork
{
    public partial class ViewZoneValves : System.Web.UI.Page
    {
        ZoneDAO objZoneDAO = null;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (objZoneDAO == null)
                objZoneDAO = new ZoneDAO();
            if (Request.QueryString["ZoneID"] != null)
            {
                if (!IsPostBack)
                    BindZoneImage();
                DrawValves(int.Parse(Request.QueryString["ZoneID"]));
            }


        }
        private void BindZoneImage()
        {
            ZoneMaster objZoneMaster = objZoneDAO.GetZoneDetails(int.Parse(Request.QueryString["ZoneID"]));
            // string base64String = Convert.ToBase64String(objZoneMaster.ZoneLayOut, 0, objZoneMaster.ZoneLayOut.Length);
            rbZone.ImageUrl = "~/Controls/ZoneImage.ashx?ZoneID=" + Request.QueryString["ZoneID"]+"&ID=" + new Random().Next();
            ViewState["IMG"] = objZoneMaster.ZoneLayOut;
        }
        public void DrawValves(int ZoneID)
        {
            try
            {
                ValveDAO objValveDAO = new ValveDAO();
                var AllValves = objValveDAO.getZoneValves(ZoneID, 0);

                int intXCoordinate = 0;
                int intYCoordinate = 0;
                LinkButton lbnValve;
                System.Drawing.Image imgZone;
                byte[] imgData = (byte[])ViewState["IMG"];
                Bitmap bitMapYardImage = null;
                MemoryStream memoryStream = new MemoryStream(imgData, false);
                imgZone = System.Drawing.Image.FromStream(memoryStream);
                bitMapYardImage = new System.Drawing.Bitmap(imgZone);
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
                    decimal Xpadding = (Convert.ToDecimal(intXCoordinate) / bitMapYardImage.Width) * 100;
                    decimal Ypadding = (Convert.ToDecimal(intYCoordinate) / bitMapYardImage.Height) * 100;
                    lbnValve.Style.Add("top", Ypadding + "%");
                    lbnValve.Style.Add("left", Xpadding + "%");
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
                    img1.ImageUrl = Valve.Status == 0 ? "~/images/OpenedValve.png" : "~/images/ClosedValve.png";
                    lbnValve.Controls.Add(img1);

                    this.divImageMapper.Controls.Add(lbnValve);
                    this.RadToolTipManager1.TargetControls.Add(lbnValve.ClientID, Valve.ValveID.ToString(), true);
                    lbnValve.Enabled = false;
                }

            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage("DrawValves:" + ex.Message, true, ex, false);

            }



        }

        protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
        {
            this.UpdateToolTip(args.Value, args.UpdatePanel);
        }
        private void UpdateToolTip(string elementID, UpdatePanel panel)
        {
            Session["ValveID"] = elementID;
            Control ctrl = Page.LoadControl("~/Controls/ValveDetails.ascx" );
            panel.ContentTemplateContainer.Controls.Add(ctrl);

        }

        protected void btnReload_Click(object sender, EventArgs e)
        {

        }
    }
}