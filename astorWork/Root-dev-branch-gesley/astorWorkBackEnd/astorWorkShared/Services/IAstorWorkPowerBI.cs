using astorWorkShared.Utilities;
using Microsoft.PowerBI.Api.V2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public interface IAstorWorkPowerBI
    {
        Task<PowerBIAuthResult> AuthenticatePowerBIAsync(PowerBICredentials credentials);

        Task<IEnumerable<Report>> GetReportsAsync(string pbToken, string workSpaceGUID);

        Task<EmbedToken> GetEmbedTokenAsync(string pbToken, string workSpaceGUID, string reportGUID);
    }

    public class PowerBIAuthResult
    {
        public string token_type;
        public string scope { get; set; }
        public int expires_in { get; set; }
        public long expires_on { get; set; }
        public long not_before { get; set; }
        public string resource { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
    }
}
