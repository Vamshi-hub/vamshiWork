using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace astorWork.Utilities
{
    public class AzureHelper
    {
        private static CloudFileShare GetBIMVisualizationShare()
        {
            string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1};EndpointSuffix=core.windows.net", ConfigurationManager.AppSettings["AzureAccountName"], ConfigurationManager.AppSettings["AzureAccountKey"]);

            CloudStorageAccount account = CloudStorageAccount.Parse(storageConnectionString);
            // Create a CloudFileClient object for credentialed access to Azure Files.
            CloudFileClient fileClient = account.CreateCloudFileClient();
            // Create a new file share, if it does not already exist.
            return fileClient.GetShareReference(ConfigurationManager.AppSettings["AzureBIMFileShare"]);
        }

        public static async Task<string> UploadBIMVisualization(int userId, byte[] content)
        {
            string fileName = string.Empty;
            try
            {
                var share = GetBIMVisualizationShare();
                await share.CreateIfNotExistsAsync();
                share.Properties.Quota = 1;
                await share.SetPropertiesAsync();
                CloudFileDirectory dir = share.GetRootDirectoryReference().GetDirectoryReference(userId.ToString());
                await dir.CreateIfNotExistsAsync();
                // Create a new file in the root directory.
                CloudFile sourceFile = dir.GetFileReference(Guid.NewGuid() + ".mp4");
                await sourceFile.UploadFromByteArrayAsync(content, 0, content.Length);

                fileName = sourceFile.Name;
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
            }

            return fileName;
        }

        public static async Task<Uri> GetBIMVisualizationUrl(int userId, string fileName)
        {
            Uri fileUri = null;
            try
            {
                var share = GetBIMVisualizationShare();
                // Get existing permissions for the share.
                FileSharePermissions permissions = await share.GetPermissionsAsync();
                string policyName = "bimSharePolicy";
                SharedAccessFilePolicy sharedPolicy = null;
                if (!permissions.SharedAccessPolicies.ContainsKey(policyName))
                {
                    // Create a new shared access policy and define its constraints.
                    sharedPolicy = new SharedAccessFilePolicy()
                    {
                        SharedAccessExpiryTime = DateTime.MaxValue,
                        Permissions = SharedAccessFilePermissions.Read | SharedAccessFilePermissions.List
                    };
                    // Add the shared access policy to the share's policies. Note that each policy must have a unique name.
                    permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
                    share.SetPermissions(permissions);
                }
                else
                    sharedPolicy = permissions.SharedAccessPolicies[policyName];

                // Generate a SAS for a file in the share and associate this access policy with it.
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();
                CloudFileDirectory dir = rootDir.GetDirectoryReference(userId.ToString());
                CloudFile file = dir.GetFileReference(fileName);
                var headers = new SharedAccessFileHeaders();
                headers.ContentDisposition = "inline";
                headers.ContentType = "binary";
                string sasToken = file.GetSharedAccessSignature(sharedPolicy, headers);
                fileUri = new Uri(file.Uri.ToString() + sasToken);
            }
            catch (Exception exc)
            {
                Debug.WriteLine(exc.Message);
                Debug.WriteLine(exc.StackTrace);
            }

            return fileUri;
        }
    }
}