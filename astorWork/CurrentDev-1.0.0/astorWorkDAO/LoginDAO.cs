
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using astorWorkDAO;

namespace astorWorkDAO
{
   
    public class LoginDAO
    {
        #region Declaration
        astorWorkEntities objastorWorkEntities;
        public LoginDAO()
        {
            objastorWorkEntities = new astorWorkEntities();
        }
        #endregion

        #region Methods
        public int LoginValidation(string strUserID, string strPassword, Guid tenantID)
        {

            return ValidateUser(strUserID, strPassword);
        }

        public int ValidateUser(string strUserID, string strPassword)
        {
            int intResult = -1;
            try
            {
                UserMaster objUser = objastorWorkEntities.UserMasters.Where(u => u.UserID.ToLower() == strUserID).FirstOrDefault();
                if (objUser == null)
                    intResult = 1;
                //else if (objUser.Status == 2)
                //    intResult = 2;
                else if (objUser.UserPwd != strPassword)
                    intResult = 3;
                else
                    intResult = 4;
            }
            catch (Exception)
            {
                intResult = -1;
            }
            
            return intResult;
        }


      
        #endregion
    }
}
