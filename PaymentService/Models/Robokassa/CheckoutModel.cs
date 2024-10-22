using System.Text.Json.Serialization;

namespace PaymentService.Models.Robokassa
{
    public class CheckoutModel
    {
        [JsonPropertyName("invoiceID")]
        public string InvoiceId { get; set; } = string.Empty;

        [JsonPropertyName("error")]
        public RobokassaError? Error { get; set; }
    }
}
