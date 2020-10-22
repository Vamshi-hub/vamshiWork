using astorWorkShared.Models;
using Autodesk.Forge.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public interface IAstorWorkForge
    {
        Task<ForgeToken> GetAuthenticationResult();
        Task<IEnumerable<ForgeBucket>> GetBuckets();
        Task<ForgeBucket> AddBucket(ForgeBucket bucket);
        Task DeleteBucket(string bucketKey);

        Task<IEnumerable<ForgeObject>> GetObjects(string bucketId);
        Task<ForgeObject> AddObject(ForgeObject forgeObject);
        Task<dynamic> TranslateObject(string objectId);
        Task DeleteObject(string bucketKey, string objectKey);

        Task<DynamicDictionary> GetManifest(string objectId);
        Task<DynamicDictionary> GetMetadata(string objectId);
        Task<DynamicDictionary> GetViewMetadata(string objectId, string viewGUID);
    }
}
