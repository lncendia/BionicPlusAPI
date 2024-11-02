using DomainObjects.Pregnancy.Localizations;

namespace PaymentService.Models.Emails;

/// <summary>
/// Родительская модель для формирования письма
/// </summary>
public class PaymentEmailModel
{
    private const string CancelSubscriptionBaseUrl = "https://localhost/payment/api/Subscription/cancel";
    
    public string Email { get; set; } = string.Empty;
    public LocalizationsLanguage Language { get; } = LocalizationsLanguage.ru;
    public string UserId { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;

    public string GenerateCancelSubscriptionUrl()
    {
        return $"{CancelSubscriptionBaseUrl}?userId={UserId}&hash={Hash.Replace("=", "%3D").Replace("+", "%2B")}&language={Language}";
    } 
}