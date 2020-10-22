using astorWorkDAO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace astorWork.Controls
{
    /// <summary>
    /// Summary description for ZoneImage
    /// </summary>
    public class ZoneImage : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "image/jpeg";
            if (context.Request.QueryString["ZoneID"] != null)
            {
               
                ZoneDAO objZoneDAO = new ZoneDAO();
                ZoneMaster objZoneMaster = objZoneDAO.GetZoneDetails(int.Parse(context.Request.QueryString["ZoneID"]));
                using (MemoryStream memoryStream = new MemoryStream(objZoneMaster.YardLayout, false))
                {
                    if (memoryStream != null)
                    {
                        System.Drawing.Image imgFromDataBase = System.Drawing.Image.FromStream(memoryStream);
                        imgFromDataBase.Save(context.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
               
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