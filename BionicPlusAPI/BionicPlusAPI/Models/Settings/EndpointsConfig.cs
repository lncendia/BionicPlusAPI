using System.Text.Json.Serialization;

namespace BionicPlusAPI.Models.Settings
{
    public class EndpointsConfig
    {
        [JsonPropertyName("paymentServiceUrl")]
        public string PaymentServiceUrl { get; set; } = string.Empty;
    }
}
