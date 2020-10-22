using astorWorkShared.Models;
using Autodesk.Forge;
using Autodesk.Forge.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public class AstorWorkForge : IAstorWorkForge
    {
        private string _clientId;
        private string _clientSecret;

        private ForgeToken _token;

        private BucketsApi _bucketsApi;
        private TwoLeggedApi _authApi;
        private ObjectsApi _objectApi;
        private DerivativesApi _derivativesApi;

        public ForgeToken Token
        {
            get
            {
                if (_token == null || _token.ExpireTime < DateTime.Now.AddMinutes(5))
                {
                    _token = GetFullAccessToken().Result;
                }

                return _token;
            }
        }

        public AstorWorkForge(string clientId, string clientSecret)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            _authApi = new TwoLeggedApi();
            var conf = new Autodesk.Forge.Client.Configuration
            {
                AccessToken = Token.access_token
            };
            _bucketsApi = new BucketsApi(conf);
            _objectApi = new ObjectsApi(conf);
            _derivativesApi = new DerivativesApi(conf);
        }

        public async Task<ForgeBucket> AddBucket(ForgeBucket bucket)
        {

            var payload = new PostBucketsPayload(bucket.BucketKey, null,
                (PostBucketsPayload.PolicyKeyEnum)bucket.PolicyKey);
            var result = await _bucketsApi.CreateBucketAsync(payload);

            return bucket;
        }

        public Task DeleteBucket(string bucketKey)
        {
            return _bucketsApi.DeleteBucketAsync(bucketKey);
        }

        private async Task<ForgeToken> GetFullAccessToken()
        {
            var scope = new Scope[] {
                Scope.DataRead,
                Scope.DataWrite,
                Scope.BucketCreate,
                Scope.BucketRead,
                Scope.ViewablesRead,
                Scope.BucketDelete
            };

            var bearer = await _authApi.AuthenticateAsyncWithHttpInfo(_clientId, _clientSecret,
                oAuthConstants.CLIENT_CREDENTIALS,
                scope);

            var token = JsonConvert.DeserializeObject<ForgeToken>(JsonConvert.SerializeObject(bearer.Data));
            token.ExpireTime = DateTime.Now.AddSeconds(token.expires_in);

            return token;
        }

        public async Task<ForgeToken> GetAuthenticationResult()
        {
            var scope = new Scope[] {
                Scope.DataRead,
                Scope.DataWrite,
                Scope.BucketCreate,
                Scope.BucketRead,
                Scope.ViewablesRead
            };

            var bearer = await _authApi.AuthenticateAsyncWithHttpInfo(_clientId, _clientSecret,
                oAuthConstants.CLIENT_CREDENTIALS,
                scope);

            var token = JsonConvert.DeserializeObject<ForgeToken>(JsonConvert.SerializeObject(bearer.Data));
            token.ExpireTime = DateTime.Now.AddSeconds(token.expires_in);

            return token;
        }

        public async Task<IEnumerable<ForgeBucket>> GetBuckets()
        {
            var json = await _bucketsApi.GetBucketsAsync();

            var buckets = new List<ForgeBucket>();
            foreach (KeyValuePair<string, dynamic> bucket in new DynamicDictionaryItems(json.items))
            {
                buckets.Add(JsonConvert.DeserializeObject<ForgeBucket>(JsonConvert.SerializeObject(bucket.Value)));
            }

            return buckets;
        }

        public async Task<IEnumerable<ForgeObject>> GetObjects(string bucketId)
        {
            var json = await _objectApi.GetObjectsAsync(bucketId);

            var objects = new List<ForgeObject>();
            foreach (KeyValuePair<string, dynamic> forgeObject in new DynamicDictionaryItems(json.items))
            {
                objects.Add(JsonConvert.DeserializeObject<ForgeObject>(JsonConvert.SerializeObject(forgeObject.Value)));
            }

            return objects;
        }

        public Task DeleteObject(string bucketKey, string objectKey)
        {
            return _objectApi.DeleteObjectAsync(bucketKey, objectKey);
        }

        public async Task<ForgeObject> AddObject(ForgeObject forgeObject)
        {
            int contentSize = (int)forgeObject.ModelFile.Length;

            using (var body = forgeObject.ModelFile.OpenReadStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    await body.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    //body.ReadTimeout = 30 * 1000;
                    var result = await _objectApi.UploadObjectAsync(
                        forgeObject.BucketKey, forgeObject.ObjectKey, contentSize, memoryStream, forgeObject.ModelFile.ContentDisposition);
                }
            }

            return forgeObject;
        }

        private string GetURN(string objectId)
        {
            char[] padding = { '=' };
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(objectId))
                 .TrimEnd(padding).Replace('+', '-').Replace('/', '_');

        }

        public async Task<dynamic> TranslateObject(string objectId)
        {
            var xAdsForce = false;
            var urn = GetURN(objectId);

            var job = new JobPayload
            {
                Input = new JobPayloadInput(urn),
                Output = new JobPayloadOutput(new List<JobPayloadItem>
                    {
                        new JobPayloadItem
                        (
                            JobPayloadItem.TypeEnum.Svf,
                            new List<JobPayloadItem.ViewsEnum>
                            {
                                JobPayloadItem.ViewsEnum._3d,
                                JobPayloadItem.ViewsEnum._2d
                            },
                            null
                        )
                    }
                )
            };

            var result = await _derivativesApi.TranslateAsync(job, xAdsForce);

            return result;
        }

        public async Task<DynamicDictionary> GetManifest(string objectId)
        {
            var urn = GetURN(objectId);
            var result = await _derivativesApi.GetManifestAsync(urn);

            return result;
        }

        public async Task<DynamicDictionary> GetMetadata(string objectId)
        {
            var urn = GetURN(objectId);
            var result = await _derivativesApi.GetMetadataAsync(urn);

            return result;
        }

        public async Task<DynamicDictionary> GetViewMetadata(string objectId, string viewGUID)
        {
            var urn = GetURN(objectId);
            var result = await _derivativesApi.GetModelviewMetadataAsync(urn, viewGUID);

            return result;
        }
    }
}
