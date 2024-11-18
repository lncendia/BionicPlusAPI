using System.Text.Json.Serialization;

namespace PaymentService.Models.GooglePlayBilling;

/// <summary>
/// Класс, представляющий основное сообщение.
/// </summary>
public class GoogleCallback
{
    /// <summary>
    /// Версия сообщения.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// Имя пакета.
    /// </summary>
    [JsonPropertyName("packageName")] 
    public string PackageName { get; init; } = string.Empty;

    /// <summary>
    /// Время события в миллисекундах.
    /// </summary>
    [JsonPropertyName("eventTimeMillis")]
    public string EventTimeMillis { get; init; } = string.Empty;

    /// <summary>
    /// Уведомление о подписке.
    /// </summary>
    [JsonPropertyName("subscriptionNotification")]
    public SubscriptionNotification SubscriptionNotification { get; init; } = new();
}