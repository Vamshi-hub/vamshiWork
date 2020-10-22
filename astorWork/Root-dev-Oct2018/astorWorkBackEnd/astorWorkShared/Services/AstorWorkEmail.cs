using astorWorkShared.Utilities;
using DinkToPdf;
using DinkToPdf.Contracts;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
            var apiKey = AppConfiguration.GetSendGridKey();
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

            var apiKey = AppConfiguration.GetSendGridKey();
            var msg = new SendGridMessage()
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

        public async Task<bool> SendBulk(string[] recipientAddrs, string[] recipientNames,
            string subject, int notificationCode = -1,
            string[] notificationParams = null, List<string> attachmentPaths = null)
        {
            bool result = false;
            try
            {
                if (_emailClient != null && recipientAddrs.Length == recipientNames.Length)
                {
                    string[] recipients = recipientAddrs;
                    string binDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

                    string htmlTemplatePath = "", txtTemplatePath = "";

                    switch (notificationCode)
                    {
                        case (int)Enums.NotificationCode.CreateMRF:
                            htmlTemplatePath = @"/EmailTemplates/mrf_creation.html";
                            txtTemplatePath = @"/EmailTemplates/mrf_creation.txt";
                            break;
                        case (int)Enums.NotificationCode.BIMSync:
                            htmlTemplatePath = @"/EmailTemplates/bim_sync.html";
                            txtTemplatePath = @"/EmailTemplates/bim_sync.txt";
                            break;
                        case (int)Enums.NotificationCode.QCFailed:
                            htmlTemplatePath = @"/EmailTemplates/qc_Failed.html";
                            txtTemplatePath = @"/EmailTemplates/qc_Failed.txt";
                            break;
                        case (int)Enums.NotificationCode.QCRectified:
                            htmlTemplatePath = @"/EmailTemplates/qc_Failed.html";
                            txtTemplatePath = @"/EmailTemplates/qc_Failed.txt";
                            break;
                        case (int)Enums.NotificationCode.CloseMRF:
                            htmlTemplatePath = @"/EmailTemplates/mrf_close.html";
                            txtTemplatePath = @"/EmailTemplates/mrf_close.txt";
                            break;
                        case (int)Enums.NotificationCode.DelayInDelivery:
                            htmlTemplatePath = @"/EmailTemplates/delayed_delivery.html";
                            txtTemplatePath = @"/EmailTemplates/delayed_delivery.txt";
                            break;
                        case (int)Enums.NotificationCode.TodayExpectedDelivery:
                            htmlTemplatePath = @"/EmailTemplates/delayed_delivery.html";
                            txtTemplatePath = @"/EmailTemplates/delayed_delivery.txt";
                            break;
                        default:
                            break;
                    }

                    string htmlTemplate = await File.ReadAllTextAsync(binDir + htmlTemplatePath);
                    string txtTemplate = await File.ReadAllTextAsync(binDir + txtTemplatePath);
                    string htmlTemplateBody = string.Format(htmlTemplate, notificationParams);
                    string txtTemplateBody = string.Format(txtTemplate, notificationParams);

                    var apiKey = AppConfiguration.GetSendGridKey();
                    var msg = new SendGridMessage()
                    {
                        From = new EmailAddress("support@astorwork.com", "astorWork Team"),
                        Subject = subject,
                        PlainTextContent = txtTemplateBody,
                        HtmlContent = htmlTemplateBody
                    };

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

                    for (int i = 0; i < recipientAddrs.Length; i++)
                    {
                        msg.AddTo(new EmailAddress(recipientAddrs[i], recipientNames[i]));
                    }
                    var response = await _emailClient.SendEmailAsync(msg);

                    result = response.StatusCode.Equals(System.Net.HttpStatusCode.Accepted);
                    if (!result)
                    {
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

        protected PdfPTable CreatePDFTable(DataTable dt)
        {
            Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);

            int columnCount = dt.Columns.Count;

            PdfPTable table = new PdfPTable(columnCount);
            List<float> widths = new List<float>();

            for (int colIndex = 0; colIndex < columnCount; colIndex++)
                widths.Add(4f);

            table.SetWidths(widths.ToArray());
            table.WidthPercentage = 100;

            PdfPCell cell = new PdfPCell(new Phrase("Products"));

            cell.Colspan = columnCount;

            foreach (DataColumn c in dt.Columns)
                table.AddCell(new Phrase(c.ColumnName, font5));

            foreach (DataRow row in dt.Rows)
                if (dt.Rows.Count > 0)
                    for (int colIndex = 0; colIndex < columnCount; colIndex++)
                        table.AddCell(new Phrase(row[colIndex].ToString(), font5));

            return table;
        }

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

        public string CreateDocument(string filePath, string header, string subHeader, string tblContent, IConverter _converter)
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = filePath
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = GenerateHtmlContent(tblContent, header, subHeader),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" },
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings },
            };

            _converter.Convert(pdf);

            return filePath;
        }

        public static string GenerateHtmlContent(string tblContent, string header, string subHeader)
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
                                            GetImage("./Images/logo.png") +
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
            return "<img width='67' height='45' src=\"data:image/"
                        + Path.GetExtension(imgFile).Replace(".", "")
                        + ";base64,"
                        + Convert.ToBase64String(File.ReadAllBytes(imgFile)) + "\" />";
        }

        //public void UpdateNotificationAudit(List<UserMaster> recipients, string id)
        //{

        //    if (recipients.Count > 0)
        //    {
        //        var notification = new NotificationAudit
        //        {
        //            Code = 0,
        //            Type = 0,
        //            Reference = id,
        //            CreatedDate = DateTimeOffset.Now,
        //            ProcessedDate = null
        //        };
        //        var userNotificationAssociation = recipients.Select(
        //            receipient => new UserNotificationAssociation
        //            {
        //                Receipient = receipient,
        //                Notification = notification
        //            });

        //        _context.NotificationAudit.Add(notification);
        //        _context.UserNotificationAssociation.AddRange(userNotificationAssociation);

        //        _context.SaveChanges();
        //    }
        //}
    }
}
