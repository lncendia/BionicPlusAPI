using System.Text.Json.Serialization;
using AuthService.Configuration;

namespace AuthService.Models.ClientConfiguration;

/// <summary>
/// Конфигурация клиента.
/// </summary>
public class ClientConfig
{
    /// <summary>
    /// Конфигурация конечных точек клиента.
    /// </summary>
    [JsonPropertyName("endpoints")]
    public ClientEndpointsConfig? Endpoints { get; set; }

    /// <summary>
    /// Конфигурация настроек клиента.
    /// </summary>
    [JsonPropertyName("settings")]
    public ClientSettingsConfig? Settings { get; set; }

    /// <summary>
    /// Конфигурация платежей.
    /// </summary>
    [JsonPropertyName("payment")]
    public PaymentConfig? Payment { get; set; }
}