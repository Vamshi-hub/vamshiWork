using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkDAO
{
    public class MaterialDAO
    {
        #region Declaration
        string ConnectionString = ConfigurationManager.ConnectionStrings["astorTrack"].ToString();
        SqlDataReader rdr = null;
        SqlConnection con = null;
        SqlCommand cmd = null;
        #endregion

        public List<MaterialDetail> GetListMaterialDetail()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
@"SELECT [MaterialNo], [MarkingNo], [Stage], [CreatedDate]
  FROM[dbo].[MaterialDetail]";
            cmd.Connection = con;

            List<MaterialDetail> result = new List<MaterialDetail>();
            try
            {
                con.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var materialDetail = new MaterialDetail()
                    {
                        MaterialNo = rdr.GetString(rdr.GetOrdinal("MaterialNo")),
                        MarkingNo = rdr.GetString(rdr.GetOrdinal("MarkingNo")),
                        Stage = rdr.GetString(rdr.GetOrdinal("Stage")),
                        CreatedDate = rdr.GetDateTime(rdr.GetOrdinal("CreatedDate"))
                    };
                    result.Add(materialDetail);
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

            return result;
        }

        public List<MaterialMaster> GetActiveMaterials()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
@"SELECT [MaterialNo], [MarkingNo], [Status], [RFIDTagID], [LocationID]
  FROM [dbo].[MaterialMaster]
  WHERE [Status] IN ('Delivered', 'Installed', 'Produced')";
            cmd.Connection = con;

            List<MaterialMaster> result = new List<MaterialMaster>();
            try
            {
                con.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var materialMaster = new MaterialMaster()
                    {
                        MaterialNo = rdr.GetString(rdr.GetOrdinal("MaterialNo")),
                        MarkingNo = rdr.GetString(rdr.GetOrdinal("MarkingNo")),
                        Status = rdr.GetString(rdr.GetOrdinal("Status")),
                        RFIDTagID = rdr.GetString(rdr.GetOrdinal("RFIDTagID"))
                    };
                    result.Add(materialMaster);
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

            return result;
        }

        public List<MaterialMaster> GetActiveMaterialsByZone(int zoneID)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText =
@"SELECT [MaterialNo], [MarkingNo], [Status], [RFIDTagID], [LocationID]
  FROM [dbo].[MaterialMaster]
  WHERE [Status] IN ('Delivered', 'Installed', 'Produced','Requested') AND [MRFNo] IS NOT NULL AND [LocationID] = " + zoneID;
            cmd.Connection = con;

            List<MaterialMaster> result = new List<MaterialMaster>();
            try
            {
                con.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    int rdrOrdinal = rdr.GetOrdinal("RFIDTagID");
                    string RFIDTag = "";
                    //string status = rdr.GetString(rdr.GetOrdinal("Status"));

                    if (!rdr.IsDBNull(rdrOrdinal))
                        RFIDTag = rdr.GetString(rdrOrdinal);

                    var materialMaster = new MaterialMaster()
                    {
                        MaterialNo = rdr.GetString(rdr.GetOrdinal("MaterialNo")),
                        MarkingNo = rdr.GetString(rdr.GetOrdinal("MarkingNo")),
                        Status = rdr.GetString(rdr.GetOrdinal("Status")),
                        RFIDTagID = RFIDTag
                    };
                    result.Add(materialMaster);
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

            return result;
        }

        public List<MaterialCountModel> GetActiveMaterialCount()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = @"SELECT * FROM vw_MaterialCountByLocation";
            cmd.Connection = con;


            List<MaterialCountModel> result = new List<MaterialCountModel>();
            try
            {
                con.Open();
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var materialCount = new MaterialCountModel()
                    {
                        MaterialCount = rdr.GetInt32(0),
                        ZoneID = rdr.GetInt64(1),
                    };
                    result.Add(materialCount);
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

            return result;
        }
    }

    public class MaterialCountModel
    {
        public long ZoneID { get; set; }
        public int MaterialCount { get; set; }
    }

    public class MaterialMaster
    {
        public string MaterialNo { get; set; }
        public string Status { get; set; }
        public string MarkingNo { get; set; }
        public string RFIDTagID { get; set; }
        public int LocationID { get; set; }
    }

    public class MaterialDetail
    {
        public string MaterialNo { get; set; }
        public string Stage { get; set; }
        public DateTime CreatedDate { get; set; }
        public string MarkingNo { get; set; }
    }
}
