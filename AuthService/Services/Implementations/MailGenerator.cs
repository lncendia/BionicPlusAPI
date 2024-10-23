using AuthService.Constants;
using DomainObjects.Pregnancy.Localizations;
using MailSenderLibrary.Models;

namespace AuthService.Services.Implementations
{
    public static class MailGenerator
    {
        public static EmailMessage GenerateTokenMessage(string token, string email, LocalizationsLanguage language)
        {
            var mailContent  = EmailTemplatesPath.EmailConfirmation(language);

            mailContent = File.ReadAllText(mailContent)
                .Replace("{{token}}", token);
            
            var recurrentMessage = MessageFormatter(email, mailContent, "Уведомление о подписке");
            return recurrentMessage;
        }
    
        private static EmailMessage MessageFormatter(string email, string content, string subject) => new EmailMessage(new List<string> { email }, subject, content);
    }
}
