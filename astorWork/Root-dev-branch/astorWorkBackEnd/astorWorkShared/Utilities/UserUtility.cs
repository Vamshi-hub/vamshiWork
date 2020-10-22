using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace astorWorkShared.Utilities
{
    public static class UserUtility
    {
        public static string REDIS_ACCESS_TOKEN_KEY = "ACCESS_TOKEN_{0}_{1}";
        public static string REDIS_REFRESH_TOKEN_KEY = "REFRESH_TOKEN_{0}_{1}";

        public static string HashPassword(string plainPwd, string salt)
        {
            var pwdHashed = string.Empty;
            using (var algorithm = SHA256.Create())
            {
                var pwdSalted = Encoding.ASCII.GetBytes(plainPwd).Concat(new Guid(salt).ToByteArray());
                var hash = algorithm.ComputeHash(pwdSalted.ToArray());
                pwdHashed = Convert.ToBase64String(hash);
            }
            return pwdHashed;
        } 
        
        public static string GenerateRandomPassword(int length)
        {
            //create constant strings for each type of characters
            string alphaCaps = "QWERTYUIOPASDFGHJKLZXCVBNM";
            string alphaLow = "qwertyuiopasdfghjklzxcvbnm";
            string numerics = "1234567890";
            string special = "@#$";

            //create another string which is a concatenation of all above
            string allChars = alphaCaps + alphaLow + numerics + special;

            Random r = new Random();
            String generatedPassword = "";
            for (int i = 0; i < length; i++)
            {
                double rand = r.NextDouble();
                if (i == 0)
                    //First character is an upper case alphabet
                    generatedPassword += alphaCaps.ToCharArray()[(int)Math.Floor(rand * alphaCaps.Length)];
                else
                    generatedPassword += allChars.ToCharArray()[(int)Math.Floor(rand * allChars.Length)];
            }

            return generatedPassword;
        }
    }
}
