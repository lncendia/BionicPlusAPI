using DomainObjects.Pregnancy.Localizations;

namespace AuthService.Constants;

public class EmailTemplatesPath
{
    private const string BasePath = "Templates/Mailing";
    private const string Format = ".htm";
    
    public static string EmailConfirmation(LocalizationsLanguage language) => $"{BasePath}/email-confirmation.{language.ToString()}{Format}";
}