using astorWorkShared.Utilities;
using DinkToPdf;
using DinkToPdf.Contracts;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkShared.Services
{
    public class AstorWorkEmail : IAstorWorkEmail
    {
        //protected astorWorkDbContext _context;
        SendGridClient _emailClient;
        string _errorMessage;

        public AstorWorkEmail()
        {
            string apiKey = AppConfiguration.GetSendGridKey();
            if (!string.IsNullOrEmpty(apiKey))
            {
                try
                {
                    _emailClient = new SendGridClient(apiKey);
                }
                catch (Exception exc)
                {
                    _errorMessage = exc.Message;
                    Console.WriteLine(exc.Message);
                }
            }
        }

        public async Task<bool> SendSingle(string recipientAddr, string recipientName, string subject, string body, List<string> attachmentPaths = null)
        {
            if (_emailClient == null)
                return false;

            string apiKey = AppConfiguration.GetSendGridKey();
            SendGridMessage msg = new SendGridMessage()
            {
                From = new EmailAddress("support@astorwork.com", "astorWork Team"),
                Subject = subject,
                HtmlContent = body
            };

            msg.AddTo(new EmailAddress(recipientAddr, recipientName));

            if (attachmentPaths != null)
            {
                foreach (string path in attachmentPaths)
                {
                    string[] pathSplit = path.Split('/');
                    string fileName = pathSplit[pathSplit.Length - 1];
                    byte[] bytes = File.ReadAllBytes(path);
                    string file = Convert.ToBase64String(bytes);
                    msg.AddAttachment(fileName, file);
                }
            }
            var response = await _emailClient.SendEmailAsync(msg);

            return response.StatusCode.Equals(System.Net.HttpStatusCode.Accepted);
        }

        private string GetTemplatePath(int notificationCode) {
            switch (notificationCode)
            {
                case (int)Enums.NotificationCode.CreateMRF:
                    return @"/EmailTemplates/mrf_creation";
                case (int)Enums.NotificationCode.BIMSync:
                    return @"/EmailTemplates/bim_sync";
                case (int)Enums.NotificationCode.QCFailed:
                    return @"/EmailTemplates/qc_Failed";
                case (int)Enums.NotificationCode.QCRectified:
                    return @"/EmailTemplates/qc_Failed";
                case (int)Enums.NotificationCode.CloseMRF:
                    return @"/EmailTemplates/mrf_close";
                case (int)Enums.NotificationCode.DelayInDelivery:
                    return @"/EmailTemplates/delayed_delivery";
                case (int)Enums.NotificationCode.TodayExpectedDelivery:
                    return @"/EmailTemplates/delayed_delivery";
                case (int)Enums.NotificationCode.JobAssigned:
                    return @"/EmailTemplates/job_assigned";
                case (int)Enums.NotificationCode.JobCompleted:
                    return @"/EmailTemplates/job_completed";
                case (int)Enums.NotificationCode.JobQCPassed:
                    return @"/EmailTemplates/job_qc_passed";
                case (int)Enums.NotificationCode.JobQCFailed:
                    return @"/EmailTemplates/job_qc_failed";
                case (int)Enums.NotificationCode.JobQCAccepted:
                    return @"/EmailTemplates/job_qc_accepted";
                case (int)Enums.NotificationCode.JobQCRejected:
                    return @"/EmailTemplates/job_qc_rejected";
                case (int)Enums.NotificationCode.MaterialQCPassed:
                    return @"/EmailTemplates/material_qc_passed";
                case (int)Enums.NotificationCode.MaterialQCFailed:
                    return @"/EmailTemplates/material_qc_failed";
            }
            return "";
        }

        private SendGridMessage AddAttachments(SendGridMessage msg, List<string> attachmentPaths) {
            if (attachmentPaths != null)
            {
                foreach (string path in attachmentPaths)
                {
                    string[] pathSplit = path.Split('\\');
                    string fileName = pathSplit[pathSplit.Length - 1];
                    byte[] bytes = File.ReadAllBytes(path);
                    string file = Convert.ToBase64String(bytes);
                    msg.AddAttachment(fileName, file);
                }
            }

            return msg;
        }

        private async Task<SendGridMessage> CreateEmailMessage(string subject, string templatePath, string[] notificationParams) {
            string binDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            string htmlTemplate = await File.ReadAllTextAsync(binDir + templatePath + ".html");
            string txtTemplate = await File.ReadAllTextAsync(binDir + templatePath + ".txt");

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("support@astorwork.com", "astorWork Team"),
                Subject = subject,
                PlainTextContent = string.Format(txtTemplate, notificationParams),
                HtmlContent = string.Format(htmlTemplate, notificationParams)
            };

            return msg;
        }

        private SendGridMessage AddReceipients(SendGridMessage msg, string[] recipientAddrs, string[] recipientNames) {
            for (int i = 0; i < recipientAddrs.Length; i++)
                msg.AddTo(new EmailAddress(recipientAddrs[i], recipientNames[i]));

            return msg;
        }

        public async Task<bool> SendBulk(string[] recipientAddrs, string[] recipientNames,
            string subject, int notificationCode = -1,
            string[] notificationParams = null, List<string> attachmentPaths = null)
        {
            bool result = false;
            try
            {
                if (_emailClient != null && recipientAddrs.Length == recipientNames.Length)
                {   
                    string templatePath = GetTemplatePath(notificationCode);
                    SendGridMessage msg = await CreateEmailMessage(subject, templatePath, notificationParams);
                    AddAttachments(msg, attachmentPaths);
                    AddReceipients(msg, recipientAddrs, recipientNames);
                    
                    Response response = await _emailClient.SendEmailAsync(msg);

                    result = response.StatusCode.Equals(System.Net.HttpStatusCode.Accepted);

                    if (!result)
                    {
                        string apiKey = AppConfiguration.GetSendGridKey();
                        Console.WriteLine($"Sending email with key {apiKey} failed: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        /*
        public List<string> CreateDocument(string tblContent)
        {
            List<string> attachmentPaths = new List<string>();
            string filePath = string.Format("{0}/{1}.pdf", Path.GetTempPath(), fileName);
            attachmentPaths.Add(filePath);

            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            document.Open();

            Paragraph title = new Paragraph(fileName);
            Paragraph space = new Paragraph(" ");
            PdfPTable table = CreatePDFTable(dataTable);

            document.Add(title);
            document.Add(space);
            document.Add(table);
            document.Close();

            return attachmentPaths;
        }

        public Image InsertImageToPDF() {
            string imageURL = Server.MapPath(".") + "/image2.jpg";
            return Image.GetInstance(imageURL);
        }
        */

        public DataTable CreateDataTable(string tableName, List<string> columnNames)
        {
            // Create a new DataTable.
            DataTable table = new DataTable(tableName);
            // Declare variables for DataColumn and DataRow objects.
            DataColumn column;

            // Create new DataColumn, set DataType, 
            // ColumnName and add to DataTable.    
            for (int i = 0; i < columnNames.Count; i++)
            {
                column = new DataColumn();
                column.DataType = System.Type.GetType("System.String");
                column.ColumnName = columnNames[i];
                if (i == 0)
                {
                    column.ReadOnly = true;
                    column.Unique = false;

                    // Make the ID column the primary key column.
                    DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                    PrimaryKeyColumns[0] = table.Columns[columnNames[i]];
                    //table.PrimaryKey = PrimaryKeyColumns;
                }
                else
                {
                    column.ReadOnly = false;
                    column.Unique = false;
                }

                // Add the Column to the DataColumnCollection.
                table.Columns.Add(column);
            }

            return table;
        }

        public string CreateDocument(string filePath, string header, string subHeader, string tblContent, IConverter _converter, string tenantLogoUrl = null)
        {
            GlobalSettings globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = filePath
            };

            ObjectSettings objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = GenerateHtmlContent(tblContent, header, subHeader, tenantLogoUrl),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" },
            };

            HtmlToPdfDocument pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },
            };

            _converter.Convert(pdf);

            return filePath;
        }

        public static string GenerateHtmlContent(string tblContent, string header, string subHeader, string tenantLogoUrl)
        {
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                            </head>
                            <body>
                                <p>&nbsp;</p>
                                <table>
	                                 <tr>
	                                      <td valign='middle' style='width:550.4pt;padding:0cm 5.4pt 0cm 5.4pt'>
		                                    <div><b style='font-size:16.0pt;font-family: arial;'>" + header + @"</b></div>
                                            <div style='font-family: arial;'>" + subHeader + @"</div>
	                                      </td>
	                                      <td width='101' valign='top' style='width:75.4pt;padding:0cm 5.4pt 0cm 5.4pt'>" +
                                            GetImage(tenantLogoUrl) +
                                          @"</td>
	                                 </tr>
	                            </table>
                                <p>&nbsp;</p>");

            sb.Append(tblContent);
                                
            sb.Append(@"    </body>
                        </html>");

            return sb.ToString();
        }

        public static string GetImage(string imgFile)
        {
            //return "<img width='67' height='45' src=\"data:image/"
            //            + Path.GetExtension(imgFile).Replace(".", "")
            //            + ";base64,"
            //            + Convert.ToBase64String(File.ReadAllBytes(imgFile)) + "\" />";

            return "<img width='67' height='45' src=\""
                        + imgFile + "\" />";
        }
    }
}
