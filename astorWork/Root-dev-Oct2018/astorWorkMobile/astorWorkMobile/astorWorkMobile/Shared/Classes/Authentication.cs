using astorWorkMobile.MaterialTrack.Entities;
using astorWorkMobile.Shared.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace astorWorkMobile.Shared.Classes
{
    public class AuthResult
    {
        private string _refreshToken;
        public string RefreshToken
        {
            get
            {
                return _refreshToken;
            }
            set
            {
                _refreshToken = value;
                if (string.IsNullOrEmpty(_refreshToken))
                    App.Current.Properties.Remove("refresh_token");
                else
                    App.Current.Properties["refresh_token"] = value;
            }
        }

        private string _userID;
        public string UserID
        {
            get
            {
                return _userID;
            }
            set
            {
                _userID = value;
                int userId = 0;
                if (int.TryParse(_userID, out userId))
                {
                    if(userId > 0)
                        App.Current.Properties["user_id"] = userId;
                }
            }
        }

        private string _accessToken;
        public string AccessToken
        {
            get
            {
                return _accessToken;
            }
            set
            {
                _accessToken = value;
                if (string.IsNullOrEmpty(_accessToken))
                {
                    App.Current.Properties.Remove("access_token");
                }
                else
                {
                    App.Current.Properties["access_token"] = value;
                    try
                    {
                        string[] tokenComponents = _accessToken.Split('.');
                        if (tokenComponents.Length == 3)
                        {
                            var tokenPayloadBase64 = tokenComponents[1];
                            while (tokenPayloadBase64.Length % 4 != 0)
                                tokenPayloadBase64 += "=";
                            byte[] data = Convert.FromBase64String(tokenPayloadBase64);
                            string decodedString = Encoding.UTF8.GetString(data);
                            Payload = JsonConvert.DeserializeObject<AccessTokenPayload>(decodedString);
                            App.Current.Properties["vendor_id"] = Payload.VendorId;
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }
                }
            }
        }

        private int _expiresIn;
        public int ExpiresIn
        {
            get
            {
                return _expiresIn;
            }
            set
            {
                _expiresIn = value;
                if (_expiresIn > 0)
                {
                    Application.Current.Properties["access_token_expires"] = DateTime.UtcNow.AddSeconds(value);
                    Console.WriteLine($"{_expiresIn} seconds until session expires");
                    Device.StartTimer(TimeSpan.FromSeconds(value - 5 * 60), RefreshAuth);
                }
                else
                {
                    Application.Current.Properties.Remove("access_token_expires");
                }
            }
        }

        private bool RefreshAuth()
        {
            if (this != null && Application.Current.Properties.ContainsKey("access_token_expires"))
            {
                Console.WriteLine("Refreshing with auth result " + this.GetHashCode());
                Task.Run(() => ApiClient.Instance.AuthRefresh(RefreshToken, UserID)).ContinueWith((Task<ApiClient.ApiResult> t) =>
                {
                    var authResult = t.Result.data as AuthResult;
                    if (t.Result.status != 0)
                        Device.StartTimer(TimeSpan.FromSeconds(30), this.RefreshAuth);
                });
            }

            return false;
        }

        public string TokenType { get; set; }

        public AccessTokenPayload Payload { get; set; }
    }

    public class AccessTokenPayload
    {
        public String Sub { get; set; }
        public UserRole Role { get; set; }
        public string VendorId { get; set; }
    }

    public class UserRole
    {
        public int UserId { get; set; }
        public int MobileEntryPoint { get; set; }
    }
}
