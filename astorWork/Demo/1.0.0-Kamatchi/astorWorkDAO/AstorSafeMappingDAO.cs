using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkDAO
{
    public class AstorSafeMappingDAO
    {
        #region Declaration
        string ConnectionString = ConfigurationManager.ConnectionStrings["astorSafe"].ToString();
        SqlDataReader rdr = null;
        SqlConnection con = null;
        SqlCommand cmd = null;
        astorWorkEntities objastorWorkEntities = null;
        public AstorSafeMappingDAO()
        {
            objastorWorkEntities = new astorWorkEntities();
        }
        #endregion
        public YardMapping GetMappingYardID(int yardID)
        {
            YardMapping yardMappingInfo = null;

            try
           {
                yardMappingInfo = objastorWorkEntities.YardMappings.Where(z => z.AstorWorkYardID == yardID).FirstOrDefault();

            }
            catch (Exception ex)
            {

            }

            return yardMappingInfo;
        }
        public List<EmployeeCountModel> GetEmpCountDetailsByYardID(int paramYardID)
        {

            var employeemodellist = new List<EmployeeCountModel>();
            try
            {
                // Open connection to the database

                con = new SqlConnection(ConnectionString);
                con.Open();

                // Set up a command with the given query and associate
                // this with the current connection.
                string CommandText = "SELECT ZM.ZoneID,(SELECT  Count(EZC.EmployeeKey) from EmployeeZoneClockTime EZC Where EZC.ZoneID=ZM.ZoneID) AS EmployeeCount from ZoneMaster ZM Where ZM.ZoneYardID=@YardID";
                cmd = new SqlCommand(CommandText);
                cmd.Connection = con;
                cmd.Parameters.Add(
                   new SqlParameter(
                   "@YardID", // The name of the parameter to map
                   System.Data.SqlDbType.Int, // SqlDbType values
                   20, // The width of the parameter
                   "ZoneYardID"));  // The name of the source column

                // Fill the parameter with the value retrieved
                // from the text field
                cmd.Parameters["@YardID"].Value = paramYardID;
                // Execute the query
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var employeeModel = new EmployeeCountModel
                    {
                        ZoneID = rdr.GetInt32(rdr.GetOrdinal("ZoneID")),
                        EmployeeCount = rdr.GetInt32(rdr.GetOrdinal("EmployeeCount"))
                    };
                    employeemodellist.Add(employeeModel);
                }

            }
            catch (Exception ex)
            {
                // Print error message
                //  MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();

                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }

            return employeemodellist;

        }


        public EmployeeCountModel GetEmpCountDetailsByZoneID(int paramZoneID)
        {
            SqlDataReader rdr = null;
            SqlConnection con = null;
            SqlCommand cmd = null;
            var employeemodel = new EmployeeCountModel();
            try
            {
                // Open connection to the database
                string ConnectionString = ConfigurationManager.ConnectionStrings["astorSafe"].ToString();
                con = new SqlConnection(ConnectionString);
                con.Open();

                // Set up a command with the given query and associate
                // this with the current connection.
                string CommandText = "SELECT ZM.ZoneID,(SELECT  Count(EZC.EmployeeKey) from EmployeeZoneClockTime EZC Where EZC.ZoneID=ZM.ZoneID) AS EmployeeCount from ZoneMaster ZM Where ZM.ZoneID=@ZoneID";
                cmd = new SqlCommand(CommandText);
                cmd.Connection = con;
                cmd.Parameters.Add(
                   new SqlParameter(
                   "@ZoneID", // The name of the parameter to map
                   System.Data.SqlDbType.Int, // SqlDbType values
                   20, // The width of the parameter
                   "ZoneID"));  // The name of the source column

                // Fill the parameter with the value retrieved
                // from the text field
                cmd.Parameters["@ZoneID"].Value = paramZoneID;
                // Execute the query
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {

                    employeemodel.ZoneID = rdr.GetInt32(rdr.GetOrdinal("ZoneID"));
                    employeemodel.EmployeeCount = rdr.GetInt32(rdr.GetOrdinal("EmployeeCount"));

                }

            }
            catch (Exception ex)
            {
                // Print error message
                //  MessageBox.Show(ex.Message);
            }
            finally
            {
                // Close data reader object and database connection
                if (rdr != null)
                    rdr.Close();

                if (con.State == System.Data.ConnectionState.Open)
                    con.Close();
            }

            return employeemodel;

        }


        public List<CurrentZoneEmployees> GetCurrentZoneEmployees(int intZoneID, string strCompanyIDs, string strDepartmentIDs, string strTradeIDs, int intCSID = -1)
        {
            var lstCurrentZoneEmployees = new List<CurrentZoneEmployees>();
           
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_GET_CurrentZoneEmployees";
            cmd.Parameters.Add("@ZoneID", SqlDbType.Int).Value = intZoneID;
            cmd.Parameters.Add("@CSID", SqlDbType.Int).Value = intCSID;
            cmd.Parameters.Add("@SelectedCompanyIDs", SqlDbType.VarChar).Value = strCompanyIDs;
            cmd.Parameters.Add("@SelectedDepartmentIDs", SqlDbType.VarChar).Value = strDepartmentIDs;
            cmd.Parameters.Add("@SelectedTradeIDs", SqlDbType.VarChar).Value = strTradeIDs;
            cmd.Parameters.Add("@USERID", SqlDbType.VarChar, 20).Value = "admin";
            cmd.Connection = con;
            try
            {
                con.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var employeeModel = new CurrentZoneEmployees
                    {
                        EmployeeID = rdr.GetString(rdr.GetOrdinal("EmployeeID")),
                        EmployeeKey = rdr.GetInt64(rdr.GetOrdinal("EmployeeKey")),
                        EmployeeName = rdr.GetString(rdr.GetOrdinal("EmployeeName")),
                        CompanyName = rdr.GetString(rdr.GetOrdinal("CompanyName")),
                        DepartmentName = rdr.GetString(rdr.GetOrdinal("DepartmentName")),
                        TradeName = rdr.GetString(rdr.GetOrdinal("TradeName")),
                        ClockDate = rdr.GetDateTime(rdr.GetOrdinal("ClockDate")),
                        TimeIn = rdr.GetDateTime(rdr.GetOrdinal("TimeIn"))
                    };
                    lstCurrentZoneEmployees.Add(employeeModel);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            return lstCurrentZoneEmployees;
        }
    }
    public class EmployeeCountModel
    {
        #region Properties  
        public int ZoneID { get; set; }
        public int EmployeeCount { get; set; }

        #endregion
    }

    public class CurrentZoneEmployees
    {
        public long EmployeeKey { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string CompanyName { get; set; }
        public string TradeName { get; set; }
        public DateTime ClockDate { get; set; }
        public DateTime TimeIn { get; set; }
        public bool HasIncident { get; set; }
        public bool IsCSEmployee { get; set; }
        public string GangwayName { get; set; }
        public string CSName { get; set; }
        public DateTime CSTimeIn { get; set; }
        public string DepartmentName { get; set; }
        public int IncidentType { get; set; }
    }
}
