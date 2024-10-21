using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace TaskForge.Service
{
    public class EmailService
    {
        private readonly string _smtpServer = "localhost"; // Sử dụng localhost cho Papercut SMTP
        private readonly int _smtpPort = 25;               // Cổng mặc định của Papercut SMTP

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.EnableSsl = false; // Không cần SSL khi sử dụng localhost
                client.Credentials = CredentialCache.DefaultNetworkCredentials;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("noreply@taskforge.local"), // Địa chỉ email giả lập
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
