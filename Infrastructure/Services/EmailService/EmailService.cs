using Infrastructure.Services.EmailService.Interface;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;
        public EmailService(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }
        public async Task SendEmailAsync(List<string> emailId, string subject, string message)
        {
            var client = new SmtpClient(_emailOptions.SMTP, _emailOptions.Port)
            {
                Credentials = new NetworkCredential(_emailOptions.UserName, _emailOptions.Password)
            };

            await client.SendMailAsync(_emailOptions.Sender, string.Join(";", emailId), subject, message); //

        }
    }
}
