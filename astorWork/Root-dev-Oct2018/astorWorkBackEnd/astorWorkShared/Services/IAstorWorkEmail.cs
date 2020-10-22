using DinkToPdf.Contracts;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public interface IAstorWorkEmail
    {
        Task<bool> SendSingle(string recipientAddr, string recipientName, string subject, 
            string body, List<string> attachmentPath = null);

        Task<bool> SendBulk(string[] recipientAddrs, string[] recipientNames, string subject,
            int notificationCode = -1, string[] notificationParams = null, List<string> attachmentPaths = null);

        string CreateDocument(string filePath, string header, string subHeader, string tblContent, IConverter _converter);
        DataTable CreateDataTable(string tableName, List<string> columnNames);
    }
}
