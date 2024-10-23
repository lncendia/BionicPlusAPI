using MailSenderLibrary.Models;

namespace MailSenderLibrary.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message);
}