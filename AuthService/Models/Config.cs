using System.Text.Json.Serialization;

namespace AuthService.Models
{
    public class Config
    {
        [JsonPropertyName("endpoints")]
        public EndpointsConfig? Endpoints { get; set; }

        [JsonPropertyName("settings")]
        public ClientSettingsConfig? Settings { get; set; }

        [JsonPropertyName("payment")]
        public PaymentConfig? Payment { get; set; }
    }
}
