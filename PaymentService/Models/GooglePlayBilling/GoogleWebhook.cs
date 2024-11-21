using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentService.Models.GooglePlayBilling;

/// <summary>
/// Class representing a webhook from Google.
/// </summary>
public class GoogleWebhook
{
    /// <summary>
    /// Message.
    /// </summary>
    [JsonPropertyName("message")]
    [Required]
    public WebhookMessage Message { get; init; } = null!;

    /// <summary>
    /// Subscription.
    /// </summary>
    [JsonPropertyName("subscription")]
    [Required]
    public string Subscription { get; init; } = string.Empty;
}
