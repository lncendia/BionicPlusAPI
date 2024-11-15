using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentService.Models.GooglePlayBilling;

/// <summary>
/// Класс, представляющий вебхук от гугл.
/// </summary>
public class GoogleWebhook
{
    /// <summary>
    /// Сообщение.
    /// </summary>
    [JsonPropertyName("message")]
    [Required]
    public WebhookMessage Message { get; init; } = null!;

    /// <summary>
    /// Подписка.
    /// </summary>
    [JsonPropertyName("subscription")]
    [Required]
    public string Subscription { get; init; } = string.Empty;
}