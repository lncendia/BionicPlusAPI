using System.Web;
using DomainObjects.Pregnancy.Localizations;

namespace PaymentService.Models.Emails;

/// <summary>
/// Родительская модель для формирования письма
/// </summary>
public class PaymentEmailModel
{
    public string CancelSubscriptionBaseUrl { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public LocalizationsLanguage Language { get; } = LocalizationsLanguage.ru;
    public string UserId { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;

    public string GenerateCancelSubscriptionUrl() =>
        $"{CancelSubscriptionBaseUrl}?userId={UserId}&hash={HttpUtility.UrlEncode(Hash)}&language={Language}";
}