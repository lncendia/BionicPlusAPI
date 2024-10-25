using System.Text.Json.Serialization;
using AuthService.Configuration;

namespace AuthService.Models.ClientConfiguration;

public class ClientConfig
{
    [JsonPropertyName("endpoints")]
    public ClientEndpointsConfig? Endpoints { get; set; }

    [JsonPropertyName("settings")]
    public ClientSettingsConfig? Settings { get; set; }

    [JsonPropertyName("payment")]
    public PaymentConfig? Payment { get; set; }
}