using astorWorkNavis.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace astorWorkNavis.Classes
{
    public class AuthResult
    {
        private Timer _refreshTimer;

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
                Properties.Settings.Default.ACCESS_TOKEN = value;

                if (!string.IsNullOrEmpty(_accessToken))
                {
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
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.Message);
                    }
                }
            }
        }

        private string _refreshToken;
        public string RefreshToken { get
            {
                return _refreshToken;
            }
            set
            {
                _refreshToken = value;
                Properties.Settings.Default.REFRESH_TOKEN = value;

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
                Properties.Settings.Default.ACCESS_TOKEN_EXPIRES = DateTimeOffset.UtcNow.AddSeconds(value);
                StartRefreshTimer(value);
            }
        }

        private void StartRefreshTimer(int expireSeconds)
        {
            try
            {
                if (_refreshTimer != null && _refreshTimer.Enabled)
                {
                    _refreshTimer.Stop();
                    _refreshTimer.Dispose();
                }

                _refreshTimer = new Timer((expireSeconds - 60) * 1000);
                _refreshTimer.Elapsed += (sender, e) => StartRefreshToken();
                _refreshTimer.AutoReset = false;
                _refreshTimer.Start();
            }
            catch (Exception exc)
            {
                Console.Write(exc);
            }
        }

        private void StartRefreshToken()
        {
            // Check if the object is removed from memory
            if (this != null)
            {
                Task.Run(() => ApiClient.Instance.AuthRefresh(_refreshToken)).ContinueWith(t =>
                {
                    // If cannot refresh, retry in 30 seconds
                    if (t.Result.Status != 0)
                    {
                        StartRefreshTimer(30);
                    }
                });
            }
        }

        public string TokenType { get; set; }

        public AccessTokenPayload Payload { get; set; }
    }

    public class AccessTokenPayload
    {
        public String Sub { get; set; }
        public UserRole Role { get; set; }
    }

    public class UserRole
    {
        public int UserId { get; set; }
        public int MobileEntryPoint { get; set; }
    }
}
