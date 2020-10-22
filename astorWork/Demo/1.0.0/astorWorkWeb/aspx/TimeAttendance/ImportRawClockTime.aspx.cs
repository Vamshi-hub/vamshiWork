using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Telerik.Web.UI;
using System.IO;

using System.Reflection;
using astorWorkDAO;
using System.Collections.Generic;
using System.Linq;
using Excel;

namespace astorWork.aspx.TimeAttendance
{
    public partial class ImportRawClockTime : System.Web.UI.Page
    {
        #region Declaration
        UsersDAO objUsersDAO;
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            rfuImport.UseApplicationPoolImpersonation = true;
            if (hdnDB.Value == "")
            {
                if (objUsersDAO == null)
                    objUsersDAO = new UsersDAO();
                ConfigurationMaster objUserMaster = objUsersDAO.GetastorTimeDB();
                hdnDB.Value = objUserMaster.Setting;
            }
            if (!IsPostBack)
            {
                BindDropDown();
            }
        }
        protected void rbtnImport_Click(object sender, EventArgs e)
        {
            IntializeExcel();
        }

        #endregion

        #region Private Methods

        public void BindDropDown()
        {
            ddlDirection.Items.Insert(0, new DropDownListItem("OUT", "2"));
            ddlDirection.Items.Insert(0, new DropDownListItem("IN", "1"));
        }
        public void IntializeExcel()
        {
            string folderPath = Server.MapPath("~/ImportExcelFiles/");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            foreach (UploadedFile file in rfuImport.UploadedFiles)
            {
                if (file.FileName != null)
                {
                    string strFileName = Server.HtmlEncode(file.FileName);
                    string strExtension = System.IO.Path.GetExtension(strFileName);

                    string strUploadFileName = "~/ImportExcelFiles/" + DateTime.Now.ToString("yyyyMMddHHmmss") + strExtension;
                    file.SaveAs(Server.MapPath(strUploadFileName));

                    DataSet dsExcel = ProcessExcel(Server.MapPath(strUploadFileName), strExtension.ToLower());
                    //var LocationName = (dsExcel.Tables[0].Select("convert(Column1 ,'System.String') = 'LOCATION'")).FirstOrDefault();
                    if (dsExcel!=null && dsExcel.Tables[0].Rows.Count >= 10)
                    {
                        var LocationName = dsExcel.Tables[0].Rows[3];
                        if (LocationName != null)
                        {
                            DataTable dtResult = new DataTable("RawClockTime");
                            DataColumn[] columns = new DataColumn[] {
                            new DataColumn("EmployeeID",typeof(System.String))
                            ,new DataColumn("BiometricID",typeof(System.String))
                             ,new DataColumn("LocationName",typeof(System.String))
                              ,new DataColumn("RawClockTime",typeof(System.DateTime))
                              ,new DataColumn("Direction",typeof(System.Int16))
                        };
                            dtResult.Columns.AddRange(columns);
                            var rows = dsExcel.Tables[0].AsEnumerable().Skip(10);
                            int row = 1;
                            foreach (DataRow drow in rows)
                            {
                                if (drow.Table.Columns.Count >= 10)
                                {
                                    try
                                    {


                                        // Get the converted date from the OLE automation date.
                                        DateTime conv;

                                        if (drow[10].ToString() != "" && ddlDirection.SelectedValue == "1")
                                        {
                                            conv = DateTime.FromOADate(double.Parse(drow[10].ToString()));
                                            dtResult.Rows.Add(drow[1], drow[2], LocationName[3], Convert.ToDateTime(DateTime.FromOADate(double.Parse(drow[9].ToString())).ToShortDateString() + " " + conv.TimeOfDay.ToString()), 1);
                                        }

                                        if (drow[11].ToString() != "" && ddlDirection.SelectedValue == "2")
                                        {
                                            conv = DateTime.FromOADate(double.Parse(drow[11].ToString()));
                                            dtResult.Rows.Add(drow[1], drow[2], LocationName[3], Convert.ToDateTime(DateTime.FromOADate(double.Parse(drow[9].ToString())).ToShortDateString() + " " + conv.TimeOfDay.ToString()), 2);
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        string str = ex.Message; Master.DisplayNotificationMessage("Error:Row" + row + str);
                                        return;
                                    }
                                    row += 1;
                                }
                            }
                            if (dtResult.Rows.Count > 0)
                            {
                                astorTimeDAO objastorTimeDAO = new astorTimeDAO(hdnDB.Value);
                                objastorTimeDAO.ImportRawClockTime(dtResult, "astoria", -1);
                                Master.DisplayNotificationMessage(dtResult.Rows.Count + " records are imported");
                            }
                        }
                    }
                    if ((System.IO.File.Exists(strFileName)))
                    {
                        System.IO.File.Delete(strFileName);
                    }
                }
            }
        }

        private DataSet ProcessExcel(string filepath, string extension)
        {
            FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read);

            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
            IExcelDataReader excelReader;
            if (extension == ".xls")
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            else
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            DataSet result = excelReader.AsDataSet();


            excelReader.Close();
            return result;

        }

        #endregion

    }

}