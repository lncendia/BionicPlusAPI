using MailKit.Net.Smtp;
using MailSenderLibrary.Interfaces;
using MailSenderLibrary.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MailSenderLibrary.Implementations
{
    public class MailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        private readonly ILogger<MailService> _logger;
        
        private const string FromName = "Baby Tips";

        public MailService(IOptions<EmailConfiguration> emailConfig, ILogger<MailService> logger)
        {
            _emailConfig = emailConfig.Value;
            _logger = logger;
        }
        
        public async Task SendEmail(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            await Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.Content
            };

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(FromName, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private async Task Send(MimeMessage message)
        {
            using var smtpClient = new SmtpClient();
            try
            {
                await smtpClient.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
                await smtpClient.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);

                await smtpClient.SendAsync(message);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
            }
        }
    }
}
