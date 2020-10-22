using System;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace astorWorkMobile.Shared.Utilities
{
    public sealed class EmailClient
    {
        private static volatile EmailClient instance;
        private static object syncRoot = new object();
        private SmtpClient _smtpClient;
        
        public static EmailClient Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new EmailClient();
                        }
                    }
                }

                return instance;
            }
        }

        private EmailClient()
        {            
            _smtpClient = new SmtpClient("smtp.sendgrid.net", Convert.ToInt32(587));
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(
                App.SEND_GRID_USER_NAME, App.SEND_GRID_USER_PWD);

            _smtpClient.Credentials = credentials;
        }

        public async Task SendSingleAsync(string toAddr, string toName, string subject, string bodyHTML)
        {

            try
            {
                MailMessage mailMsg = new MailMessage();

                // To
                mailMsg.To.Add(new MailAddress(toAddr, toName));

                // From
                mailMsg.From = new MailAddress("support@astorwork.com", 
                    "astorWork Team");

                // Subject and multipart/alternative Body
                mailMsg.Subject = subject;
                string text = "Please enable HTML content for this Email";

                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyHTML, null, MediaTypeNames.Text.Html));

                // Init SmtpClient and send

                await _smtpClient.SendMailAsync(mailMsg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
