using System.Text.Json.Serialization;

namespace PaymentService.Models.Robokassa
{
    public class RobokassaError
    {
        [JsonPropertyName("header")]
        public string Header { get; set; } = string.Empty;

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("code")]
        public int Code { get; set; }
    }
}
