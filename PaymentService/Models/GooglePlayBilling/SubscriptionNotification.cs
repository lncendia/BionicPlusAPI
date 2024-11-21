using System.Text.Json.Serialization;

namespace PaymentService.Models.GooglePlayBilling;

/// <summary>
/// Class representing a subscription notification.
/// </summary>
public class SubscriptionNotification
{
    /// <summary>
    /// Version of the notification.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// Type of the notification.
    /// </summary>
    [JsonPropertyName("notificationType")]
    public SubscriptionNotificationType NotificationType { get; init; }

    /// <summary>
    /// Purchase token.
    /// </summary>
    [JsonPropertyName("purchaseToken")]
    public string PurchaseToken { get; init; } = string.Empty;

    /// <summary>
    /// Subscription identifier.
    /// </summary>
    [JsonPropertyName("subscriptionId")]
    public string SubscriptionId { get; init; } = string.Empty;
}
