using System.Text.Json.Serialization;

namespace PaymentService.Models.GooglePlayBilling;

/// <summary>
/// Class representing the main message.
/// </summary>
public class GoogleCallback
{
    /// <summary>
    /// Version of the message.
    /// </summary>
    [JsonPropertyName("version")]
    public string Version { get; init; } = string.Empty;

    /// <summary>
    /// Package name.
    /// </summary>
    [JsonPropertyName("packageName")]
    public string PackageName { get; init; } = string.Empty;

    /// <summary>
    /// Event time in milliseconds.
    /// </summary>
    [JsonPropertyName("eventTimeMillis")]
    public string EventTimeMillis { get; init; } = string.Empty;

    /// <summary>
    /// Subscription notification.
    /// </summary>
    [JsonPropertyName("subscriptionNotification")]
    public SubscriptionNotification? SubscriptionNotification { get; init; }
}
