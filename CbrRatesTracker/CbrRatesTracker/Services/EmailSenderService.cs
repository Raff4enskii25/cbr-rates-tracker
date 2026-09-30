using CbrRatesTracker.Interfaces;
using MimeKit;
using MailKit.Net.Smtp;

namespace CbrRatesTracker.Services
{
    public class EmailSenderService : INotificationService
    {
        private readonly IConfiguration _configuration;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMessageAsync(string recipient, string subject, string body)
        {
            string senderName = _configuration["EmailSettings:SenderName"];
            string senderEmail = _configuration["EmailSettings:SenderEmail"];
            string server = _configuration["EmailSettings:Server"];
            int port = int.Parse(_configuration["EmailSettings:Port"]);
            string password = _configuration["EmailSettings:Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderEmail));
            message.To.Add(new MailboxAddress("", recipient));
            message.Subject = subject;
            message.Body = new TextPart("Html") { Text = body };

            using var client = new SmtpClient();

            await client.ConnectAsync(server, port, MailKit.Security.SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(senderEmail, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
