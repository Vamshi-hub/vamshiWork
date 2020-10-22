using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using astorWorkDAO;

namespace astorWorkDAO
{
    public class UsersDAO
    {
        #region Declaration
        astorWorkEntities objastorWorkEntities = null;
        public UsersDAO()
        {
            objastorWorkEntities = new astorWorkEntities();
        }
        #endregion

        #region GET
        public List<UserMaster> getAllUsers()
        {
            List<UserMaster> objUsersList;
            try
            {
                objUsersList = objastorWorkEntities.UserMasters.ToList();

            }
            catch (Exception)
            {

                throw;
            }
            return objUsersList;
        }

        public bool CheckUserIDExists(string UserID)
        {
            bool Result = true;
            try
            {
                List<UserMaster> objUserList = objastorWorkEntities.UserMasters.Where(UM => UM.UserID == UserID).ToList();
                Result = objUserList.Count > 0;
            }
            catch (Exception)
            {

                throw;
            }
            return Result;
        }

        public UserMaster GetUserByUserID(string UserID)
        {
            UserMaster objUserMaster;
            try
            {
                objUserMaster = objastorWorkEntities.UserMasters.Where(UM => UM.UserID == UserID).First();
            }
            catch (Exception)
            {
                
                throw;
            }
            return objUserMaster;
        }

        public ConfigurationMaster GetastorTimeDB()
        {
            ConfigurationMaster objConfigurationMaster;
            try
            {
                objConfigurationMaster = objastorWorkEntities.ConfigurationMasters.Where(UM => UM.Configuration == "astorTimeCustomer").First();
            }
            catch (Exception)
            {

                throw;
            }
            return objConfigurationMaster;
        }
        #endregion

        #region INS_UPD_DEL
        public bool insertUser(UserMaster  objUserMaster)
        {
            bool isInserted = false;
            try
            {
                objastorWorkEntities.UserMasters.Add(objUserMaster);
                objastorWorkEntities.SaveChanges();
                isInserted = true;
            }
            catch (Exception)
            {
                
                throw;
            }
            return isInserted;
        }

        public bool updateUser(UserMaster objUserMaster)
        {
            bool isUpdated = false;
            try
            {
                var originalUser = objastorWorkEntities.UserMasters.Find(objUserMaster.UserID);
                originalUser.UserName = objUserMaster.UserName;
                originalUser.UserPwd = objUserMaster.UserPwd;
                objastorWorkEntities.SaveChanges();
                isUpdated = true;
            }
            catch (Exception)
            {

                throw;
            }
            return isUpdated;
        }
        #endregion
    }
}
