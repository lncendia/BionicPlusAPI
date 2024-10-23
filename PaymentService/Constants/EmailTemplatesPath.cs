using DomainObjects.Pregnancy.Localizations;

namespace PaymentService.Constants;

/// <summary>
/// Типы отправляемых писем
/// </summary>
public static class EmailTemplatesPath
{
    private const string BasePath = "Templates/Mailing";
    private const string Format = ".htm";
    
    public static string PaymentNotification(LocalizationsLanguage language) => $"{BasePath}/payment-notification.{language.ToString()}{Format}";
    public static string PaymentFailure(LocalizationsLanguage language) => $"{BasePath}/payment-failure.{language.ToString()}{Format}";
    public static string PaymentConfirmation(LocalizationsLanguage language) => $"{BasePath}/payment-confirmation.{language.ToString()}{Format}";
    public static string SubscriptionCancelled(LocalizationsLanguage language) => $"{BasePath}/subscription-cancelled.{language.ToString()}{Format}";
    public static string SubscriptionConfirmation(LocalizationsLanguage language) => $"{BasePath}/subscription-confirmation.{language.ToString()}{Format}";
}