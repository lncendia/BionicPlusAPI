using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentService.Models.GooglePlayBilling;

/// <summary>
/// Class representing a message.
/// </summary>
public class WebhookMessage
{
    /// <summary>
    /// Message data.
    /// </summary>
    [JsonPropertyName("data")]
    [Required]
    public string Data { get; init; } = string.Empty;

    /// <summary>
    /// Message identifier.
    /// </summary>
    [JsonPropertyName("messageId")]
    [Required]
    public string MessageId { get; init; } = string.Empty;

    /// <summary>
    /// Message publish time.
    /// </summary>
    [JsonPropertyName("publishTime")]
    [Required]
    public DateTime PublishTime { get; init; }
}
