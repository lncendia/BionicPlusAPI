using MailKit.Net.Smtp;
using MailSenderLibrary.Interfaces;
using MailSenderLibrary.Models;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MailSenderLibrary.Implementations
{
    public class MailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        private const string FROM_NAME = "Baby Tips";

        public MailService(IOptions<EmailConfiguration> emailConfig)
        {
            _emailConfig = emailConfig.Value;  
        }
        public void SendEmail(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = message.Content;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(FROM_NAME, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage message)
        {
            using var smtpClient = new SmtpClient();
            try
            {
                smtpClient.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");
                smtpClient.Authenticate(_emailConfig.Username, _emailConfig.Password);

                smtpClient.Send(message);
            }
            catch
            {
                // log
                throw;
            }
            finally
            {
                smtpClient.Disconnect(true);
                smtpClient.Dispose();
            }
        }
    }
}
