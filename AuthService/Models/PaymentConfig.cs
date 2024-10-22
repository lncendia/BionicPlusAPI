using System.Text.Json.Serialization;

namespace AuthService.Models
{
    public class PaymentConfig
    {
        [JsonPropertyName("interceptUrl")]
        public string InterceptUrl { get; set; } = string.Empty;
    }
}