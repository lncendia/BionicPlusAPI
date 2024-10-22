using MailSenderLibrary.Models;

namespace MailSenderLibrary.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(EmailMessage message);
    }
}
