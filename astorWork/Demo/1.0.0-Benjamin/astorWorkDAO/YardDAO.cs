using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkDAO
{
  public  class YardDAO
    {
        #region Declaration
        astorWorkEntities objastorWorkEntities = null;
        public YardDAO()
        {
            objastorWorkEntities = new astorWorkEntities();
        }
        #endregion
        #region Methods
        public List<YardMaster> GetAllYards()
        {
            List<YardMaster> lstYardMaster = null;
            // base.SetLog(this.GetType().ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name.ToString(), base.LogCategory);
            try
            {
                lstYardMaster = objastorWorkEntities.YardMasters.OrderBy(YM => YM.YardName).ToList();
                // base.Log.LogType = (int)Enums.LogType.Information;
            }
            catch (Exception ex)
            {
                //base.Log.LogType = (int)Enums.LogType.Error;
                //base.Log.Severity = (int)Enums.LogSeverity.Critical;
                //base.Log.ExceptionMessage = ex.Message + ex.StackTrace;
                //base.ThrowException(ex);
            }
            finally
            {
                //base.Log.ProcessEndTime = base.RecordTimeStamp;
                //base.WriteLog();
            }
            return lstYardMaster;
        }
        public List<YardMaster> GetAllZoneYards()
        {
            List<YardMaster> lstYardMaster = null;
        //    base.SetLog(this.GetType().ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name.ToString(), base.LogCategory);
            try
            {
                lstYardMaster = (from y in objastorWorkEntities.YardMasters
                                 orderby y.YardName
                                 select new
                                 {
                                     y.YardID,
                                     y.YardName,
                                     y.UTCOffset,
                                     y.IsDefault,
                                     y.EnableYardTimeCapture
                                 }).ToList().Select(d => new YardMaster
                                 {
                                     YardID = d.YardID,
                                     YardName = d.YardName,
                                     IsDefault = d.IsDefault,
                                     UTCOffset = d.UTCOffset,
                                     EnableYardTimeCapture = d.EnableYardTimeCapture
                                 }).ToList();


             //   base.Log.LogType = (int)Enums.LogType.Information;
            }
            catch (Exception ex)
            {
                //base.Log.LogType = (int)Enums.LogType.Error;
                //base.Log.Severity = (int)Enums.LogSeverity.Critical;
                //base.Log.ExceptionMessage = ex.Message + ex.StackTrace;
                //base.ThrowException(ex);
            }
            finally
            {
                //base.Log.ProcessEndTime = base.RecordTimeStamp;
                //base.WriteLog();
            }
            return lstYardMaster;
        }
        public YardMaster GetYardByYardID(int intYardID)
        {
            YardMaster objYardMaster = null;
          //  base.SetLog(this.GetType().ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name.ToString(), base.LogCategory);
            try
            {
                objYardMaster = objastorWorkEntities.YardMasters.Where(ym => ym.YardID == intYardID).FirstOrDefault();
                //base.Log.LogType = (int)Enums.LogType.Information;
            }
            catch (Exception ex)
            {
                //base.Log.LogType = (int)Enums.LogType.Error;
                //base.Log.Severity = (int)Enums.LogSeverity.Critical;
                //base.Log.ExceptionMessage = ex.Message + ex.StackTrace;
                //base.ThrowException(ex);
            }
            finally
            {
                //base.Log.ProcessEndTime = base.RecordTimeStamp;
                //base.WriteLog();
            }
            return objYardMaster;
        }
        public YardMaster GetYardByDeafault()
        {
            YardMaster objYardMaster = null;
            //  base.SetLog(this.GetType().ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name.ToString(), base.LogCategory);
            try
            {
                objYardMaster = objastorWorkEntities.YardMasters.Where(ym => ym.IsDefault == true).FirstOrDefault();
                //base.Log.LogType = (int)Enums.LogType.Information;
            }
            catch (Exception ex)
            {
                //base.Log.LogType = (int)Enums.LogType.Error;
                //base.Log.Severity = (int)Enums.LogSeverity.Critical;
                //base.Log.ExceptionMessage = ex.Message + ex.StackTrace;
                //base.ThrowException(ex);
            }
            finally
            {
                //base.Log.ProcessEndTime = base.RecordTimeStamp;
                //base.WriteLog();
            }
            return objYardMaster;
        }
        public bool CheckYardName(string strYardName)
        {
            bool blnStatus = false;
          //  base.SetLog(this.GetType().ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name.ToString(), base.LogCategory);
            try
            {
                blnStatus = objastorWorkEntities.YardMasters.Any(y => y.YardName.Equals(strYardName, StringComparison.InvariantCultureIgnoreCase));
            //    base.Log.LogType = (int)Enums.LogType.Information;
            }
            catch (Exception ex)
            {
                //base.Log.LogType = (int)Enums.LogType.Error;
                //base.Log.Severity = (int)Enums.LogSeverity.Critical;
                //base.Log.ExceptionMessage = ex.Message + ex.StackTrace;
                //base.ThrowException(ex);
            }
            finally
            {
                //base.Log.ProcessEndTime = base.RecordTimeStamp;
                //base.WriteLog();
            }
            return blnStatus;
        }
        public bool InsertYard(YardMaster objYardMaster)
        {
            bool IsInsert = false;
            List<YardMaster> lstYardMaster = null;
            try
            {

                if (objYardMaster.YardID == 0)
                {
                    lstYardMaster = GetAllYards();
                    objYardMaster.YardID = lstYardMaster.Count > 0 ? lstYardMaster.Max(Yid => Yid.YardID) + 1 : 1;
                    objastorWorkEntities.YardMasters.Add(objYardMaster);
                }
               
                objastorWorkEntities.SaveChanges();
                IsInsert = true;
            }
            catch
            {

            }
            return IsInsert;
        }
       
        public List<TimeZoneMaster> GetAllTimeZones()
        {
            List<TimeZoneMaster> lstTimeZone = null;
         //   base.SetLog(this.GetType().ToString(), System.Reflection.MethodBase.GetCurrentMethod().Name.ToString(), base.LogCategory);
            try
            {
                lstTimeZone = (from timeZone in objastorWorkEntities.TimeZoneMasters.OrderBy(TM => TM.TimeZoneID)
                               select new
                               {
                                   timeZone.TimeZoneID,
                                   timeZone.DisplayName,
                                   timeZone.UTCOffset

                               }).ToList().Select(t => new TimeZoneMaster
                               {
                                   TimeZoneID = t.TimeZoneID,
                                   DisplayName = t.DisplayName,
                                   UTCOffset = t.UTCOffset,
                               }).ToList();
              //  base.Log.LogType = (int)Enums.LogType.Information;
            }
            catch (Exception ex)
            {
                //base.Log.LogType = (int)Enums.LogType.Error;
                //base.Log.Severity = (int)Enums.LogSeverity.Critical;
                //base.Log.ExceptionMessage = ex.Message + ex.StackTrace;
                //base.ThrowException(ex);
            }
            finally
            {
                //base.Log.ProcessEndTime = base.RecordTimeStamp;
                //base.WriteLog();
            }
            return lstTimeZone;
        }
        #endregion


    }
}
