using System.Text.Json.Serialization;
using AuthService.Configuration;

namespace AuthService.Models.ClientConfiguration;

/// <summary>
/// Configuration for client applications.
/// </summary>
public class ClientConfig
{
    /// <summary>
    /// Configuration for client endpoints.
    /// </summary>
    [JsonPropertyName("endpoints")]
    public ClientEndpointsConfig? Endpoints { get; set; }

    /// <summary>
    /// Configuration for client settings.
    /// </summary>
    [JsonPropertyName("settings")]
    public ClientSettingsConfig? Settings { get; set; }

    /// <summary>
    /// Configuration for payments.
    /// </summary>
    [JsonPropertyName("payment")]
    public PaymentConfig? Payment { get; set; }
}