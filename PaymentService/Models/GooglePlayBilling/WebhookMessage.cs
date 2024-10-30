using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PaymentService.Models.GooglePlayBilling;

/// <summary>
/// Класс, представляющий сообщение.
/// </summary>
public class WebhookMessage
{
    /// <summary>
    /// Данные сообщения.
    /// </summary>
    [JsonPropertyName("data")]
    [Required]
    public string Data { get; init; } = string.Empty;

    /// <summary>
    /// Идентификатор сообщения.
    /// </summary>
    [JsonPropertyName("messageId")]
    [Required]
    public string MessageId { get; init; } = string.Empty;

    /// <summary>
    /// Время публикации сообщения.
    /// </summary>
    [JsonPropertyName("publishTime")]
    [Required]
    public DateTime PublishTime { get; init; }
}