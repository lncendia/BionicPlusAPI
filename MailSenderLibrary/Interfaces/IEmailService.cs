using MailSenderLibrary.Models;

namespace MailSenderLibrary.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(EmailMessage message);
    }
}
