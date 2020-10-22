using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using astorWorkDAO;
using System.Xml;
using System.Collections;
namespace astorWork.aspx.Configuration
{
    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            MemoryStream memoryStream = new MemoryStream();
            ZoneMaster objZoneMaster = null;
            ZoneDAO objZoneDAO = new ZoneDAO();
            YardDAO objYardDAO = new YardDAO();
            if (objZoneDAO == null)
                objZoneDAO = new ZoneDAO();
            if (objYardDAO == null)
                objYardDAO = new YardDAO();
            int intXCoordinate = 0;
            int intYCoordinate = 0;
            System.Drawing.Image imgZone;
            //     byte[] imgData = null;
            Bitmap bitMapYardImage = null;
            int ZoneID = 0;
            int YardID = 0;

            if (context.Request.QueryString["ZoneID"] != null)
            {
                ZoneID = Convert.ToInt32(context.Request.QueryString["ZoneID"]);
                objZoneMaster = objZoneDAO.GetZoneDetails(ZoneID);
                YardID = Convert.ToInt32(objZoneMaster.YardID);
                YardMaster objYardMaster = objYardDAO.GetYardByYardID(YardID);
                memoryStream = new MemoryStream(objYardMaster.YardLayout, false);
            }
            else if (context.Request.QueryString["YardID"] != null && context.Request.QueryString["YardID"].ToString() != "-1")
            {
                YardID = Convert.ToInt32(context.Request.QueryString["YardID"]);
                YardMaster objYardMaster = objYardDAO.GetYardByYardID(YardID);
                YardID = Convert.ToInt32(objYardMaster.YardID);
                memoryStream = new MemoryStream(objYardMaster.YardLayout, false);
            }
            else
            {
                YardMaster objYardMaster = objYardDAO.GetYardByDeafault();
                YardID = Convert.ToInt32(objYardMaster.YardID);
                memoryStream = new MemoryStream(objYardMaster.YardLayout, false);
            }
            imgZone = System.Drawing.Image.FromStream(memoryStream);
            bitMapYardImage = new System.Drawing.Bitmap(imgZone);
            Graphics graphicYardImage = Graphics.FromImage(bitMapYardImage);
            Pen blackPen = new Pen(Color.Black, 3);
            List<Point> ptl = null;

            if (context.Request.QueryString["ID"].ToString() =="1")
            {
                var varAllZones = objZoneDAO.GetAstorYardDetails(YardID);

                foreach (var Zone in varAllZones)
                {
                    ptl = new List<Point>();
                    var xml = new XmlDocument();
                    xml.LoadXml(Zone.ZoneCoordinates);
                    List<Coordinates> LstCoordinates = new List<Coordinates>();
                    Coordinates objCoordinates;
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
                    }
                    intXCoordinate = (from x in ptl orderby x.X ascending select x.X).Distinct().First();
                    intYCoordinate = (from y in ptl orderby y.X ascending select y.Y).Distinct().First();
                    int xc = (from x in ptl orderby x.X ascending select x.X).Distinct().Skip(1).First();
                    int yc = (from y in ptl orderby y.X ascending select y.Y).Distinct().Skip(1).First();
                    intXCoordinate = (intXCoordinate + xc) / 2;
                    intYCoordinate = (intYCoordinate + yc) / 2;
                    Color clr = System.Drawing.ColorTranslator.FromHtml(Zone.ZoneColor.ToString());
                    clr = Color.FromArgb(140, clr.R, clr.G, clr.B);
                    SolidBrush sbPolygon = new SolidBrush(clr);
                    graphicYardImage.FillPolygon(sbPolygon, ptl.ToArray());
                    Font fontZone = new System.Drawing.Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
                    graphicYardImage.DrawString(Zone.ZoneName, fontZone, new SolidBrush(Color.Black), new PointF(intXCoordinate, intYCoordinate));
                    memoryStream = new MemoryStream();
                    bitMapYardImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    graphicYardImage.Save();
                }
                graphicYardImage.SmoothingMode = SmoothingMode.AntiAlias;
                bitMapYardImage.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
            }
            else
            {
                var varAllZones = objZoneDAO.GetYardDetails(YardID);

                foreach (var Zone in varAllZones)
                {
                    ptl = new List<Point>();
                    var xml = new XmlDocument();
                    xml.LoadXml(Zone.ZoneCoordinates);
                    List<Coordinates> LstCoordinates = new List<Coordinates>();
                    Coordinates objCoordinates;
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
                    }
                    intXCoordinate = (from x in ptl orderby x.X ascending select x.X).Distinct().First();
                    intYCoordinate = (from y in ptl orderby y.X ascending select y.Y).Distinct().First();
                    int xc = (from x in ptl orderby x.X ascending select x.X).Distinct().Skip(1).First();
                    int yc = (from y in ptl orderby y.X ascending select y.Y).Distinct().Skip(1).First();
                    intXCoordinate = (intXCoordinate + xc) / 2;
                    intYCoordinate = (intYCoordinate + yc) / 2;
                    Color clr = System.Drawing.ColorTranslator.FromHtml(Zone.ZoneColor.ToString());
                    clr = Color.FromArgb(140, clr.R, clr.G, clr.B);
                    SolidBrush sbPolygon = new SolidBrush(clr);
                    graphicYardImage.FillPolygon(sbPolygon, ptl.ToArray());
                    Font fontZone = new System.Drawing.Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
                    graphicYardImage.DrawString(Zone.ZoneName, fontZone, new SolidBrush(Color.Black), new PointF(intXCoordinate, intYCoordinate));
                    memoryStream = new MemoryStream();
                    bitMapYardImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    graphicYardImage.Save();
                }
                graphicYardImage.SmoothingMode = SmoothingMode.AntiAlias;
                bitMapYardImage.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Png);
            }

        }
    
    
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}
}