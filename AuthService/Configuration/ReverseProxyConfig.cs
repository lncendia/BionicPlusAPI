using System.Text.Json.Serialization;

namespace AuthService.Configuration;

public class ReverseProxyConfig
{
    [JsonPropertyName("loginProxy")] public string LoginProxy { get; init; } = string.Empty;

    [JsonPropertyName("apiProxy")] public string ApiProxy { get; init; } = string.Empty;

    [JsonPropertyName("paymentProxy")] public string PaymentProxy { get; init; } = string.Empty;
}