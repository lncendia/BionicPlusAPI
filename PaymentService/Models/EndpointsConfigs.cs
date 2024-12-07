using System.Text.Json.Serialization;

namespace PaymentService.Models;

public class EndpointsConfig
{
    [JsonPropertyName("authServiceUrl")]
    public string AuthServiceUrl { get; set; } = string.Empty;
    
    [JsonPropertyName("cancelSubscriptionBaseUrl")]
    public string CancelSubscriptionBaseUrl { get; set; } = string.Empty;
}