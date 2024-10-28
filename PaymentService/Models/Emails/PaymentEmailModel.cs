using DomainObjects.Pregnancy.Localizations;

namespace PaymentService.Models.Emails;

/// <summary>
/// Родительская модель для формирования письма
/// </summary>
public class PaymentEmailModel
{
    public string Email { get; set; } = string.Empty;
    public LocalizationsLanguage Language { get; } = LocalizationsLanguage.ru;
    public string CancelSubscription { get; set; } = "https://web.babytips.me/billing/pricing"; 
}