using astorWorkShared.Models;
using astorWorkShared.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public class AstorWorkBlobStorage : IAstorWorkBlobStorage
    {
        public CloudStorageAccount _storageAccount;
        CloudBlobClient _blobClient;

        private bool _initSuccess = false;
        private string _errorMessage = string.Empty;

        private SharedAccessBlobPolicy _sasConstraints;

        public AstorWorkBlobStorage()
        {            
            var connectionString = AppConfiguration.GetCloudStorageAccount();
            if (!string.IsNullOrEmpty(connectionString))
            {
                try
                {
                    _storageAccount = CloudStorageAccount.Parse(connectionString);
                    _blobClient = _storageAccount.CreateCloudBlobClient();
                    _initSuccess = true;
                    _sasConstraints = new SharedAccessBlobPolicy();
                    _sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24);
                    _sasConstraints.Permissions = SharedAccessBlobPermissions.Read;
                    

                }
                catch (Exception exc)
                {
                    _errorMessage = exc.Message;
                    Console.WriteLine(exc.Message);
                }
            }
        }

        public string ErrorMessage()
        {
            return _errorMessage;
        }
       
        public string GetSignedURL(string containerName, string fileName)
        {
            string url = string.Empty;
            if (_initSuccess && !string.IsNullOrEmpty(containerName) && !string.IsNullOrEmpty(fileName))
            {
                try
                {
                    CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                    if (_sasConstraints.SharedAccessExpiryTime.Value - DateTimeOffset.UtcNow < TimeSpan.FromHours(1))
                        _sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24);

                    string sasContainerToken = blockBlob.GetSharedAccessSignature(_sasConstraints);

                    url = blockBlob.Uri + sasContainerToken;
                }
                catch (Exception exc)
                {
                    _errorMessage = exc.Message;
                    Console.Write(exc);
                }
            }

            return url;
        }

        public async Task<bool> UploadFile(string containerName, string fileName, Stream fs)
        {
            bool success = false;
            if (_initSuccess && !string.IsNullOrEmpty(containerName) && !string.IsNullOrEmpty(fileName))
            {
                try
                {
                    // Get a reference to the container
                    CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
                    await container.CreateIfNotExistsAsync();

                    // Get a reference to the blob
                    CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                    // Upload the file stream
                    await blockBlob.UploadFromStreamAsync(fs);
                    success = true;

                }
                catch (Exception exc)
                {
                    _errorMessage = exc.Message;
                    Console.WriteLine(exc.Message);
                }
                finally
                {
                    fs.Dispose();
                }
            }

            return success;
        }

        public async Task<string> UploadRandomNameFile(string containerName, string extension, Stream fs)
        {
            string fileName = string.Empty;
            if (_initSuccess && !string.IsNullOrEmpty(containerName) && !string.IsNullOrEmpty(extension))
            {
                // Get a reference to the blob
                fileName = string.Format("{0}.{1}", Guid.NewGuid().ToString(), extension);
                var success = await UploadFile(containerName, fileName, fs);
                if (!success)
                    fileName = string.Empty;
            }

            return fileName;
        }

        public string GetContainerHost()
        {
            return _storageAccount.BlobStorageUri.PrimaryUri.ToString()+ AppConfiguration.GetQCContainerName()+"/";
        }

        public string GetContainerAccessToken()
        {
            CloudBlobContainer container = _blobClient.GetContainerReference(AppConfiguration.GetQCContainerName());
            if (_sasConstraints.SharedAccessExpiryTime.Value - DateTimeOffset.UtcNow < TimeSpan.FromHours(1))
                _sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddHours(24);
            string sasContainerToken = container.GetSharedAccessSignature(_sasConstraints);

            return sasContainerToken;
        }

        public async Task<string> UploadSignature(string signature)
        {
            byte[] data = Convert.FromBase64String(signature);
            string fileName = Guid.NewGuid() + ".jpg";

            await UploadFile(AppConfiguration.GetQCContainerName(), fileName, new MemoryStream(data));

            return GetContainerHost() + fileName.ToString();
        }

        public async Task<string> UploadImage(string containerName, string Image)
        {
            byte[] data = Convert.FromBase64String(Image);
            string fileName = Guid.NewGuid() + ".jpg";
            await UploadFile(containerName, fileName, new MemoryStream(data));

            return GetContainerHost() + fileName.ToString();
        }
    }
}
