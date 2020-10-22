using astorWorkDAO.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkDAO
{
    public class astorTimeDAO
    {
        #region Declaration
        SqlConnection _sqlConnection;
        #endregion

        #region Constructors
        public astorTimeDAO(string DBConnectionString)
        {
            _sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["astorTime-" + DBConnectionString].ConnectionString);
        }
        #endregion

        #region PublicMethods

        public bool ImportRawClockTime(DataTable  dtRawClockTime, string strAdapterCode, int intLocationID = -1)
        {
            int intTotalRecords = 0;
            int intInvalidBioIDRecords = 0;
            int intInvalidLocRecords = 0;
            int intValidRecords = 0;

            DateTime dtClockTime = default(DateTime);

            string strException = string.Empty;
            bool blnResult = false;

            if (_sqlConnection.State == ConnectionState.Closed)
                _sqlConnection.Open();
            SqlTransaction sqlTransaction = null;
            try
            {
                intTotalRecords = dtRawClockTime.Rows.Count;

                sqlTransaction = _sqlConnection.BeginTransaction(IsolationLevel.ReadCommitted);
                string strSQL = "usp_SAV_RawClockTime";

                foreach (DataRow drClockTime in dtRawClockTime.Rows)
                {
                    SqlCommand sqlCmd = new SqlCommand(strSQL, _sqlConnection, sqlTransaction);

                    if (strAdapterCode.ToUpper() == "ASTORIA")
                        dtClockTime = (DateTime)drClockTime["RawClockTime"];

                    dtClockTime = dtClockTime.AddSeconds(-dtClockTime.Second);
                    dtClockTime = dtClockTime.AddMilliseconds(-dtClockTime.Millisecond);

                    SqlParameter[] _params = new SqlParameter[]
                    {
                        new SqlParameter("@BiometricID",drClockTime["BiometricID"].ToString())
                        ,new SqlParameter("@ClockTime",dtClockTime)
                         ,new SqlParameter("@LocationID",intLocationID)
                          ,new SqlParameter("@DeviceID",-1)
                           ,new SqlParameter("@AdapterCode",strAdapterCode)
                            ,new SqlParameter("@UserID","System")
                            ,new SqlParameter("@LocationName",drClockTime["LocationName"].ToString())
                             ,new SqlParameter("@EmployeeID",drClockTime["EmployeeID"].ToString())
                             ,new SqlParameter("@Direction",drClockTime["Direction"].ToString())
                              ,new SqlParameter("@RecordTimestamp",DateTime.Now)
                               ,new SqlParameter("@InvalidBioIDRec",1) {Direction=ParameterDirection.Output }
                                ,new SqlParameter("@InvalidLocRec",1) {Direction=ParameterDirection.Output }
                                 ,new SqlParameter("@ValidRec",1) {Direction=ParameterDirection.Output }
                    };

                    sqlCmd.Parameters.AddRange(_params);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.ExecuteNonQuery();

                    intInvalidBioIDRecords = intInvalidBioIDRecords + Int32.Parse(sqlCmd.Parameters["@InvalidBioIDRec"].Value.ToString());
                    intInvalidLocRecords = intInvalidLocRecords + Int32.Parse(sqlCmd.Parameters["@InvalidLocRec"].Value.ToString());
                    intValidRecords = intValidRecords + Int32.Parse(sqlCmd.Parameters["@ValidRec"].Value.ToString());
                }
                sqlTransaction.Commit();
                blnResult = true;

            }
            catch (Exception ex)
            {
                blnResult = false;
                strException = ex.Message;
                sqlTransaction.Rollback();
            }
            finally
            {
                if (_sqlConnection.State == ConnectionState.Open)
                    _sqlConnection.Close();

                sqlTransaction.Dispose();

            }

            return blnResult;
        }

        #region ConfigurationMaster
        public ConfigSetting GetConfiguration()
        {

            string strQuery = "Select Configuration,Setting From ConfigurationMaster";
            SqlCommand sqlCmd = new SqlCommand(strQuery, _sqlConnection);
            sqlCmd.CommandType = System.Data.CommandType.Text;
            ConfigSetting objConfig = new ConfigSetting();
            try
            {
                _sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCmd.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    PropertyInfo objProp = objConfig.GetType().GetProperty(sqlDataReader["Configuration"].ToString());
                    objProp.SetValue(objConfig, Convert.ChangeType(sqlDataReader["Setting"].ToString(), objProp.PropertyType), null);
                }
                _sqlConnection.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return objConfig;
        }
        #endregion

        #region JobDeploymentDetail Report
        public JobAllocationRpts DisplayJobDeploymentDetailRpt(DateTime FromDate, DateTime ToDate)
        {

            return DisplayJobDeploymentDetailRpt(FromDate, ToDate, "-1", GetAllCompanyIds(), GetAllGroupIds(), GetAllTradeIds(), "", "", "-1");
        }
        #endregion

        #endregion

        #region PrivateMethods

        private string GetAllCompanyIds()
        {
            string strQuery = "DECLARE   @ConcatString VARCHAR(4000);SELECT @ConcatString = COALESCE(@ConcatString + ',', '') + CAST(CompanyId AS VARCHAR(10)) FROM CompanyMaster;SELECT @ConcatString AS Companies ";
            SqlCommand sqlCmd = new SqlCommand(strQuery, _sqlConnection);
            sqlCmd.CommandType = System.Data.CommandType.Text;
            string strCompany = "";
            try
            {
                _sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCmd.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    strCompany = sqlDataReader["Companies"].ToString();
                }
                _sqlConnection.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return strCompany;
        }

        private string GetAllGroupIds()
        {
            string strQuery = "DECLARE   @ConcatString VARCHAR(4000);SELECT @ConcatString = COALESCE(@ConcatString + ',', '') + CAST(GroupID AS VARCHAR(10)) FROM GroupMaster;SELECT @ConcatString AS Groups";
            SqlCommand sqlCmd = new SqlCommand(strQuery, _sqlConnection);
            sqlCmd.CommandType = System.Data.CommandType.Text;
            string strGroups = "";
            try
            {
                _sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCmd.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    strGroups = sqlDataReader["Groups"].ToString();
                }
                _sqlConnection.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return strGroups;
        }

        private string GetAllTradeIds()
        {
            string strQuery = "DECLARE   @ConcatString VARCHAR(4000);SELECT @ConcatString = COALESCE(@ConcatString + ',', '') + CAST(TradeID AS VARCHAR(10)) FROM TradeMaster;SELECT @ConcatString AS Trades";
            SqlCommand sqlCmd = new SqlCommand(strQuery, _sqlConnection);
            sqlCmd.CommandType = System.Data.CommandType.Text;
            string strTrades = "";
            try
            {
                _sqlConnection.Open();
                SqlDataReader sqlDataReader = sqlCmd.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    strTrades = sqlDataReader["Trades"].ToString();
                }
                _sqlConnection.Close();
            }
            catch (Exception ex)
            {
                return null;
            }

            return strTrades;
        }

        private JobAllocationRpts DisplayJobDeploymentDetailRpt(DateTime FromDate, DateTime ToDate, string strEmployeeKeys, string CompanyIDs, string GroupIDs, string TradeIDs, string strJobNo, string strSubCode, string strDataSource)
        {

            SqlCommand sqlCmd = new SqlCommand("usp_GET_JobDeploymentDetail_RPT", _sqlConnection);
            sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

            SqlParameter[] sqlParams = new SqlParameter[] {
                new SqlParameter("@FromDate",FromDate),
                new SqlParameter( "@ToDate", ToDate),
                new SqlParameter( "@EmployeeKeys", strEmployeeKeys),
                new SqlParameter( "@CompanyIDs", CompanyIDs),
                new SqlParameter( "@GroupIDs", GroupIDs),
                new SqlParameter( "@TradeIDs", TradeIDs),
                new SqlParameter( "@JobNos", strJobNo),
                new SqlParameter( "@SubCodes", strSubCode),
                new SqlParameter( "@DataSource", strDataSource)
        };
            sqlCmd.Parameters.AddRange(sqlParams);

            JobAllocationRpts dsJobDeploymentDetail = new JobAllocationRpts();
            try
            {
                sqlCmd.CommandTimeout = 500;
                _sqlConnection.Open();
                SqlDataAdapter da = new SqlDataAdapter(sqlCmd);
                da.Fill(dsJobDeploymentDetail.Tables["JobDeploymentDetail"]);
                _sqlConnection.Close();


            }
            catch (Exception ex)
            {
                return null;
            }

            return dsJobDeploymentDetail;
        }
        #endregion

    }
    public class ConfigSetting
    {
        public bool EnableTimeCleanerLeaveEntry { get; set; }
        public bool EnableClockTimeAdjustment { get; set; }
        public bool EnableDefaultShiftTiming { get; set; }
        public bool EnableJobAllocationInTimeCleaner { get; set; }
        public bool EnableSubCodeInTimeCleaner { get; set; }
        public bool EnableFirstTimeInLastTimeOut { get; set; }
        public bool EnableNoBreakInTimeCleaner { get; set; }
        public bool EnableEarlyOTInTimeCleaner { get; set; }
        public bool EnableShiftStartEndTime { get; set; }
        public bool EnableSubCodeInJobAllocation { get; set; }
        public bool IncludeFWLInProjectCosting { get; set; }
        public bool IncludeAllowanceInProjectCosting { get; set; }
        public bool EnableSPSelectionInTimeCleaner { get; set; }
        public bool AlignJobDeploymentTimeWithCCT { get; set; }
        public bool UseAstorSafeJobNoInJobDeployment { get; set; }
        public bool EnableFlexibleOffday { get; set; }
        public bool AllowAdhocJobEntryInHandheld { get; set; }
        public bool EnableActivityEntryInTimeCleaner { get; set; }
        public bool EnableJobCodeInJobNoMaster { get; set; }

        public string TimeCleanerPolicy { get; set; }
        public string TimeProcessorPolicy { get; set; }
        public string JobBasedTimeProcessorPolicy { get; set; }
        public string TSPayrollIntegration { get; set; }
        public string JobAllocationPage { get; set; }
        public string SSPLSubConImport { get; set; }

        public int CustomerUTCOffset { get; set; }
        public int SafeSmartDeviceFailDuration { get; set; }
        public string CustomerCurrency { get; set; }
        public int FingerprintMatchScoreThreshold { get; set; }
        public int FingerprintMatchGoodScore { get; set; }
        public int MaxRestrictedConsecutiveWorkDays { get; set; }
        public int LastAssignedJobNoDuration { get; set; }
        public bool UseIndividualOTRateSalaryCalc { get; set; }
        public bool EnableBarCodeScan { get; set; }
        public string DefaultTransitJobNo { get; set; }
        public bool AutoFillJobTransitSlot { get; set; }
        public string DefaultTransitSubCode { get; set; }

        public bool FingerprintMatchAtCodeLevel { get; set; }
        public bool ValidateBarcodeWithFINICNo { get; set; }

        public bool AutoFilledTimings { get; set; }

    }
}
