using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;
using Telerik.Web.UI;
using System.Drawing;

namespace astorWork.aspx.Configuration
{
    public partial class AddValveMaster : System.Web.UI.Page
    {
        #region Declaration
        ValveDAO objValveDAO;
        Enums.Operation enumOperation;
        int ZoneID = 0;
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindZones();

            if (Request.QueryString["ZoneID"] != null)
            {
                ZoneID = int.Parse(Request.QueryString["ZoneID"]);

                if (objValveDAO == null)
                    objValveDAO = new ValveDAO();
                if (!IsPostBack)
                {
                    GetZoneName();
                    BindValveTypes();
                }
            }
            if (Request.QueryString["ValveID"] != null)
            {
                hdnValveID.Value = Request.QueryString["ValveID"];
                rbnSave.Text = "Update";
                enumOperation = Enums.Operation.Update;
                Page.Title = "Update Valve Master";
                if (!IsPostBack)
                    BindValveDetails(int.Parse(Request.QueryString["ValveID"]));

            }
            else
            {
                enumOperation = Enums.Operation.Save;
                rbnSave.Text = "Save";
            }

        }

        protected void rbnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ZoneID = int.Parse(rddlZone.SelectedValue);
                ValveMaster objValveMaster = new ValveMaster()
                {
                    ZoneID = ZoneID
                    ,
                    ValveDescription = rtbDesc.Text.Trim()
                    ,
                    ValveTypeID = int.Parse(rddlValveType.SelectedItem.Value)
                    ,
                    ValveName = rtbValveName.Text.Trim()
                    ,
                    ValveCoordinates = tbImageCoordinates.Text.Trim()
                };
                if (rAsyncUValveImage.UploadedFiles.Count > 0)
                {
                    UploadedFile file = rAsyncUValveImage.UploadedFiles[0];
                    Bitmap imgLayout = new Bitmap(file.InputStream);
                    byte[] ValveImage = new byte[file.InputStream.Length];
                    file.InputStream.Read(ValveImage, 0, (int)file.InputStream.Length);
                    objValveMaster.ValveImage = ValveImage;
                }
                bool isSaved = false;
                if (Request.QueryString["ValveID"] != null)
                {
                    int ValveID = int.Parse(Request.QueryString["ValveID"]);
                    objValveMaster.ValveID = ValveID;
                    isSaved = objValveDAO.updateValve(objValveMaster);
                }
                else
                {
                    isSaved = objValveDAO.insertValve(objValveMaster);
                }
                if (isSaved)
                    ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "onClose", "<script>CloseAndReload('" + enumOperation + "');</script>", false);
                else
                    Master.DisplayAlertMessage("Valve Master failed to save", true, null, false);
            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(ex.Message, true, ex, false);
                throw;
            }
        }
        #endregion

        #region PrivateMethods
        private void GetZoneName()
        {
            try
            {
                ZoneDAO objZoneDAO = new ZoneDAO();
                ZoneMaster objZoneMaster = objZoneDAO.GetZoneDetails(ZoneID);
                rlblZone.Text = objZoneMaster.ZoneName;
                rddlZone.SelectedValue = ZoneID.ToString();
                rddlZone.SelectedItem.Text = objZoneMaster.ZoneName + " (Current)";
            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(ex.Message, true, ex, false);
            }

        }

        private void BindValveTypes()
        {
            try
            {

                rddlValveType.DataSource = Enum.GetNames(typeof(Enums.ValveTypes)).
        Select(o => new { Text = o, Value = (int)(Enum.Parse(typeof(Enums.ValveTypes), o)) });
                rddlValveType.DataTextField = "Text";
                rddlValveType.DataValueField = "Value";
                rddlValveType.DataBind();

                rddlValveType.Items.Insert(0, new DropDownListItem("--Select--", "-1"));
            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(ex.Message, true, ex, false);
            }

        }

        private void BindValveDetails(int ValveID)
        {
            try
            {
                List<ValveMaster> objValveList = objValveDAO.getZoneValves(ZoneID, ValveID);
                if (objValveList.Count > 0)
                {
                    rddlValveType.ClearSelection();
                    rddlValveType.SelectedValue = objValveList[0].ValveTypeID.ToString();
                    rtbDesc.Text = objValveList[0].ValveDescription;
                    tdImgValve.Visible = true;
                    rbImgValve.Visible = true;
                    rbImgValve.DataValue = objValveList[0].ValveImage;
                    
                    hdnFileCount.Value = "1";
                    rtbValveName.Text = objValveList[0].ValveName;
                    tbImageCoordinates.Text = objValveList[0].ValveCoordinates;
                }
            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(ex.Message, true, ex, false);
            }

        }

        private void BindZones()
        {
            ZoneDAO objZoneDAO = new ZoneDAO();
            rddlZone.DataSource = objZoneDAO.GetAllZones();
            rddlZone.DataTextField = "ZoneName";
            rddlZone.DataValueField = "ZoneID";
            rddlZone.DataBind();
            rddlZone.Items.Insert(0, new DropDownListItem("--Select--", "-1"));
        }
        #endregion



    }
}