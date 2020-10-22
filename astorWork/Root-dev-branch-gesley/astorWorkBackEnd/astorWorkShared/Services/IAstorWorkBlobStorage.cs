using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public interface IAstorWorkBlobStorage
    {
        Task<string> UploadRandomNameFile(string containerName, string extension, Stream fs);
        Task<bool> UploadFile(string containerName, string fileName, Stream fs);
        string GetSignedURL(string containerName, string fileName);
        Task<string> UploadSignature(string signature);
        Task<string> UploadImage(string containerName, string Image);
        string ErrorMessage();
        string GetContainerHost();
        string GetContainerAccessToken();
    }
}
