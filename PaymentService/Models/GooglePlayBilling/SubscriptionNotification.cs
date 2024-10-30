using System.Text.Json.Serialization;

namespace PaymentService.Models.GooglePlayBilling;

/// <summary>
/// Класс, представляющий уведомление о подписке.
/// </summary>
public class SubscriptionNotification
{
    /// <summary>
    /// Версия уведомления.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// Тип уведомления.
    /// </summary>
    [JsonPropertyName("notificationType")]
    public SubscriptionNotificationType NotificationType { get; init; }

    /// <summary>
    /// Токен покупки.
    /// </summary>
    [JsonPropertyName("purchaseToken")]
    public string PurchaseToken { get; init; } = string.Empty;

    /// <summary>
    /// Идентификатор подписки.
    /// </summary>
    [JsonPropertyName("subscriptionId")]
    public string SubscriptionId { get; init; } = string.Empty;
}