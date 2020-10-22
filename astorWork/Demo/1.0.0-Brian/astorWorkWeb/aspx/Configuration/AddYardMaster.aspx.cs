using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using astorWorkDAO;
using Telerik.Web.UI;

namespace astorWork.aspx.Configuration
{
    public partial class AddYardMaster : System.Web.UI.Page
    {
        #region Declaration

        YardDAO objYardDAO = null;
        int intYardID;
        YardMaster objYardMaster = new YardMaster();
        Enums.Operation eOperation;
        DataTable dtGangway;
        public static DataTable dtUnassignedDevices;

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (objYardDAO == null)
                objYardDAO = new YardDAO();

            if (!IsPostBack)
            {
                //rbtnPreview.Style.Add("display", "none");
                this.Title = Request.QueryString["YardID"] != string.Empty ? "Edit Yard" : "Add Yard";
                hdnYardID.Value = Request.QueryString["YardID"];
                BindTimeZone();
                LoadPageContent();
                dtUnassignedDevices = null;
            }

        }
        protected void Page_PreInit(object sender, EventArgs e)
        {
            //Page.Theme = objCommon.objUserProfile.UserTheme;
        }
        protected override void InitializeCulture()
        {

            base.InitializeCulture();
        }


        #endregion

        #region Button Events


        protected void rbtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                intYardID = string.IsNullOrEmpty(hdnYardID.Value) ? 0 : Convert.ToInt32(hdnYardID.Value);
                eOperation = intYardID != 0 ? Enums.Operation.Update : Enums.Operation.Save;
                byte[] yardLayout = null;
              
                if (intYardID != 0)
                    objYardMaster = objYardDAO.GetYardByYardID(intYardID);
                else
                    objYardMaster = new YardMaster();

                if (rAsyncUYardLayout.UploadedFiles.Count > 0)
                {
                    UploadedFile file = rAsyncUYardLayout.UploadedFiles[0];
                    Bitmap imgLayout = new Bitmap(file.InputStream);
                    yardLayout = new byte[file.InputStream.Length];
                    file.InputStream.Read(yardLayout, 0, (int)file.InputStream.Length);

                    if (yardLayout != null)
                        objYardMaster.YardLayout = yardLayout;
                    objYardMaster.LayoutDimension = imgLayout.Width.ToString() + "," + imgLayout.Height.ToString();
                }
                objYardMaster.YardName = tbYardName.Text.Trim();
                objYardMaster.TimeZoneID = Convert.ToInt32(rcbUTCOffset.SelectedValue.Split(',')[0]);
                objYardMaster.UTCOffset = Convert.ToInt32(rcbUTCOffset.SelectedValue.Split(',')[1]);

