using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using astorWorkDAO;

namespace astorWorkDAO
{
    public class ZoneDAO
    {
        #region Declaration
        astorWorkEntities objastorWorkEntities = null;
        public ZoneDAO()
        {
            objastorWorkEntities = new astorWorkEntities();
        }
        #endregion

        #region INS_UPD_DEL


        public bool InsertZone(ZoneMaster zoneEntity)
        {
            bool IsInsert = false;
            zoneEntity.ModifiedBy = zoneEntity.CreatedBy = "admin";
            zoneEntity.ModifiedDate = zoneEntity.CreatedDate = DateTime.UtcNow;
            try
            {
                objastorWorkEntities.ZoneMasters.Add(zoneEntity);
                objastorWorkEntities.SaveChanges();
                IsInsert = true;
            }
            catch (DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

            }
            return IsInsert;
        }

        public bool UpdateZone(ZoneMaster updatedZoneEntity)
        {
            bool IsUpdated = false;
            updatedZoneEntity.ModifiedBy = "admin";
            updatedZoneEntity.ModifiedDate = DateTime.UtcNow;
            try
            {
                var Zone = objastorWorkEntities.ZoneMasters.Find(updatedZoneEntity.ZoneID);
                Zone.ModifiedBy = "admin";
                Zone.ModifiedDate = DateTime.UtcNow;
                if (updatedZoneEntity.YardLayout != null)
                    Zone.YardLayout = updatedZoneEntity.YardLayout;
                Zone.ZoneDescription = updatedZoneEntity.ZoneDescription;
                Zone.ZoneCoordinates = updatedZoneEntity.ZoneCoordinates;
                Zone.ZoneName = updatedZoneEntity.ZoneName;
                Zone.ZoneColor = updatedZoneEntity.ZoneColor;
                objastorWorkEntities.SaveChanges();
                IsUpdated = true;
            }
            catch
            {

            }
            return IsUpdated;
        }
        #endregion

        #region GETMethods
        public List<ZoneMaster> GetAllZonesByYardID(int yardID)
        {
            List<ZoneMaster> lstYardMaster = null;

            try
            {
                lstYardMaster = objastorWorkEntities.ZoneMasters.Where(x => x.YardID == yardID).ToList();

            }
            catch (Exception ex)
            {

            }

            return lstYardMaster;
        }

        public ZoneMaster GetZoneDetails(int ZoneID)
        {
            try
            {
                List<ZoneMaster> objZoneDetailsList = objastorWorkEntities.ZoneMasters.Where(z => z.ZoneID == ZoneID).ToList();
                return objZoneDetailsList[0];
            }
            catch (Exception)
            {
                return null;
                throw;
            }

        }

        public List<usp_GET_YardSummaryInfo_Result> GetYardDetails(int yardID)
        {
            List<usp_GET_YardSummaryInfo_Result> lstYardSummaryInfo = null;

            try
            {
                lstYardSummaryInfo = objastorWorkEntities.usp_GET_YardSummaryInfo(yardID).ToList();

            }
            catch (Exception ex)
            {

            }

            return lstYardSummaryInfo;
        }
        public List<usp_GET_AstorSafeYardSummary_Result> GetAstorYardDetails(int yardID)
        {
            List<usp_GET_AstorSafeYardSummary_Result> lstYardSummaryInfo = null;

            try
            {
                lstYardSummaryInfo = objastorWorkEntities.usp_GET_AstorSafeYardSummary(yardID).ToList();

            }
            catch (Exception ex)
            {

            }

            return lstYardSummaryInfo;
        }

        public ZoneMapping GetMappingZoneID(int zoneID)
        {
            ZoneMapping lstZoneMappingInfo = null;

            try
            {
                lstZoneMappingInfo = objastorWorkEntities.ZoneMappings.Where(z => z.AstorWorkZoneID == zoneID).FirstOrDefault();

            }
            catch (Exception ex)
            {

            }

            return lstZoneMappingInfo;
        }

        public ZoneMapping GetMappingZoneID(int zoneID, int source)
        {
            ZoneMapping lstZoneMappingInfo = null;

            try
            {
                switch (source)
                {
                    case 0:
                        lstZoneMappingInfo = objastorWorkEntities.ZoneMappings.Where(z => z.AstorWorkZoneID == zoneID).FirstOrDefault();
                        break;
                    case 1:
                        lstZoneMappingInfo = objastorWorkEntities.ZoneMappings.Where(z => z.AstorSafeZoneID == zoneID).FirstOrDefault();
                        break;
                    case 2:
                        lstZoneMappingInfo = objastorWorkEntities.ZoneMappings.Where(z => z.AstorTrackZoneID == zoneID).FirstOrDefault();
                        break;
                }

            }
            catch (Exception ex)
            {

            }

            return lstZoneMappingInfo;
        }

        public bool CheckZoneNameExists(int intZoneID, string strZoneName)
        {
            bool IsExists = false;
            try
            {
                List<ZoneMaster> objZoneList = new List<ZoneMaster>();
                if (intZoneID == 0)
                    objZoneList = objastorWorkEntities.ZoneMasters.Where(Z => Z.ZoneName.ToLower() == strZoneName.ToLower()).ToList();
                else
                    objZoneList = objastorWorkEntities.ZoneMasters.Where(Z => Z.ZoneID != intZoneID && Z.ZoneName.ToLower() == strZoneName.ToLower()).ToList();
                IsExists = objZoneList.Count > 0;
            }
            catch (Exception)
            {

                throw;
            }
            return IsExists;
        }
        #endregion
    }
}
