using System.Text.Json.Serialization;

namespace AuthService.Configuration;

/// <summary>
/// Конфигурация обратного прокси.
/// </summary>
public class ReverseProxyConfig
{
    /// <summary>
    /// Прокси для входа.
    /// </summary>
    [JsonPropertyName("loginProxy")]
    public string LoginProxy { get; init; } = string.Empty;

    /// <summary>
    /// Прокси для API.
    /// </summary>
    [JsonPropertyName("apiProxy")]
    public string ApiProxy { get; init; } = string.Empty;

    /// <summary>
    /// Прокси для платежей.
    /// </summary>
    [JsonPropertyName("paymentProxy")]
    public string PaymentProxy { get; init; } = string.Empty;
}
