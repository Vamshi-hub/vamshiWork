using astorWorkUserManage.Models;
using System.Linq;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using astorWorkShared.Utilities;
using astorWorkShared.GlobalModels;

namespace astorWorkUserManage.Repositories
{
    public class TokenRepository 
    {
        IDistributedCache _cache;
        TenantInfo _tenant;

        TimeSpan AccessTokenExpiry = TimeSpan.FromHours(1);
        TimeSpan RefreshTokenExpiry = TimeSpan.FromDays(7);

        public TokenRepository(IDistributedCache cache, TenantInfo tenantInfo)
        {
            _cache = cache;
            _tenant = tenantInfo;
        }

        public bool AddToken(TokenResponse token)
        {
            try
            {
                var cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(AccessTokenExpiry);
                //_cache.SetString(token.refreshToken, JsonConvert.SerializeObject(token).ToString(), cacheEntryOptions);
                _cache.Set(token.accessToken, Guid.NewGuid().ToByteArray(), cacheEntryOptions);
                cacheEntryOptions = new DistributedCacheEntryOptions().SetSlidingExpiration(RefreshTokenExpiry);
                _cache.SetString(token.refreshToken, JsonConvert.SerializeObject(token).ToString(), cacheEntryOptions);

                return true;
            }
            catch { return false; }
        }

        public bool RemoveToken(string refresh_token)
        {
            try
            {
                _cache.Remove(refresh_token);
                return true;
            }
            catch { return false; }
        }

        public TokenResponse GetToken(string refresh_token)
        {
            try
            {
                var value = _cache.GetString(refresh_token);
                if (value != null)
                    return JsonConvert.DeserializeObject<TokenResponse>(value);
                else
                    return null;
            }
            catch(Exception exc)
            {
                Console.WriteLine("Redis cache failed: " + exc.Message);
                return null;
            }
        }
    }
}
