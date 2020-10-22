using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using astorWorkDAO;
using Telerik.Web.UI;
using System.Drawing;
using System.IO;

namespace astorWork.aspx.Configuration
{
    public partial class AddZoneMaster : System.Web.UI.Page
    {
        #region Declaration
        ZoneDAO objZoneDAO;
        YardDAO objYardDAO;
        int ZoneID = 0;
        int YardID = 0;
        int IsEdit = 0;
        Enums.Operation eOperation;
        AstorSafeMappingDAO objastorSafe = new AstorSafeMappingDAO();
        #endregion

        #region PageEvents

        protected void Page_Load(object sender, EventArgs e)
        {
            if (objZoneDAO == null)
                objZoneDAO = new ZoneDAO();
            if (objYardDAO == null)
                objYardDAO = new YardDAO();
            if (Request.QueryString["YardID"] != null && Request.QueryString["YardID"] != string.Empty)
            {
                YardID = int.Parse(Request.QueryString["YardID"]);
                IsEdit = int.Parse(Request.QueryString["Edit"]);
                hdnYardID.Value = YardID.ToString();
                if (!IsPostBack)
                {
                    hdnIsEdit.Value = IsEdit.ToString();
                    GetZoneDetails();

                    if (IsEdit == 1)
                    {
                        ZoneID = int.Parse(Request.QueryString["ZoneID"]);
                        rbnSaveZone.Text = "Update";
                    }
                    else
                    {
                        ZoneID = 0;
                        rbnSaveZone.Text = "Save";
                    }
                }

            }

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (hdnCoord.Value != "")
            {
                try
                {
                   
                        YardMaster objYard = objYardDAO.GetYardByYardID(Convert.ToInt32(Request.QueryString["YardID"]));
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        string Coordinates = hdnCoord.Value.Replace("\r\n", "").Replace("@", "");
                        List<Coordinates> records = serializer.Deserialize<List<Coordinates>>(Coordinates);
                        string strCoordinates = "<ZonDetailPoints xmlns=\"www.astoriasolutions.com\">";

                        foreach (Coordinates row in records)
                            strCoordinates += "<Coordinates x=\"" + row.x + "\"" + " y=\"" + row.y + "\" />";

                        strCoordinates += "</ZonDetailPoints>";
                        ZoneMaster objZoneMaster = new ZoneMaster()
                        {
                            YardID = Convert.ToInt32(Request.QueryString["YardID"])
                            ,
                            ZoneDescription = rtbDesc.Text.Trim()
                            ,
                            ZoneCoordinates = strCoordinates
                            ,
                            ZoneName = rtbZoneName.Text.Trim()
                            ,
                            ZoneColor = System.Drawing.ColorTranslator.ToHtml(rcpkrColor.SelectedColor)
                            ,
                            CreatedBy = "Admin",
                            YardLayout= objYard.YardLayout
                        };
                        bool IsSaved = false;

                    if (rbnSaveZone.Text == "Save")
                    {
                        if (!objZoneDAO.CheckZoneNameExists(ZoneID, rtbZoneName.Text.Trim()))
                        {
                            IsSaved = objZoneDAO.InsertZone(objZoneMaster);
                        }
                        else
                            Master.DisplayAlertMessage("Zone name Already Exists.Pease try with different.", true, null, false);
                    }
                    else
                    {
                        objZoneMaster.ZoneID = int.Parse(Request.QueryString["ZoneID"]);
                        IsSaved = objZoneDAO.UpdateZone(objZoneMaster);
                    }

                        if (IsSaved)
                        {
                            eOperation = rbnSaveZone.Text == "Save" ? Enums.Operation.Save : Enums.Operation.Update;
                            hdnCoord.Value = rtbDesc.Text = rtbZoneName.Text = string.Empty;
                            rcpkrColor.SelectedColor = System.Drawing.Color.Red;
                            Response.Redirect("ViewZoneMaster.aspx?Operation=" + eOperation);

                        }

                        else
                            Master.DisplayAlertMessage("Failed to save Zone Master ", true, null, false);
                    
                    
                }
                catch (Exception ex)
                {
                    Master.DisplayAlertMessage("Error while saving:" + ex.Message, true, ex, false);
                    // throw;
                }
            }
        }

        #endregion

        #region PrivateMethods

        private void GetZoneDetails()
        {
            try
            {
                if (IsEdit != 0)
                {
                    hdnFileCount.Value = "1";
                    //  imgPreview.Visible = true;
                    ZoneMaster objZoneDetails = objZoneDAO.GetZoneDetails(int.Parse(Request.QueryString["ZoneID"]));

                    rtbDesc.Text = objZoneDetails.ZoneDescription;
                    rtbZoneName.Text = objZoneDetails.ZoneName;
                    XmlDocument xDoc = new XmlDocument();
                    xDoc.LoadXml(objZoneDetails.ZoneCoordinates);
                    string json = Newtonsoft.Json.JsonConvert.SerializeXmlNode(xDoc);
                    var data = Newtonsoft.Json.Linq.JObject.Parse(json);
                    hdnCoord.Value = data["ZonDetailPoints"]["Coordinates"].ToString();
                    rcpkrColor.SelectedColor = System.Drawing.ColorTranslator.FromHtml(objZoneDetails.ZoneColor);
                    string strZoneImage = Convert.ToBase64String(objZoneDetails.YardLayout);
                    hdnImageByte.Value = Convert.ToBase64String(objZoneDetails.YardLayout);
                    //   this.RadToolTipManager1.TargetControls.Add(imgPreview.ClientID, strZoneImage, true);
                }
            }
            catch (Exception ex)
            {

                Master.DisplayAlertMessage(ex.Message, true, ex, false);
            }



        }

        #endregion

        #region Public Methods
        protected void OnAjaxUpdate(object sender, ToolTipUpdateEventArgs args)
        {
            this.UpdateToolTip(args.Value, args.UpdatePanel);
        }
        private void UpdateToolTip(string zoneLayOut, UpdatePanel panel)
        {
            RadBinaryImage rbi = new RadBinaryImage();
            rbi.DataValue = Convert.FromBase64String(zoneLayOut);
            panel.ContentTemplateContainer.Controls.Add(rbi);

        }
        #endregion

        //protected void rddlYard_SelectedIndexChanged(object sender, DropDownListEventArgs e)
        //{

        //    YardMaster objYard = objYardDAO.GetYardByYardID(Convert.ToInt16(Convert.ToInt32(rddlYard.SelectedValue.Split('@')[0])));
        //    string strZoneImage = Convert.ToBase64String(objYard.YardLayout);
        //    //  this.RadToolTipManager1.TargetControls.Add(imgPreview.ClientID, strZoneImage, true);
        //}
    }
}