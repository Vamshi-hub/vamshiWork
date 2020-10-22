using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace astorWorkDAO
{

    #region Entities

    public class LocationEntity
    {
        public int SNO { get; set; }
        public string ProjectName { get; set; }
        public string Project { get; set; }
        public string Coordinates { get; set; }
        public string PM { get; set; }
        public string Contractor { get; set; }
        public string Unit { get; set; }
        public string MOM_Reportable_Accident { get; set; }
        public string Accidents_fatalities { get; set; }
        public string NonMom_Reportable_Incident { get; set; }
        public string Workers_Site { get; set; }
        public string Manhours_Month { get; set; }
        public string Mandays_lost_month { get; set; }
        public string Dangerous_Occurrence { get; set; }
        public string NearMiss { get; set; }
        public string Division { get; set; }
        public double AFR { get; set; }
        public double ASR { get; set; }
        public double FAR { get; set; }
        public double Incident_Accumulated { get; set; }
        public string ProjectDesc { get; set; }
        public string ProjectType { get; set; }
        public DateTime LastInspectionDate { get; set; }
    }
    #endregion

    public class IncidentDAO
    {

        string _strConnectionString = ConfigurationManager.ConnectionStrings["astorIncident"].ConnectionString;
        public List<LocationEntity> getLocations()
        {
            int _sno = 0;
            List<LocationEntity> lstLocations = new List<LocationEntity>();
            string strText = "dbo.usp_GET_AllLocations";
            using (SqlDataReader reader = IncidentSQLHelper.ExecuteReader(_strConnectionString, CommandType.StoredProcedure, strText))
            {

                while (reader.Read())
                {
                    string[] strCoordinates = Regex.Split(reader["Coordinates"].ToString(), "and");
                    LocationEntity locEntity = new LocationEntity()
                    {
                        Project = reader["Project"].ToString(),
                        Coordinates = strCoordinates[0],
                        PM = reader["PM"].ToString(),
                        Contractor = reader["Contractor"].ToString(),

                        MOM_Reportable_Accident = reader["MOM_Reportable_Accident"].ToString()
                        ,

                        NonMom_Reportable_Incident = reader["NonMom_Reportable_Incident"].ToString()
                        ,
                        Workers_Site = reader["Workers_Site"].ToString()
                        ,

                        NearMiss = reader["NearMiss"].ToString()
                        ,
                        SNO = int.Parse(reader["SNO"].ToString())
                        ,
                        ProjectDesc = reader["ProjectDesc"].ToString(),
                        Division = reader["Division"].ToString(),
                        Incident_Accumulated = int.Parse(reader["Incident_Accumulated"].ToString())
                        ,
                        ProjectType = reader["ProjectType"].ToString()
                        ,
                        ASR = double.Parse(reader["ASR"].ToString())
                        ,
                        AFR = double.Parse(reader["AFR"].ToString())
                        ,
                        FAR = double.Parse(reader["FAR"].ToString())
                        ,
                        LastInspectionDate = Convert.ToDateTime(reader["LastInspectionDate"].ToString())
                    };
                    lstLocations.Add(locEntity);
                    if (strCoordinates.Count() > 1)
                    {
                        locEntity.Coordinates = strCoordinates[1];
                        lstLocations.Add(locEntity);
                    }

                }
            }
            return lstLocations;
        }

        public List<LocationEntity> getASR_AFR_Line()
        {
            List<LocationEntity> lstLocations = new List<LocationEntity>();
            string strText = "select [Division],SUM(CASE WHEN AFR IS NULL THEN 0 ELSE AFR END) AFR,SUM(CASE WHEN ASR IS NULL THEN 0 ELSE ASR END) ASR from LocationMaster WHERE Month='June' GROUP BY Division";
            using (SqlDataReader reader = IncidentSQLHelper.ExecuteReader(_strConnectionString, CommandType.Text, strText))
            {

                while (reader.Read())
                {
                    LocationEntity locEntity = new LocationEntity()
                    {
                        Division = reader["Division"].ToString()
                       ,
                        ASR = double.Parse(reader["ASR"].ToString())
                        ,
                        AFR = double.Parse(reader["AFR"].ToString())

                    };
                    lstLocations.Add(locEntity);

                }
            }
            return lstLocations;
        }

        public DataTable getIncidentDetails(string strProject, string strDivision, string strContractor)
        {
            DataTable dt ;
            List<LocationEntity> lstLocations = new List<LocationEntity>();
            string strText = "dbo.usp_GET_IncidentDetails";
            SqlParameter[] prms = new SqlParameter[]
            {
                new SqlParameter("@Project",strProject)
                ,new SqlParameter("@Division",strDivision)
                ,new SqlParameter("@Contractor",strContractor)
            };
            dt = IncidentSQLHelper.ExecuteDataset(_strConnectionString, CommandType.StoredProcedure, strText, prms).Tables[0];
            return dt;
        }

        public DataTable getInspectionDetails(string strProject, string strDivision, string strContractor)
        {
            DataTable dt;
            List<LocationEntity> lstLocations = new List<LocationEntity>();
            string strText = "[dbo].[usp_GET_InspectionDetails]";
            SqlParameter[] prms = new SqlParameter[]
            {
                new SqlParameter("@Project",strProject)
                ,new SqlParameter("@Division",strDivision)
                ,new SqlParameter("@Contractor",strContractor)
            };
            dt = IncidentSQLHelper.ExecuteDataset(_strConnectionString, CommandType.StoredProcedure, strText, prms).Tables[0];
            return dt;
        }
    }
}