                objYardMaster.EnableYardTimeCapture = rbtnIsYardTimeCapture.SelectedValue == "Yes" ? true : false;
                if (rbtnIsDefault.SelectedValue == "Yes")
                {
                    var list = objYardDAO.GetAllYards().Where(i => i.YardID != intYardID && i.IsDefault == true).ToList();
                    if (list.Count != 0)
                    {
                        foreach (var lists in list)
                        {
                            lists.IsDefault = false;
                        }
                    }
                    objYardMaster.IsDefault = true;
                    objYardDAO.InsertYard(objYardMaster);
                    ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "onClose", "<script>CloseAndReload('" + eOperation + "');</script>", false);
                }
                else
                {
                    objYardMaster.IsDefault = false;
                    objYardDAO.InsertYard(objYardMaster);
                    ScriptManager.RegisterClientScriptBlock(Page, typeof(Page), "onClose", "<script>CloseAndReload('" + eOperation + "');</script>", false);
                }

                Master.DisplayAlertMessage(eOperation, "Yard");
                //}

            }
            catch (Exception ex)
            {
                Master.DisplayAlertMessage(eOperation, ex, "Yard");
                return;
            }
        }
        //protected void rbtnPreview_Click(object sender, System.EventArgs e)
        //{
        //    byte[] yardLayout = null;
        //    yardLayout = (byte[])Session["YardImage"];
        //    imgYardLayout.DataValue = yardLayout;
        //    rttPreview.Show();
        //    rttPreview.TargetControlID = "lblToolTip";
        //    rbtnPreview.Style.Add("display", "block");

        //}
        protected void rbtnSaveSession_Click(object sender, System.EventArgs e)
        {
            Session["YardImage"] = Session["Image"];
            //  rbtnPreview.Style.Add("display", "block");
        }

        #endregion

        #region Helper Methods

        //private void FillConnectingYardDDL()
        //{
        //    try
        //    {
        //        intYardID = hdnYardID.Value == string.Empty ? 0 : Convert.ToInt32(hdnYardID.Value);
        //        rddlConnectingYard.Items.Clear();
        //        List<YardMaster> lstYardMaster = objYardDAO.GetAllYards().Where(y => y.EnableYardTimeCapture == true).ToList();
        //        if (rbtnSave.Text == "Update")
        //            lstYardMaster.Remove(lstYardMaster.Where(zm => zm.YardID == intYardID).FirstOrDefault());
        //        rddlConnectingYard.DataSource = lstYardMaster.OrderBy(zm => zm.YardName);
        //        rddlConnectingYard.DataTextField = "YardName";
        //        rddlConnectingYard.DataValueField = "YardID";
        //        rddlConnectingYard.AppendDataBoundItems = true;
        //        rddlConnectingYard.Items.Add(new DropDownListItem(Constants.DDL_SELECT_ITEM, "-1"));
        //        rddlConnectingYard.DataBind();
        //    }
        //    catch (Exception ex)
        //    {
        //    //    Master.DisplayAlertMessage(Resources.Messages.LoadingError, true, ex);
        //    }
        //}

        //private void FillDeviceDropDown()
        //{
        //    try
        //    {
        //        rddlDevice.Items.Clear();
        //        List<DeviceMaster> lstDevices = objUtils.GetAllDevicesForCSandGangway();
        //        //Restricting Duplicate device value in Device Dropdown
        //        if (objDeviceMaster != null)
        //        {
        //            lstDevices = objUtils.GetAllDevicesForCSandGangway().Where(d => d.DeviceID != objDeviceMaster.DeviceID).ToList();
        //            lstDevices.Add(objDeviceMaster);
        //        }
        //        else
        //            lstDevices = objUtils.GetAllDevicesForCSandGangway();
        //        rddlDevice.DataSource = lstDevices.OrderBy(d => d.DeviceName);
        //        rddlDevice.DataTextField = "DeviceName";
        //        rddlDevice.DataValueField = "DeviceID";
        //        rddlDevice.AppendDataBoundItems = true;
        //        rddlDevice.Items.Add(new DropDownListItem("Not Assigned", "-1"));
        //        rddlDevice.DataBind();
        //        rddlDevice.SelectedIndex = 0;
        //        //Storing Unassigned devices in a DataTable
        //        if (dtUnassignedDevices == null)
        //        {
        //            dtUnassignedDevices = new DataTable();
        //            dtUnassignedDevices.Columns.Add("DeviceName", typeof(string));
        //            dtUnassignedDevices.Columns.Add("DeviceID", typeof(int));
        //            //for (int i = 1; i < rddlDevice.Items.Count; i++)
        //            foreach (DropDownListItem l in rddlDevice.Items)
        //            {
        //                //dtUnassignedDevices.Rows.Add(rddlDevice.Items[i].Text, rddlDevice.Items[i].Value);
        //                if (l.Value != hdnDeviceID.Value)
        //                    dtUnassignedDevices.Rows.Add(l.Text, l.Value);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Master.DisplayAlertMessage(Resources.Messages.LoadingError, true, ex);
        //    }
        //}

        //private void ResetGangWay()
        //{
        //    lblErrorMsgofConntYard.Text = string.Empty;
        //    lblDeviceVal.Text = string.Empty;
        //    lblGangWayNameVal.Text = string.Empty;
        //    lblErrMsg.Text = string.Empty;
        //    tbGangwayName.Text = string.Empty;
        //    rtbGangwayDesc.Text = string.Empty;
        //    rddlConnectingYard.SelectedIndex = 0;
        //    rddlDevice.SelectedIndex = 0;
        //    hdnGangwayID.Value = string.Empty;
        //    hdnGangwayName.Value = string.Empty;
        //}

        private void Reset()
        {
            lblErrMsg.Text = string.Empty;
            tbYardName.Text = string.Empty;
            hdnYardName.Value = string.Empty;
            tbYardName.Enabled = true;
            hdnYardID.Value = string.Empty;
            rbtnIsDefault.SelectedValue = "No";
            rbtnIsYardTimeCapture.SelectedValue = "No";
        }

        private DataTable CreateDataTable()
        {
            var rgGangway = new DataTable();
            rgGangway.Columns.Add("GangwayID", typeof(long));
            rgGangway.Columns.Add("GangwayName", typeof(string));
            rgGangway.Columns.Add("GangwayDescription", typeof(string));
            rgGangway.Columns.Add("ConnectingYardID", typeof(int));
            rgGangway.Columns.Add("ConnectingYardName", typeof(string));
            rgGangway.Columns.Add("DeviceID", typeof(int));
            rgGangway.Columns.Add("DeviceName", typeof(string));
            rgGangway.Columns.Add("IsNewlyAdded", typeof(bool));
            rgGangway.Columns.Add("GangwayStatus", typeof(int));
            rgGangway.Columns.Add("GangwayLocationID", typeof(int));
            rgGangway.Columns.Add("GangwayDeviceHistory", typeof(string));
            return rgGangway;
        }

        public void LoadPageContent()
        {

            if (Request.QueryString.HasKeys() && Request.QueryString["YardID"] != string.Empty)
            {
                objYardMaster = new YardMaster();
                try
                {
                    objYardMaster = objYardDAO.GetYardByYardID(Convert.ToInt32(hdnYardID.Value));
                }
                catch (Exception ex)
                {
                    Master.DisplayAlertMessage("", true, ex);
                    return;
                }
                lblErrMsg.Text = "";
                hdnYardID.Value = objYardMaster.YardID.ToString();
                tbYardName.Text = objYardMaster.YardName;
                tbYardName.Enabled = false;
                hdnYardName.Value = objYardMaster.YardName;
                rcbUTCOffset.SelectedValue = objYardMaster.TimeZoneID.ToString() + "," + objYardMaster.UTCOffset.ToString();
                rbtnIsDefault.SelectedValue = objYardMaster.IsDefault ? "Yes" : "No";
                if (rbtnIsDefault.SelectedValue == "Yes")
                    rbtnIsDefault.Enabled = false;
                rbtnIsYardTimeCapture.SelectedValue = objYardMaster.EnableYardTimeCapture ? "Yes" : "No";
                hdnOrginalIsTimeCapture.Value = rbtnIsYardTimeCapture.SelectedValue;
                cvYardLayout.Enabled = false;
                rbtnSave.Text = "Update";
                rbtnSave.CommandName = "Update";
                //  rddlConnectingYard.Enabled = true;
            }
            else
            {
                rbtnSave.Text = "Save";
                rbtnSave.CommandName = "Insert";
                Reset();
                cvYardLayout.Enabled = true;
                //rddlConnectingYard.Enabled = true;
            }

        }

        public void BindTimeZone()
        {
            List<TimeZoneMaster> lstTimeZone = objYardDAO.GetAllTimeZones();

            var TimeZone = from TZ in lstTimeZone
                           select new
                           {
                               TZ.DisplayName,
                               TimeZone = TZ.TimeZoneID.ToString() + "," + TZ.UTCOffset.ToString()
                           };
            rcbUTCOffset.DataSource = TimeZone;
            rcbUTCOffset.DataTextField = "DisplayName";
            rcbUTCOffset.DataValueField = "TimeZone";
            rcbUTCOffset.DataBind();

        }

        //private string GetDeviceStatus()
        //{
        //    string strDeviceIDs = string.Empty;
        //    string strDeviceName = string.Empty;
        //    string strassigned = string.Empty;
        //    string strunassigned = string.Empty;
        //    DataTable dtCheckDeviceStatus = (DataTable)ViewState["dtGangway"];
        //    if (dtCheckDeviceStatus != null && dtUnassignedDevices != null)
        //    {
        //        foreach (DataRow row in dtCheckDeviceStatus.Rows)
        //        {
        //            strassigned = row["DeviceID"].ToString();
        //            if (Convert.ToInt32(strassigned) != 0)
        //                for (int i = 0; i < dtUnassignedDevices.Rows.Count; i++)
        //                {
        //                    strunassigned = dtUnassignedDevices.Rows[i]["DeviceID"].ToString();
        //                    if (strassigned == strunassigned)
        //                    {
        //                        strDeviceIDs = strDeviceIDs + dtUnassignedDevices.Rows[i]["DeviceID"] + ",";
        //                        break;
        //                    }
        //                }
        //        }
        //        if (!string.IsNullOrEmpty(strDeviceIDs))
        //            strDeviceName = objGangwayDAO.FindDeviceGangway(strDeviceIDs);
        //    }
        //    return strDeviceName;
        //}

        #endregion
    }
}